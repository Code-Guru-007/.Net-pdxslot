using PdxSlots.API.Dtos.GameMaths;

namespace PdxSlots.API.Services.Interfaces
{
    public interface IGameMathsService
    {
        Task<GameMathsGetDto> CreateGameMath(GameMathPostDto gameMathPostDto);
        Task<GameMathsGetDto> EditGameMath(int id, GameMathPutDto gameMathPutDto);
        Task<GameMathsGetDto> GetGameMath(int id);
    }
}
