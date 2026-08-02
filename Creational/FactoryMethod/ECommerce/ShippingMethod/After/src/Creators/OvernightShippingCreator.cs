using ShippingMethod.After.Abstracts;
using ShippingMethod.After.Methods;

namespace ShippingMethod.After.Creators
{
    public class OvernightShippingCreator : ShippingCreator
    {
        protected override IShippingMethod CreateShippingMethod()
        {
            return new OvernightShipping();
        }
    }
}
