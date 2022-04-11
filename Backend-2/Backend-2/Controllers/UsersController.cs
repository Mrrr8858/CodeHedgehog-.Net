using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Backend_2.Models;
using Backend_2.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Backend_2.Exeptions;

namespace Backend_2.Controllers
{
    [Route("users")]
    [ApiController]
    public class UsersController : Controller
    {
        private static TestContext _context;
        public static void Initialize(IServiceProvider serviceProvider)
        {
            _context = new TestContext(serviceProvider.GetRequiredService<
                    DbContextOptions<TestContext>>());
        }


        UserService userService = new UserService(_context);

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult<UserDto[]> Get()
        {
            return Ok(userService.GetUser());
        }

        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<UserByIdDto> Get(int id)
        {
            User user = _context.User.FirstOrDefault(x => x.Username == User.Identity.Name);
            if (user is null)
            {
                throw new ObjectNotFoundExeption("Такой пользователь не найден");
            }
            if (user.Id == id || User.IsInRole("admin"))
            {
                return Ok(userService.GetUserById(id));
            }
            else
            {
                throw new ForbiddenExeption("Вы не можете посмотреть информацию о другом пользователе");
            }

        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await userService.DeleteUser(id);
            return Ok();
        }

        [HttpPost("{id}/role")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Post(UserRoleDto model, int id)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationExeption("Вы ввели некорректные данные");
            }

            await userService.AddRole(model, id);
            return Ok();
        }

        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> Patch(UserRegDto model, int id)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationExeption("Вы ввели некорректные данные");
            }
            var user = _context.User.FirstOrDefault(x => x.Name == User.Identity.Name);
            if (user.Id == id || User.IsInRole("admin"))
            {
                await userService.Edit(model, id);
                return Ok(userService.GetUserById(id));
            }
            else
            {
                throw new ForbiddenExeption("Вы не можете изменять информацию о другом пользователе");
            }

        }

    }
}
