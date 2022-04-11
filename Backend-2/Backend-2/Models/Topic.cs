namespace Backend_2.Models
{
    public class Topic
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
    }

    public class TopicDto
    {
        public string Name { get; set; }
        public int? ParentId { get; set; }
    }

    public class TopicWithChildsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public List<Topic> Childs { get; set; }
    }

}
