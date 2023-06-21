using FirebaseAdmin.Auth;
using AutoMapper;

namespace PdxSlots.API.Common.Firebase
{
    public class FirebaseClient : IFirebaseClient
    {
        public FirebaseClient(IMapper mapper)
        {
            _mapper = mapper;
        }
        private readonly IMapper _mapper;

        public async Task<FirebaseUser> GetFirebaseUser(string id)
        {
            var user = await FirebaseAuth.DefaultInstance.GetUserAsync(id);
            return _mapper.Map<FirebaseUser>(user);
        }

        public async Task<FirebaseUser> GetFirebaseUserByEmail(string email)
        {
            var user = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email);

            return _mapper.Map<FirebaseUser>(user);
        }

        public async Task<FirebaseUser> CreateFirebaseUser(string email)
        {
            var user = await FirebaseAuth.DefaultInstance.CreateUserAsync(new UserRecordArgs { Email = email });

            return _mapper.Map<FirebaseUser>(user);
        }

        public async Task<string> GetPasswordResetLink(string email, string redirect)
        {
            return await FirebaseAuth.DefaultInstance.GeneratePasswordResetLinkAsync(email, new ActionCodeSettings { Url = redirect, HandleCodeInApp = false});
        }

        public async Task UpdateUserCustomClaims(string id, Dictionary<string, object> claims)
        {
            await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(id, claims);
        }

        public async Task<IEnumerable<UserRecord>> GetUsers(List<string> ids)
        {
            var split = ids.SplitList(100);

            List<UserRecord> users = new List<UserRecord>();

            foreach (var splitIDs in split)
            {
                var userIdentifiers = new List<UserIdentifier>();

                splitIDs.ForEach(id => userIdentifiers.Add(new UidIdentifier(id)));

                GetUsersResult result = await FirebaseAuth.DefaultInstance.GetUsersAsync(userIdentifiers);

                users.AddRange(result.Users);
            }

            return users;
        }
    }
}
