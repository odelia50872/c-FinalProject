namespace GatherUp.core.DO
{
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
