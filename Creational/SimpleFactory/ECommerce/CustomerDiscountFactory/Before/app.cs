using System;
using System.Collections.Generic;

// ❌ PROBLEM: Hard-coded discount levels everywhere

public class Customer
{
    public string CustomerId { get; set; }
    public string Name { get; set; }
    public string TierLevel { get; set; } // "Silver", "Gold", "Bronze"
    public decimal Balance { get; set; }
}

public class OrderProcessor
{
    // ❌ PROBLEM: Discount logic scattered and duplicated
    public decimal CalculateDiscount(Customer customer, decimal orderTotal)
    {
        // Hard-coded discount rules by tier
        if (customer.TierLevel == "Bronze")
        {
            return orderTotal * 0.05m; // 5% discount
        }
        else if (customer.TierLevel == "Silver")
        {
            return orderTotal * 0.10m; // 10% discount
        }
        else if (customer.TierLevel == "Gold")
        {
            return orderTotal * 0.15m; // 15% discount
        }
        else
        {
            return 0; // No discount for regular customers
        }
    }

    // ❌ PROBLEM: Shipping calculation also hard-coded
    public decimal CalculateShipping(Customer customer, decimal orderTotal)
    {
        if (customer.TierLevel == "Bronze")
        {
            return 10m; // Fixed shipping
        }
        else if (customer.TierLevel == "Silver")
        {
            return 5m; // Reduced shipping
        }
        else if (customer.TierLevel == "Gold")
        {
            return 0m; // Free shipping
        }
        else
        {
            return 15m; // Standard shipping
        }
    }

    // ❌ PROBLEM: Loyalty points also hard-coded
    public int CalculateLoyaltyPoints(Customer customer, decimal orderTotal)
    {
        if (customer.TierLevel == "Bronze")
        {
            return (int)(orderTotal / 10); // 1 point per $10
        }
        else if (customer.TierLevel == "Silver")
        {
            return (int)(orderTotal / 5); // 1 point per $5
        }
        else if (customer.TierLevel == "Gold")
        {
            return (int)(orderTotal / 2); // 1 point per $2
        }
        else
        {
            return 0;
        }
    }
}

// ❌ PROBLEM: Customer service also duplicates logic
public class CustomerService
{
    public void UpgradeCustomer(Customer customer)
    {
        // ❌ Duplicate tier checking logic
        if (customer.Balance > 5000 && customer.TierLevel == "Bronze")
        {
            customer.TierLevel = "Silver";
            Console.WriteLine($"✓ {customer.Name} upgraded to Silver");
        }
        else if (customer.Balance > 15000 && customer.TierLevel == "Silver")
        {
            customer.TierLevel = "Gold";
            Console.WriteLine($"✓ {customer.Name} upgraded to Gold");
        }
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("❌ PROBLEM: Hard-coded discount levels\n");

        var bronze = new Customer { CustomerId = "C001", Name = "John", TierLevel = "Bronze" };
        var silver = new Customer { CustomerId = "C002", Name = "Jane", TierLevel = "Silver" };
        var gold = new Customer { CustomerId = "C003", Name = "Jack", TierLevel = "Gold" };

        var processor = new OrderProcessor();
        decimal orderTotal = 100m;

        Console.WriteLine($"Order Total: ${orderTotal}\n");

        // Bronze Customer
        var bronzeDiscount = processor.CalculateDiscount(bronze, orderTotal);
        var bronzeShipping = processor.CalculateShipping(bronze, orderTotal);
        var bronzePoints = processor.CalculateLoyaltyPoints(bronze, orderTotal);
        Console.WriteLine($"Bronze ({bronze.Name}):");
        Console.WriteLine($"  Discount: ${bronzeDiscount}");
        Console.WriteLine($"  Shipping: ${bronzeShipping}");
        Console.WriteLine($"  Loyalty Points: {bronzePoints}");
        Console.WriteLine($"  Final Total: ${orderTotal - bronzeDiscount + bronzeShipping}\n");

        // Silver Customer
        var silverDiscount = processor.CalculateDiscount(silver, orderTotal);
        var silverShipping = processor.CalculateShipping(silver, orderTotal);
        var silverPoints = processor.CalculateLoyaltyPoints(silver, orderTotal);
        Console.WriteLine($"Silver ({silver.Name}):");
        Console.WriteLine($"  Discount: ${silverDiscount}");
        Console.WriteLine($"  Shipping: ${silverShipping}");
        Console.WriteLine($"  Loyalty Points: {silverPoints}");
        Console.WriteLine($"  Final Total: ${orderTotal - silverDiscount + silverShipping}\n");

        // Gold Customer
        var goldDiscount = processor.CalculateDiscount(gold, orderTotal);
        var goldShipping = processor.CalculateShipping(gold, orderTotal);
        var goldPoints = processor.CalculateLoyaltyPoints(gold, orderTotal);
        Console.WriteLine($"Gold ({gold.Name}):");
        Console.WriteLine($"  Discount: ${goldDiscount}");
        Console.WriteLine($"  Shipping: ${goldShipping}");
        Console.WriteLine($"  Loyalty Points: {goldPoints}");
        Console.WriteLine($"  Final Total: ${orderTotal - goldDiscount + goldShipping}\n");

        Console.WriteLine("⚠️  PROBLEMS:");
        Console.WriteLine("  • Discount logic duplicated in 3 methods");
        Console.WriteLine("  • If we need to change Bronze discount, update 3 places");
        Console.WriteLine("  • Adding new tier requires changes everywhere");
        Console.WriteLine("  • Hard-coded tier names (\"Bronze\", \"Silver\", \"Gold\")");
        Console.WriteLine("  • No way to extend without code modification");
    }
}
