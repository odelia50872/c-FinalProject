using System.Xml.Serialization;

namespace GatherUp.core.DO
{
    public class PollResponse
    {
        public int ParticipantId { get; set; }
        public string Response { get; set; } = string.Empty;
    }

    public class PollQuestion
    {
        public PollQuestion()
        {
            Options = new List<string>();
            Responses = new List<PollResponse>();
        }

        public int Id { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public List<string> Options { get; set; }
        public List<PollResponse> Responses { get; set; }

        [XmlIgnore]
        public Dictionary<int, string> ParticipantResponses
        {
            get => Responses.ToDictionary(r => r.ParticipantId, r => r.Response);
            set => Responses = value.Select(kvp => new PollResponse { ParticipantId = kvp.Key, Response = kvp.Value }).ToList();
        }
    }
}
