namespace Backend_2.Models
{
    public class Tasks
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TopicId { get; set; }
        public string Discrinption { get; set; }
        public int Price { get; set; }
        public bool IsDraft { get; set; }
        public string? Input { get; set; }
        public string? Output { get; set; }
    }
    public class TaskByIdDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TopicId { get; set; }
        public string Discrinption { get; set; }
        public int Price { get; set; }
        public bool IsDraft { get; set; }
    }

    public class TaskDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TopicId { get; set; }
    }
    public class TaskPostDto
    {
        public string Name { get; set; }
        public int TopicId { get; set; }
        public string Discrinption { get; set; }
        public int Price { get; set; }
    }
}
