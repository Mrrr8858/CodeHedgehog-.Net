using Backend_2.Exeptions;
using Backend_2.Models;

namespace Backend_2.Services
{
    public interface ISolutionSevice
    {
        SolutionsGetDto[] Get();
        SolutionsGetDto AddSolution(int taskId, int userId, SolutionsDto model);
        SolutionsGetDto AddVerdict(int id, VerdictDto verdict);
        SolutionsGetDto GetById(int id);
    }
    public class SolutionSevice : ISolutionSevice
    {
        private readonly TestContext _context;
        public SolutionSevice(TestContext context)
        {
            _context = context;
        }
        public SolutionsGetDto[] Get()
        {
            return _context.Solutions.Select(x => new SolutionsGetDto
            {
                Id = x.Id,
                SourseCode = x.SourseCode,
                ProgrammingLanguage = x.ProgrammingLanguage.ToString(),
                Verdict = x.Verdict.ToString(),
                AuthorId = x.AuthorId,
                TaskId = x.TaskId,

            }).ToArray();
        }

        public SolutionsGetDto GetById(int id)
        {
            Solutions solutions = _context.Solutions.Find(id);
            if (solutions != null)
            {
                return new SolutionsGetDto
                {
                    Id = solutions.Id,
                    SourseCode = solutions.SourseCode,
                    ProgrammingLanguage = solutions.ProgrammingLanguage.ToString(),
                    Verdict = solutions.Verdict.ToString(),
                    AuthorId = solutions.AuthorId,
                    TaskId = solutions.TaskId,
                };
            }
            else
            {
                throw new Exception();
            }
        }


        public SolutionsGetDto AddSolution(int taskId, int userId, SolutionsDto model)
        {
            Solutions solutions = new Solutions
            {
                SourseCode = model.SourseCode,
                ProgrammingLanguage = Language(model.ProgrammingLanguage),
                Verdict = Verdict.Pending,
                AuthorId = userId,
                TaskId = taskId,
            };
            _context.Solutions.Add(solutions);
            _context.SaveChanges();
            return GetById(solutions.Id);
        }

        private ProgrammingLanguage Language(string language)
        {
            switch (language)
            {
                case "Python":
                    return ProgrammingLanguage.Python;
                case "C++":
                    return ProgrammingLanguage.Cpp;
                case "C#":
                    return ProgrammingLanguage.Csh;
                case "Java":
                    return ProgrammingLanguage.Java;
                default:
                    throw new ObjectNotFoundExeption($"Programming language {language} не найден");
            }
        }

        private Verdict GetVerdict(string verdict)
        {
            switch (verdict)
            {
                case "Pending":
                    return Verdict.Pending;
                case "OK":
                    return Verdict.OK;
                case "Rejected":
                    return Verdict.Rejected;
                default:
                    throw new ObjectNotFoundExeption($"Verdict {verdict} не найден");
            }
        }

        public SolutionsGetDto AddVerdict(int id, VerdictDto verdict)
        {
            Solutions solutions = _context.Solutions.Find(id);
            if (solutions is null)
            {
                throw new ObjectNotFoundExeption($"Решение с id = {id} не найдено");
            }

            solutions.Verdict = GetVerdict(verdict.Verdict);
             _context.SaveChanges();
            return GetById(solutions.Id);
        }
    }
}
