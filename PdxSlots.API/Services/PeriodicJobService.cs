using AutoMapper;
using AutoWrapper.Wrappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PdxSlots.API.Common;
using PdxSlots.API.Common.Configurations;
using PdxSlots.API.Data;
using PdxSlots.API.Dtos.Games;
using PdxSlots.API.Dtos.PeriodicJobs;
using PdxSlots.API.Models;
using PdxSlots.API.Services.Interfaces;
using PdxSlots.IGPClient.Services;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using Wangkanai.Detection.Services;

namespace PdxSlots.API.Services
{
    public class PeriodicJobService : IPeriodicJobService
    {
        private readonly PdxSlotsContext _pdxSlotsContext;
        private readonly IMapper _mapper;
        private readonly IEventService _eventService;
        private readonly ISendGridClient _sendGridClient;

        public PeriodicJobService(PdxSlotsContext pdxSlotsContext, IMapper mapper, IEventService eventService, ISendGridClient sendGridClient)
        {
            _pdxSlotsContext = pdxSlotsContext;
            _mapper = mapper;
            _eventService = eventService;
            _sendGridClient = sendGridClient;
        } 

        public async Task<PeriodicJobGetDto> StartJob()
        {
            var job = new PeriodicJob()
            {
                Start = DateTime.UtcNow,
                End = null,
            };

            await _pdxSlotsContext.PeriodicJobs.AddAsync(job);
            await _pdxSlotsContext.SaveChangesAsync();

            IList<string> jobLogs = new List<string>();

            var jobLog = $"Started Job {job.Id}";
            await _eventService.CreateEvent(Constants.EventNames.Create.ToString(), jobLog);
            jobLogs.Add(jobLog);

            var games = await _pdxSlotsContext
                .Games
                .Include(x => x.GameMaths)
                    .ThenInclude(x => x.Operator)
                .Include(x => x.GameMaths)
                    .ThenInclude(x => x.MathFileUpload)
                    .ThenInclude(x => x.ZippedFiles)
                .Include(x => x.DesktopZipFileUpload)
                    .ThenInclude(x => x.ZippedFiles)
                .Include(x => x.MobileZipFileUpload)
                    .ThenInclude(x => x.ZippedFiles)
                .ToListAsync();

            foreach (var game in games)
            {
                if (game.DesktopZipFileUpload != null)                
                    await ProcessHash(job.Id, jobLogs, "Desktop Content", game.DesktopZipFileUpload.ZippedFiles, job);                
                if (game.MobileZipFileUpload != null)                
                    await ProcessHash(job.Id, jobLogs, "Mobile Content Content", game.DesktopZipFileUpload.ZippedFiles, job);

                foreach(var gameMath in game.GameMaths)
                {
                    if(gameMath.MathFileUpload != null)
                    {
                        await ProcessHash(job.Id, jobLogs, $"Game Math {gameMath.Id} Content", gameMath.MathFileUpload.ZippedFiles, job);
                    }
                }                
            }

            job.End = DateTime.UtcNow;
            await _pdxSlotsContext.SaveChangesAsync();
            jobLog = $"Ended Job {job.Id} - {job.End - job.Start}";
            await _eventService.CreateEvent(Constants.EventNames.Create.ToString(), jobLog);
            jobLogs.Add(jobLog);

            var emails = await _pdxSlotsContext.PeriodicJobEmails.ToListAsync();

            var msg = new SendGridMessage()
            {
                From = new EmailAddress("pdxslots@savagesoftwarecorp.com", "PdxSlots"),
                Subject = $"Job Id {job.Id} Results"
            };
            msg.AddContent(MimeType.Text, String.Join("\r\n", jobLogs));

            foreach(var email in emails)            
                msg.AddTo(new EmailAddress(email.Email));            

            var response = await _sendGridClient.SendEmailAsync(msg).ConfigureAwait(false);

            return _mapper.Map<PeriodicJobGetDto>(job);
        }

        private async Task ProcessHash(int jobId, IList<string> jobLogs, string content, ICollection<ZippedFile> zipFiles, PeriodicJob job)
        {
            string jobLog = $"Job {jobId} - {content} Hash Check Started";
            await _eventService.CreateEvent(Constants.EventNames.Create.ToString(), jobLog);
            jobLogs.Add(jobLog);

            foreach (var zippedFile in zipFiles)
            {
                try
                {

                    if (zippedFile.HashCheck)
                    {
                        var currentHash = zippedFile.LocalFilePath.CheckMd5();
                        var oldHash = zippedFile.Hash;

                        var periodicZipFile = new PeriodicJobZippedFile()
                        {
                            OriginalHash = oldHash,
                            Created = DateTime.UtcNow,
                            CurrentHash = currentHash,
                            HashCheck = zippedFile.HashCheck,
                            HashEquals = currentHash == oldHash,
                            ZippedFileId = zippedFile.Id,
                            PeriodicJobId = job.Id                            
                        };

                        job.PeriodicJobZippedFiles.Add(periodicZipFile);

                        await _pdxSlotsContext.SaveChangesAsync();

                        if (currentHash == oldHash)
                            jobLog = $"Job {jobId} - {content} Hash Check File {zippedFile.LocalFilePath} OK, original: {oldHash} current: {currentHash}";
                        else
                            jobLog = $"Job {jobId} - {content} Hash Check File {zippedFile.LocalFilePath} Mismatch, original: {oldHash} current: {currentHash}";

                        await _eventService.CreateEvent(Constants.EventNames.Create.ToString(), jobLog);
                        jobLogs.Add(jobLog);
                    }
                    else
                    {
                        jobLog = $"Job {jobId} - {content} Hash Check File {zippedFile.LocalFilePath} skipped since hash check is false.";
                        await _eventService.CreateEvent(Constants.EventNames.Create.ToString(), jobLog);
                        jobLogs.Add(jobLog);
                    }
                }
                catch (Exception ex)
                {
                    jobLog = $"Job {jobId} - {content} Hash Check Error : {ex.Message}.";
                    await _eventService.CreateEvent(Constants.EventNames.Create.ToString(), jobLog);
                    jobLogs.Add(jobLog);
                }
            }
        }

        public async Task<IEnumerable<PeriodicJobGetDto>> GetJobs()
        {
            var jobs = await _pdxSlotsContext.PeriodicJobs
                .Include(x=>x.PeriodicJobZippedFiles).ThenInclude(x=>x.ZippedFile)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PeriodicJobGetDto>>(jobs);
        }

        public async Task<PeriodicJobEmailsGetDto> GetEmails()
        {
            var emails = await _pdxSlotsContext.PeriodicJobEmails.ToListAsync();

            return new PeriodicJobEmailsGetDto()
            {
                Emails = string.Join(",", emails.Select(x => x.Email))
            };
        }

        public async Task<PeriodicJobEmailsGetDto> UpdateEmails(PeriodicJobEmailsPostDto periodicJobEmailsPostDto)
        {
            var existingEmails = await _pdxSlotsContext.PeriodicJobEmails.ToListAsync();

            foreach(var email in existingEmails)
            {
                _pdxSlotsContext.Remove(email);
            }

            await _pdxSlotsContext.SaveChangesAsync();

            var newEmails = periodicJobEmailsPostDto.Emails.Split(",").ToList();

            foreach(var email in newEmails)
            {
                await _pdxSlotsContext.PeriodicJobEmails.AddAsync(new PeriodicJobEmail()
                {
                    Email = email,
                });
            }

            await _pdxSlotsContext.SaveChangesAsync();

            return await GetEmails();
        }
    }
}
