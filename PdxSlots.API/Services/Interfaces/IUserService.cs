using PdxSlots.API.Dtos.Users;

namespace PdxSlots.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserGetDto> CreateMe(UserPostDto userPostDto);
        Task<UserGetDto> GetMe();
        Task<IEnumerable<AdminUserGetDto>> GetUsers(string search);
        Task<bool> UpdateUser(int id, AdminUserPutDto adminUserPutDto);
        Task<AdminUserGetDto> InviteUser(UserInviteDto userInviteDto);
    }
}
