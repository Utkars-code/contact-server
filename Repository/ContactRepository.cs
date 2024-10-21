using contacts_server.Model;
using System.Text.Json;

namespace contacts_server.Repository
{
    public class ContactRepository : IContactRepository
    {
        private readonly string _jsonFilePath = "Data/ContactJsonData.json";
        

        private JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true, // Optional, for pretty printing the JSON
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping // Allows special characters
        };

        private async Task<List<Contact>> ReadFromJsonAsync()
        {
            if (!File.Exists(_jsonFilePath))
            {
                return new List<Contact>();
            }

            try
            {
                var jsonData = await File.ReadAllTextAsync(_jsonFilePath);
                return JsonSerializer.Deserialize<List<Contact>>(jsonData, _jsonOptions) ?? new List<Contact>();
            }
            catch (JsonException ex)
            {
                throw new Exception("Error deserializing the JSON data. Check for invalid characters or formatting issues.", ex);
            }
        }

        private async Task WriteToJsonAsync(List<Contact> contacts)
        {
            try
            {
                var jsonData = JsonSerializer.Serialize(contacts, _jsonOptions);
                await File.WriteAllTextAsync(_jsonFilePath, jsonData);
            }
            catch (Exception ex)
            {
                throw new Exception("Error writing to JSON file", ex);
            }
        }

        public async Task<IEnumerable<Contact>> GetAllContactsAsync()
        {
            return await ReadFromJsonAsync();
        }

        public async Task<Contact> GetContactByIdAsync(int id)
        {
            var contacts = await ReadFromJsonAsync();
            return contacts.FirstOrDefault(c => c.Id == id);
        }

        public async Task AddContactAsync(Contact contact)
        {
            var contacts = await ReadFromJsonAsync();
            contact.Id = contacts.Count > 0 ? contacts.Max(c => c.Id) + 1 : 1; // Generate new ID
            contacts.Add(contact);
            await WriteToJsonAsync(contacts);
        }

        public async Task<bool> UpdateContactAsync(Contact contact)
        {
            var contacts = await ReadFromJsonAsync();
            var existingContact = contacts.FirstOrDefault(c => c.Id == contact.Id);
            if (existingContact == null)
                return false;

            existingContact.FirstName = contact.FirstName;
            existingContact.LastName = contact.LastName;
            existingContact.Email = contact.Email;

            await WriteToJsonAsync(contacts);
            return true;
        }

        public async Task<bool> DeleteContactAsync(int id)
        {
            var contacts = await ReadFromJsonAsync();
            var contact = contacts.FirstOrDefault(c => c.Id == id);
            if (contact == null)
                return false;

            contacts.Remove(contact);
            await WriteToJsonAsync(contacts);
            return true;
        }

        public async Task<bool> IsEmailUniqueAsync(string email, int? contactId = null)
        {
            var contacts = await ReadFromJsonAsync();
            return !contacts.Any(c => c.Email == email && c.Id != contactId);
        }
    }
}
