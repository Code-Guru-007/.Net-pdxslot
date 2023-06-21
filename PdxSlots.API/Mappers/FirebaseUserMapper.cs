using AutoMapper;
using FirebaseAdmin.Auth;
using PdxSlots.API.Common.Firebase;

namespace PdxSlots.API.Mappers
{
    public class FirebaseUserMapper : Profile
    {
        public FirebaseUserMapper()
        {
            CreateMap<UserRecord, FirebaseUser>();
        }
    }
}
