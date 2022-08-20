using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace FIT5032_MyCodeFirst.Models
{
    public class FIT5032_CodeFirstContext : DbContext
    {
        public FIT5032_CodeFirstContext() : base()
        {

        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Unit> Units { get; set; }
    }
}