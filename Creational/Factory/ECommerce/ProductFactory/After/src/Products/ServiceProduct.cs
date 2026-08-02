namespace ProductFactory.Products
{
    /// <summary>
    /// Service product: Professional services, no physical item
    /// Single Responsibility: Manage service product specifics
    /// </summary>
    public class ServiceProduct : IProduct
    {
        public string SKU { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal DurationHours { get; set; }
        public string ServiceType { get; set; }
        public bool Available { get; set; } = true;

        public decimal CalculateShippingCost(decimal weight = 0) => 0m; // No shipping
        public decimal CalculateTax(string location)
        {
            // Service tax varies significantly by location
            return location?.ToLower() switch
            {
                "ca" => Price * 0.0825m,  // 8.25% California
                "ny" => Price * 0.085m,   // 8.5% New York
                _ => Price * 0.07m        // 7% default
            };
        }

        public string GetFulfillmentMethod() => "Service Delivery";
        public bool IsInStock() => Available;
        public string GetDeliveryEstimate() => $"Scheduled - {DurationHours} hours";
        public string GetProductTypeDescription() => $"Service - {ServiceType}, {DurationHours} hours";
    }
}
