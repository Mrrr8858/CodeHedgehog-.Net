using Microsoft.AspNetCore.Mvc;
using Backend_2.Models;
using Backend_2.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Backend_2.Exeptions;

namespace Backend_2.Controllers
{
    [ApiController]
    public class SolutionsController : Controller
    {

        private static TestContext _context;
        public static void Initialize(IServiceProvider serviceProvider)
        {
            _context = new TestContext(serviceProvider.GetRequiredService<
                    DbContextOptions<TestContext>>());
        }
        SolutionSevice solutionService = new SolutionSevice(_context);

        [Route("tasks/{taskId}/solution")]
        [HttpPost]
        //[Authorize]
        public ActionResult<SolutionsGetDto[]> Post(SolutionsDto model, int taskId)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationExeption("Вы ввели некорректные данные");
            }

            User user = _context.User.FirstOrDefault(x => x.Username == User.Identity.Name);
            if (user is null)
            {
                throw new ForbiddenExeption("Вы должны быть авторизированы");
            }

            return Ok(solutionService.AddSolution(taskId, user.Id, model));
        }

        [Route("solutions")]
        [HttpGet]
        public ActionResult<SolutionsGetDto[]> Get()
        {
            return Ok(solutionService.Get());
        }

        [Route("solution/{solutionId}/posmoderation")]
        [HttpPost]
        //[Authorize(Roles = "admin")]
        public ActionResult<SolutionsGetDto[]> PostSolution(VerdictDto model, int solutionId)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationExeption("Вы ввели некорректные данные");
            }

            //solutionService.AddVerdict(solutionId, model);
            return Ok(solutionService.AddVerdict(solutionId, model));
        }
    }
}
