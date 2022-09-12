using System.Data.Entity;

namespace FIT5032_MyCodeFirst.Models
{
    public class FIT5032_CodeFirstContext2 : DbContext
    {
        public FIT5032_CodeFirstContext2() : base()
        {

        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Unit> Units { get; set; }
    }
}