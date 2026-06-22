using GatherUp.API.DTOs;
using GatherUp.core.DO;
using GatherUp.core.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GatherUp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ParticipantsController : ControllerBase
    {
        private readonly IParticipantService _participantService;
        private readonly IEventService _eventService;

        public ParticipantsController(IParticipantService participantService, IEventService eventService)
        {
            _participantService = participantService;
            _eventService = eventService;
        }

        [HttpGet("{id}")]
        public IActionResult GetParticipant(int id)
        {
            var p = _participantService.GetParticipantById(id);
            // isAttending and hasPaid are now per-event; return only profile data here
            return Ok(new {
                p.Id, p.Name, p.Email, p.PhoneNumber,
                mailingPreferences = (int)p.MailingPreferences,
                role = p.Role.ToString()
            });
        }

        [HttpGet("{id}/event/{eventId}/status")]
        public IActionResult GetParticipantEventStatus(int id, int eventId)
        {
            var ev = _eventService.GetEventById(eventId);
            var data = ev.ParticipantData.FirstOrDefault(d => d.ParticipantId == id);
            return Ok(new
            {
                isAttending = data?.IsAttending,
                hasPaid = data?.HasPaid ?? false,
                amountContributed = data?.AmountContributed ?? 0m
            });
        }

        [HttpPost("{id}/confirm-attendance")]
        public IActionResult ConfirmAttendance(int id, [FromBody] ConfirmAttendanceRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (userId != id) return Forbid();
            _participantService.ConfirmAttendance(id, request.IsAttending);
            return Ok();
        }

        [HttpPut("{id}/mailing-preference")]
        public IActionResult UpdateMailingPreference(int id, [FromBody] MailingPreferenceRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (userId != id) return Forbid();
            _participantService.UpdateMailingPreference(id, (MailingPreference)request.Preference);
            return Ok();
        }

        [HttpPost("events/{eventId}/send-invitations")]
        public IActionResult SendInvitations(int eventId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (!_eventService.IsManager(eventId, userId)) return Forbid();
            _participantService.SendPendingInvitations(eventId);
            return Ok();
        }
    }
}
