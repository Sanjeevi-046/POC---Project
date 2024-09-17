using Microsoft.EntityFrameworkCore;
using POC.DataLayer.Models;
using Poc.CommonModel.Models;
using System.Security.Cryptography;
using AutoMapper;
using POC.CommonModel.Models;
using System.Net.Mail;
namespace POC.DataLayer.Repository
{
    public class LoginRepository : ILoginRepo
    {
        private readonly DemoProjectContext _context;
        private readonly IMapper _mapper;
        public LoginRepository(DemoProjectContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<UserValidationResult> GetUserIdAsync(string userName)
        {
            var userData = await _context.Logins.FirstOrDefaultAsync(u => u.Name == userName);
            if (userData != null)
            {
                return new UserValidationResult { IsValid = true, Message = userData.Id.ToString() };
            }
            return new UserValidationResult { IsValid = false, Message = "Name not found" };
        }
        public async Task<CommonLoginModel> GetLoginUserAsync(string userId)
        {
            var userData = await _context.Logins.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
            if (userData != null)
            {
                return _mapper.Map<CommonLoginModel>(userData);
            }
            else
            {
                return null;
            }
        }

        public  async Task<UserValidationResult> ValidateUserAsync(string name, string password)
        {
            var userData = await _context.Logins.FirstOrDefaultAsync(c => c.Name == name);
            if (userData == null)
            {
                return new UserValidationResult { IsValid = false, Message = "The user name or password provided is incorrect." };
            }
            if (userData.Password == password)
            {
                return new UserValidationResult { IsValid = true, Message = "Login successful.", Mail = userData.Email, Role = userData.Role, userId = userData.Id };
            }
            else
            {
                return new UserValidationResult { IsValid = false, Message = "The user name or password provided is incorrect." };
            }
        }

        public async Task<UserValidationResult> RegisterUserAsync(CommonLoginModel login, string rePassword)
        {
            var userdetail = new User();
            var LoginData = _mapper.Map<Login>(login);
            var userData = await _context.Logins.FirstOrDefaultAsync(u => u.Name == LoginData.Name);
            LoginData.Role = "Customer";
            if (userData == null)
            {
                if (rePassword == login.Password)
                {
                    _context.Logins.Add(LoginData);
                    userdetail.Email = LoginData.Email;
                    userdetail.Name = LoginData.Name;
                    userdetail.UserId = login.Id;
                    _context.Users.Add(userdetail);                   
                    await _context.SaveChangesAsync();
                    var smtpClient = new SmtpClient("smtp.gmail.com", 587)
                    {
                        Credentials = new System.Net.NetworkCredential("sanjeevivenkatachalapathy@gmail.com", "daol sakx jqvg nrgg"), 
                        EnableSsl = true
                    };
                    //smtpClient.UseDefaultCredentials = true;
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("sanjeevivenkatachalapathy@gmail.com"),
                        Subject = "Testing out email sending",
                        Body = "<b>Successfully Registered From App Store.</b>",
                        IsBodyHtml = true,
                    };

                    mailMessage.To.Add(new MailAddress(LoginData.Email, LoginData.Name));
                    try
                    {
                        smtpClient.Send(mailMessage);
                    }
                    catch (Exception ex) {
                        Console.WriteLine("Mail send Unsuccessful ",ex);
                        return new UserValidationResult { IsValid = true, Message = "Registered successfully.", Mail = "Mail was Not Send" };
                    }
                    
                    return new UserValidationResult { IsValid = true, Message = "Registered successfully.",Mail="Mail was Created Successfully" };
                }
                else
                {
                    return new UserValidationResult { IsValid = false, Message = "Password miss match." };
                }
            }
            else
            {
                return new UserValidationResult { IsValid = false, Message = "The User Name is already registered." };
            }
        }
        public async Task<CommonRefereshToken> GetRefreshTokenAsync(int userId)
        {
            var TokenId = await _context.Refreshtokens.FirstOrDefaultAsync(u => u.Id == userId);
            if (TokenId == null)
            {
                var refreshToken = new Refreshtoken
                {
                    RefreshToken1 = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                    ExpirationTime = DateTime.UtcNow.AddDays(1),
                    UserId = userId
                };
                _context.Refreshtokens.Add(refreshToken);
                await _context.SaveChangesAsync();

                return _mapper.Map<CommonRefereshToken>(refreshToken);
            }
            else
            {
                return null;
            }
        }
        public async Task<CommonRefereshToken> ValidateRefreshTokenAsync(string token)
        {
            var refreshToken = await _context.Refreshtokens
                .FirstOrDefaultAsync(t => t.RefreshToken1 == token && t.ExpirationTime > DateTime.UtcNow);

            return _mapper.Map<CommonRefereshToken>(refreshToken);
        }
        public async Task<UserValidationResult> InvalidateRefreshTokenAsync(string token)
        {
            var refreshToken = await _context.Refreshtokens.FirstOrDefaultAsync(t => t.RefreshToken1 == token);
            if (refreshToken != null)
            {
                _context.Refreshtokens.Remove(refreshToken);
                await _context.SaveChangesAsync();
                return new UserValidationResult { IsValid = true };
            }
            return new UserValidationResult { IsValid = false };
        }

    }
}
