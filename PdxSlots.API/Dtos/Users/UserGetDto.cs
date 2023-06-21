namespace PdxSlots.API.Dtos.Users
{
    public class UserGetDto
    {
        public int Id { get; set; }
        public string UserIdentityId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public DateTime Created { get; set; }
    }
}
