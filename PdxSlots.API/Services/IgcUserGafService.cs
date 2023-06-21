using AutoMapper;
using AutoWrapper.Wrappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PdxSlots.API.Common;
using PdxSlots.API.Common.Configurations;
using PdxSlots.API.Data;
using PdxSlots.API.Dtos.Games;
using PdxSlots.API.Dtos.IGCUserGaf;
using PdxSlots.API.Models;
using PdxSlots.API.Services.Interfaces;
using PdxSlots.IGPClient.Services;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using Wangkanai.Detection.Services;

namespace PdxSlots.API.Services
{
    public class IgcUserGafService : IIgcUserGafService
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
        private readonly IEventService _eventService;

        public IgcUserGafService(IHttpContextAccessor httpContextAccessor, PdxSlotsContext pdxSlotsContext, IMapper mapper,
            IGCPClientService iGCPClientService, IDetectionService detectionService, IOptions<GameConfiguration> _gameConfigurationOptions,
            IHostEnvironment environment, IAzureService azureService, IUserService userService, IEventService eventService)
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
            _eventService = eventService;
        }

        public async Task<GameGetDto> CreateGame(GamePostDto gamePostDto)
        {
            var game = _mapper.Map<Models.Game>(gamePostDto);

            await _pdxSlotsContext.Games.AddAsync(game);
            await _pdxSlotsContext.SaveChangesAsync();

            return _mapper.Map<GameGetDto>(game);
        }

        public async Task<IgcUserGafGetDto> CreateUserGaf(IgcUserGafPostDto igcUserGafPostDto)
        {
            var userGaf = _mapper.Map<IgcuserGaf>(igcUserGafPostDto);

            await _pdxSlotsContext.IgcuserGafs.AddAsync(userGaf);
            await _pdxSlotsContext.SaveChangesAsync();

            return _mapper.Map<IgcUserGafGetDto>(userGaf);
        }

        public async Task<bool> DeleteUserGaf(int id)
        {
            var userGaf = await _pdxSlotsContext.IgcuserGafs.FirstOrDefaultAsync(x => x.Id == id);

            if (userGaf == null)
            {
                await _eventService.CreateEvent($"{Constants.EventNames.Error}", $"Deleting User Gaf - " +
                    $"GAF id : {id}, Error: [Gaf not found.]");
                throw new ApiException("Gaf not found.", (int)HttpStatusCode.NotFound);
            }

            _pdxSlotsContext.Remove(userGaf);            

            return await _pdxSlotsContext.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<IgcUserGafGetDto>> GetUserGafs()
        {
            var gafs = await _pdxSlotsContext.IgcuserGafs.ToListAsync();

            return _mapper.Map<IEnumerable<IgcUserGafGetDto>>(gafs);
        }

        public async Task<IgcUserGafGetDto> UpdateUserGaf(int id, IgcUserGafPutDto igcUserGafPutDto)
        {
            var userGaf = await _pdxSlotsContext.IgcuserGafs.FirstOrDefaultAsync(x => x.Id == id);

            if (userGaf == null)
            {
                await _eventService.CreateEvent($"{Constants.EventNames.Error}", $"Updating User Gaf - " +
                    $"GAF id : {id}, Error: [Gaf not found.]");
                throw new ApiException("Gaf not found.", (int)HttpStatusCode.NotFound);
            }

            _mapper.Map(igcUserGafPutDto, userGaf);

            await _pdxSlotsContext.SaveChangesAsync();

            return _mapper.Map<IgcUserGafGetDto>(userGaf);
        }
    }
}
