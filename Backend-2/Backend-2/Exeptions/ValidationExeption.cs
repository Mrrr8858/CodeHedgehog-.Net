namespace Backend_2.Exeptions
{
    public class ValidationExeption : Exception
    {
        public ValidationExeption(string message) : base(message) { }
    }
}
