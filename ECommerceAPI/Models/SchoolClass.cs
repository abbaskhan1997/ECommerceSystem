using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Models
{
    public class SchoolClass
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public int SchoolId { get; set; }

        public School? School { get; set; }


    }
}
