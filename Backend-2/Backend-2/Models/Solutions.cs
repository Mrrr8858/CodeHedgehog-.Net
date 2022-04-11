namespace Backend_2.Models
{
    public class Solutions
    {
        public int Id { get; set; }
        public string SourseCode { get; set; }
        public ProgrammingLanguage ProgrammingLanguage { get; set; }
        public Verdict Verdict { get; set; }
        public int AuthorId { get; set; }
        public int TaskId { get; set; }
    }

    public class SolutionsGetDto
    {
        public int Id { get; set; }
        public string SourseCode { get; set; }
        public string ProgrammingLanguage { get; set; }
        public string Verdict { get; set; }
        public int AuthorId { get; set; }
        public int TaskId { get; set; }
    }

    public class SolutionsDto
    {
        public string SourseCode { get; set; }
        public string ProgrammingLanguage { get; set; }
    }
    public class VerdictDto
    {
        public string Verdict { get; set; }
    }

    public enum ProgrammingLanguage
    {
        Python, Cpp, Csh, Java
    }
    public enum Verdict
    {
        Pending, OK, Rejected
    }
}
