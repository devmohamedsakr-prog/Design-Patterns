using OrderValidation.After.Handlers;
using OrderValidation.After.Models;

namespace OrderValidation.After.Builders
{
    /// <summary>
    /// ValidationChainBuilder: Fluent builder for constructing validation chains
    /// SRP: Only responsible for assembling handler chains
    /// </summary>
    public class ValidationChainBuilder
    {
        private ValidationHandler _firstHandler;
        private ValidationHandler _lastHandler;

        public ValidationChainBuilder AddInventoryCheck(int maxQuantity = 1000, int minQuantity = 1)
        {
            var handler = new InventoryHandler(maxQuantity, minQuantity);
            AppendHandler(handler);
            return this;
        }

        public ValidationChainBuilder AddPaymentCheck(decimal minAmount = 0.01m, decimal maxAmount = 999999m)
        {
            var handler = new PaymentHandler(minAmount, maxAmount);
            AppendHandler(handler);
            return this;
        }

        public ValidationChainBuilder AddFraudCheck(decimal highAmountThreshold = 10000m, int highQuantityThreshold = 500)
        {
            var handler = new FraudHandler(highAmountThreshold, highQuantityThreshold);
            AppendHandler(handler);
            return this;
        }

        public ValidationChainBuilder AddShippingCheck(int minAddressLength = 10)
        {
            var handler = new ShippingHandler(minAddressLength);
            AppendHandler(handler);
            return this;
        }

        public ValidationChainBuilder AddHandler(ValidationHandler handler)
        {
            AppendHandler(handler);
            return this;
        }

        private void AppendHandler(ValidationHandler handler)
        {
            if (_firstHandler == null)
            {
                _firstHandler = handler;
                _lastHandler = handler;
            }
            else
            {
                _lastHandler.SetNext(handler);
                _lastHandler = handler;
            }
        }

        public ValidationHandler Build()
        {
            if (_firstHandler == null)
                throw new InvalidOperationException("Chain must have at least one handler");
            
            return _firstHandler;
        }

        public ValidationResult Validate(Order order)
        {
            var chain = Build();
            return chain.Handle(order);
        }
    }
}
