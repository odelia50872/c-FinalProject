namespace GatherUp.core.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string entityName, int id)
            : base($"{entityName} with ID {id} was not found.") { }
    }

    public class ImmutableReceiptException : Exception
    {
        public ImmutableReceiptException()
            : base("A receipt is immutable and cannot be modified or deleted after creation.") { }
    }

    public class InvalidEventDataException : Exception
    {
        public InvalidEventDataException(string message) : base(message) { }
    }

    public class DuplicateUserException : Exception
    {
        public DuplicateUserException(string email)
            : base($"A user with email '{email}' already exists.") { }
    }

    public class AccessDeniedException : Exception
    {
        public AccessDeniedException(string message = "You are not authorized to perform this action.")
            : base(message) { }
    }
}
