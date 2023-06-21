using AutoMapper;
using AutoWrapper.Wrappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PdxSlots.API.Common;
using PdxSlots.API.Common.Configurations;
using PdxSlots.API.Common.Firebase;
using PdxSlots.API.Data;
using PdxSlots.API.Dtos.Users;
using PdxSlots.API.Models;
using PdxSlots.API.Services.Interfaces;
using PdxSlots.Common;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;

namespace PdxSlots.API.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly PdxSlotsContext _pdxSlotsContext;
        private readonly IFirebaseClient _firebaseClient;
        private readonly PdxConfiguration _pdxConfiguration;
        private readonly ISendGridClient _sendGridClient;

        public UserService(ILogger<UserService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor, PdxSlotsContext pdxSlotsContext, IFirebaseClient firebaseClient, IOptions<PdxConfiguration> pdxConfiguration, ISendGridClient sendGridClient)
        {
            _logger = logger;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _pdxSlotsContext = pdxSlotsContext;
            _firebaseClient = firebaseClient;
            _pdxConfiguration = pdxConfiguration.Value;
            _sendGridClient = sendGridClient;
        }

        public async Task<UserGetDto> CreateMe(UserPostDto userPostDto)
        {
            var externalUserId = GetUserId();
            var email = GetEmail();
            if (string.IsNullOrWhiteSpace(externalUserId) || string.IsNullOrWhiteSpace(email))
            {
                _logger.LogError("Error getting Name Identifier, cannot validate user.");
                throw new ApiException("Error during User Validation process.", (int)HttpStatusCode.BadRequest);
            }

            var user = new User
            {
                Email = email,
                UserIdentityId = externalUserId,
                FirstName = userPostDto.FirstName,
                LastName = userPostDto.LastName,
                Title = userPostDto.Title,
            };

            await _pdxSlotsContext.Users.AddAsync(user);
            await _pdxSlotsContext.SaveChangesAsync();

            return _mapper.Map<UserGetDto>(user);
        }

        public async Task<UserGetDto> GetMe()
        {
            var userId = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await _pdxSlotsContext.Users
                .FirstOrDefaultAsync(x => x.UserIdentityId == userId);

            return _mapper.Map<UserGetDto>(user);
        }

        public async Task<IEnumerable<AdminUserGetDto>> GetUsers(string search)
        {
            IList<User> users = new List<User>();

            if (!string.IsNullOrEmpty(search))
            {
                users = await _pdxSlotsContext
                    .Users
                    .Where(x => x.Email.Contains(search) || x.FirstName.Contains(search) || x.LastName.Contains(search))
                    .ToListAsync();
            }
            else
            {
                users = await _pdxSlotsContext
                    .Users
                    .ToListAsync();
            }

            var firebaseIds = users.Select(x => x.UserIdentityId).ToList();

            var firebaseUsers = await _firebaseClient.GetUsers(firebaseIds);
            var mappedUsers = _mapper.Map<IEnumerable<AdminUserGetDto>>(users);

            var firebaseMappedUsers = new List<AdminUserGetDto>();
            foreach (var user in mappedUsers)
            {
                var firebaseUser = firebaseUsers.FirstOrDefault(x => x.Uid == user.UserIdentityId);
                if (firebaseUser != null)
                {
                    if (firebaseUser.CustomClaims.ContainsKey(Constants.Firebase_Claims_IsAdmin))
                        user.IsAdmin = (bool)firebaseUser.CustomClaims[Constants.Firebase_Claims_IsAdmin];

                    firebaseMappedUsers.Add(user);
                }
            }

            return firebaseMappedUsers;
        }

        public async Task<bool> UpdateUser(int id, AdminUserPutDto adminUserPutDto)
        {
            var user = await _pdxSlotsContext.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user != null)
            {
                var firebaseUser = await _firebaseClient.GetFirebaseUser(user.UserIdentityId);
                if (firebaseUser != null)
                {
                    var claims = new Dictionary<string, object>(firebaseUser.CustomClaims);
                    claims[Constants.Firebase_Claims_IsAdmin] = adminUserPutDto.IsAdmin;

                    await _firebaseClient.UpdateUserCustomClaims(firebaseUser.Uid, claims);

                    return true;
                }
            }

            throw new ApiException("There was an issue updating the corresponding user.", (int)HttpStatusCode.NotFound);
        }

        public async Task<AdminUserGetDto> InviteUser(UserInviteDto userInviteDto)
        {
            var newFirebaseUser = await _firebaseClient.CreateFirebaseUser(userInviteDto.Email);
            var newUser = new User
            {
                Email = userInviteDto.Email,
                UserIdentityId = newFirebaseUser.Uid,
                FirstName = userInviteDto.FirstName,
                Title = userInviteDto.Title,
                LastName = userInviteDto.LastName,
            };

            await _pdxSlotsContext.Users.AddAsync(newUser);
            await _pdxSlotsContext.SaveChangesAsync();

            // Get password reset link
            var passwordResetLink = await _firebaseClient.GetPasswordResetLink(userInviteDto.Email, $"{_pdxConfiguration.WebAppBaseUrl}authentication/login?returnUrl=%2Frgs%2Frounds");
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("pdxslots@savagesoftwarecorp.com", "PdxSlots"),
                Subject = $"You've Been Invited - Set Your Password"
            };

            msg.AddContent(MimeType.Text, $"You have been invited to PdxSlots. Please click the following link to set your password:\r\n{passwordResetLink}");
            msg.AddTo(new EmailAddress(userInviteDto.Email));

            var response = await _sendGridClient.SendEmailAsync(msg).ConfigureAwait(false);

            return _mapper.Map<AdminUserGetDto>(newUser);
        }


        public string GetUserId()
        {
            return _httpContextAccessor.HttpContext.User.GetUserId();
        }

        public string GetEmail()
        {
            return _httpContextAccessor.HttpContext.User.GetEmail();
        }
    }
}
