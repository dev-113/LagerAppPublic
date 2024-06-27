namespace LagerApp.Models;

public partial class Product
{
    public int Id { get; set; }

    public string ArticleNumber { get; set; } = null!;

    public decimal PurchasePrice { get; set; }

    public decimal SellingPrice { get; set; }

    public decimal Weight { get; set; }

    public decimal? Dimension { get; set; }

    public string Material { get; set; } = null!;

    public int Quantity { get; set; }
}
