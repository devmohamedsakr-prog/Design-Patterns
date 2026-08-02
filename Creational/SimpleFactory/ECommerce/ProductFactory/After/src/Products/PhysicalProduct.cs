namespace ProductFactory.Products
{
    /// <summary>
    /// Physical product: Has weight, dimensions, requires shipping
    /// Single Responsibility: Manage physical product specifics
    /// </summary>
    public class PhysicalProduct : IProduct
    {
        public string SKU { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal Weight { get; set; }
        public string Dimensions { get; set; }
        public bool InStock { get; set; } = true;

        public decimal CalculateShippingCost(decimal weight = 0)
        {
            // Shipping based on weight
            var productWeight = weight > 0 ? weight : Weight;
            return (decimal)(System.Math.Ceiling(productWeight) * 5m); // $5 per pound
        }

        public decimal CalculateTax(string location)
        {
            // Tax varies by location (simplified)
            return location?.ToLower() switch
            {
                "ca" => Price * 0.0725m,  // 7.25% California
                "ny" => Price * 0.08m,    // 8% New York
                "tx" => Price * 0.0625m,  // 6.25% Texas
                _ => Price * 0.07m        // 7% default
            };
        }

        public string GetFulfillmentMethod() => "Warehouse Fulfillment";
        public bool IsInStock() => InStock;
        public string GetDeliveryEstimate() => "3-5 business days";
        public string GetProductTypeDescription() => $"Physical Product - {Dimensions}, {Weight} lbs";
    }
}
