using System;

namespace ArithmeticInterpreter.After.Context
{
    public abstract class Expression
    {
        public abstract int Interpret();
    }

    public class Number : Expression
    {
        private int _value;
        public Number(int value) => _value = value;
        public override int Interpret() => _value;
    }

    public class Add : Expression
    {
        private Expression _left, _right;

        public Add(Expression left, Expression right)
        {
            _left = left;
            _right = right;
        }

        public override int Interpret() => _left.Interpret() + _right.Interpret();
    }

    public class Subtract : Expression
    {
        private Expression _left, _right;

        public Subtract(Expression left, Expression right)
        {
            _left = left;
            _right = right;
        }

        public override int Interpret() => _left.Interpret() - _right.Interpret();
    }

    public class Multiply : Expression
    {
        private Expression _left, _right;

        public Multiply(Expression left, Expression right)
        {
            _left = left;
            _right = right;
        }

        public override int Interpret() => _left.Interpret() * _right.Interpret();
    }

    public class Divide : Expression
    {
        private Expression _left, _right;

        public Divide(Expression left, Expression right)
        {
            _left = left;
            _right = right;
        }

        public override int Interpret()
        {
            var divisor = _right.Interpret();
            if (divisor == 0)
                throw new InvalidOperationException("Division by zero");
            return _left.Interpret() / divisor;
        }
    }

    public class Modulo : Expression
    {
        private Expression _left, _right;

        public Modulo(Expression left, Expression right)
        {
            _left = left;
            _right = right;
        }

        public override int Interpret() => _left.Interpret() % _right.Interpret();
    }

    public class Negate : Expression
    {
        private Expression _expr;

        public Negate(Expression expr) => _expr = expr;

        public override int Interpret() => -_expr.Interpret();
    }

    public class ExpressionParser
    {
        public Expression Parse(string expression)
        {
            var tokens = expression.Split(' ');
            return BuildExpression(tokens, ref _index = 0);
        }

        private int _index = 0;

        private Expression BuildExpression(string[] tokens, ref int index)
        {
            var left = BuildTerm(tokens, ref index);
            
            while (index < tokens.Length && (tokens[index] == "+" || tokens[index] == "-"))
            {
                var op = tokens[index++];
                var right = BuildTerm(tokens, ref index);
                left = op == "+" ? new Add(left, right) : (Expression)new Subtract(left, right);
            }
            
            return left;
        }

        private Expression BuildTerm(string[] tokens, ref int index)
        {
            var left = BuildFactor(tokens, ref index);
            
            while (index < tokens.Length && (tokens[index] == "*" || tokens[index] == "/" || tokens[index] == "%"))
            {
                var op = tokens[index++];
                var right = BuildFactor(tokens, ref index);
                left = op == "*" ? new Multiply(left, right) : 
                       op == "/" ? new Divide(left, right) : (Expression)new Modulo(left, right);
            }
            
            return left;
        }

        private Expression BuildFactor(string[] tokens, ref int index)
        {
            var token = tokens[index++];
            
            if (token == "-")
                return new Negate(BuildFactor(tokens, ref index));
            
            if (int.TryParse(token, out var num))
                return new Number(num);
            
            throw new InvalidOperationException($"Unknown token: {token}");
        }
    }
}
