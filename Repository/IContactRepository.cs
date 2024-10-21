using contacts_server.Model;

namespace contacts_server.Repository
{
    public interface IContactRepository
    {
        Task<IEnumerable<Contact>> GetAllContactsAsync();
        Task<Contact> GetContactByIdAsync(int id);
        Task AddContactAsync(Contact contact);
        Task<bool> UpdateContactAsync(Contact contact);
        Task<bool> DeleteContactAsync(int id);
        Task<bool> IsEmailUniqueAsync(string email, int? contactId = null);
    }
}
