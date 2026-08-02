using System;

namespace OrderSystem.Before
{
    // BEFORE: Anti-pattern - Class explosion
    // Problem: Adding pricing features requires exponential classes

    public class Order
    {
        public string OrderId { get; set; }
        public decimal BasePrice { get; set; }

        public Order(string orderId, decimal basePrice)
        {
            OrderId = orderId;
            BasePrice = basePrice;
        }

        public virtual decimal GetTotal() => BasePrice;

        public override string ToString() =>
            $"Order {OrderId}: ${GetTotal():F2}";
    }

    // Anti-pattern class 1: Discount only
    public class OrderWithDiscount : Order
    {
        private decimal _discountPercent;

        public OrderWithDiscount(string orderId, decimal basePrice, decimal discount)
            : base(orderId, basePrice)
        {
            _discountPercent = discount;
        }

        public override decimal GetTotal() => BasePrice * (1 - _discountPercent);
    }

    // Anti-pattern class 2: Tax only
    public class OrderWithTax : Order
    {
        private decimal _taxRate;

        public OrderWithTax(string orderId, decimal basePrice, decimal tax)
            : base(orderId, basePrice)
        {
            _taxRate = tax;
        }

        public override decimal GetTotal() => BasePrice * (1 + _taxRate);
    }

    // Anti-pattern class 3: Discount + Tax (code duplication!)
    public class OrderWithDiscountAndTax : Order
    {
        private decimal _discountPercent;
        private decimal _taxRate;

        public OrderWithDiscountAndTax(string orderId, decimal basePrice, decimal discount, decimal tax)
            : base(orderId, basePrice)
        {
            _discountPercent = discount;
            _taxRate = tax;
        }

        public override decimal GetTotal()
        {
            decimal afterDiscount = BasePrice * (1 - _discountPercent);
            return afterDiscount * (1 + _taxRate);
        }
    }

    // Anti-pattern class 4: Shipping only
    public class OrderWithShipping : Order
    {
        private decimal _shippingCost;

        public OrderWithShipping(string orderId, decimal basePrice, decimal shipping)
            : base(orderId, basePrice)
        {
            _shippingCost = shipping;
        }

        public override decimal GetTotal() => BasePrice + _shippingCost;
    }

    // Anti-pattern class 5: Discount + Shipping (more duplication!)
    public class OrderWithDiscountAndShipping : Order
    {
        private decimal _discountPercent;
        private decimal _shippingCost;

        public OrderWithDiscountAndShipping(string orderId, decimal basePrice, decimal discount, decimal shipping)
            : base(orderId, basePrice)
        {
            _discountPercent = discount;
            _shippingCost = shipping;
        }

        public override decimal GetTotal() =>
            (BasePrice * (1 - _discountPercent)) + _shippingCost;
    }

    // Anti-pattern class 6: Discount + Tax + Shipping (extreme duplication!)
    public class OrderWithDiscountTaxAndShipping : Order
    {
        private decimal _discountPercent;
        private decimal _taxRate;
        private decimal _shippingCost;

        public OrderWithDiscountTaxAndShipping(string orderId, decimal basePrice, 
            decimal discount, decimal tax, decimal shipping)
            : base(orderId, basePrice)
        {
            _discountPercent = discount;
            _taxRate = tax;
            _shippingCost = shipping;
        }

        public override decimal GetTotal()
        {
            decimal afterDiscount = BasePrice * (1 - _discountPercent);
            decimal withTax = afterDiscount * (1 + _taxRate);
            return withTax + _shippingCost;
        }
    }

    // Application showing the problem
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Decorator Pattern: BEFORE (Anti-pattern) ===\n");
            Console.WriteLine("Problem: Class Explosion with Pricing Features\n");

            // Scenario 1: Simple order
            Console.WriteLine("--- Scenario 1: Simple Order (base price only) ---");
            var order1 = new Order("ORD001", 100m);
            Console.WriteLine(order1);
            Console.WriteLine();

            // Scenario 2: With discount
            Console.WriteLine("--- Scenario 2: Order with 10% Discount ---");
            var order2 = new OrderWithDiscount("ORD002", 100m, 0.10m);
            Console.WriteLine(order2);
            Console.WriteLine($"  Problem: Need separate class for this combination");
            Console.WriteLine();

            // Scenario 3: With tax
            Console.WriteLine("--- Scenario 3: Order with 8% Tax ---");
            var order3 = new OrderWithTax("ORD003", 100m, 0.08m);
            Console.WriteLine(order3);
            Console.WriteLine($"  Problem: Need yet another class");
            Console.WriteLine();

            // Scenario 4: With discount + tax
            Console.WriteLine("--- Scenario 4: Order with 10% Discount + 8% Tax ---");
            var order4 = new OrderWithDiscountAndTax("ORD004", 100m, 0.10m, 0.08m);
            Console.WriteLine(order4);
            Console.WriteLine($"  Base: $100.00");
            Console.WriteLine($"  After 10% discount: $90.00");
            Console.WriteLine($"  After 8% tax: ${order4.GetTotal():F2}");
            Console.WriteLine();

            // Scenario 5: With shipping
            Console.WriteLine("--- Scenario 5: Order with $15 Shipping ---");
            var order5 = new OrderWithShipping("ORD005", 100m, 15m);
            Console.WriteLine(order5);
            Console.WriteLine();

            // Scenario 6: With discount + shipping
            Console.WriteLine("--- Scenario 6: Order with 10% Discount + $15 Shipping ---");
            var order6 = new OrderWithDiscountAndShipping("ORD006", 100m, 0.10m, 15m);
            Console.WriteLine(order6);
            Console.WriteLine();

            // Scenario 7: Complex combination
            Console.WriteLine("--- Scenario 7: Order with 10% Discount + 8% Tax + $15 Shipping ---");
            var order7 = new OrderWithDiscountTaxAndShipping("ORD007", 100m, 0.10m, 0.08m, 15m);
            Console.WriteLine(order7);
            Console.WriteLine($"  Base: $100.00");
            Console.WriteLine($"  After 10% discount: $90.00");
            Console.WriteLine($"  After 8% tax: $97.20");
            Console.WriteLine($"  After $15 shipping: ${order7.GetTotal():F2}");
            Console.WriteLine();

            // Show the problem
            Console.WriteLine("--- THE PROBLEM ---");
            Console.WriteLine("✗ Already need 6 different classes for 3 features (2^3 = 8 combinations)");
            Console.WriteLine("✗ Adding loyalty discounts? Need 12 more classes");
            Console.WriteLine("✗ Adding insurance? Need 24 more classes");
            Console.WriteLine("✗ Adding gift wrapping? Need 48 more classes");
            Console.WriteLine("✗ Code duplication in every class (pricing logic repeated)");
            Console.WriteLine("✗ Bug fixes must be applied to many classes");
            Console.WriteLine("✗ Testing becomes exponentially complex");
            Console.WriteLine();

            Console.WriteLine("SOLUTION: Use Decorator Pattern! (see After/)");
            Console.WriteLine("- Single Order class");
            Console.WriteLine("- Composable decorators for each feature");
            Console.WriteLine("- Dynamic pricing combinations");
            Console.WriteLine("- No code duplication");
        }
    }
}
