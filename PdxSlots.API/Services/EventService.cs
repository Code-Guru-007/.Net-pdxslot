using AutoMapper;
using AutoWrapper.Wrappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PdxSlots.API.Common;
using PdxSlots.API.Common.Configurations;
using PdxSlots.API.Data;
using PdxSlots.API.Dtos.Events;
using PdxSlots.API.Dtos.Games;
using PdxSlots.API.Models;
using PdxSlots.API.Services.Interfaces;
using PdxSlots.IGPClient.Services;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using Wangkanai.Detection.Services;

namespace PdxSlots.API.Services
{
    public class EventService : IEventService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly PdxSlotsContext _pdxSlotsContext;
        private readonly IMapper _mapper;
        private readonly IDetectionService _detectionService;
        private readonly IUserService _userService;
        private readonly ILogger<GameService> _logger;

        public EventService(IHttpContextAccessor httpContextAccessor, PdxSlotsContext pdxSlotsContext, IMapper mapper,
            IDetectionService detectionService,  IUserService userService, ILogger<GameService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _pdxSlotsContext = pdxSlotsContext;
            _mapper = mapper;
            _detectionService = detectionService;
            _userService = userService;
            _logger = logger;
        }

        public async Task<EventGetDto> CreateEvent(EventPostDto eventPostDto)
        {
            var @event = _mapper.Map<Event>(eventPostDto);

            var user = await _userService.GetMe();
            var device = GetDomainDevice();

            device.UserId = user.Id;
            @event.Device = device;
            @event.UserId = user.Id;

            await _pdxSlotsContext.Events.AddAsync(@event);
            await _pdxSlotsContext.SaveChangesAsync();

            return _mapper.Map<EventGetDto>(@event);
        }

        private Device GetDomainDevice()
        {
            var device = _detectionService.Device;
            var browser = _detectionService.Browser;

            return new Device()
            {
                Browser = browser.Name.ToString(),
                Created = DateTime.UtcNow,
                IpAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString(),
                Mobile = device.Type == Wangkanai.Detection.Models.Device.Mobile,
                OperatingSystem = _detectionService.Platform.Name.ToString(),
            };
        }

        public async Task<EventGetDto> CreateEvent(string name, string description, int? gameId, int? gameMathId, int? operatorId, int? roundId)
        {
            var @event = new Event()
            {
                Device = GetDomainDevice(),
                Name = name,
                Description = description,
                GameId = gameId,
                GameMathId = gameMathId,
                OperatorId = operatorId,
                RoundId = roundId,
                Created = DateTime.UtcNow,
            };

            var user = await _userService.GetMe();
            var device = GetDomainDevice();

            if (user != null) @event.UserId = user.Id;
            if(device != null)
            {
                @event.DeviceId = device.Id;

                if (user != null) device.UserId = user.Id;                
            }

            await _pdxSlotsContext.Events.AddAsync(@event);
            await _pdxSlotsContext.SaveChangesAsync();

            return _mapper.Map<EventGetDto>(@event);
        }

        public async Task<IEnumerable<EventGetDto>> GetEvents(DateTime start, DateTime? end)
        {
            IList<Event> events = new List<Event>();


            if (end.HasValue)
            {
                events = await _pdxSlotsContext
                    .Events
                    .Include(x => x.User)
                    .Include(x => x.Device)
                    .Include(x => x.Game)
                    .Include(x => x.GameMath)
                    .Include(x => x.Operator)
                    .Include(x => x.Round)
                        .ThenInclude(x => x.Game)
                    .Include(x => x.Round)
                        .ThenInclude(x => x.Operator)
                    .Include(x => x.Round)
                        .ThenInclude(x => x.Device)
                    .Include(x => x.Round)
                        .ThenInclude(x => x.Status)
                    .Where(x => x.Created > start && x.Created < end)
                    .ToListAsync();
            }
            else
            {
                events = await _pdxSlotsContext
                    .Events
                    .Include(x => x.Round)
                    .Include(x => x.User)
                    .Include(x => x.Device)
                    .Include(x => x.Game)
                    .Include(x => x.GameMath)
                    .Where(x => x.Created > start)
                    .ToListAsync();
            }

            events = events.OrderByDescending(x => x.Id).ToList();

            return _mapper.Map<IEnumerable<EventGetDto>>(events);   
        }
    }
}
