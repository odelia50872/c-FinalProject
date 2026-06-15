using GatherUp.core.DO;
using GatherUp.core.interfaces;

namespace GatherUp.BL.Services
{
    public class PollService : IPollService
    {
        private readonly IRepository<Poll> _pollRepo;
        private readonly IRepository<Event> _eventRepo;
        private readonly IMailService _mailService;
        private readonly IEventService _eventService;

        public PollService(
            IRepository<Poll> pollRepo,
            IRepository<Event> eventRepo,
            IMailService mailService,
            IEventService eventService)
        {
            _pollRepo = pollRepo;
            _eventRepo = eventRepo;
            _mailService = mailService;
            _eventService = eventService;
        }

        public Poll CreatePoll(int eventId, string name, List<(string QuestionText, List<string> Options)> questions)
        {
            var ev = _eventRepo.GetById(eventId) ?? throw new Exception("אירוע לא נמצא");

            var newId = _pollRepo.GetAll().Any() ? _pollRepo.GetAll().Max(p => p.Id) + 1 : 1;
            var poll = new Poll { Id = newId, Name = name };

            poll.Questions.AddRange(questions.Select((q, i) => new PollQuestion
            {
                Id = newId * 100 + i,
                QuestionText = q.QuestionText,
                Options = q.Options
            }));

            _pollRepo.Add(poll);
            ev.PollIds.Add(poll.Id);
            _eventRepo.Update(ev);

            _eventService.RaisePollCreated(poll.Id);
            return poll;
        }

        public void SubmitVote(int pollId, int questionId, int participantId, string answer)
        {
            var poll = _pollRepo.GetById(pollId) ?? throw new Exception("סקר לא נמצא");
            var question = poll.Questions.FirstOrDefault(q => q.Id == questionId)
                ?? throw new Exception("שאלה לא נמצאה");

            var existing = question.Responses.FirstOrDefault(r => r.ParticipantId == participantId);
            if (existing != null)
                question.Responses.Remove(existing);

            question.Responses.Add(new PollResponse { ParticipantId = participantId, Response = answer });
            _pollRepo.Update(poll);

            var ev = _eventRepo.GetAll().FirstOrDefault(e => e.PollIds.Contains(pollId));
            if (ev != null)
                _eventService.RaisePollAnswerSubmitted(ev.Id, pollId);
        }

        public IEnumerable<PollResultDTO> GetPollResults(int pollId)
        {
            var poll = _pollRepo.GetById(pollId) ?? throw new Exception("סקר לא נמצא");

            return poll.Questions.Select(q =>
            {
                int total = q.Responses.Count;
                return new PollResultDTO
                {
                    QuestionText = q.QuestionText,
                    Results = q.Options.Select(opt => new OptionResultDTO
                    {
                        Option = opt,
                        Votes = q.Responses.Count(r => r.Response == opt),
                        Percentage = total == 0 ? 0 : Math.Round(q.Responses.Count(r => r.Response == opt) * 100.0 / total, 1)
                    })
                };
            });
        }
    }
}
