namespace ProductFactory.Products
{
    /// <summary>
    /// Digital product: Downloaded instantly, no shipping
    /// Single Responsibility: Manage digital product specifics
    /// </summary>
    public class DigitalProduct : IProduct
    {
        public string SKU { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string FileFormat { get; set; }
        public decimal FileSize { get; set; } // in MB

        public decimal CalculateShippingCost(decimal weight = 0) => 0m; // No shipping
        public decimal CalculateTax(string location) => 0m; // No tax
        public string GetFulfillmentMethod() => "Instant Download";
        public bool IsInStock() => true; // Always in stock
        public string GetDeliveryEstimate() => "Instant (after purchase)";
        public string GetProductTypeDescription() => $"Digital Product - {FileFormat}, {FileSize}MB";
    }
}
