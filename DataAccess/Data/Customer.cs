using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Data
{
    public partial class Customer
    {
        [Key]
        public int CustomerId { get; set; }
        public string Name { get; set; }

        // Navigation property
        public ICollection<Post> Posts { get; set; }
    }
}
