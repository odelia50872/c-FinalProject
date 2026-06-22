namespace GatherUp.core.DO
{
    [Flags]
    public enum MailingPreference
    {
        None = 0,
        AttendanceConfirmation = 1,
        PaymentConfirmation = 2,
        PollResponses = 4
    }

    public enum UserRole { Participant, Manager, Host }
}
