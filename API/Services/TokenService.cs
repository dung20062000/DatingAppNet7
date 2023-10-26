using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key; // lưu khóa sử dụng để tạo hoặc xác minh mã thông báo xác thực token

        public TokenService(IConfiguration config)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }
        public string CreateToken(AppUser user)
        {
            var claims = new List<Claim> // tạo 1 danh sách xác nhận quyền 
            {
              new Claim(JwtRegisteredClaimNames.NameId, user.UserName) //đặt ds quyền phụ thuộc vào tên người dùng
            };
            
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature); // tạo thông tin chữ kí, và xác minh JWT

            var tokenDescriptor = new SecurityTokenDescriptor() // tạo thông tin mã token 
            {
                Subject = new ClaimsIdentity(claims),  // chủ đề của mã (ở đây là thông tin người dùng vd: username)
                Expires = DateTime.Now.AddDays(7), // thời gian hết hạn mã
                SigningCredentials = creds  // 
            };

            var tokenHandler = new JwtSecurityTokenHandler();  // tạo 1 object quản lý JWT.

            var token = tokenHandler.CreateToken(tokenDescriptor);  // tạo 1 mã token với thông tin đã định
            return tokenHandler.WriteToken(token); // trả về chuỗi JWT
        }
    }
}