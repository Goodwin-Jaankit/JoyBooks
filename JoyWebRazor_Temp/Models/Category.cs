using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace JoyWebRazor_Temp.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        [DisplayName("Name")]
        public string Name { get; set; }

        [Range(1, 100, ErrorMessage = "Display Order Must be 1 - 100")]
        [DisplayName("Display Order")]
        public int DisplayOrder { get; set; }

    }
}
