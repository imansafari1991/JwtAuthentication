using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using jwt_authentication.Models;
using jwt_authentication.Models.Context;
using jwt_authentication.ViewModels;
using System.Security.Cryptography;
using jwt_authentication.Utilities;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace jwt_authentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly IUserService _userService;

        public UsersController(MyDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }
        

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }
        [HttpPost("Login")]
        public IActionResult Login(LoginViewModel model)
        {
            var response = _userService.Authenticate(model);

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(string id, User user)
        {
            //if (id != user.Id)
            //{
            //    return BadRequest();
            //}

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                //if (!UserExists(id))
                //{
                //    return NotFound();
                //}
                //else
                //{
                //    throw;
                //}
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("SignUp")]
        public async Task<ActionResult<User>> SignUp(SignUpViewModels signUpViewModels)
        {
            var hashsalt = Security.EncryptPassword(signUpViewModels.Password);

            User user = new User
            {
                FirstName = signUpViewModels.FirstName,
                LastName = signUpViewModels.LastName,
                CreateDate = DateTime.Now.Date,
                Email = signUpViewModels.Email,
                Mobile = signUpViewModels.Mobile,
                UserName = signUpViewModels.UserName,
                Password = hashsalt.Hash,
                StoredSalt = hashsalt.Salt,


            };


            await _context.Users.AddAsync(user);


            if (UserExists(user.UserName, user.Email))
            {
                return Conflict("This user is AllReady SignedUp");
            }
            else
            { 
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }



        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(string userName, string Email)
        {
            return _context.Users.Any(e => e.UserName == userName || e.Email == Email);
        }


    }
}
