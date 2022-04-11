namespace Backend_2.Exeptions
{
    public class ForbiddenExeption: Exception
    {
        public ForbiddenExeption(string message) : base(message) { }
    }
}
