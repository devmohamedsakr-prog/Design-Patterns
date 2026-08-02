using DiscountStrategy.After.Abstracts;
using DiscountStrategy.After.Strategies;

namespace DiscountStrategy.After.Creators
{
    public class FixedDiscountCreator : DiscountCreator
    {
        private readonly decimal _fixedAmount;

        public FixedDiscountCreator(decimal fixedAmount = 10) => _fixedAmount = fixedAmount;

        protected override IDiscountStrategy CreateDiscountStrategy()
        {
            return new FixedDiscount(_fixedAmount);
        }
    }
}
