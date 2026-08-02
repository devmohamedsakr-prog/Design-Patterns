using System;
using System.Threading.Tasks;
using ShippingMethod.After.Abstracts;

namespace ShippingMethod.After.Methods
{
    public class OvernightShipping : IShippingMethod
    {
        public string GetMethodName() => "Overnight";

        public async Task<ShippingResult> CalculateAsync(decimal weight, string destination, decimal packageValue)
        {
            await Task.Delay(50);
            decimal cost = weight * 3.0m; // $3.00 per lb
            
            return new ShippingResult
            {
                Success = true,
                TrackingId = $"ovr_{destination}_{DateTime.Now.Ticks}",
                MethodName = GetMethodName(),
                Cost = cost,
                DeliveryDays = 1,
                Message = $"Overnight shipping (next business day)"
            };
        }
    }
}
