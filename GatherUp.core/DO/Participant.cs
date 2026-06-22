namespace GatherUp.core.DO
{
    // Participant inherits from Person (Id, Name, Email, PhoneNumber, Password)
    // and adds event-specific fields: attendance, payment, mailing preferences and role.
    public class Participant : Person
    {
        public Participant() : base() { }
        public bool? IsAttending { get; set; }
        public bool HasPaid { get; set; }
        public decimal AmountContributed { get; set; }
        public MailingPreference MailingPreferences { get; set; }
        public UserRole Role { get; set; } = UserRole.Participant;
    }
}
