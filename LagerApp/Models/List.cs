namespace LagerApp.Models;

public partial class List
{
    public int Id { get; set; }

    public string ArticleNumber { get; set; } = null!;

    public int Quantity { get; set; }
}
