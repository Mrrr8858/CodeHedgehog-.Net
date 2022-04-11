using Microsoft.AspNetCore.Mvc;
using Backend_2.Models;
using Backend_2.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Backend_2.Exeptions;
using Microsoft.AspNetCore.Hosting;

namespace Backend_2.Controllers
{
    [Route("tasks")]
    [ApiController]
    public class TaskController : Controller
    {
        IWebHostEnvironment _appEnvironment;
        ITaskService taskService;

        public TaskController(ITaskService service, IWebHostEnvironment appEnvironment)
        {
            taskService = service;
            _appEnvironment = appEnvironment;
        }

        [HttpGet]
        public ActionResult<TaskDto[]> Get(string? name, int? topic)
        {
            return Ok(taskService.GetTasks(name, topic));
        }

        [HttpGet("{taskId}")]
        public ActionResult<TaskByIdDto> Get(int taskId)
        {
            return Ok(taskService.GetTaskById(taskId));

        }


        [HttpDelete("{taskId}")]
        //[Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int taskId)
        {
            await taskService.DeleteTaskWithSave(taskId);
            return Ok();
        }

        [HttpPost]
        //[Authorize(Roles = "admin")]
        public ActionResult<TaskDto[]> Post(TaskPostDto model)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationExeption("Вы ввели некорректные данные");
            }

            //await taskService.Add(model);
            return Ok(taskService.Add(model));
        }

        [HttpPost("{taskId}/input")]
        //[Authorize(Roles = "admin")]
        public async Task<IActionResult> PostInput(int taskId, IFormFile uploadedFile)
        {
            if (uploadedFile != null)
            {
                string path = _appEnvironment.ContentRootPath + "/Files/Input/" + uploadedFile.FileName + "_" + taskId;
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                await taskService.AddInput(path, taskId);
                return Ok();
            }
            throw new ValidationExeption("Загрузите файл");
        }

        [HttpDelete("{taskId}/input")]
        //[Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteInput(int taskId)
        {
            await taskService.DeleteInput(taskId);
            return Ok();
        }

        [HttpGet("{taskId}/input")]
        //[Authorize(Roles = "admin")]
        public FileResult GetInput(int taskId)
        {
            var task = taskService.GetTasks(taskId);
            if(task.Input is null)
            {
                throw new ObjectNotFoundExeption("У этого задания нет input файлов");
            }
            byte[] mas = System.IO.File.ReadAllBytes(task.Input);
            string file_type = "application/txt";
            string file_name = "input.txt";
            return File(mas, file_type, file_name);
        }

        [HttpPost("{taskId}/output")]
        //[Authorize(Roles = "admin")]
        public async Task<IActionResult> PostOutput(int taskId, IFormFile uploadedFile)
        {
            if (uploadedFile != null)
            {
                string path = _appEnvironment.ContentRootPath + "/Files/Output/" + uploadedFile.FileName + "_" + taskId;
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                await taskService.AddOutput(path, taskId);
                return Ok();
            }
            throw new ValidationExeption("Загрузите файл");
        }

        [HttpDelete("{taskId}/output")]
        //[Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteOutput(int taskId)
        {
            await taskService.DeleteOutput(taskId);
            return Ok();
        }

        [HttpGet("{taskId}/output")]
        //[Authorize(Roles = "admin")]
        public FileResult GetOutput(int taskId)
        {
            var task = taskService.GetTasks(taskId);
            if (task.Output is null)
            {
                throw new ObjectNotFoundExeption("У этого задания нет output файлов");
            }
            byte[] mas = System.IO.File.ReadAllBytes(task.Output);
            string file_type = "application/txt";
            string file_name = "input.txt";
            return File(mas, file_type, file_name);
        }
    }
}
