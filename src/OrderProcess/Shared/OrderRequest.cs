namespace Shared;

public class OrderRequest
{
    public string OrderId { get; set; }
    public int CustomerId { get; set; }
    public List<OrderItem> Items { get; set; }
    public string Status { get; set; }
}

public class OrderItem
{
    public string Sku { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}