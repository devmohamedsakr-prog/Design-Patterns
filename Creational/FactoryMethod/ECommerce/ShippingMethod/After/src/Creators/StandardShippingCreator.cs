using ShippingMethod.After.Abstracts;
using ShippingMethod.After.Methods;

namespace ShippingMethod.After.Creators
{
    public class StandardShippingCreator : ShippingCreator
    {
        protected override IShippingMethod CreateShippingMethod()
        {
            return new StandardShipping();
        }
    }
}
