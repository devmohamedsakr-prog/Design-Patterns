using NUnit.Framework;
using ExpenseApproval.After.Context;

namespace ExpenseApproval.After.Tests
{
    [TestFixture]
    public class ExpenseApprovalTests
    {
        private ExpenseApprovalChain _chain;

        [SetUp]
        public void Setup() => _chain = new ExpenseApprovalChain();

        [Test]
        public void TeamLeadApproval_LowAmount()
        {
            var expense = new ExpenseRequest { EmployeeId = "E001", Amount = 300, Description = "Office supplies" };
            _chain.ProcessExpense(expense);
            
            Assert.That(expense.Status, Is.EqualTo("Approved"));
            Assert.That(expense.ApprovedBy, Is.EqualTo("TeamLead"));
        }

        [Test]
        public void ManagerApproval_MediumAmount()
        {
            var expense = new ExpenseRequest { EmployeeId = "E002", Amount = 2000, Description = "Conference ticket" };
            _chain.ProcessExpense(expense);
            
            Assert.That(expense.Status, Is.EqualTo("Approved"));
            Assert.That(expense.ApprovedBy, Is.EqualTo("DepartmentManager"));
        }

        [Test]
        public void DirectorApproval_HighAmount()
        {
            var expense = new ExpenseRequest { EmployeeId = "E003", Amount = 30000, Description = "Equipment purchase" };
            _chain.ProcessExpense(expense);
            
            Assert.That(expense.Status, Is.EqualTo("Approved"));
            Assert.That(expense.ApprovedBy, Is.EqualTo("Director"));
        }

        [Test]
        public void VpApproval_MaximumAmount()
        {
            var expense = new ExpenseRequest { EmployeeId = "E004", Amount = 100000, Description = "Major investment" };
            _chain.ProcessExpense(expense);
            
            Assert.That(expense.Status, Is.EqualTo("Approved"));
            Assert.That(expense.ApprovedBy, Is.EqualTo("VpFinance"));
        }

        [Test]
        public void Escalation_ThroughMultipleLevels()
        {
            var expense = new ExpenseRequest { EmployeeId = "E005", Amount = 5000, Description = "Travel expenses" };
            _chain.ProcessExpense(expense);
            
            Assert.That(expense.Status, Is.EqualTo("Approved"));
        }
    }
}
