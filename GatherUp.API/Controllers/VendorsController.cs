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
    public class VendorsController : ControllerBase
    {
        private readonly IVendorService _vendorService;
        private readonly IEventService _eventService;

        public VendorsController(IVendorService vendorService, IEventService eventService)
        {
            _vendorService = vendorService;
            _eventService = eventService;
        }

        [HttpGet("events/{eventId}")]
        public IActionResult GetVendors(int eventId)
        {
            return Ok(_vendorService.GetVendors(eventId));
        }

        [HttpPost("events/{eventId}")]
        public IActionResult AddVendor(int eventId, [FromBody] AddVendorRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (!_eventService.IsManager(eventId, userId)) return Forbid();
            var vendor = _vendorService.AddVendor(eventId, request.Name, request.AmountOwed);
            return Ok(vendor);
        }

        [HttpPost("events/{eventId}/{vendorId}/receipts")]
        public IActionResult AddReceipt(int eventId, int vendorId, [FromBody] AddReceiptRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (!_eventService.IsManager(eventId, userId)) return Forbid();
            var receiptNumber = $"REC-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";
            _vendorService.AddReceipt(eventId, vendorId, new ReceiptDetails(receiptNumber, request.Amount, request.Date));
            return Ok();
        }
    }
}
