namespace Todo.Identity.Domain.Exceptions
{
    public class EmailFormatException : DomainException
    {
        private const string CODE = "Identity.Domain.EmailFormat";
        public EmailFormatException(string message) : base(CODE, message)
        {

        }
    }
}
