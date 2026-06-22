namespace GatherUp.core.DO
{
    // PollResultDTO is produced by Poll — it wraps the poll's questions with calculated results.
    // Managed by PollService.GetPollResults() which calculates percentages per option.
    public class PollResultDTO
    {
        public string QuestionText { get; set; } = string.Empty;
        public IEnumerable<OptionResultDTO> Results { get; set; } = Enumerable.Empty<OptionResultDTO>();
    }

    public class OptionResultDTO
    {
        public string Option { get; set; } = string.Empty;
        public int Votes { get; set; }
        public double Percentage { get; set; }
    }
}
