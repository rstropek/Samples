using Microsoft.EntityFrameworkCore;

namespace StartupPerfApi
{
    class AddressBookContext : DbContext
    {
        public AddressBookContext(DbContextOptions<AddressBookContext> options) : base(options)
        {
        }

        public DbSet<Person> Persons { get; set; }
    }
}
