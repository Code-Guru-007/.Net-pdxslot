namespace PdxSlots.API.Dtos.Users
{
    public class AdminUserGetDto : UserGetDto
    {
        public bool IsAdmin { get; set; } = false;
    }
}
