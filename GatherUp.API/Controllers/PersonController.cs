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
    public class PersonController : ControllerBase
    {
        private readonly IPersonService _personService;

        public PersonController(IPersonService personService)
        {
            _personService = personService;
        }

        [HttpGet("{id}")]
        public IActionResult GetPerson(int id)
        {
            var person = _personService.GetById(id);
            return Ok(new { person.Id, person.Name, person.Email, person.PhoneNumber, role = person.Role.ToString() });
        }

        [HttpPut("{id}")]
        public IActionResult UpdatePerson(int id, [FromBody] UpdatePersonRequest request)
        {
            var callerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (callerId != id) return Forbid();
            var updated = _personService.UpdateParticipant(id, request.Name, request.Phone);
            return Ok(new { updated.Id, updated.Name, updated.Email, updated.PhoneNumber });
        }
    }
}
