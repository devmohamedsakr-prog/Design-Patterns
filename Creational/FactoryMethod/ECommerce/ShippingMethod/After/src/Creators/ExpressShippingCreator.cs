using ShippingMethod.After.Abstracts;
using ShippingMethod.After.Methods;

namespace ShippingMethod.After.Creators
{
    public class ExpressShippingCreator : ShippingCreator
    {
        protected override IShippingMethod CreateShippingMethod()
        {
            return new ExpressShipping();
        }
    }
}
