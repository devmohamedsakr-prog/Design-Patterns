using System;
using System.Threading.Tasks;
using ShippingMethod.After.Abstracts;

namespace ShippingMethod.After.Methods
{
    public class StandardShipping : IShippingMethod
    {
        public string GetMethodName() => "Standard";

        public async Task<ShippingResult> CalculateAsync(decimal weight, string destination, decimal packageValue)
        {
            await Task.Delay(50);
            decimal cost = weight * 0.5m; // $0.50 per lb
            
            return new ShippingResult
            {
                Success = true,
                TrackingId = $"std_{destination}_{DateTime.Now.Ticks}",
                MethodName = GetMethodName(),
                Cost = cost,
                DeliveryDays = 5,
                Message = $"Standard shipping (5 business days)"
            };
        }
    }
}
