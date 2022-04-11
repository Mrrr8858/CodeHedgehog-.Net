using Backend_2.Exeptions;
using Backend_2.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_2.Services
{
    public interface ITaskService
    {
        TaskDto[] GetTasks(string? name, int? topic);
        TaskByIdDto GetTaskById(int id);
        Task DeleteTask(int id);
        TaskByIdDto Add(TaskPostDto task);
        Task AddInput(string path, int id);
        Task DeleteInput(int id);
        Task AddOutput(string path, int id);
        Task DeleteOutput(int id);
        Tasks GetTasks(int id);
        Task DeleteTaskWithSave(int id);
    }
    public class TaskService : ITaskService
    {
        private readonly TestContext _context;
        public TaskService(TestContext context)
        {
            _context = context;
        }

        public TaskByIdDto Add(TaskPostDto task)
        {
            Tasks task1 = new Tasks
            {
                Name = task.Name,
                TopicId = task.TopicId,
                Discrinption = task.Discrinption,
                Price = task.Price,
                IsDraft = false,
            };
            _context.Tasks.Add(task1);
            _context.SaveChanges();
            return GetTaskById(task1.Id);
        }

        public async Task DeleteTask(int id)
        {
            var task = _context.Tasks.Find(id);
            if (task is null)
            {
                throw new ObjectNotFoundExeption("Такое задание не найдено");
            }
            if (task.Input is not null)
            {
                File.Delete(task.Input);
            }
            if (task.Output is not null)
            {
                File.Delete(task.Output);
            }
            _context.Tasks.Remove(task);
        }
        public async Task DeleteTaskWithSave(int id)
        {
             DeleteTask(id);
             _context.SaveChanges();
        }


        public TaskByIdDto GetTaskById(int id)
        {
            var task = _context.Tasks.Find(id);
            if (task is null)
            {
                throw new ObjectNotFoundExeption("Такое задание не найдено");
            }
            return new TaskByIdDto
            {
                Id = task.Id,
                Name = task.Name,
                TopicId = task.TopicId,
                Discrinption = task.Discrinption,
                Price = task.Price,
                IsDraft = task.IsDraft,
            };
        }

        private TaskDto[] HelpGet(Tasks[] tasks)
        {
            return tasks.Select(x => new TaskDto
            {
                Id = x.Id,
                Name = x.Name,
                TopicId = x.TopicId,
            }).ToArray();
        }

        public TaskDto[] GetTasks(string? name, int? topic)
        {
            TaskDto[] tasks;
            Tasks[] test;
            if (name != null && topic != null)
            {
                test = _context.Tasks.Where(x => x.TopicId == topic && x.Name == name).ToArray();
                tasks = HelpGet(test);
            }
            else if (name != null)
            {
                test = _context.Tasks.Where(x => x.Name == name).ToArray();
                tasks = HelpGet(test);
            }
            else if (topic != null)
            {
                test = _context.Tasks.Where(x => x.TopicId == topic).ToArray();
                tasks = HelpGet(test);
            }
            else
            {
                tasks = _context.Tasks.Select(x => new TaskDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    TopicId = x.TopicId,

                }).ToArray();
            }

            return tasks;
        }

        public async Task DeleteInput(int id)
        {
            var task = _context.Tasks.Find(id);
            if (task is null)
            {
                throw new ObjectNotFoundExeption("Такое задание не найдено");
            }
            if (task.Input is null)
            {
                throw new ObjectNotFoundExeption("Input файла у этого задания нет");
            }

            File.Delete(task.Input);
            task.Input = null;
            await _context.SaveChangesAsync();
        }

        public async Task AddInput(string path, int id)
        {
            var task = _context.Tasks.Find(id);
            if (task is null)
            {
                throw new ObjectNotFoundExeption("Такое задание не найдено");
            }
            if (task.Input is not null)
            {
                File.Delete(task.Input);
            }
            task.Input = path;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteOutput(int id)
        {
            var task = _context.Tasks.Find(id);
            if (task is null)
            {
                throw new ObjectNotFoundExeption("Такое задание не найдено");
            }
            if (task.Output is null)
            {
                throw new ObjectNotFoundExeption("Output файла у этого задания нет");
            }

            File.Delete(task.Output);
            task.Input = null;
            await _context.SaveChangesAsync();
        }

        public async Task AddOutput(string path, int id)
        {
            var task = _context.Tasks.Find(id);
            if (task is null)
            {
                throw new ObjectNotFoundExeption("Такое задание не найдено");
            }
            if (task.Output is not null)
            {
                File.Delete(task.Output);
            }
            task.Output = path;
            await _context.SaveChangesAsync();
        }
        public Tasks GetTasks(int id)
        {
            var task = _context.Tasks.Find(id);
            if (task is null)
            {
                throw new ObjectNotFoundExeption("Такое задание не найдено");
            }

            return task;
        }
    }
}
