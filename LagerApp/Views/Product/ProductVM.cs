#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;

namespace LagerApp.Views.Product
{
    public class ProductVM
    {
        public int Id { get; set; }

        [Display(Name = "Artikel nummer")]
        [Required(ErrorMessage = "Artikel nummer är obligatorisk")]
        public string ArticleNumber { get; set; }

        [Display(Name = "Inköpspris")]
        [Required(ErrorMessage = "Inköpspris är obligatorisk")]
        [Range(0, double.MaxValue, ErrorMessage = "Ange ett giltigt värde")]
        public decimal PurchasePrice { get; set; }

        [Display(Name = "Pris")]
        [Required(ErrorMessage = "Pris är obligatorisk")]
        [Range(0, double.MaxValue, ErrorMessage = "Ange ett giltigt värde")]
        public decimal SellingPrice { get; set; }

        [Display(Name = "Vikt")]
        [Required(ErrorMessage = "Vikt är obligatorisk")]
        [Range(0, double.MaxValue, ErrorMessage = "Ange ett giltigt värde")]

        public decimal Weight { get; set; }

        [Display(Name = "Mått")]
        public decimal? Dimension { get; set; }

        [Display(Name = "Material")]
        [Required(ErrorMessage = "Material är obligatorisk")]
        public string Material { get; set; }

        [Display(Name = "Antal")]
        [Required(ErrorMessage = "Antal är obligatorisk")]
        [Range(1, int.MaxValue, ErrorMessage = "Ange ett giltigt värde")]
        public int Quantity { get; set; }
    }
}
