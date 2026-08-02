using System;
using System.Collections.Generic;

namespace RulesEngine.After.Context
{
    public class Order
    {
        public decimal Amount { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; } = "";
    }

    public class Customer
    {
        public int Age { get; set; }
        public decimal CreditScore { get; set; }
        public int YearsMember { get; set; }
    }

    public abstract class RuleExpression
    {
        public abstract bool Evaluate(Customer customer, Order order);
    }

    public class AgeCondition : RuleExpression
    {
        private int _threshold;

        public AgeCondition(int age) => _threshold = age;

        public override bool Evaluate(Customer customer, Order order) => customer.Age > _threshold;
    }

    public class OrderAmountCondition : RuleExpression
    {
        private decimal _amount;

        public OrderAmountCondition(decimal amount) => _amount = amount;

        public override bool Evaluate(Customer customer, Order order) => order.Amount > _amount;
    }

    public class CreditScoreCondition : RuleExpression
    {
        private decimal _score;

        public CreditScoreCondition(decimal score) => _score = score;

        public override bool Evaluate(Customer customer, Order order) => customer.CreditScore >= _score;
    }

    public class LoyaltyCondition : RuleExpression
    {
        private int _years;

        public LoyaltyCondition(int years) => _years = years;

        public override bool Evaluate(Customer customer, Order order) => customer.YearsMember >= _years;
    }

    public class AndRule : RuleExpression
    {
        private RuleExpression _left, _right;

        public AndRule(RuleExpression left, RuleExpression right)
        {
            _left = left;
            _right = right;
        }

        public override bool Evaluate(Customer customer, Order order)
            => _left.Evaluate(customer, order) && _right.Evaluate(customer, order);
    }

    public class OrRule : RuleExpression
    {
        private RuleExpression _left, _right;

        public OrRule(RuleExpression left, RuleExpression right)
        {
            _left = left;
            _right = right;
        }

        public override bool Evaluate(Customer customer, Order order)
            => _left.Evaluate(customer, order) || _right.Evaluate(customer, order);
    }

    public class NotRule : RuleExpression
    {
        private RuleExpression _expr;

        public NotRule(RuleExpression expr) => _expr = expr;

        public override bool Evaluate(Customer customer, Order order)
            => !_expr.Evaluate(customer, order);
    }

    public class DiscountRule
    {
        public RuleExpression Condition { get; set; }
        public decimal DiscountPercent { get; set; }

        public DiscountRule(RuleExpression condition, decimal discount)
        {
            Condition = condition;
            DiscountPercent = discount;
        }

        public decimal CalculateDiscount(Customer customer, Order order)
            => Condition.Evaluate(customer, order) ? order.Amount * (DiscountPercent / 100) : 0;
    }

    public class RulesEngine
    {
        private List<DiscountRule> _rules = new();

        public void AddRule(DiscountRule rule) => _rules.Add(rule);

        public decimal CalculateTotalDiscount(Customer customer, Order order)
        {
            decimal totalDiscount = 0;
            foreach (var rule in _rules)
                totalDiscount += rule.CalculateDiscount(customer, order);
            return totalDiscount;
        }
    }
}
