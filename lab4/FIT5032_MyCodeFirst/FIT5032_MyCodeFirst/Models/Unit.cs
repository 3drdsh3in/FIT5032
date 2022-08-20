namespace FIT5032_MyCodeFirst.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Unit", Schema = "dbo")]
    public partial class Unit
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int StudentId { get; set; }

        public virtual Student Student { get; set; }
    }
}