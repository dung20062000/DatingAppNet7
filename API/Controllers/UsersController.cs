using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class UsersController : BaseApiController
    {
        private readonly DataContext _context;

        //DataContext nó được truyền vào hàm tạo UsersController để tạo ra một lớp mới UsersController có thể được sử dụng DataContext để tương tác với dữ liệu.
        public UsersController(DataContext context) 
        {
            _context = context;
        }

        //ActionResult cung cấp khả năng trả về nhiều loại dữ liệu
        //IEnumerable là một trong những kiểu dữ liệu phổ biến để lưu trữ và truy cập dữ liệu, bao gồm danh sách, mảng, tập hợp, và nhiều cấu trúc dữ liệu khác
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return users;
        }

        [HttpGet("{id}")] //       /api/users/3
        public async Task<ActionResult<AppUser>> GetUser(int id) 
        {
            return await _context.Users.FindAsync(id);
        }
    }
}