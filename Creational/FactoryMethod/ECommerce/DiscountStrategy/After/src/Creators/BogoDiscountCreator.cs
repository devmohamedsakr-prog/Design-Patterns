using DiscountStrategy.After.Abstracts;
using DiscountStrategy.After.Strategies;

namespace DiscountStrategy.After.Creators
{
    public class BogoDiscountCreator : DiscountCreator
    {
        protected override IDiscountStrategy CreateDiscountStrategy()
        {
            return new BuyOneGetOneDiscount();
        }
    }
}
