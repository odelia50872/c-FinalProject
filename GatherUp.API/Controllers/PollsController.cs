using GatherUp.API.DTOs;
using GatherUp.core.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GatherUp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PollsController : ControllerBase
    {
        private readonly IPollService _pollService;
        private readonly IEventService _eventService;

        public PollsController(IPollService pollService, IEventService eventService)
        {
            _pollService = pollService;
            _eventService = eventService;
        }

        [HttpPost("events/{eventId}")]
        public IActionResult CreatePoll(int eventId, [FromBody] CreatePollRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (!_eventService.IsManager(eventId, userId)) return Forbid();
            var poll = _pollService.CreatePoll(eventId, request.Name,
                request.Questions.Select(q => (q.QuestionText, q.Options)).ToList());
            return Ok(poll);
        }

        [HttpGet("{pollId}/results")]
        public IActionResult GetResults(int pollId)
        {
            return Ok(_pollService.GetPollResults(pollId));
        }

        [AllowAnonymous]
        [HttpGet("{pollId}/questions")]
        public IActionResult GetQuestions(int pollId)
        {
            return Ok(_pollService.GetPollQuestions(pollId));
        }

        [HttpPost("{pollId}/vote")]
        public IActionResult Vote(int pollId, [FromBody] VoteRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            _pollService.SubmitVote(pollId, request.QuestionId, userId, request.Answer);
            return Ok();
        }
    }
}
