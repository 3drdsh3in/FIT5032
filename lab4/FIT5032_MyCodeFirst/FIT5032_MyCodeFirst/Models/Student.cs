namespace FIT5032_MyCodeFirst.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Student", Schema = "dbo")]
    public partial class Student
    {
        public Student()
        {
            this.Units = new HashSet<Unit>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [ForeignKey("StudentId")]
        public ICollection<Unit> Units { get; set; }
    }
}