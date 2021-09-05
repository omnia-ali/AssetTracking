using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AuthController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        public AuthController(DataContext context, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(UserForRegisterDto UserForRegisterDto)
        {
            if (await UserExists(UserForRegisterDto.Username))
                return BadRequest("Username already exists");
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(UserForRegisterDto.Password, out passwordHash, out passwordSalt);
            var user = new AppUser
            {
                Name= UserForRegisterDto.Username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };
            await _context.AppUsers.AddAsync(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                UserName = user.Name,
                token = _tokenService.CreateToken(user)
            };
        }


        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private async Task<bool> UserExists(string username)
        {
            if (await _context.AppUsers.AnyAsync(x => x.Name == username))
                return true;

            return false;
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(UserForLoginDto userForLoginDto)
        {

            var user = await _context.AppUsers.FirstOrDefaultAsync(x => x.Name == userForLoginDto.Username);

            if (user == null)
                return Unauthorized("Invalid user name");

            if (!VerifyPasswordHash(userForLoginDto.Password, user.PasswordHash, user.PasswordSalt))
                return Unauthorized("Invalid password");


            return new UserDto
            {
                UserName = user.Name,
                token = _tokenService.CreateToken(user)
            };
            
        }
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i]) return false;
                }
            }
            return true;
        }
    }
}