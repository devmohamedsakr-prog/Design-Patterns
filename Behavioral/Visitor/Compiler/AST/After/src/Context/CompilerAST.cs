using System;
using System.Collections.Generic;

namespace CompilerAST.After.Context
{
    public interface IASTNode
    {
        void Accept(IASTVisitor visitor);
    }

    public interface IASTVisitor
    {
        void Visit(NumberNode node);
        void Visit(BinaryOpNode node);
        void Visit(UnaryOpNode node);
        void Visit(VariableNode node);
        void Visit(FunctionCallNode node);
    }

    public class NumberNode : IASTNode
    {
        public double Value { get; set; }
        public void Accept(IASTVisitor visitor) => visitor.Visit(this);
    }

    public class VariableNode : IASTNode
    {
        public string Name { get; set; } = "";
        public void Accept(IASTVisitor visitor) => visitor.Visit(this);
    }

    public class BinaryOpNode : IASTNode
    {
        public string Operator { get; set; } = "";
        public IASTNode Left { get; set; }
        public IASTNode Right { get; set; }
        public void Accept(IASTVisitor visitor) => visitor.Visit(this);
    }

    public class UnaryOpNode : IASTNode
    {
        public string Operator { get; set; } = "";
        public IASTNode Operand { get; set; }
        public void Accept(IASTVisitor visitor) => visitor.Visit(this);
    }

    public class FunctionCallNode : IASTNode
    {
        public string FunctionName { get; set; } = "";
        public List<IASTNode> Arguments { get; set; } = new();
        public void Accept(IASTVisitor visitor) => visitor.Visit(this);
    }

    public class TypeChecker : IASTVisitor
    {
        public List<string> Errors { get; set; } = new();
        public Dictionary<string, string> VariableTypes { get; set; } = new();

        public void Visit(NumberNode node)
        {
            Console.WriteLine($"✓ Type: Number = double");
        }

        public void Visit(VariableNode node)
        {
            if (!VariableTypes.ContainsKey(node.Name))
                Errors.Add($"Undefined variable: {node.Name}");
            else
                Console.WriteLine($"✓ Type: {node.Name} = {VariableTypes[node.Name]}");
        }

        public void Visit(BinaryOpNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            Console.WriteLine($"✓ Binary operation: {node.Operator}");
        }

        public void Visit(UnaryOpNode node)
        {
            node.Operand.Accept(this);
            Console.WriteLine($"✓ Unary operation: {node.Operator}");
        }

        public void Visit(FunctionCallNode node)
        {
            Console.WriteLine($"✓ Function call: {node.FunctionName}({node.Arguments.Count} args)");
        }
    }

    public class CodeGenerator : IASTVisitor
    {
        public List<string> Instructions { get; set; } = new();

        public void Visit(NumberNode node)
        {
            Instructions.Add($"PUSH {node.Value}");
        }

        public void Visit(VariableNode node)
        {
            Instructions.Add($"LOAD {node.Name}");
        }

        public void Visit(BinaryOpNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            Instructions.Add($"OP {node.Operator}");
        }

        public void Visit(UnaryOpNode node)
        {
            node.Operand.Accept(this);
            Instructions.Add($"UNARY {node.Operator}");
        }

        public void Visit(FunctionCallNode node)
        {
            foreach (var arg in node.Arguments)
                arg.Accept(this);
            Instructions.Add($"CALL {node.FunctionName}");
        }
    }

    public class OptimizationAnalyzer : IASTVisitor
    {
        public List<string> Optimizations { get; set; } = new();

        public void Visit(NumberNode node) { }

        public void Visit(VariableNode node) { }

        public void Visit(BinaryOpNode node)
        {
            if (node.Left is NumberNode ln && node.Right is NumberNode rn)
            {
                Optimizations.Add($"Constant folding: {ln.Value} {node.Operator} {rn.Value}");
            }
            node.Left.Accept(this);
            node.Right.Accept(this);
        }

        public void Visit(UnaryOpNode node)
        {
            node.Operand.Accept(this);
        }

        public void Visit(FunctionCallNode node)
        {
            foreach (var arg in node.Arguments)
                arg.Accept(this);
        }
    }
}
