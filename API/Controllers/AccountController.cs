using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(DataContext context, ITokenService tokenService )
        {
            _tokenService = tokenService;
            _context = context;
        }
        [HttpPost("register")] // /api/account/register
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if(await UserExists(registerDto.UserName)) return BadRequest("Username is taken");

            using var hmac = new HMACSHA512();  
            var user = new AppUser
            {
                UserName = registerDto.UserName.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key //má khóa để giải mã của PasswordHash
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == loginDto.Username);
            if(user == null) return Unauthorized("invalid Username");

            // mã  hóa mật khẩu khi đăng nhâp và so sánh với trong DB
            using var hmac = new HMACSHA512(user.PasswordSalt); //sử dụng khóa Key lưu trong BD để giải mã
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password)); // mk người dùng nhập vào được păm với cùng 1 loại mã khóa Key trước đó là hmac
            for(int i=0; i< computedHash.Length; i++)
            {
                if(computedHash[i] != user.PasswordHash[i]) return Unauthorized("invalid Password");

            }

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };

        }

        //UserExists lấy dl từ db so danh với username người dùng nhập đã có > true , chưa có > false
        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }

    }
}