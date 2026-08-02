using System;
using System.Text.RegularExpressions;

namespace RegexMatcher.After.Context
{
    public abstract class RegexExpression
    {
        public abstract bool Match(string input);
        public abstract string GetPattern();
    }

    public class LiteralCharacter : RegexExpression
    {
        private char _char;

        public LiteralCharacter(char ch) => _char = ch;

        public override bool Match(string input) => input.Length > 0 && input[0] == _char;

        public override string GetPattern() => _char.ToString();
    }

    public class AnyCharacter : RegexExpression
    {
        public override bool Match(string input) => input.Length > 0;

        public override string GetPattern() => ".";
    }

    public class CharacterClass : RegexExpression
    {
        private string _chars;

        public CharacterClass(string chars) => _chars = chars;

        public override bool Match(string input) => input.Length > 0 && _chars.Contains(input[0]);

        public override string GetPattern() => $"[{_chars}]";
    }

    public class ZeroOrMore : RegexExpression
    {
        private RegexExpression _expr;

        public ZeroOrMore(RegexExpression expr) => _expr = expr;

        public override bool Match(string input)
        {
            int pos = 0;
            while (pos < input.Length && _expr.Match(input.Substring(pos)))
                pos++;
            return true;
        }

        public override string GetPattern() => $"({_expr.GetPattern()})*";
    }

    public class OneOrMore : RegexExpression
    {
        private RegexExpression _expr;

        public OneOrMore(RegexExpression expr) => _expr = expr;

        public override bool Match(string input)
        {
            if (!_expr.Match(input))
                return false;
            int pos = 1;
            while (pos < input.Length && _expr.Match(input.Substring(pos)))
                pos++;
            return true;
        }

        public override string GetPattern() => $"({_expr.GetPattern()})+";
    }

    public class Optional : RegexExpression
    {
        private RegexExpression _expr;

        public Optional(RegexExpression expr) => _expr = expr;

        public override bool Match(string input) => input.Length == 0 || _expr.Match(input);

        public override string GetPattern() => $"({_expr.GetPattern()})?";
    }

    public class Sequence : RegexExpression
    {
        private RegexExpression[] _expressions;

        public Sequence(params RegexExpression[] expressions) => _expressions = expressions;

        public override bool Match(string input)
        {
            int pos = 0;
            foreach (var expr in _expressions)
            {
                if (pos >= input.Length)
                    return false;
                if (!expr.Match(input.Substring(pos)))
                    return false;
                pos++;
            }
            return pos == input.Length;
        }

        public override string GetPattern() 
            => string.Concat(Array.ConvertAll(_expressions, e => e.GetPattern()));
    }

    public class Alternation : RegexExpression
    {
        private RegexExpression _left, _right;

        public Alternation(RegexExpression left, RegexExpression right)
        {
            _left = left;
            _right = right;
        }

        public override bool Match(string input) 
            => _left.Match(input) || _right.Match(input);

        public override string GetPattern() => $"({_left.GetPattern()}|{_right.GetPattern()})";
    }

    public class SimpleRegexValidator
    {
        public bool ValidateEmail(string email)
        {
            var pattern = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
            return pattern.IsMatch(email);
        }

        public bool ValidatePhoneNumber(string phone)
        {
            var pattern = new Regex(@"^\d{3}-\d{3}-\d{4}$");
            return pattern.IsMatch(phone);
        }

        public bool ValidateUrl(string url)
        {
            var pattern = new Regex(@"^https?://[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}");
            return pattern.IsMatch(url);
        }
    }
}
