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
    public class FinancialController : ControllerBase
    {
        private readonly IFinancialService _financialService;
        private readonly IEventService _eventService;

        public FinancialController(IFinancialService financialService, IEventService eventService)
        {
            _financialService = financialService;
            _eventService = eventService;
        }

        [HttpGet("events/{eventId}/summary")]
        public IActionResult GetSummary(int eventId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (!_eventService.IsManager(eventId, userId)) return Forbid();
            var summary = _financialService.GetFinancialSummary(eventId);
            return Ok(new
            {
                summary.TotalIncome,
                summary.TotalOutgoing,
                summary.Balance,
                PaidParticipants = summary.PaidParticipants.Select(p => new { p.Participant.Id, p.Participant.Name, p.AmountContributed }),
                Vendors = summary.Vendors.Select(v => new { v.Id, v.Name, v.AmountOwed })
            });
        }

        [HttpGet("events/{eventId}/receipts")]
        public IActionResult GetReceipts(int eventId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (!_eventService.IsManager(eventId, userId)) return Forbid();
            var receipts = _financialService.GetAllReceiptsSorted(eventId);
            return Ok(receipts.Select(r => new { receiptNumber = r.ReceiptNumber, amount = r.Amount }));
        }

        [HttpPost("participants/{participantId}/payment")]
        public IActionResult RegisterPayment(int participantId, [FromBody] PaymentRequest request)
        {
            var ev = _eventService.GetEventByParticipantId(participantId);
            if (ev != null)
            {
                var data = ev.ParticipantData.FirstOrDefault(d => d.ParticipantId == participantId);
                if (data?.IsAttending != true)
                    return BadRequest(new { error = "Participant must confirm attendance before payment can be registered." });
            }
            _financialService.RegisterPayment(participantId, request.Amount);
            return Ok();
        }

        [HttpPut("events/{eventId}/vendors/{vendorId}/amount")]
        public IActionResult SetVendorAmount(int eventId, int vendorId, [FromBody] AmountRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (!_eventService.IsManager(eventId, userId)) return Forbid();
            _financialService.SetVendorAmount(eventId, vendorId, request.Amount);
            return Ok();
        }

        [HttpPost("events/{eventId}/send-payment-reminders")]
        public IActionResult SendPaymentReminders(int eventId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (!_eventService.IsManager(eventId, userId)) return Forbid();
            var count = _financialService.SendPaymentReminders(eventId);
            return Ok(new { count, message = count == 0 ? "All participants have already paid!" : $"{count} reminder(s) sent." });
        }
    }
}
