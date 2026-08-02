using NUnit.Framework;
using System;
using VacationApproval.After.Models;
using VacationApproval.After.Approvers;
using VacationApproval.After.Builders;

namespace VacationApproval.After.Tests
{
    [TestFixture]
    public class ApprovalChainTests
    {
        private VacationRequest _validRequest;

        [SetUp]
        public void Setup()
        {
            _validRequest = new VacationRequest("REQ001", "EMP001", "John Smith",
                DateTime.Now.AddDays(10), DateTime.Now.AddDays(15), 5, 1000, "Summer vacation");
        }

        // Individual approver tests
        [Test]
        public void ManagerApprover_ValidRequest()
        {
            var approver = new ManagerApprover("Manager A");
            var result = approver.Process(_validRequest);
            Assert.That(result.IsApproved, Is.True);
        }

        [Test]
        public void ManagerApprover_InvalidDays_Zero()
        {
            var approver = new ManagerApprover("Manager A");
            var request = new VacationRequest("REQ002", "EMP002", "Jane Doe",
                DateTime.Now.AddDays(10), DateTime.Now.AddDays(10), 0, 500);
            var result = approver.Process(request);
            Assert.That(result.IsApproved, Is.False);
        }

        [Test]
        public void ManagerApprover_ExceedsLimit()
        {
            var approver = new ManagerApprover("Manager A", maxDaysWithoutDirector: 5);
            var request = new VacationRequest("REQ003", "EMP003", "Bob Johnson",
                DateTime.Now.AddDays(10), DateTime.Now.AddDays(35), 25, 5000);
            var result = approver.Process(request);
            Assert.That(result.IsApproved, Is.False);
        }

        [Test]
        public void DirectorApprover_ValidRequest()
        {
            var approver = new DirectorApprover("Director A", maxDays: 15, budgetLimit: 5000);
            var request = new VacationRequest("REQ004", "EMP004", "Alice Williams",
                DateTime.Now.AddDays(10), DateTime.Now.AddDays(20), 10, 3000);
            var result = approver.Process(request);
            Assert.That(result.IsApproved, Is.True);
        }

        [Test]
        public void DirectorApprover_ExceedsBudget()
        {
            var approver = new DirectorApprover("Director A", maxDays: 15, budgetLimit: 5000);
            var request = new VacationRequest("REQ005", "EMP005", "Charlie Brown",
                DateTime.Now.AddDays(10), DateTime.Now.AddDays(20), 10, 8000);
            var result = approver.Process(request);
            Assert.That(result.IsApproved, Is.False);
        }

        [Test]
        public void ExecutiveApprover_ValidRequest()
        {
            var approver = new ExecutiveApprover("VP A", maxUrgentDays: 20);
            var request = new VacationRequest("REQ006", "EMP006", "David Lee",
                DateTime.Now.AddDays(10), DateTime.Now.AddDays(30), 20, 7000);
            var result = approver.Process(request);
            Assert.That(result.IsApproved, Is.True);
        }

        [Test]
        public void ExecutiveApprover_HighCost()
        {
            var approver = new ExecutiveApprover("VP A", highCostThreshold: 10000);
            var request = new VacationRequest("REQ007", "EMP007", "Eve Wilson",
                DateTime.Now.AddDays(10), DateTime.Now.AddDays(20), 10, 15000);
            var result = approver.Process(request);
            Assert.That(result.IsApproved, Is.False);
        }

        [Test]
        public void HRApprover_ValidRequest()
        {
            var approver = new HRApprover("HR Manager");
            var result = approver.Process(_validRequest);
            Assert.That(result.IsApproved, Is.True);
        }

        // Chain tests
        [Test]
        public void SimpleChain_AllApprove()
        {
            var chain = new ManagerApprover("Manager")
                .SetNext(new HRApprover("HR"));

            var result = chain.Process(_validRequest);
            Assert.That(result.IsApproved, Is.True);
        }

        [Test]
        public void CompleteChain_AllApprove()
        {
            var chain = new ManagerApprover("Manager", maxDaysWithoutDirector: 3)
                .SetNext(new DirectorApprover("Director", maxDays: 20, budgetLimit: 10000))
                .SetNext(new ExecutiveApprover("VP", maxUrgentDays: 30))
                .SetNext(new HRApprover("HR"));

            var result = chain.Process(_validRequest);
            Assert.That(result.IsApproved, Is.True);
        }

        [Test]
        public void CompleteChain_FailsAtManager()
        {
            var chain = new ManagerApprover("Manager", maxDaysWithoutDirector: 100)
                .SetNext(new DirectorApprover("Director"))
                .SetNext(new HRApprover("HR"));

            var request = new VacationRequest("REQ008", "EMP008", "Frank Miller",
                DateTime.Now.AddDays(10), DateTime.Now.AddDays(35), 25, 5000);
            var result = chain.Process(request);
            Assert.That(result.IsApproved, Is.False);
        }

