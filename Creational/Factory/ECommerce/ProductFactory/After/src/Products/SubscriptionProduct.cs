namespace ProductFactory.Products
{
    /// <summary>
    /// Subscription product: Recurring billing, auto-renewal
    /// Single Responsibility: Manage subscription product specifics
    /// </summary>
    public class SubscriptionProduct : IProduct
    {
        public string SKU { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string BillingInterval { get; set; } // Monthly, Yearly, etc.
        public bool AutoRenewal { get; set; } = true;
        public int TrialDays { get; set; } = 0;

        public decimal CalculateShippingCost(decimal weight = 0) => 0m; // No shipping
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

        public string GetFulfillmentMethod() => "Subscription Service";
        public bool IsInStock() => true; // Always available
        public string GetDeliveryEstimate()
        {
            if (TrialDays > 0)
                return $"Trial: {TrialDays} days, then {BillingInterval}";
            return BillingInterval;
        }

        public string GetProductTypeDescription() => 
            $"Subscription - {BillingInterval}, Auto-renew: {AutoRenewal}, Trial: {TrialDays} days";
    }
}
