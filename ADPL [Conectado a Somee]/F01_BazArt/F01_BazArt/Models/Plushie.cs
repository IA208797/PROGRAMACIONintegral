using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace F01_BazArt.Models
{
	public class Plushie
	{
		[Key]
		public int Id { get; set; }
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
		[Display(Name = "Imagen")]
		public string ImageName { get; set; }
		[Display(Name = "Miniatura")]
		public string ImageThumbnail { get; set; }
	}
}
