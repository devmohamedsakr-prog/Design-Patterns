namespace ProductFactory
{
    /// <summary>Physical product: Has weight, dimensions, requires shipping</summary>
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
            return (decimal)(Math.Ceiling(productWeight) * 5m); // $5 per pound
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

        public string GetFulfillmentMethod()
        {
            return "Warehouse Fulfillment";
        }

        public bool IsInStock()
        {
            return InStock;
        }

        public string GetDeliveryEstimate()
        {
            return "3-5 business days";
        }

        public string GetProductTypeDescription()
        {
            return $"Physical Product - {Dimensions}, {Weight} lbs";
        }
    }

    /// <summary>Digital product: Downloaded instantly, no shipping</summary>
    public class DigitalProduct : IProduct
    {
        public string SKU { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string FileFormat { get; set; }
        public decimal FileSize { get; set; } // in MB

        public decimal CalculateShippingCost(decimal weight = 0)
        {
            // No shipping for digital products
            return 0m;
        }

        public decimal CalculateTax(string location)
        {
            // Digital goods typically no sales tax (varies by jurisdiction)
            return 0m;
        }

        public string GetFulfillmentMethod()
        {
            return "Instant Download";
        }

        public bool IsInStock()
        {
            // Digital products always in stock
            return true;
        }

        public string GetDeliveryEstimate()
        {
            return "Instant (after purchase)";
        }

        public string GetProductTypeDescription()
        {
            return $"Digital Product - {FileFormat}, {FileSize}MB";
        }
    }

    /// <summary>Service product: Professional services, no physical item</summary>
    public class ServiceProduct : IProduct
    {
        public string SKU { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal DurationHours { get; set; }
        public string ServiceType { get; set; }
        public bool Available { get; set; } = true;

        public decimal CalculateShippingCost(decimal weight = 0)
        {
            // No shipping for services
            return 0m;
        }

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

        public string GetFulfillmentMethod()
        {
            return "Service Delivery";
        }

        public bool IsInStock()
        {
            return Available;
        }

        public string GetDeliveryEstimate()
        {
            return $"Scheduled - {DurationHours} hours";
        }

        public string GetProductTypeDescription()
        {
            return $"Service - {ServiceType}, {DurationHours} hours";
        }
    }

    /// <summary>Subscription product: Recurring billing, auto-renewal</summary>
    public class SubscriptionProduct : IProduct
    {
        public string SKU { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string BillingInterval { get; set; } // Monthly, Yearly, etc.
        public bool AutoRenewal { get; set; } = true;
        public int TrialDays { get; set; } = 0;

        public decimal CalculateShippingCost(decimal weight = 0)
        {
            // No shipping for subscriptions
            return 0m;
        }

        public decimal CalculateTax(string location)
        {
            // Subscription tax rules
            return location?.ToLower() switch
            {
                "ca" => Price * 0.0825m,
                "ny" => Price * 0.08m,
                _ => Price * 0.07m
            };
        }

        public string GetFulfillmentMethod()
        {
            return "Subscription Service";
        }

        public bool IsInStock()
        {
            // Subscriptions always available
            return true;
        }

        public string GetDeliveryEstimate()
        {
            if (TrialDays > 0)
                return $"Trial: {TrialDays} days, then {BillingInterval}";
            return BillingInterval;
        }

        public string GetProductTypeDescription()
        {
            return $"Subscription - {BillingInterval}, Auto-renew: {AutoRenewal}, Trial: {TrialDays} days";
        }
    }

    /// <summary>Bundle product: Multiple items sold together</summary>
    public class BundleProduct : IProduct
    {
        public string SKU { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public List<string> BundleItems { get; set; } = new();
        public decimal BundleDiscount { get; set; } // Percentage discount

        public decimal CalculateShippingCost(decimal weight = 0)
        {
            // Bundle shipping - typically weight-based
            return weight > 0 ? (decimal)(Math.Ceiling(weight) * 5m) : 10m;
        }

        public decimal CalculateTax(string location)
        {
            // Apply tax to bundle price
            return location?.ToLower() switch
            {
                "ca" => Price * 0.0725m,
                "ny" => Price * 0.08m,
                _ => Price * 0.07m
            };
        }

        public string GetFulfillmentMethod()
        {
            return "Bundle Fulfillment";
        }

        public bool IsInStock()
        {
            return true; // Simplified
        }

        public string GetDeliveryEstimate()
        {
            return "3-5 business days";
        }

        public string GetProductTypeDescription()
        {
            return $"Bundle - {BundleItems.Count} items, {BundleDiscount}% discount";
        }
    }
}
