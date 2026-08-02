using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlParser.After.Context
{
    /// <summary>
    /// Abstract expression
    /// </summary>
    public abstract class Expression
    {
        public abstract string Interpret();
    }

    /// <summary>
    /// Terminal expressions
    /// </summary>
    public class Column : Expression
    {
        private string _name;
        public Column(string name) => _name = name;
        public override string Interpret() => _name;
    }

    public class Value : Expression
    {
        private string _value;
        public Value(string value) => _value = value;
        public override string Interpret() => $"'{_value}'";
    }

    public class Operator : Expression
    {
        private string _op;
        public Operator(string op) => _op = op;
        public override string Interpret() => _op;
    }

    /// <summary>
    /// Nonterminal expressions
    /// </summary>
    public class Condition : Expression
    {
        private Expression _left, _operator, _right;

        public Condition(Expression left, Expression op, Expression right)
        {
            _left = left;
            _operator = op;
            _right = right;
        }

        public override string Interpret() 
            => $"{_left.Interpret()} {_operator.Interpret()} {_right.Interpret()}";
    }

    public class AndExpression : Expression
    {
        private Expression _left, _right;

        public AndExpression(Expression left, Expression right)
        {
            _left = left;
            _right = right;
        }

        public override string Interpret() 
            => $"({_left.Interpret()} AND {_right.Interpret()})";
    }

    public class OrExpression : Expression
    {
        private Expression _left, _right;

        public OrExpression(Expression left, Expression right)
        {
            _left = left;
            _right = right;
        }

        public override string Interpret() 
            => $"({_left.Interpret()} OR {_right.Interpret()})";
    }

    public class SelectStatement : Expression
    {
        private List<string> _columns;
        private string _table;
        private Expression _whereClause;

        public SelectStatement(List<string> columns, string table, Expression whereClause = null)
        {
            _columns = columns;
            _table = table;
            _whereClause = whereClause;
        }

        public override string Interpret()
        {
            var sql = $"SELECT {string.Join(", ", _columns)} FROM {_table}";
            if (_whereClause != null)
                sql += $" WHERE {_whereClause.Interpret()}";
            return sql;
        }
    }

    /// <summary>
    /// SQL Parser Context
    /// </summary>
    public class SqlParserContext
    {
        public Expression ParseSimpleCondition(string column, string op, string value)
            => new Condition(new Column(column), new Operator(op), new Value(value));

        public Expression ParseAndCondition(Expression left, Expression right)
            => new AndExpression(left, right);

        public Expression ParseOrCondition(Expression left, Expression right)
            => new OrExpression(left, right);

        public Expression ParseSelect(List<string> columns, string table, Expression where = null)
            => new SelectStatement(columns, table, where);
    }
}