        [Test]
        public void CompleteChain_FailsAtDirector()
        {
            var chain = new ManagerApprover("Manager", maxDaysWithoutDirector: 3)
                .SetNext(new DirectorApprover("Director", maxDays: 10, budgetLimit: 2000))
                .SetNext(new HRApprover("HR"));

            var request = new VacationRequest("REQ009", "EMP009", "Grace Lee",
                DateTime.Now.AddDays(10), DateTime.Now.AddDays(25), 15, 5000);
            var result = chain.Process(request);
            Assert.That(result.IsApproved, Is.False);
        }

        // Builder tests
        [Test]
        public void Builder_CreateStandardChain()
        {
            var builder = new ApprovalChainBuilder()
                .AddManagerApproval("Manager A")
                .AddDirectorApproval("Director B")
                .AddHRApproval("HR C");

            var result = builder.Process(_validRequest);
            Assert.That(result.IsApproved, Is.True);
        }

        [Test]
        public void Builder_CreateFullChain()
        {
            var builder = new ApprovalChainBuilder()
                .AddManagerApproval("Manager")
                .AddDirectorApproval("Director")
                .AddExecutiveApproval("VP")
                .AddHRApproval("HR");

            var result = builder.Process(_validRequest);
            Assert.That(result.IsApproved, Is.True);
        }

        [Test]
        public void Builder_SelectiveApprovers()
        {
            var builder = new ApprovalChainBuilder()
                .AddManagerApproval("Manager")
                .AddHRApproval("HR");

            var result = builder.Process(_validRequest);
            Assert.That(result.IsApproved, Is.True);
        }

        [Test]
        public void Builder_EmptyChain_Throws()
        {
            var builder = new ApprovalChainBuilder();
            Assert.Throws<InvalidOperationException>(() => builder.Build());
        }

        // Real-world scenarios
        [Test]
        public void Scenario_StandardVacation()
        {
            var builder = new ApprovalChainBuilder()
                .AddManagerApproval("John (Manager)")
                .AddDirectorApproval("Sarah (Director)")
                .AddHRApproval("HR Team");

            var request = new VacationRequest("VAC001", "EMP100", "Michael Chen",
                DateTime.Now.AddDays(15), DateTime.Now.AddDays(20), 5, 1500, "Summer trip");

            var result = builder.Process(request);
            Assert.That(result.IsApproved, Is.True);
        }

        [Test]
        public void Scenario_ExtendedVacation()
        {
            var builder = new ApprovalChainBuilder()
                .AddManagerApproval("John", maxDaysWithoutDirector: 3)
                .AddDirectorApproval("Sarah", maxDays: 20, budgetLimit: 8000)
                .AddHRApproval("HR");

            var request = new VacationRequest("VAC002", "EMP101", "Lisa Park",
                DateTime.Now.AddDays(20), DateTime.Now.AddDays(35), 15, 5000, "International trip");

            var result = builder.Process(request);
            Assert.That(result.IsApproved, Is.True);
        }

        [Test]
        public void Scenario_HighCostVacation()
        {
            var builder = new ApprovalChainBuilder()
                .AddManagerApproval("John")
                .AddDirectorApproval("Sarah", maxDays: 30, budgetLimit: 15000)
                .AddExecutiveApproval("VP Tom", maxUrgentDays: 30, highCostThreshold: 20000)
                .AddHRApproval("HR");

            var request = new VacationRequest("VAC003", "EMP102", "Robert Davis",
                DateTime.Now.AddDays(25), DateTime.Now.AddDays(40), 15, 12000, "Executive conference + vacation");

            var result = builder.Process(request);
            Assert.That(result.IsApproved, Is.True);
        }

        [Test]
        public void Scenario_UrgentRequest()
        {
            var builder = new ApprovalChainBuilder()
                .AddManagerApproval("John")
                .AddDirectorApproval("Sarah")
                .AddExecutiveApproval("VP", maxUrgentDays: 15)
                .AddHRApproval("HR");

            var request = new VacationRequest("VAC004", "EMP103", "Susan Martinez",
                DateTime.Now.AddDays(5), DateTime.Now.AddDays(12), 7, 2000, "Family emergency")
            {
                IsUrgent = true
            };

            var result = builder.Process(request);
            Assert.That(result.IsApproved, Is.True);
        }

        [Test]
        public void DifferentApproverOrder()
        {
            // Director first, then Manager
            var chain = new DirectorApprover("Sarah")
                .SetNext(new ManagerApprover("John"))
                .SetNext(new HRApprover("HR"));

            var result = chain.Process(_validRequest);
            Assert.That(result.IsApproved, Is.True);
        }

        [Test]
        public void SingleApprover_Manager()
        {
            var chain = new ManagerApprover("John", maxDaysWithoutDirector: 10);
            var result = chain.Process(_validRequest);
            Assert.That(result.IsApproved, Is.True);
        }
    }
}
