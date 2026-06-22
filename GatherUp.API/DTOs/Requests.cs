using GatherUp.core.DO;

namespace GatherUp.API.DTOs
{
    // Auth
    public record LoginRequest(string Email, string Password);
    public record RegisterRequest(string Name, string Email, string Phone, string Password);

    // Events
    public record CreateEventRequest(string Title, DateTime Date, string Location, int HostId);
    public record UpdateEventRequest(string Title, DateTime Date, string Location);

    // Participants
    public record ConfirmAttendanceRequest(bool IsAttending);
    public record MailingPreferenceRequest(int Preference);

    // Financial
    public record PaymentRequest(decimal Amount);
    public record AmountRequest(decimal Amount);

    // Vendors
    public record AddVendorRequest(string Name, decimal AmountOwed);
    public record AddReceiptRequest(decimal Amount, DateTime Date);

    // Polls
    public record CreatePollRequest(string Name, List<QuestionRequest> Questions);
    public record QuestionRequest(string QuestionText, List<string> Options);
    public record VoteRequest(int QuestionId, string Answer);

    // Person
    public record UpdatePersonRequest(string Name, string Phone);
}
