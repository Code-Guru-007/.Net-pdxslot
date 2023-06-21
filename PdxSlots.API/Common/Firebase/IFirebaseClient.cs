using FirebaseAdmin.Auth;

namespace PdxSlots.API.Common.Firebase
{
    public interface IFirebaseClient
    {
        Task<FirebaseUser> GetFirebaseUser(string id);
        Task UpdateUserCustomClaims(string id, Dictionary<string, object> claims);
        Task<IEnumerable<UserRecord>> GetUsers(List<string> ids);
        Task<FirebaseUser> GetFirebaseUserByEmail(string email);
        Task<FirebaseUser> CreateFirebaseUser(string email);
        Task<string> GetPasswordResetLink(string email, string redirect);
    }
}
