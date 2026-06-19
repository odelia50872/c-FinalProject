using GatherUp.core.DO;

namespace GatherUp.core.interfaces
{
    public interface IPollService
    {
        event Action<int>? OnPollCreated;
        event Action<int, int>? OnPollAnswerSubmitted;

        Poll CreatePoll(int eventId, string name, List<(string QuestionText, List<string> Options)> questions);
        void SubmitVote(int pollId, int questionId, int participantId, string answer);
        IEnumerable<PollResultDTO> GetPollResults(int pollId);
    }
}
