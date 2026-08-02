using NUnit.Framework;
using CompilerAST.After.Context;

namespace CompilerAST.After.Tests
{
    [TestFixture]
    public class CompilerASTTests
    {
        [Test]
        public void TypeChecker_ValidateNumbers() 
        { 
            var num = new NumberNode { Value = 42.0 };
            var checker = new TypeChecker();
            num.Accept(checker);
            Assert.That(checker.Errors.Count, Is.EqualTo(0));
        }

        [Test]
        public void TypeChecker_UndefinedVariable()
        {
            var var_node = new VariableNode { Name = "x" };
            var checker = new TypeChecker();
            var_node.Accept(checker);
            Assert.That(checker.Errors.Count, Is.GreaterThan(0));
        }

        [Test]
        public void CodeGenerator_BinaryOp()
        {
            var left = new NumberNode { Value = 10 };
            var right = new NumberNode { Value = 5 };
            var binop = new BinaryOpNode { Operator = "+", Left = left, Right = right };
            var gen = new CodeGenerator();
            binop.Accept(gen);
            Assert.That(gen.Instructions.Count, Is.GreaterThan(0));
        }

        [Test]
        public void OptimizationAnalyzer_ConstantFolding()
        {
            var left = new NumberNode { Value = 10 };
            var right = new NumberNode { Value = 5 };
            var binop = new BinaryOpNode { Operator = "+", Left = left, Right = right };
            var analyzer = new OptimizationAnalyzer();
            binop.Accept(analyzer);
            Assert.That(analyzer.Optimizations.Count, Is.GreaterThan(0));
        }

        [Test]
        public void FunctionCall_CodeGeneration()
        {
            var func = new FunctionCallNode { FunctionName = "sqrt", Arguments = new() { new NumberNode { Value = 16 } } };
            var gen = new CodeGenerator();
            func.Accept(gen);
            Assert.That(gen.Instructions.Count, Is.GreaterThan(0));
        }
    }
}
