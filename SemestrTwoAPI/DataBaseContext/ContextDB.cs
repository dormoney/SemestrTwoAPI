using Microsoft.EntityFrameworkCore;
using SemestrTwoAPI.Model;

namespace SemestrTwoAPI.DataBaseContext
{
    public class ContextDB : DbContext
    {
        public ContextDB(DbContextOptions options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
    }
}
