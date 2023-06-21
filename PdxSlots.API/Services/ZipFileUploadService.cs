using AutoMapper;
using AutoWrapper.Wrappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PdxSlots.API.Common;
using PdxSlots.API.Common.Configurations;
using PdxSlots.API.Data;
using PdxSlots.API.Dtos.Games;
using PdxSlots.API.Dtos.ZipFileUpload;
using PdxSlots.API.Dtos.ZippedFile;
using PdxSlots.API.Models;
using PdxSlots.API.Services.Interfaces;
using PdxSlots.IGPClient.Services;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using Wangkanai.Detection.Services;

namespace PdxSlots.API.Services
{
    public class ZipFileUploadService : IZipFileUploadService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly PdxSlotsContext _pdxSlotsContext;
        private readonly IMapper _mapper;
        private readonly IGCPClientService _iGCPClientService;
        private readonly IDetectionService _detectionService;
        private readonly IHostEnvironment _environment;
        private readonly IAzureService _azureService;
        private readonly IUserService _userService;
        private readonly GameConfiguration _gameConfiguration;
        private readonly ILogger<GameService> _logger;
        private readonly IEventService _eventService;

        public ZipFileUploadService(IHttpContextAccessor httpContextAccessor, PdxSlotsContext pdxSlotsContext, IMapper mapper,
            IGCPClientService iGCPClientService, IDetectionService detectionService, IOptions<GameConfiguration> _gameConfigurationOptions,
            IHostEnvironment environment, IAzureService azureService, IUserService userService, ILogger<GameService> logger, IEventService eventService)
        {
            _httpContextAccessor = httpContextAccessor;
            _pdxSlotsContext = pdxSlotsContext;
            _mapper = mapper;
            _iGCPClientService = iGCPClientService;
            _detectionService = detectionService;
            _environment = environment;
            _azureService = azureService;
            _userService = userService;
            _gameConfiguration = _gameConfigurationOptions.Value;
            _logger = logger;
            _eventService = eventService;
        }

        public async Task<ZipFileUploadGetDto> GetZipFileUpload(int id)
        {
            var zipFileUpload = await _pdxSlotsContext
                .ZipFileUploads
                .Include(x => x.ZippedFiles)
                .FirstOrDefaultAsync(x => x.Id == id);

            return _mapper.Map<ZipFileUploadGetDto>(zipFileUpload);
        }

        public async Task<ZippedFileGetDto> UpdateZippedFile(int id, ZippedFilePutDto zippedFilePutDto)
        {
            var zippedFile = await _pdxSlotsContext.ZippedFiles.FirstOrDefaultAsync(x => x.Id == id);

            if (zippedFile == null)
            {
                await _eventService.CreateEvent($"{Constants.EventNames.Error}", $"Update Zipped File - " +
                    $"Zipped File Id: {id}, Error: [Zipped file not found.]");
                throw new ApiException("Zipped file not found.", (int)HttpStatusCode.NotFound);
            }

            _mapper.Map(zippedFilePutDto, zippedFile);

            await _pdxSlotsContext.SaveChangesAsync();

            return _mapper.Map<ZippedFileGetDto>(zippedFile);
        }
    }
}
