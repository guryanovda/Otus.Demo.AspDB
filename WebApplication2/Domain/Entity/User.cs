using WebApplication2.Abstractions;

namespace WebApplication2.Domain.Entity
{
    public class User: BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
