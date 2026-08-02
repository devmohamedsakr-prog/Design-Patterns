using System;
using System.Threading.Tasks;
using ShippingMethod.After.Abstracts;

namespace ShippingMethod.After.Methods
{
    public class ExpressShipping : IShippingMethod
    {
        public string GetMethodName() => "Express";

        public async Task<ShippingResult> CalculateAsync(decimal weight, string destination, decimal packageValue)
        {
            await Task.Delay(50);
            decimal cost = weight * 1.5m; // $1.50 per lb
            
            return new ShippingResult
            {
                Success = true,
                TrackingId = $"exp_{destination}_{DateTime.Now.Ticks}",
                MethodName = GetMethodName(),
                Cost = cost,
                DeliveryDays = 2,
                Message = $"Express shipping (2 business days)"
            };
        }
    }
}
