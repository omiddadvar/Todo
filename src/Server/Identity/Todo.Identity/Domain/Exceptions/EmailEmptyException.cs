namespace Todo.Identity.Domain.Exceptions
{
    public class EmailEmptyException : DomainException
    {
        private const string CODE = "Identity.Domain.EmailEmpty";
        public EmailEmptyException(string message) : base(CODE, message)
        {

        }
    }
}
