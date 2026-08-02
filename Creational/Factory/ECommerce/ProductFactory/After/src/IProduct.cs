namespace ProductFactory
{
    /// <summary>
    /// Represents a product in the e-commerce system
    /// Different product types have different business logic
    /// </summary>
    public interface IProduct
    {
        string SKU { get; }
        string Name { get; }
        decimal Price { get; }
        
        /// <summary>Calculate shipping cost for this product</summary>
        decimal CalculateShippingCost(decimal weight = 0);
        
        /// <summary>Calculate tax based on product type and location</summary>
        decimal CalculateTax(string location);
        
        /// <summary>Get fulfillment method for this product</summary>
        string GetFulfillmentMethod();
        
        /// <summary>Check if product is in stock</summary>
        bool IsInStock();
        
        /// <summary>Get delivery time estimate</summary>
        string GetDeliveryEstimate();
        
        /// <summary>Get product type description</summary>
        string GetProductTypeDescription();
    }
}
