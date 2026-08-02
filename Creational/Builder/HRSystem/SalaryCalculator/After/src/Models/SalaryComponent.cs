namespace SalaryCalculator.After.Models
{
    /// <summary>
    /// SalaryComponent: Represents a single component of salary (bonus, tax, etc.)
    /// SRP: Only responsible for storing component data (earning or deduction)
    /// </summary>
    public class SalaryComponent
    {
        public string Name { get; set; } = "";
        public decimal Amount { get; set; }
        public ComponentType Type { get; set; }

        public SalaryComponent(string name, decimal amount, ComponentType type)
        {
            Name = name;
            Amount = amount;
            Type = type;
        }

        public override string ToString() => $"{Name}: ${Amount:F2} ({Type})";
    }
}
