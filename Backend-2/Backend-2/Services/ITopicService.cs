using Backend_2.Exeptions;
using Backend_2.Models;

namespace Backend_2.Services
{
    public interface ITopicService
    {
        Topic[] GetTopics(string? name, int? parentId);
        TopicWithChildsDto Add(TopicDto user);
        TopicWithChildsDto GetTopicById(int id);
        Task DeleteTopic(int id);
        Topic[] GetChilds(int id);
        TopicWithChildsDto AddChilds(List<int> nChilds, int id);
        Task DeleteChilds(List<int> nChilds, int id);
        TopicWithChildsDto Edit(TopicDto topic, int id);
    }
    public class TopicService : ITopicService
    {
        private readonly TestContext _context;
        private readonly ITaskService _taskService;
        public TopicService(TestContext context)
        {
            _context = context;
            _taskService = new TaskService(context);
        }

        public TopicWithChildsDto Add(TopicDto topic)
        {
            Topic topic1 = new Topic
            {
                Name = topic.Name,
                ParentId = topic.ParentId,
            };
            _context.Topics.Add(topic1);
            _context.SaveChanges();

            if (topic1.ParentId != null)
            {
                return GetTopicById((int)topic1.ParentId);
            }
            else
            {
                return GetTopicById(topic1.Id);
            }
        }

        public Topic[] GetTopics(string? name, int? parentId)
        {
            Topic[] topics;
            if (name != null && parentId != null)
            {
                topics = _context.Topics.Where(x => x.ParentId == parentId && x.Name == name).ToArray();
            }
            else if (name != null)
            {
                topics = _context.Topics.Where(x => x.Name == name).ToArray();
            }
            else if (parentId != null)
            {
                topics = _context.Topics.Where(x => x.ParentId == parentId).ToArray();
            }
            else
            {
                topics = _context.Topics.Select(x => new Topic
                {
                    Id = x.Id,
                    Name = x.Name,
                    ParentId = x.ParentId,

                }).ToArray();
            }
            if (topics is not null)
            {
                return topics;
            }
            else
            {
                throw new ObjectNotFoundExeption("Такой топик не найден");
            }

        }
        public TopicWithChildsDto Edit(TopicDto topic, int id)
        {
            Topic topic1 = _context.Topics.Find(id);
            if (topic is null)
            {
                throw new ObjectNotFoundExeption($"Топика с id = {id} не существует");
            }
            topic1.Name = topic.Name;
            topic1.ParentId = topic.ParentId;
            _context.SaveChanges();
            return GetTopicById(topic1.Id);
        }
       
        public TopicWithChildsDto GetTopicById(int id)
        {
            var topic = _context.Topics.Find(id);
            if (topic == null)
            {
                throw new ObjectNotFoundExeption($"Топик с id = {id} не найден");
            }

            List<Topic> childs = _context.Topics.Where(x => x.ParentId == id).ToList();
            return new TopicWithChildsDto
            {
                Id = topic.Id,
                Name = topic.Name,
                ParentId = topic.ParentId,
                Childs = childs
            };
        }

        public Topic[] GetChilds(int id)
        {
            return _context.Topics.Where(x => x.ParentId == id).ToArray();
        }

        public TopicWithChildsDto AddChilds(List<int> nChilds, int id)
        {
            if (_context.Topics.Find(id) is null)
            {
                throw new ObjectNotFoundExeption($"Топик с id = {id} не найден");
            }
            foreach (var child in nChilds)
            {
                Topic topic = _context.Topics.Find(child);
                if (topic is null)
                {
                    throw new ObjectNotFoundExeption($"Топик с id = {id} не найден");
                }
                topic.ParentId = id;
                _context.SaveChanges();
            }
            return GetTopicById(id);
        }

        public async Task DeleteChilds(List<int> nChilds, int id)
        {
            if (_context.Topics.Find(id) is null)
            {
                throw new ObjectNotFoundExeption($"Топик с id = {id} не найден");
            }
            foreach (var child in nChilds)
            {
                DeleteTopicRec(child);
                _context.SaveChanges();
            }

        }
        public async Task DeleteTopic(int id)
        {
             DeleteTopicRec(id);
            _context.SaveChanges();
        }

        public async Task DeleteTopicRec(int id)
        {
            Topic topic = _context.Topics.Find(id);
            if (topic is null)
            {
                throw new ObjectNotFoundExeption($"Топика с id = {id} не существует");
            }
            List<Tasks> tasks = _context.Tasks.Where(task => task.TopicId == id).ToList();
            foreach (var task in tasks)
            {
                await _taskService.DeleteTask(task.Id);
            }
            List<Topic> childs = _context.Topics.Where(x => x.ParentId == id).ToList();
            foreach (Topic child in childs)
            {
                await DeleteTopicRec(child.Id);
            }

            _context.Topics.Remove(topic);

        }
    }
}
