using System;
using OrderSystem.After.Models;
using OrderSystem.After.Decorators;

namespace OrderSystem.After.Demo
{
    /// <summary>
    /// AFTER: Decorator Pattern Solution
    /// Demonstrates clean composition-based order decoration
    /// No class explosion, no code duplication, unlimited flexibility
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("════════════════════════════════════════════════════════════════");
            Console.WriteLine("  Decorator Pattern: eCommerce Order System");
            Console.WriteLine("  AFTER (Clean Design with Composition)");
            Console.WriteLine("════════════════════════════════════════════════════════════════\n");

            Console.WriteLine("✓ Single Order class");
            Console.WriteLine("✓ 4 focused decorator classes");
            Console.WriteLine("✓ Unlimited combinations without class explosion");
            Console.WriteLine("✓ Each decorator handles ONE concern (SRP)\n");

            // Scenario 1: Base order only
            Console.WriteLine("┌─ Scenario 1: Base Order (no decorators) ─────────────────────┐");
            {
                var order = new Order("ORD001", 100m);
                Console.WriteLine($"Order: {order.OrderId}");
                Console.WriteLine($"Base Price: ${order.BasePrice:F2}");
                Console.WriteLine($"Total: ${order.GetTotal():F2}\n");
            }

            // Scenario 2: With discount
            Console.WriteLine("┌─ Scenario 2: Order with 10% Discount ────────────────────────┐");
            {
                var order = new Order("ORD002", 100m);
                var discounted = new DiscountDecorator(order, 0.10m);
                Console.WriteLine($"Order: {order.OrderId}");
                Console.WriteLine($"Base Price: ${order.BasePrice:F2}");
                Console.WriteLine($"Discount 10%: -${order.BasePrice * 0.10m:F2}");
                Console.WriteLine($"Total: ${discounted.GetTotal():F2}\n");
            }

            // Scenario 3: With tax
            Console.WriteLine("┌─ Scenario 3: Order with 8% Tax ──────────────────────────────┐");
            {
                var order = new Order("ORD003", 100m);
                var taxed = new TaxDecorator(order, 0.08m);
                Console.WriteLine($"Order: {order.OrderId}");
                Console.WriteLine($"Base Price: ${order.BasePrice:F2}");
                Console.WriteLine($"Tax 8%: +${order.BasePrice * 0.08m:F2}");
                Console.WriteLine($"Total: ${taxed.GetTotal():F2}\n");
            }

            // Scenario 4: Discount + Tax
            Console.WriteLine("┌─ Scenario 4: Discount → Tax (Composition) ────────────────────┐");
            {
                var order = new Order("ORD004", 100m);
                var discounted = new DiscountDecorator(order, 0.10m);
                var withTax = new TaxDecorator(discounted, 0.08m);
                
                Console.WriteLine($"Order: {order.OrderId}");
                Console.WriteLine($"Base Price: ${order.BasePrice:F2}");
                Console.WriteLine($"Step 1 - Apply 10% Discount: ${discounted.GetTotal():F2}");
                Console.WriteLine($"Step 2 - Apply 8% Tax: ${withTax.GetTotal():F2}\n");
            }

            // Scenario 5: Complex combination
            Console.WriteLine("┌─ Scenario 5: Discount → Tax → Shipping ──────────────────────┐");
            {
                var order = new Order("ORD005", 100m);
                var discounted = new DiscountDecorator(order, 0.10m);
                var taxed = new TaxDecorator(discounted, 0.08m);
                var shipped = new ShippingDecorator(taxed, 15m);
                
                Console.WriteLine($"Order: {order.OrderId}");
                Console.WriteLine($"Base Price: ${order.BasePrice:F2}");
                Console.WriteLine($"Step 1 - Apply 10% Discount: ${discounted.GetTotal():F2}");
                Console.WriteLine($"Step 2 - Apply 8% Tax: ${taxed.GetTotal():F2}");
                Console.WriteLine($"Step 3 - Add $15 Shipping: ${shipped.GetTotal():F2}\n");
            }

            // Scenario 6: All decorators
            Console.WriteLine("┌─ Scenario 6: Full Stack (Discount → Tax → Shipping → Insurance)┐");
            {
                var order = new Order("ORD006", 200m);
                var discounted = new DiscountDecorator(order, 0.15m);
                var taxed = new TaxDecorator(discounted, 0.08m);
                var shipped = new ShippingDecorator(taxed, 20m);
                var insured = new InsuranceDecorator(shipped, 0.02m);
                
                Console.WriteLine($"Order: {order.OrderId}");
                Console.WriteLine($"Base Price: ${order.BasePrice:F2}");
                Console.WriteLine($"Step 1 - Apply 15% Discount: ${discounted.GetTotal():F2}");
                Console.WriteLine($"Step 2 - Apply 8% Tax: ${taxed.GetTotal():F2}");
                Console.WriteLine($"Step 3 - Add $20 Shipping: ${shipped.GetTotal():F2}");
                Console.WriteLine($"Step 4 - Add 2% Insurance: ${insured.GetTotal():F2}\n");
            }

