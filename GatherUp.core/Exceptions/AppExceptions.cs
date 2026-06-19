namespace GatherUp.core.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string entityName, int id)
            : base($"{entityName} with ID {id} was not found.") { }
    }

    public class LockedReceiptException : Exception
    {
        public LockedReceiptException()
            : base("This receipt is locked and cannot be modified.") { }
    }

    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message) { }
    }

    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message = "You are not authorized to perform this action.")
            : base(message) { }
    }
}
