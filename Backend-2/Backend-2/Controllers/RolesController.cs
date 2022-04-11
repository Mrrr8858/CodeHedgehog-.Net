using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Backend_2.Models;
using Backend_2.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Backend_2.Controllers
{
    [Route("roles")]
    [ApiController]
    public class RolesController : Controller
    {
        private static TestContext _context;
        public static void Initialize(IServiceProvider serviceProvider)
        {
            _context = new TestContext(serviceProvider.GetRequiredService<
                    DbContextOptions<TestContext>>());
        }

        RoleService _roleService = new RoleService(_context);

        [HttpGet]
        [Authorize]
        public Role[] Get()
        {
            return _roleService.GetRole();
        }
       /* [HttpPost]
        public async Task<IActionResult> Post()
        {
            await _roleService.AddRole();
            return Ok();
        }*/

        [HttpGet("{id}")]
        [Authorize]
        public Role Get(int id)
        {
            return _roleService.GetById(id);
        }

    }
}