            // Scenario 7: Different order (Tax before Discount)
            Console.WriteLine("┌─ Scenario 7: Different Order (Tax → Discount) ─────────────────┐");
            {
                var order = new Order("ORD007", 100m);
                var taxed = new TaxDecorator(order, 0.08m);
                var discounted = new DiscountDecorator(taxed, 0.10m);
                
                Console.WriteLine($"Order: {order.OrderId}");
                Console.WriteLine($"Base Price: ${order.BasePrice:F2}");
                Console.WriteLine($"Step 1 - Apply 8% Tax: ${taxed.GetTotal():F2}");
                Console.WriteLine($"Step 2 - Apply 10% Discount: ${discounted.GetTotal():F2}");
                Console.WriteLine($"Note: Different order = different result!\n");
            }

            // Scenario 8: Loyalty customer scenario
            Console.WriteLine("┌─ Scenario 8: Loyalty Customer (High Discount + Benefits) ─────┐");
            {
                var baseOrder = new Order("LOY001", 500m);
                
                // Loyalty gets 20% discount
                var discounted = new DiscountDecorator(baseOrder, 0.20m);
                // Plus regional tax
                var taxed = new TaxDecorator(discounted, 0.08m);
                // Plus free shipping (0 cost decorator)
                var shipped = new ShippingDecorator(taxed, 0m);
                
                Console.WriteLine($"Order: {baseOrder.OrderId} (Loyalty Member)");
                Console.WriteLine($"Original Price: ${baseOrder.BasePrice:F2}");
                Console.WriteLine($"With 20% Loyalty Discount: ${discounted.GetTotal():F2}");
                Console.WriteLine($"Plus 8% Tax: ${taxed.GetTotal():F2}");
                Console.WriteLine($"Plus FREE Shipping: ${shipped.GetTotal():F2}");
                Console.WriteLine($"Total Savings: ${baseOrder.BasePrice - shipped.GetTotal():F2}\n");
            }

            // Scenario 9: Dynamic decoration based on conditions
            Console.WriteLine("┌─ Scenario 9: Dynamic Decoration (Conditional) ────────────────┐");
            {
                var order = new Order("DYN001", 300m);
                Console.WriteLine($"Base Order: {order.OrderId} - ${order.BasePrice:F2}");
                
                bool isLoyalMember = true;
                bool needsShipping = true;
                bool isPremium = false;
                
                if (isLoyalMember)
                {
                    order = new DiscountDecorator(order, 0.15m);
                    Console.WriteLine($"✓ Applied Loyalty Discount (15%): ${order.GetTotal():F2}");
                }
                
                if (needsShipping)
                {
                    order = new ShippingDecorator(order, 12m);
                    Console.WriteLine($"✓ Applied Shipping: ${order.GetTotal():F2}");
                }
                
                if (isPremium)
                {
                    order = new InsuranceDecorator(order, 0.05m);
                    Console.WriteLine($"✓ Applied Premium Insurance: ${order.GetTotal():F2}");
                }
                else
                {
                    order = new InsuranceDecorator(order, 0.02m);
                    Console.WriteLine($"✓ Applied Standard Insurance: ${order.GetTotal():F2}");
                }
                
                Console.WriteLine($"Final Total: ${order.GetTotal():F2}\n");
            }

            // Scenario 10: International order
            Console.WriteLine("┌─ Scenario 10: International Order (Special Rules) ─────────────┐");
            {
                var order = new Order("INTL001", 250m);
                
                // International: Higher tax + expensive shipping
                var taxed = new TaxDecorator(order, 0.12m);      // Higher tax
                var shipped = new ShippingDecorator(taxed, 40m); // Expensive international shipping
                var insured = new InsuranceDecorator(shipped, 0.03m); // Higher insurance for international
                
                Console.WriteLine($"Order: {order.OrderId} (International)");
                Console.WriteLine($"Base Price: ${order.BasePrice:F2}");
                Console.WriteLine($"Regional Tax (12%): ${taxed.GetTotal():F2}");
                Console.WriteLine($"International Shipping: ${shipped.GetTotal():F2}");
                Console.WriteLine($"Transit Insurance (3%): ${insured.GetTotal():F2}");
                Console.WriteLine($"Total: ${insured.GetTotal():F2}\n");
            }

            // Summary
            Console.WriteLine("════════════════════════════════════════════════════════════════");
            Console.WriteLine("  KEY BENEFITS OF DECORATOR PATTERN");
            Console.WriteLine("════════════════════════════════════════════════════════════════");
            Console.WriteLine("✓ No Class Explosion - 1 Order + 4 Decorators handles all combinations");
            Console.WriteLine("✓ Flexible Composition - Mix decorators in any order");
            Console.WriteLine("✓ No Code Duplication - Each decorator handles one concern");
            Console.WriteLine("✓ Dynamic Runtime Changes - Apply decorators based on conditions");
            Console.WriteLine("✓ Easy to Test - Test each decorator independently");
            Console.WriteLine("✓ Easy to Extend - Add new decorators without changing existing code");
            Console.WriteLine("✓ Clear Responsibilities - Single Responsibility Principle applied");
            Console.WriteLine("\n════════════════════════════════════════════════════════════════");
        }
    }
}
