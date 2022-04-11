using Microsoft.AspNetCore.Mvc;
using Backend_2.Models;
using Backend_2.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Backend_2.Exeptions;

namespace Backend_2.Controllers
{
    [Route("topics")]
    [ApiController]
    public class TopicsController : Controller
    {
        private static TestContext _context;
        public static void Initialize(IServiceProvider serviceProvider)
        {
            _context = new TestContext(serviceProvider.GetRequiredService<
                    DbContextOptions<TestContext>>());
        }


        TopicService topicService = new TopicService(_context);

        [HttpGet]
        public ActionResult<Topic[]> Get(string? name, int? parentId)
        {
            return Ok(topicService.GetTopics(name, parentId));
        }

        [HttpGet("{topicId}")]
        public ActionResult<TopicWithChildsDto> Get(int topicId)
        {
            return Ok(topicService.GetTopicById(topicId));
        }


        [HttpDelete("{topicId}")]
        //[Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int topicId)
        {
            topicService.DeleteTopic(topicId);
            return Ok();
        }

        [HttpPost]
        //[Authorize(Roles = "admin")]
        public ActionResult<TopicWithChildsDto> Post(TopicDto model)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationExeption("Вы ввели некорректные данные");
            }

            return Ok(topicService.Add(model));
        }

        [HttpGet("{topicId}/childs")]
        public ActionResult<Topic[]> GetChilds(int topicId)
        {
            return Ok(topicService.GetChilds(topicId));
        }

        [HttpPost("{topicId}/childs")]
        //[Authorize(Roles = "admin")]
        public ActionResult<TopicWithChildsDto> PostChilds(List<int> model, int topicId)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationExeption("Вы ввели некорректные данные");
            }

            return Ok(topicService.AddChilds(model, topicId));

        }
        [HttpDelete("{topicId}/childs")]
        //[Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteChilds(List<int> model, int topicId)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationExeption("Вы ввели некорректные данные");
            }
            topicService.DeleteChilds(model, topicId);
            return Ok();
        }
        [HttpPatch("{topicId}")]
        //[Authorize(Roles = "admin")]
        public ActionResult<TopicWithChildsDto> Patch(TopicDto model, int topicId)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationExeption("Вы ввели некорректные данные");
            }

            //topicService.Edit(model, topicId);
            return Ok(topicService.Edit(model, topicId));
        }
    }
}
