using DiscountStrategy.After.Abstracts;
using DiscountStrategy.After.Strategies;

namespace DiscountStrategy.After.Creators
{
    public class PercentageDiscountCreator : DiscountCreator
    {
        private readonly decimal _percentage;

        public PercentageDiscountCreator(decimal percentage = 10) => _percentage = percentage;

        protected override IDiscountStrategy CreateDiscountStrategy()
        {
            return new PercentageDiscount(_percentage);
        }
    }
}
