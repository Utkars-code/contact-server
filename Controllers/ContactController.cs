using contacts_server.Model;
using contacts_server.Repository;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace contacts_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IContactRepository _contactRepository;

        public ContactController(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        [HttpGet]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<ActionResult<IEnumerable<Contact>>> GetAllContacts()
        {
            var contacts = await _contactRepository.GetAllContactsAsync();
            return Ok(contacts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Contact>> GetContactById(int id)
        {
            var contact = await _contactRepository.GetContactByIdAsync(id);
            if (contact == null)
                return NotFound("Contact not found.");
            return Ok(contact);
        }

        [HttpPost]
        public async Task<ActionResult<Contact>> CreateContact([FromBody] Contact contact)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await _contactRepository.IsEmailUniqueAsync(contact.Email))
                return BadRequest("Email must be unique.");

            await _contactRepository.AddContactAsync(contact);
            return CreatedAtAction(nameof(GetContactById), new { id = contact.Id }, contact);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateContact(int id, [FromBody] Contact contact)
        {
            if (id != contact.Id)
                return BadRequest("Contact ID mismatch.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await _contactRepository.IsEmailUniqueAsync(contact.Email, contact.Id))
                return BadRequest("Email must be unique.");

            var updated = await _contactRepository.UpdateContactAsync(contact);
            if (!updated)
                return NotFound("Contact not found.");

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteContact(int id)
        {
            var deleted = await _contactRepository.DeleteContactAsync(id);
            if (!deleted)
                return NotFound("Contact not found.");

            return NoContent();
        }
    }
}
