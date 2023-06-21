using PdxSlots.API.Dtos.GameFeatures;
using PdxSlots.API.Dtos.Games;

namespace PdxSlots.API.Services.Interfaces
{
    public interface IGamesService
    {
        Task<string> OpenGame(string userId, string operatorId, string gameId, string sessionId, bool mobileOverride);
        Task<InitializeGameGetDto> InitializeGame(string userId, string operatorId, string gameId, string sessionId);
        Task<SpinGameGetDto> SpinGame(string userId, string operatorId, string gameId, string sessionId, decimal denom, decimal wager);
        Task<IEnumerable<GameGetDto>> GetGames();
        Task<GameGetDto> CreateGame(GamePostDto gamePostDto);
        Task<GameGetDto> EditGame(int id, GamePutDto gamePutDto);
        Task<GameGetDto> CreateGameContent(int id, IFormFile zipContent, bool mobile);
        Task<GameGetDto> GetGame(int id);
        Task<GameGetDto> CreateGameMathContent(int id, int gameMathId, IFormFile zipContent);
        Task<IEnumerable<GameFeatureGetDto>> GetGameFeatures();
        Task<GameFeatureGetDto> CreateGameFeature(GameFeaturePostDto gameFeaturePostDto);
        Task<GameFeatureGetDto> UpdateGameFeature(int id, GameFeaturePutDto gameFeaturePutDto);
        Task<bool> DeleteGameFeature(int id);
    }
}
