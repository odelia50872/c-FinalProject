using GatherUp.core.DO;

namespace GatherUp.core.interfaces
{
    public interface IPollService
    {
        Poll CreatePoll(int eventId, string name, List<(string QuestionText, List<string> Options)> questions);
        void SubmitVote(int pollId, int questionId, int participantId, string answer);
        IEnumerable<PollResultDTO> GetPollResults(int pollId);
    }
}
