using PdxSlots.API.Dtos.IGCUserGaf;

namespace PdxSlots.API.Services.Interfaces
{
    public interface IIgcUserGafService
    {
        Task<IEnumerable<IgcUserGafGetDto>> GetUserGafs();
        Task<IgcUserGafGetDto> UpdateUserGaf(int id, IgcUserGafPutDto igcUserGafPutDto);
        Task<IgcUserGafGetDto> CreateUserGaf(IgcUserGafPostDto igcUserGafPostDto);
        Task<bool> DeleteUserGaf(int id);
    }
}
