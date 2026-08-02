using System.Collections.Generic;

namespace ProductFactory.Products
{
    /// <summary>
    /// Null Object: Default product when no type is found
    /// Single Responsibility: Provide safe default product behavior
    /// </summary>
    public class NullProduct : IProduct
    {
        public string SKU => "NULL-PRODUCT";
        public string Name => "Unknown Product";
        public decimal Price => 0m;

        public NullProduct(Dictionary<string, object> properties = null)
        {
            // Accept but ignore properties - safe fallback
        }

        public decimal CalculateShippingCost(decimal weight = 0) => 0m;
        public decimal CalculateTax(string location) => 0m;
        public string GetFulfillmentMethod() => "No Fulfillment";
        public bool IsInStock() => false;
        public string GetDeliveryEstimate() => "Not Available";
        public string GetProductTypeDescription() => "Null Product - Invalid Type";
    }
}
