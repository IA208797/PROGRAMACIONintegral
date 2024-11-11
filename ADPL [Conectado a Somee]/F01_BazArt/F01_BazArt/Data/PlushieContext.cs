using F01_BazArt.Models;
using Microsoft.EntityFrameworkCore;

namespace F01_BazArt.Data
{
    public class PlushieContext : DbContext
    {
        public PlushieContext(DbContextOptions<PlushieContext> options) : base(options)
        {
        }

        public DbSet<Plushie> Plushies { get; set; }
    }
}
