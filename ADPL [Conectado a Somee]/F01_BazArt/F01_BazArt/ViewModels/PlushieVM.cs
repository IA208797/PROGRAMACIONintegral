using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace F01_BazArt.ViewModels
{
    public class PlushieVM
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "Plushie Tote Bag")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Descripción")]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Tipo")]
        public string Type { get; set; }
        [Required]
        [Display(Name = "Precio")]
        [Column(TypeName = "decimal(10, 2)")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        [Required]
        [Display(Name = "Existencias")]
        public int Stock { get; set; }
        public IFormFile ImagePlushie { get; set; }
        public IFormFile ImagePlushieThumbnail { get; set; }
        public string ImageNameActual { get; set; }
        public string ImageThumbnailActual { get; set; }
    }
}
