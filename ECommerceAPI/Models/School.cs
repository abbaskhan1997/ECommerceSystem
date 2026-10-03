using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Models
{
    public class School
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
