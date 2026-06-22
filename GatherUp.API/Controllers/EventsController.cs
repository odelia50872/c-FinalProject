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
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetEvent(int id)
        {
            var ev = _eventService.GetEventById(id);
            return Ok(ev);
        }

        [HttpGet("my-events")]
        public IActionResult GetMyEvents()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return Ok(new
            {
                asManager = _eventService.GetEventsByManager(userId),
                asHost = _eventService.GetEventsByHost(userId),
                asParticipant = _eventService.GetEventsByParticipant(userId)
            });
        }

        [HttpPost]
        public IActionResult CreateEvent([FromBody] CreateEventRequest request)
        {
            var managerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var ev = _eventService.CreateEvent(request.Title, request.Date, request.Location, managerId, request.HostId);
            return Ok(ev);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateEvent(int id, [FromBody] UpdateEventRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (!_eventService.IsManager(id, userId)) return Forbid();
            _eventService.UpdateEvent(id, request.Title, request.Date, request.Location);
            return Ok();
        }

        [HttpPost("{id}/participants/{participantId}")]
        public IActionResult AddParticipant(int id, int participantId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (!_eventService.IsManager(id, userId)) return Forbid();
            _eventService.AddParticipantToEvent(id, participantId);
            return Ok();
        }

        [HttpGet("{id}/participants")]
        public IActionResult GetParticipants(int id)
        {
            var ev = _eventService.GetEventById(id);
            var participants = _eventService.GetEventParticipants(id);
            return Ok(participants.Select(p =>
            {
                var data = ev.ParticipantData.FirstOrDefault(d => d.ParticipantId == p.Id);
                return new
                {
                    p.Id, p.Name, p.Email,
                    isAttending = data?.IsAttending,
                    hasPaid = data?.HasPaid ?? false,
                    amountContributed = data?.AmountContributed ?? 0m
                };
            }));
        }

        [HttpGet("all-users")]
        public IActionResult GetAllUsers()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var all = _eventService.GetAllParticipants().Where(p => p.Id != userId);
            return Ok(all.Select(p => new { p.Id, p.Name, p.Email }));
        }

        [HttpGet("{id}/participants/all")]
        public IActionResult GetAllParticipants(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (!_eventService.IsManager(id, userId)) return Forbid();
            var ev = _eventService.GetEventById(id);
            var all = _eventService.GetAllParticipants()
                .Where(p => p.Id != ev.EventManagerId
                         && p.Id != ev.EventHostId
                         && !ev.ParticipantIds.Contains(p.Id));
            return Ok(all.Select(p => new { p.Id, p.Name, p.Email }));
        }

        [HttpPost("{id}/send-host-invitation")]
        public IActionResult SendHostInvitation(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (!_eventService.IsManager(id, userId)) return Forbid();
            var mailService = HttpContext.RequestServices.GetRequiredService<GatherUp.core.interfaces.IMailService>();
            _eventService.SendHostInvitation(id, mailService);
            return Ok();
        }
    }
}
