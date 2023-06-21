using AutoMapper;
using AutoWrapper.Wrappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PdxSlots.API.Common;
using PdxSlots.API.Common.Configurations;
using PdxSlots.API.Data;
using PdxSlots.API.Dtos.GameFeatures;
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
    public class GameService : IGamesService
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

        public GameService(IHttpContextAccessor httpContextAccessor, PdxSlotsContext pdxSlotsContext, IMapper mapper,
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

        public async Task<GameGetDto> CreateGame(GamePostDto gamePostDto)
        {
            var user = await _userService.GetMe();
            var game = _mapper.Map<Models.Game>(gamePostDto);

            _logger.LogInformation($"User {user.Id} is creating game {gamePostDto.ToJSONString()}.");

            await _pdxSlotsContext.Games.AddAsync(game);
            await _pdxSlotsContext.SaveChangesAsync();

            _logger.LogInformation($"User {user.Id} created game {game.ToJSONString()}.");

            await _eventService.CreateEvent(Constants.EventNames.Create.ToString(), $"User {user.Id} created Game {game.Id}.", game.Id);

            return _mapper.Map<GameGetDto>(game);
        }

        public async Task<GameGetDto> CreateGameContent(int id, IFormFile zipContent, bool mobile)
        {
            var user = await _userService.GetMe();

            var game = await _pdxSlotsContext.Games.FirstOrDefaultAsync(x => x.Id == id);

            if (game == null)
            {
                await _eventService.CreateEvent($"{Constants.EventNames.Error}", $"Create Game Content - Game id: {id}, " +
                    $"Error: [Game not found.]");
                throw new ApiException("Game not found", (int)HttpStatusCode.NotFound);
            }

            var blobUrl = await _azureService.CreateBlobFromFormFile(zipContent, "uploads");

            var zipFileUpload = new ZipFileUpload()
            {
                BlobFileUrl = blobUrl,
                Created = DateTime.UtcNow,
                FileName = zipContent.FileName,
                UserId = user.Id,
            };

            var gameDirectoryName = $"{game.ExternalGameId}";
            if (mobile) gameDirectoryName += "_mobile";
            else gameDirectoryName += "_desktop";

            var uploadDirectory = Path.Combine("uploads");
            var gameDirectory = Path.Combine(_gameConfiguration.IGCRootDirectory, "games");
            var currentGameDirectory = Path.Combine(gameDirectory, gameDirectoryName);

            if (!Directory.Exists(uploadDirectory)) Directory.CreateDirectory(uploadDirectory);
            if (!Directory.Exists(gameDirectory)) Directory.CreateDirectory(gameDirectory);
            if (!Directory.Exists(currentGameDirectory)) Directory.CreateDirectory(currentGameDirectory);

            string zipFilePath = await CopyAndExtractZipFile(zipContent, uploadDirectory, currentGameDirectory);

            var baseUri = new Uri(_gameConfiguration.IGCBaseUrl);
            var gameUri = new Uri(baseUri, $"games/{gameDirectoryName}/");

            ProcessZipFile(zipFileUpload, currentGameDirectory, zipFilePath, gameUri);

            File.Delete(zipFilePath);

            await _pdxSlotsContext.ZipFileUploads.AddAsync(zipFileUpload);
            await _pdxSlotsContext.SaveChangesAsync();

            if (mobile) game.MobileZipFileUploadId = zipFileUpload.Id;
            else game.DesktopZipFileUploadId = zipFileUpload.Id;

            await _pdxSlotsContext.SaveChangesAsync();

            return _mapper.Map<GameGetDto>(game);
        }

        private async Task<string> CopyAndExtractZipFile(IFormFile zipContent, string uploadDirectory, string currentGameDirectory)
        {
            string zipFilePath = Path.Combine(uploadDirectory, zipContent.FileName);
            using (Stream fileStream = new FileStream(zipFilePath, FileMode.Create))
                await zipContent.CopyToAsync(fileStream);

            ZipFile.ExtractToDirectory(zipFilePath, currentGameDirectory, true);
            return zipFilePath;
        }

        private void ProcessZipFile(ZipFileUpload zipFileUpload, string currentGameDirectory, string zipFilePath, Uri gameUri)
        {
            using (var archive = ZipFile.OpenRead(zipFilePath))
            {
                foreach (var zipArchiveEntry in archive.Entries)
                {
                    var fileUrl = new Uri(gameUri, $"{zipArchiveEntry.FullName}");

                    if (zipArchiveEntry.Length > 0)
                    {
                        var unzippedFilePath = Path.GetFullPath(Path.Combine(currentGameDirectory, zipArchiveEntry.FullName)).Replace(@"\\", @"\");
                        var zippedFile = new ZippedFile()
                        {
                            Created = DateTime.UtcNow,
                            FileUrl = fileUrl.ToString(),
                            Hash = unzippedFilePath.CheckMd5(),
                            LocalFilePath = unzippedFilePath,
                            HashCheck = true
                        };

                        zipFileUpload.ZippedFiles.Add(zippedFile);
                    }
                }
            }
        }

        public async Task<GameGetDto> CreateGameMathContent(int id, int gameMathId, IFormFile zipContent)
        {
            var user = await _userService.GetMe();

            var game = await _pdxSlotsContext.Games.Include(x => x.GameMaths).FirstOrDefaultAsync(x => x.Id == id);

            if (game == null)
            {
                await _eventService.CreateEvent($"{Constants.EventNames.Error}", $"Create Game Math Content - Game id: {id}, " +
                    $"Error: [Game not found.]");
                throw new ApiException("Game not found", (int)HttpStatusCode.NotFound);
            }

            var gameMath = game.GameMaths.FirstOrDefault(x => x.Id == gameMathId);

            if (gameMath == null)
            {
                await _eventService.CreateEvent($"{Constants.EventNames.Error}", $"Create Game Math Content - Game math id: {gameMathId}, " +
                    $"Error: [Game Maths not found.]", gameId: game.Id);
                throw new ApiException("Game Maths not found", (int)HttpStatusCode.NotFound);
            }

            var blobUrl = await _azureService.CreateBlobFromFormFile(zipContent, "uploads");

            var zipFileUpload = new ZipFileUpload()
            {
                BlobFileUrl = blobUrl,
                Created = DateTime.UtcNow,
                FileName = zipContent.FileName,
                UserId = user.Id,
            };

            var gameDirectoryName = $"{game.ExternalGameId}_{gameMath.Id}";

            var uploadDirectory = Path.Combine("uploads");
            var gameMathsDirectory = Path.Combine(_gameConfiguration.IGCRootDirectory, "game-maths");
            var currentGameMathsDirectory = Path.Combine(gameMathsDirectory, gameDirectoryName);

            if (!Directory.Exists(uploadDirectory)) Directory.CreateDirectory(uploadDirectory);
            if (!Directory.Exists(gameMathsDirectory)) Directory.CreateDirectory(gameMathsDirectory);
            if (!Directory.Exists(currentGameMathsDirectory)) Directory.CreateDirectory(currentGameMathsDirectory);

            string zipFilePath = await CopyAndExtractZipFile(zipContent, uploadDirectory, currentGameMathsDirectory);

            var baseUri = new Uri(_gameConfiguration.IGCBaseUrl);
            var gameUri = new Uri(baseUri, $"game-maths/{gameDirectoryName}/");

            ProcessZipFile(zipFileUpload, currentGameMathsDirectory, zipFilePath, gameUri);

            File.Delete(zipFilePath);

            await _pdxSlotsContext.ZipFileUploads.AddAsync(zipFileUpload);
            await _pdxSlotsContext.SaveChangesAsync();

            gameMath.MathFileUpload = zipFileUpload;

            await _pdxSlotsContext.SaveChangesAsync();

            return _mapper.Map<GameGetDto>(game);
        }

        public async Task<GameGetDto> EditGame(int id, GamePutDto gamePutDto)
        {
            var game = await _pdxSlotsContext.Games.FirstOrDefaultAsync(x => x.Id == id);

            if (game == null)
            {
                await _eventService.CreateEvent($"{Constants.EventNames.Error}", $"Updating game - Game Id: {id}, " +
                    $"Error: [Game not found.]");
                throw new ApiException("Game not found", (int)HttpStatusCode.NotFound);
            }

            await _eventService.CreateEvent($"{Constants.EventNames.Update}", $"Updating game - " +
                $"Game id : {game.Id}, Updated: [{game.ToComparisonEventDescription(gamePutDto)}]", game.Id);

            _mapper.Map(gamePutDto, game);

            await _pdxSlotsContext.SaveChangesAsync();

            return _mapper.Map<GameGetDto>(game);
        }

        public async Task<GameGetDto> GetGame(int id)
        {
            var game = await _pdxSlotsContext
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
                .FirstOrDefaultAsync(x => x.Id == id);

            return _mapper.Map<GameGetDto>(game);
        }

        public async Task<IEnumerable<GameGetDto>> GetGames()
        {
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

            return _mapper.Map<IEnumerable<GameGetDto>>(games);
        }

        public async Task<InitializeGameGetDto> InitializeGame(string userId, string operatorId, string gameId, string sessionId)
        {
            // query game by game id
            var game = await _pdxSlotsContext.Games
                .Include(x => x.GameMaths).ThenInclude(x => x.Operator)
                .FirstOrDefaultAsync(x => x.ExternalGameId == gameId);

            if (game != null)
            {
                var gameMath = game.GameMaths.FirstOrDefault(x => x.ExternalOperatorId == operatorId);

                if (gameMath != null)
                {
                    var @operator = gameMath.Operator;
                    var balance = await _iGCPClientService.GetVerification(userId, @operator.VendorId, sessionId);

                    await _eventService.CreateEvent($"{Constants.EventNames.Get}", $"Initializing game - User Id {userId}, balance: {balance.CurrentBalance}, verification: {balance.Verification} " +
                        $" operator id : {operatorId} , game id : {gameId} , session id : {sessionId}", game.Id, gameMath.Id, @operator.Id);

                    return new InitializeGameGetDto()
                    {
                        current_balance = balance.CurrentBalance,
                        status = balance.Verification,
                        bets = gameMath.Bets,
                        denominations = gameMath.Denominations,
                    };
                }

                await _eventService.CreateEvent($"{Constants.EventNames.Error}", $"Initializing Game - " +
                    $"Game id: {gameId}, Error: [Unable to find game math by operator id : {operatorId}.]");
                throw new ApiException($"Unable to find game math by operator id : {operatorId}.");
            }

            await _eventService.CreateEvent($"{Constants.EventNames.Error}", $"Initializing Game - " +
                $"Game id: {gameId}, Error: [Unable to find game with id : {gameId}.]");
            throw new ApiException($"Unable to find game with id : {gameId}.");
        }

        public async Task<string> OpenGame(string userId, string operatorId, string gameId, string sessionId, bool mobileOverride)
        {
            var game = await _pdxSlotsContext.Games.FirstOrDefaultAsync(x => x.ExternalGameId == gameId);

            var @operator = await _pdxSlotsContext.Operators.FirstOrDefaultAsync(x => x.ExternalOperatorId == operatorId);

            if (game != null && @operator != null)
            {
                var device = _detectionService.Device;

                var url = $"{game.DesktopFileUrl}?user_id={userId}&operator_id={operatorId}&session_id={sessionId}&game_id={gameId}";

                if (device.Type == Wangkanai.Detection.Models.Device.Mobile || mobileOverride)
                    url = $"{game.MobileFileUrl}?user_id={userId}&operator_id={operatorId}&session_id={sessionId}&game_id={gameId}";

                await _eventService.CreateEvent($"{Constants.EventNames.Get}", $"Opening game - User Id {userId}, " +
                $" operator id : {operatorId} , game id : {gameId} , session id : {sessionId}, url : {url}", game.Id);

                return url;
            }

            return default;
        }

        public async Task<SpinGameGetDto> SpinGame(string userId, string operatorId, string gameId, string sessionId, decimal denom, decimal wager)
        {
            await _eventService.CreateEvent($"{Constants.EventNames.Get}", $"Spin Started - User Id {userId}, " +
$" operator id : {operatorId} , game id : {gameId} , session id : {sessionId}, denom : {denom}, wager: {wager}");

            var game = await _pdxSlotsContext
                .Games
                .Include(x => x.GameMaths)
                .Include(x => x.GameFeatures)
                .FirstOrDefaultAsync(x => x.ExternalGameId == gameId);

            if (game == null)
            {
                await _eventService.CreateEvent($"{Constants.EventNames.Error}", $"Spin Game - " +
                    $"Game id: {gameId}, Error: [Unable to find game with id : {gameId}.]");
                throw new ApiException($"No game found for id {gameId}", (int)HttpStatusCode.NotFound);
            }

            var @operator = await _pdxSlotsContext.Operators.FirstOrDefaultAsync(x => x.ExternalOperatorId == operatorId);

            if (@operator == null)
            {
                await _eventService.CreateEvent($"{Constants.EventNames.Error}", $"Initializing Game - " +
                    $"Game id: {gameId}, Error: [No operator found id {operatorId}]");
                throw new ApiException($"No operator found id {operatorId}", (int)HttpStatusCode.NotFound);
            }

            if (game != null && @operator != null)
            {
                var gameMath = game.GameMaths.FirstOrDefault(x => x.ExternalOperatorId == operatorId);

                if (gameMath == null)
                {
                    await _eventService.CreateEvent($"{Constants.EventNames.Error}", $"Initializing Game - " +
                        $"Game id: {gameId}, Error: [No game math found for operator id {operatorId}]");
                    throw new ApiException($"No game math found for operator id {operatorId}", (int)HttpStatusCode.NotFound);
                }

                if (game.Active == false)
                {
                    await _eventService.CreateEvent($"{Constants.EventNames.Error}", $"Spin Game Not Active - User Id {userId}, " +
                        $" operator id : {operatorId} , game id : {gameId} , session id : {sessionId}", game.Id, gameMath.Id, @operator.Id);
                    throw new ApiException($"Game is not active.");
                }

                if (gameMath.Active == false)
                {
                    await _eventService.CreateEvent($"{Constants.EventNames.Error}", $"Spin Game Math Not Active - User Id {userId}, " +
                        $" operator id : {operatorId} , game id : {gameId} , session id : {sessionId}", game.Id, gameMath.Id, @operator.Id);
                    throw new ApiException($"Game math is not active.");
                }

                if (!string.IsNullOrEmpty(gameMath.Denominations))
                {
                    var availableDenoms = gameMath.Denominations.Split(",").Select(x => decimal.Parse(x)).ToList();

                    if (!availableDenoms.Contains(denom))
                    {
                        await _eventService.CreateEvent($"{Constants.EventNames.Error}", $"Spin No Matching Denomination - User Id {userId}, " +
                            $" operator id : {operatorId} , game id : {gameId} , session id : {sessionId}, Denoms: {gameMath.Denominations} doesn't contain {denom}"
                            , game.Id, gameMath.Id, @operator.Id);
                        throw new ApiException($"Denomination '{denom}' doesn't match available denominations '{gameMath.Denominations}'.");
                    }
                }

                if (!string.IsNullOrEmpty(gameMath.Bets))
                {
                    var availableBets = gameMath.Bets.Split(",").Select(x => decimal.Parse(x)).ToList();

                    if (!availableBets.Contains(wager))
                    {
                        await _eventService.CreateEvent($"{Constants.EventNames.Error}", $"Spin No Matching Bets - User Id {userId}, " +
                            $" operator id : {operatorId} , game id : {gameId} , session id : {sessionId}, Bets: {gameMath.Bets} doesn't contain {wager}"
                            , game.Id, gameMath.Id, @operator.Id);
                        throw new ApiException($"Wager doesn't match available bets '{gameMath.Bets}'.");
                    }
                }

                var verification = await _iGCPClientService.GetVerification(userId, @operator.VendorId, sessionId);

                await _eventService.CreateEvent($"{Constants.EventNames.Get}", $"Spin Verification - User Id {userId}, " +
            $" operator id : {operatorId} , game id : {gameId} , session id : {sessionId}, verification : {verification.Verification}, balance : {verification.CurrentBalance}",
            game.Id, gameMath.Id, @operator.Id);

                // do we do logic to make sure its > wager * denom?
                if (verification.CurrentBalance > 0)
                {
                    var device = _detectionService.Device;
                    var browser = _detectionService.Browser;

                    var round = new Models.Round()
                    {
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow,
                        Denomination = denom,
                        Device = new Models.Device()
                        {
                            Browser = browser.Name.ToString(),
                            Created = DateTime.UtcNow,
                            IpAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString(),
                            Mobile = device.Type == Wangkanai.Detection.Models.Device.Mobile,
                            OperatingSystem = _detectionService.Platform.Name.ToString(),
                            UserId = null,
                        },
                        ExternalGameId = game.ExternalGameId,
                        GameId = game.Id,
                        ExternalOperatorId = @operator.ExternalOperatorId,
                        FundsStart = verification.CurrentBalance,
                        FundsEnd = null,
                        GameResult = null,
                        OperatorId = @operator.Id,
                        PayTable = gameMath.PayTable,
                        SessionId = sessionId,
                        StatusId = (int)Models.Enums.RoundStatus.Open,
                        UserId = userId,
                        Wager = wager,
                        WagerCurrency = 1,
                        WalletApprovalId = null,
                        WalletRoundId = null,
                        Win = 0,
                        WinCurrency = 0,
                        Gaf = null,
                        IncentiveWager = 0,
                        IncentiveWagerDetail = null,
                        IncentiveWin = 0,
                        NonWager = 0,
                        NonWagerDetail = null,
                        ProgressiveWin = 0,
                        ProgressiveWinCont = 0,
                        WagerDetail = null
                    };

                    await _pdxSlotsContext.Rounds.AddAsync(round);
                    await _pdxSlotsContext.SaveChangesAsync();

                    if (round.Id > 0)
                    {
                        await _eventService.CreateEvent($"{Constants.EventNames.Get}", $"Spin Round Created - User Id {userId}, " +
                        $" operator id : {operatorId} , game id : {gameId} , session id : {sessionId}, round : {round.Id}",
                        game.Id, gameMath.Id, @operator.Id, round.Id);

                        var startGameCycle = await _iGCPClientService
                            .StartGameCycle(round.Id, userId, @operator.VendorId, gameId, gameMath.PayTable, sessionId, wager, denom);

                        await _eventService.CreateEvent($"{Constants.EventNames.Get}", $"Spin Start Game Cycle - User Id {userId}, " +
$" operator id : {operatorId} , game id : {gameId} , session id : {sessionId}, round : {round.Id}, startCycle : {startGameCycle.Status}",
game.Id, gameMath.Id, @operator.Id, round.Id);

                        if (startGameCycle.Status.Equals("Approved", StringComparison.OrdinalIgnoreCase))
                        {
                            round.WalletApprovalId = startGameCycle.ApprovalId.ToString();
                            round.WalletRoundId = startGameCycle.TransactionRound.ToString();
                            round.FundsStart = startGameCycle.FundsStart;
                            round.Updated = DateTime.UtcNow;
                            await _pdxSlotsContext.SaveChangesAsync();

                            string gaf = null;
                            var userGaf = await _pdxSlotsContext.IgcuserGafs.FirstOrDefaultAsync(x => x.UserId == userId && x.OperatorId == gameMath.OperatorId);

                            if (userGaf != null)
                            {
                                gaf = userGaf.Gaf;
                                round.Gaf = gaf;

                                await _eventService.CreateEvent($"{Constants.EventNames.Get}", $"Spin Gaf - User Id {userId}, " +
                                $" operator id : {operatorId} , game id : {gameId} , session id : {sessionId}, round : {round.Id}, Gaf : {round.Gaf}",
                                game.Id, gameMath.Id, @operator.Id, round.Id);
                            }

                            List<GameFeature> gameFeatures = game.GameFeatures.Where(x => x.OperatorId == @operator.Id).ToList();
                            List<GameFeature> userFeatures = new List<GameFeature>();

                            var url = $"{gameMath.MathFileUrl}?denom={denom}&wager={wager}";

                            if (gameFeatures != null && gameFeatures.Count > 0)
                            {
                                userFeatures = gameFeatures.Where(x => x.UserId == userId).ToList();
                                var userGameFeaturesEnabled = gameFeatures.Where(x => x.UserEnabled).ToList();

                                foreach (var gameFeature in userGameFeaturesEnabled)
                                {
                                    var existing = userFeatures.FirstOrDefault(x => x.Feature == gameFeature.Feature);

                                    // create the user game feature
                                    if(existing == null)
                                    {
                                        var newFeature = new GameFeature()
                                        {
                                            GameId = game.Id,
                                            GameMathId = gameMath.Id,
                                            OperatorId = @operator.Id,
                                            IsLiability = gameFeature.IsLiability,
                                            Value = gameFeature.Value,
                                            UserEnabled = false,
                                            OperatorEnabled = false,
                                            Feature = gameFeature.Feature,
                                            UserId = userId,                                            
                                        };

                                        await _pdxSlotsContext.GameFeatures.AddAsync(newFeature);
                                        await _pdxSlotsContext.SaveChangesAsync();

                                        userFeatures.Add(newFeature);
                                    }
                                }

                                if(userFeatures != null && userFeatures.Count() > 0)                                
                                    url += $"&{string.Join("&", userFeatures.Select(x => $"{x.Feature}={x.Value}"))}";                                   
                                else                                
                                    url += $"&{string.Join("&", gameFeatures.Select(x => $"{x.Feature}={x.Value}"))}";                             
                            }

                            if (!string.IsNullOrEmpty(gaf)) url += $"&gaf={gaf}";

                            await _eventService.CreateEvent($"{Constants.EventNames.Get}", $"Spin Game Math URL - User Id {userId}, " +
                            $" operator id : {operatorId} , game id : {gameId} , session id : {sessionId}, round : {round.Id}, URL: {url}",
                            game.Id, gameMath.Id, @operator.Id, round.Id);

                            dynamic gameMathApiResponse = null;

                            try
                            {
                                gameMathApiResponse = await _iGCPClientService.GameMath(url);
                            }
                            catch (Exception ex)
                            {
                                await _eventService.CreateEvent($"{Constants.EventNames.Error}", $"Spin Game Math ERROR - User Id {userId}, " +
                                $" operator id : {operatorId} , game id : {gameId} , session id : {sessionId}, round : {round.Id}, URL: {url} , Error Message : {ex.Message} , " +
                                $"Stack: {ex.StackTrace}",
                                game.Id, gameMath.Id, @operator.Id, round.Id);
                                throw new ApiException("Error with the game math file.");
                            }

                            round.GameResult = JsonConvert.SerializeObject(gameMathApiResponse);
                            round.Win = decimal.TryParse(gameMathApiResponse.win?.ToString(), out decimal winParsed) ? winParsed : 0;
                            round.Updated = DateTime.UtcNow;
                            await _pdxSlotsContext.SaveChangesAsync();

                            await _eventService.CreateEvent($"{Constants.EventNames.Get}", $"Spin Game Math - User Id {userId}, " +
                                $" operator id : {operatorId} , game id : {gameId} , session id : {sessionId}, round : {round.Id}, Win: {round.Win},  Game Result : {round.GameResult}",
                                game.Id, gameMath.Id, @operator.Id, round.Id);

                            if (gameMathApiResponse != null)
                            {
                                var jsonObject = JObject.FromObject(gameMathApiResponse);
                                var features = jsonObject["features"];

                                foreach (JProperty property in features)
                                {
                                    var gameFeatureFromGame = userFeatures.FirstOrDefault(x => x.Feature == property.Name);

                                    if(gameFeatureFromGame != null)
                                    {
                                        var newValue = decimal.Parse(property.Value.ToString());
                                        await _eventService.CreateEvent($"{Constants.EventNames.Update}", $"Spin Update Game Feature (user) Success - User Id {userId}, " +
                                        $" operator id : {operatorId} , game id : {gameId} , session id : {sessionId}, round : {round.Id}," +
                                        $"Feature Id {gameFeatureFromGame.Id} '{gameFeatureFromGame.Feature}' ('{gameFeatureFromGame.Value}') = '{newValue}'", game.Id, gameMath.Id, @operator.Id, round.Id);
                                        gameFeatureFromGame.Value = newValue;
                                        await _pdxSlotsContext.SaveChangesAsync();
                                    }
                                    else
                                    {
                                        var gameFeaturesFromGame = gameFeatures.Where(x => x.Feature == property.Name);

                                        if(gameFeaturesFromGame.Count() == 0)
                                        {
                                            await _eventService.CreateEvent($"{Constants.EventNames.Update}", $"Spin Update Game Feature Not Found - User Id {userId}, " +
                                             $" operator id : {operatorId} , game id : {gameId} , session id : {sessionId}, round : {round.Id}," +
                                             $"Feature '{property.Name}'", game.Id, gameMath.Id, @operator.Id, round.Id);
                                        }

                                        foreach (var gf in gameFeaturesFromGame)
                                        {
                                            var newValue = decimal.Parse(property.Value.ToString());
                                            await _eventService.CreateEvent($"{Constants.EventNames.Update}", $"Spin Update Game Feature Success - User Id {userId}, " +
                                            $" operator id : {operatorId} , game id : {gameId} , session id : {sessionId}, round : {round.Id}," +
                                            $"Feature Id {gf.Id} '{gf.Feature}' ('{gf.Value}') = '{newValue}'", game.Id, gameMath.Id, @operator.Id, round.Id);
                                            gf.Value = newValue;
                                            await _pdxSlotsContext.SaveChangesAsync();
                                        }
                                    }
                                }
                            }

                            if (round.Win > gameMath.MaxLiability)
                            {
                                await _eventService.CreateEvent($"{Constants.EventNames.Error}", $"Spin Max Liability Reached - User Id {userId}, " +
                                $" operator id : {operatorId} , game id : {gameId} , session id : {sessionId}, round : {round.Id}, Win: {round.Win}," +
                                $" Win {round.Win} is greater than game max liability {gameMath.MaxLiability}, round is stuck open." +
                                $" Game Result : {round.GameResult}", game.Id, gameMath.Id, @operator.Id, round.Id);
                                throw new ApiException($"Win {round.Win} is greater than game max liability {gameMath.MaxLiability}.");
                            }

                            var transaction = await _iGCPClientService.MoneyTransactions(round.Id, userId, @operator.VendorId, sessionId, round.GameResult, round.Win);

                            await _eventService.CreateEvent($"{Constants.EventNames.Get}", $"Spin Money Transaction - User Id {userId}, " +
            $" operator id : {operatorId} , game id : {gameId} , session id : {sessionId}, round : {round.Id}, Win: {round.Win},  Transaction Status : {transaction.Status}",
            game.Id, gameMath.Id, @operator.Id, round.Id);

                            if (transaction.Status.Equals("Approved", StringComparison.OrdinalIgnoreCase))
                            {
                                var endCycle = await _iGCPClientService.EndGameCycle(round.Id, userId, @operator.VendorId, sessionId);

                                await _eventService.CreateEvent($"{Constants.EventNames.Get}", $"Spin End Game Cycle - User Id {userId}, " +
                                    $" operator id : {operatorId} , game id : {gameId} , session id : {sessionId}, round : {round.Id}, Win: {round.Win},  " +
                                    $"End Cycle Status : {endCycle.Status}, Funds End: {endCycle.FundsEnd}",
                                    game.Id, gameMath.Id, @operator.Id, round.Id);

                                if (endCycle.Status.Equals("Approved", StringComparison.OrdinalIgnoreCase))
                                {
                                    decimal.TryParse(gameMathApiResponse.comission, out decimal comission);

                                    decimal.TryParse(gameMathApiResponse.denom?.ToString(), out decimal denomination);
                                    decimal.TryParse(gameMathApiResponse.nonwager_purchase?.ToString(), out decimal nonWagerPurchase);
                                    decimal.TryParse(gameMathApiResponse.progressive_contribution?.ToString(), out decimal progressiveContribution);
                                    decimal.TryParse(gameMathApiResponse.progressive_jackpot?.ToString(), out decimal progressiveJackpot);
                                    decimal.TryParse(gameMathApiResponse.wager?.ToString(), out decimal wagerParsed);

                                    round.StatusId = (int)Models.Enums.RoundStatus.Closed;
                                    round.FundsEnd = endCycle.FundsEnd;
                                    round.Updated = DateTime.UtcNow;
                                    round.NonWager = nonWagerPurchase;
                                    round.ProgressiveWinCont = progressiveContribution;
                                    round.ProgressiveWin = progressiveJackpot;

                                    await _pdxSlotsContext.SaveChangesAsync();

                                    var spinDto = new SpinGameGetDto()
                                    {
                                        Status = endCycle.Status,
                                        CurrentBalance = round.FundsEnd.Value,
                                        GameResult = gameMathApiResponse,
                                        Comission = comission,
                                        Denom = denomination,
                                        NonWagerPurchase = nonWagerPurchase,
                                        ProgressiveContribution = progressiveContribution,
                                        ProgressiveJackpot = progressiveJackpot,
                                        Wager = wagerParsed,
                                        Win = round.Win,
                                    };

                                    await _eventService.CreateEvent($"{Constants.EventNames.Get}", $"Spin Return - User Id {userId}, " +
                                    $" operator id : {operatorId} , game id : {gameId} , session id : {sessionId}, round : {round.Id}, Win: {round.Win},  " +
                                    $"Round Status : Closed , Result : {spinDto.ToJSONString()}", game.Id, gameMath.Id, @operator.Id, round.Id);

                                    return spinDto;
                                }
                            }
                        }
                    }
                }
            }

            throw new NotImplementedException();
        }

        public async Task<IEnumerable<GameFeatureGetDto>> GetGameFeatures()
        {
            await _eventService.CreateEvent($"{Constants.EventNames.Get}", $"Game Features");

            var gameFeatures = await _pdxSlotsContext
                .GameFeatures
                .ToListAsync();

            return _mapper.Map<IEnumerable<GameFeatureGetDto>>(gameFeatures);
        }

        public async Task<GameFeatureGetDto> CreateGameFeature(GameFeaturePostDto gameFeaturePostDto)
        {
            await _eventService.CreateEvent($"{Constants.EventNames.Create}", $"Game Features - {gameFeaturePostDto.ToJSONString()}");
            var gameFeature = _mapper.Map<GameFeature>(gameFeaturePostDto);

            await _pdxSlotsContext.GameFeatures.AddAsync(gameFeature);
            await _pdxSlotsContext.SaveChangesAsync();

            return _mapper.Map<GameFeatureGetDto>(gameFeature);
        }

        public async Task<GameFeatureGetDto> UpdateGameFeature(int id, GameFeaturePutDto gameFeaturePutDto)
        {
            await _eventService.CreateEvent($"{Constants.EventNames.Update}", $"Game Features - {gameFeaturePutDto.ToJSONString()}");
            var gameFeature = await _pdxSlotsContext.GameFeatures.FirstOrDefaultAsync(x => x.Id == id);

            _mapper.Map(gameFeaturePutDto, gameFeature);

            await _pdxSlotsContext.SaveChangesAsync();

            return _mapper.Map<GameFeatureGetDto>(gameFeature);
        }

        public async Task<bool> DeleteGameFeature(int id)
        {
            await _eventService.CreateEvent($"{Constants.EventNames.Delete}", $"Game Features - {id}");
            var gameFeature = await _pdxSlotsContext.GameFeatures.FirstOrDefaultAsync(x => x.Id == id);


            _pdxSlotsContext.GameFeatures.Remove(gameFeature);


            return await _pdxSlotsContext.SaveChangesAsync() > 0;
        }
    }
}
