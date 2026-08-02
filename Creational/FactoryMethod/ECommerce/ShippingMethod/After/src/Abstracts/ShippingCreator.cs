using System;
using System.Threading.Tasks;

namespace ShippingMethod.After.Abstracts
{
    /// <summary>
    /// Shipping Creator: Abstract base for factory method pattern
    /// Each subclass creates specific shipping method implementation
    /// </summary>
    public abstract class ShippingCreator
    {
        protected abstract IShippingMethod CreateShippingMethod();

        public async Task<ShippingResult> CalculateShippingAsync(decimal weight, string destination, decimal packageValue)
        {
            try
            {
                if (!ValidateShipping(weight, destination))
                    return new ShippingResult { Success = false, Message = "Validation failed" };

                IShippingMethod method = CreateShippingMethod();
                ShippingResult result = await method.CalculateAsync(weight, destination, packageValue);

                LogShipping(destination, method.GetMethodName(), result.Success ? "SUCCESS" : "FAILED");
                return result;
            }
            catch (Exception ex)
            {
                return new ShippingResult { Success = false, Message = ex.Message };
            }
        }

        protected virtual bool ValidateShipping(decimal weight, string destination)
        {
            return weight > 0 && !string.IsNullOrEmpty(destination);
        }

        protected virtual void LogShipping(string destination, string method, string status)
        {
            Console.WriteLine($"[LOG] Destination: {destination}, Method: {method}, Status: {status}");
        }
    }

    /// <summary>Shipping Method Interface</summary>
    public interface IShippingMethod
    {
        Task<ShippingResult> CalculateAsync(decimal weight, string destination, decimal packageValue);
        string GetMethodName();
    }

    /// <summary>Shipping Result Model</summary>
    public class ShippingResult
    {
        public bool Success { get; set; }
        public string TrackingId { get; set; }
        public string MethodName { get; set; }
        public decimal Cost { get; set; }
        public int DeliveryDays { get; set; }
        public string Message { get; set; }
    }
}
