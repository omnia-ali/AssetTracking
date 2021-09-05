using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class UsersController : BaseApiController
    {
        private readonly DataContext _DataContext;
        public UsersController(DataContext DataContext)
        {
            _DataContext = DataContext;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            var users = await _DataContext.AppUsers.ToListAsync();
            return users;
        }

        [HttpGet("{id}")]
     
        public async Task<ActionResult<AppUser>> GetUser(int id)
        {
            return await _DataContext.AppUsers.FindAsync(id);
        }
    }
}