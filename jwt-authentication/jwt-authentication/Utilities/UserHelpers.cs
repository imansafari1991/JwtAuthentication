using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using jwt_authentication.Models;
using jwt_authentication.Models.Context;
using jwt_authentication.ViewModels;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace jwt_authentication.Utilities
{
    public interface IUserService
    {
        LoginResponseViewModel Authenticate(LoginViewModel model);
        IEnumerable<User> GetAll();
        User GetById(int id);
    }

    public class UserService : IUserService
    {
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
      

        private readonly AppSettings _appSettings;
        private readonly MyDbContext _context;

        public UserService(IOptions<AppSettings> appSettings,MyDbContext context)
        {
            _appSettings = appSettings.Value;
            _context = context;
        }

        public LoginResponseViewModel Authenticate(LoginViewModel model)
        {
            
            var user = _context.Users.SingleOrDefault(x => x.UserName == model.UserName);

            bool isCorrectPassword = Security.VerifyPassword(model.Password, user.StoredSalt, user.Password);
            // return null if user not found
            if (user == null || !isCorrectPassword) return null;

            // authentication successful so generate jwt token
            var token = generateJwtToken(user);

            return new LoginResponseViewModel{Id = user.Id,UserName = user.UserName,FirstName = user.FirstName,LastName = user.LastName ,token = token};
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users;
        }

        public User GetById(int id)
        {
            return _context.Users.FirstOrDefault(x => x.Id == id);
        }

        // helper methods

        private string generateJwtToken(User user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
