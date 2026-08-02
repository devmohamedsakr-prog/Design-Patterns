using NUnit.Framework;
using OrderProcessing.After.Abstracts;
using OrderProcessing.After.Commands;

namespace OrderProcessing.After.Tests
{
    [TestFixture]
    public class OrderProcessingTests
    {
        private OrderInvoker _invoker;
        private Order _order;

        [SetUp]
        public void Setup()
        {
            _invoker = new OrderInvoker();
            _order = new Order { OrderId = "ORD-001", CustomerId = "CUST-001", Total = 100m, AccountBalance = 500m };
        }

        [Test] public void ValidateOrder_Succeeds() => Assert.That(_invoker.ExecuteCommand(new ValidateOrderCommand(_order)), Is.True);
        [Test] public void ValidateOrder_SetsValid() { _invoker.ExecuteCommand(new ValidateOrderCommand(_order)); Assert.That(_order.IsValid, Is.True); }
        [Test] public void ValidateOrder_Undo() 
        { 
            _invoker.ExecuteCommand(new ValidateOrderCommand(_order));
            Assert.That(_invoker.UndoCommand(), Is.True);
            Assert.That(_order.IsValid, Is.False);
        }

        [Test] public void ProcessPayment_RequiresValidation() => Assert.That(_invoker.ExecuteCommand(new ProcessPaymentCommand(_order)), Is.False);
        [Test] public void ProcessPayment_AfterValidation()
        {
            _invoker.ExecuteCommand(new ValidateOrderCommand(_order));
            Assert.That(_invoker.ExecuteCommand(new ProcessPaymentCommand(_order)), Is.True);
            Assert.That(_order.PaymentProcessed, Is.True);
        }
        [Test] public void ProcessPayment_DeductsBalance()
        {
            _invoker.ExecuteCommand(new ValidateOrderCommand(_order));
            _invoker.ExecuteCommand(new ProcessPaymentCommand(_order));
            Assert.That(_order.AccountBalance, Is.EqualTo(400m));
        }
        [Test] public void ProcessPayment_Undo()
        {
            _invoker.ExecuteCommand(new ValidateOrderCommand(_order));
            _invoker.ExecuteCommand(new ProcessPaymentCommand(_order));
            _invoker.UndoCommand();
            Assert.That(_order.AccountBalance, Is.EqualTo(500m));
        }

        [Test] public void ReserveInventory_RequiresPayment() => Assert.That(_invoker.ExecuteCommand(new ReserveInventoryCommand(_order)), Is.False);
        [Test] public void ReserveInventory_AfterPayment()
        {
            _invoker.ExecuteCommand(new ValidateOrderCommand(_order));
            _invoker.ExecuteCommand(new ProcessPaymentCommand(_order));
            Assert.That(_invoker.ExecuteCommand(new ReserveInventoryCommand(_order)), Is.True);
        }

        [Test] public void ShipOrder_RequiresInventory() => Assert.That(_invoker.ExecuteCommand(new ShipOrderCommand(_order)), Is.False);
        [Test] public void ShipOrder_FullPipeline()
        {
            _invoker.ExecuteCommand(new ValidateOrderCommand(_order));
            _invoker.ExecuteCommand(new ProcessPaymentCommand(_order));
            _invoker.ExecuteCommand(new ReserveInventoryCommand(_order));
            Assert.That(_invoker.ExecuteCommand(new ShipOrderCommand(_order)), Is.True);
            Assert.That(_order.Shipped, Is.True);
        }

        [Test] public void CommandHistory_Tracked() 
        { 
            _invoker.ExecuteCommand(new ValidateOrderCommand(_order));
            Assert.That(_invoker.GetUndoCount(), Is.EqualTo(1));
        }

        [Test] public void UndoAll_Commands()
        {
            _invoker.ExecuteCommand(new ValidateOrderCommand(_order));
            _invoker.ExecuteCommand(new ProcessPaymentCommand(_order));
            _invoker.ExecuteCommand(new ReserveInventoryCommand(_order));
            _invoker.ExecuteCommand(new ShipOrderCommand(_order));
            
            _invoker.UndoCommand();
            _invoker.UndoCommand();
            _invoker.UndoCommand();
            _invoker.UndoCommand();

            Assert.That(_order.IsValid, Is.False);
            Assert.That(_order.PaymentProcessed, Is.False);
        }

        [Test] public void InvalidOrder_FailsValidation() 
        { 
            _order.CustomerId = null;
            Assert.That(_invoker.ExecuteCommand(new ValidateOrderCommand(_order)), Is.False);
        }

        [Test] public void InsufficientBalance_FailsPayment()
        {
            _order.AccountBalance = 50m;
            _invoker.ExecuteCommand(new ValidateOrderCommand(_order));
            Assert.That(_invoker.ExecuteCommand(new ProcessPaymentCommand(_order)), Is.False);
        }
    }

    [TestFixture]
    public class CommandUndoRedoTests
    {
        private OrderInvoker _invoker;
        private Order _order;

        [SetUp]
        public void Setup()
        {
            _invoker = new OrderInvoker();
            _order = new Order { OrderId = "ORD-UNDO-001", CustomerId = "CUST-UNDO", Total = 100m, AccountBalance = 500m };
        }

        [Test]
        public void Redo_AfterUndo_Success()
        {
            _invoker.ExecuteCommand(new ValidateOrderCommand(_order));
            Assert.That(_invoker.GetUndoCount(), Is.EqualTo(1));
            Assert.That(_invoker.GetRedoCount(), Is.EqualTo(0));

            _invoker.UndoCommand();
            Assert.That(_invoker.GetUndoCount(), Is.EqualTo(0));
            Assert.That(_invoker.GetRedoCount(), Is.EqualTo(1));

            Assert.That(_invoker.RedoCommand(), Is.True);
            Assert.That(_invoker.GetUndoCount(), Is.EqualTo(1));
            Assert.That(_invoker.GetRedoCount(), Is.EqualTo(0));
        }

        [Test]
        public void CommandClearing_AfterNewExecution()
        {
            var order2 = new Order { OrderId = "ORD-REDO-001", CustomerId = "CUST-REDO", Total = 100m, AccountBalance = 500m };
            _invoker.ExecuteCommand(new ValidateOrderCommand(order2));
            _invoker.UndoCommand();

            // After undo, we have redo available
            Assert.That(_invoker.GetRedoCount(), Is.EqualTo(1));
            
            // New command clears redo
            _invoker.ExecuteCommand(new ValidateOrderCommand(order2));
            Assert.That(_invoker.GetRedoCount(), Is.EqualTo(0));
        }

        [Test]
        public void UndoMultiple_Commands()
        {
            _invoker.ExecuteCommand(new ValidateOrderCommand(_order));
            _invoker.ExecuteCommand(new ProcessPaymentCommand(_order));
            _invoker.ExecuteCommand(new ReserveInventoryCommand(_order));

            int undone = _invoker.UndoMultiple(2);
            Assert.That(undone, Is.EqualTo(2));
            Assert.That(_invoker.GetUndoCount(), Is.EqualTo(1));
        }

        [Test]
        public void RedoMultiple_Commands()
        {
            _invoker.ExecuteCommand(new ValidateOrderCommand(_order));
            _invoker.ExecuteCommand(new ProcessPaymentCommand(_order));
            _invoker.ExecuteCommand(new ReserveInventoryCommand(_order));

            _invoker.UndoMultiple(3);
            int redone = _invoker.RedoMultiple(2);

            Assert.That(redone, Is.EqualTo(2));
            Assert.That(_invoker.GetUndoCount(), Is.EqualTo(2));
            Assert.That(_invoker.GetRedoCount(), Is.EqualTo(1));
        }

        [Test]
        public void ComplexUndoRedo_Workflow()
        {
            // Execute 4 commands
            _invoker.ExecuteCommand(new ValidateOrderCommand(_order));
            _invoker.ExecuteCommand(new ProcessPaymentCommand(_order));
            _invoker.ExecuteCommand(new ReserveInventoryCommand(_order));
            _invoker.ExecuteCommand(new ShipOrderCommand(_order));

            // Undo 2
            _invoker.UndoCommand();
            _invoker.UndoCommand();

            // Verify state
            Assert.That(_order.Shipped, Is.False);
            Assert.That(_invoker.GetUndoCount(), Is.EqualTo(2));

            // Redo 1
            _invoker.RedoCommand();
            Assert.That(_invoker.GetUndoCount(), Is.EqualTo(3));
            Assert.That(_invoker.GetRedoCount(), Is.EqualTo(1));
        }

        [Test]
        public void CanUndo_CanRedo_Checks()
        {
            Assert.That(_invoker.CanUndo(), Is.False);
            Assert.That(_invoker.CanRedo(), Is.False);

            _invoker.ExecuteCommand(new ValidateOrderCommand(_order));
            Assert.That(_invoker.CanUndo(), Is.True);
            Assert.That(_invoker.CanRedo(), Is.False);

            _invoker.UndoCommand();
            Assert.That(_invoker.CanUndo(), Is.False);
            Assert.That(_invoker.CanRedo(), Is.True);
        }

        [Test]
        public void GetUndoRedoHistory()
        {
            _invoker.ExecuteCommand(new ValidateOrderCommand(_order));
            _invoker.ExecuteCommand(new ProcessPaymentCommand(_order));
            _invoker.ExecuteCommand(new ReserveInventoryCommand(_order));

            var history = _invoker.GetHistory();
            Assert.That(history.Count, Is.EqualTo(3));

            _invoker.UndoCommand();
            _invoker.UndoCommand();

            var redoHistory = _invoker.GetRedoHistory();
            Assert.That(redoHistory.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetFullHistory_AllExecutedCommands()
        {
            _invoker.ExecuteCommand(new ValidateOrderCommand(_order));
            _invoker.ExecuteCommand(new ProcessPaymentCommand(_order));
            _invoker.UndoCommand();
            _invoker.RedoCommand();
            _invoker.ExecuteCommand(new ReserveInventoryCommand(_order));

            var fullHistory = _invoker.GetFullHistory();
            Assert.That(fullHistory.Count, Is.EqualTo(3)); // 3 unique commands executed
        }

        [Test]
        public void ClearHistory_ResetsUndoRedo()
        {
            _invoker.ExecuteCommand(new ValidateOrderCommand(_order));
            _invoker.ExecuteCommand(new ProcessPaymentCommand(_order));
            _invoker.UndoCommand();

            _invoker.ClearHistory();
            Assert.That(_invoker.CanUndo(), Is.False);
            Assert.That(_invoker.CanRedo(), Is.False);
        }

        [Test]
        public void Transaction_CommitSuccess()
        {
            _invoker.BeginTransaction();
            _invoker.ExecuteCommand(new ValidateOrderCommand(_order));
            _invoker.ExecuteCommand(new ProcessPaymentCommand(_order));
            Assert.That(_invoker.CommitTransaction(), Is.True);
            Assert.That(_invoker.GetUndoCount(), Is.EqualTo(2));
        }

        [Test]
        public void Transaction_RollbackUndo()
        {
            _invoker.BeginTransaction();
            _invoker.ExecuteCommand(new ValidateOrderCommand(_order));
            _invoker.ExecuteCommand(new ProcessPaymentCommand(_order));
            
            Assert.That(_order.IsValid, Is.True);
            Assert.That(_order.AccountBalance, Is.EqualTo(400m));

            _invoker.RollbackTransaction();
            
            Assert.That(_order.IsValid, Is.False);
            Assert.That(_order.AccountBalance, Is.EqualTo(500m));
        }

        [Test]
        public void Transaction_CommitFail_NoActiveTransaction()
        {
            Assert.That(_invoker.CommitTransaction(), Is.False);
        }

        [Test]
        public void Transaction_RollbackFail_NoActiveTransaction()
        {
            Assert.That(_invoker.RollbackTransaction(), Is.False);
        }

        [Test]
        public void NestedTransaction_Support()
        {
            _invoker.BeginTransaction();
            _invoker.ExecuteCommand(new ValidateOrderCommand(_order));
            
            _invoker.BeginTransaction();
            _invoker.ExecuteCommand(new ProcessPaymentCommand(_order));
            
            _invoker.CommitTransaction();
            Assert.That(_invoker.GetUndoCount(), Is.EqualTo(2));
            
            _invoker.CommitTransaction();
            Assert.That(_invoker.GetUndoCount(), Is.EqualTo(2));
        }

        [Test]
        public void UndoStackSnapshot()
        {
            _invoker.ExecuteCommand(new ValidateOrderCommand(_order));
            _invoker.ExecuteCommand(new ProcessPaymentCommand(_order));

            var snapshot = _invoker.GetUndoStackSnapshot();
            Assert.That(snapshot.Length, Is.EqualTo(2));
        }

        [Test]
        public void RedoStackSnapshot()
        {
            _invoker.ExecuteCommand(new ValidateOrderCommand(_order));
            _invoker.ExecuteCommand(new ProcessPaymentCommand(_order));
            _invoker.UndoCommand();
            _invoker.UndoCommand();

            var snapshot = _invoker.GetRedoStackSnapshot();
            Assert.That(snapshot.Length, Is.EqualTo(2));
        }

        [Test]
        public void UndoRedo_SequentialOperations()
        {
            var order2 = new Order { OrderId = "ORD-MULTI-001", CustomerId = "CUST-MULTI", Total = 100m, AccountBalance = 500m };
            
            // Execute single command
            _invoker.ExecuteCommand(new ValidateOrderCommand(order2));
            Assert.That(_invoker.GetUndoCount(), Is.EqualTo(1));

            // Undo
            _invoker.UndoCommand();
            Assert.That(_invoker.GetUndoCount(), Is.EqualTo(0));
            Assert.That(_invoker.GetRedoCount(), Is.EqualTo(1));

            // Redo
            _invoker.RedoCommand();
            Assert.That(_invoker.GetUndoCount(), Is.EqualTo(1));
            Assert.That(_invoker.GetRedoCount(), Is.EqualTo(0));
        }

        [Test]
        public void StatePreservation_AfterUndoRedo()
        {
            decimal initialBalance = _order.AccountBalance;

            _invoker.ExecuteCommand(new ValidateOrderCommand(_order));
            _invoker.ExecuteCommand(new ProcessPaymentCommand(_order));
            
            Assert.That(_order.AccountBalance, Is.EqualTo(initialBalance - 100m));

            _invoker.UndoCommand();
            Assert.That(_order.AccountBalance, Is.EqualTo(initialBalance));

            _invoker.RedoCommand();
            Assert.That(_order.AccountBalance, Is.EqualTo(initialBalance - 100m));
        }
    }

    [TestFixture]
    public class OrderMacroTests
    {
        private OrderInvokerWithMacro _invokerWithMacro;
        private MacroRecorder _macroRecorder;
        private Order _order;

        [SetUp]
        public void Setup()
        {
            _invokerWithMacro = new OrderInvokerWithMacro();
            _macroRecorder = _invokerWithMacro.GetMacroRecorder();
            _order = new Order { OrderId = "ORD-002", CustomerId = "CUST-002", Total = 100m, AccountBalance = 500m };
        }

        [Test]
        public void StartMacroRecording_Success()
        {
            _macroRecorder.StartRecording("BulkOrderProcess");
            Assert.That(_macroRecorder.IsRecording, Is.True);
            Assert.That(_macroRecorder.GetMacroName(), Is.EqualTo("BulkOrderProcess"));
        }

        [Test]
        public void RecordCommand_DuringRecording()
        {
            _macroRecorder.StartRecording("TestMacro");
            _invokerWithMacro.ExecuteCommand(new ValidateOrderCommand(_order));
            _invokerWithMacro.ExecuteCommand(new ProcessPaymentCommand(_order));
            
            Assert.That(_macroRecorder.GetCommandCount(), Is.EqualTo(2));
        }

        [Test]
        public void StopRecording_Success()
        {
            _macroRecorder.StartRecording("TestMacro");
            _invokerWithMacro.ExecuteCommand(new ValidateOrderCommand(_order));
            Assert.That(_macroRecorder.StopRecording(), Is.True);
            Assert.That(_macroRecorder.IsRecording, Is.False);
        }

        [Test]
        public void PlayMacro_ExecutesAllCommands()
        {
            _macroRecorder.StartRecording("FullOrderFlow");
            _invokerWithMacro.ExecuteCommand(new ValidateOrderCommand(_order));
            _invokerWithMacro.ExecuteCommand(new ProcessPaymentCommand(_order));
            _invokerWithMacro.ExecuteCommand(new ReserveInventoryCommand(_order));
            _invokerWithMacro.ExecuteCommand(new ShipOrderCommand(_order));
            _macroRecorder.StopRecording();

            var newOrder = new Order { OrderId = "ORD-003", CustomerId = "CUST-003", Total = 100m, AccountBalance = 500m };
            var invokerNew = new OrderInvokerWithMacro();
            _macroRecorder.PlayMacro(invokerNew);

            Assert.That(_macroRecorder.GetCommandCount(), Is.EqualTo(4));
        }

        [Test]
        public void PlayMacroWithoutRecording_Fails()
        {
            _macroRecorder.StartRecording("EmptyMacro");
            _macroRecorder.StopRecording();
            
            var invokerNew = new OrderInvokerWithMacro();
            Assert.That(_macroRecorder.PlayMacro(invokerNew), Is.False);
        }

        [Test]
        public void SaveMacro_CreatesFile()
        {
            var tempFile = Path.Combine(Path.GetTempPath(), "test_macro.txt");
            
            _macroRecorder.StartRecording("SaveTest");
            _invokerWithMacro.ExecuteCommand(new ValidateOrderCommand(_order));
            _invokerWithMacro.ExecuteCommand(new ProcessPaymentCommand(_order));
            _macroRecorder.StopRecording();
            
            _macroRecorder.SaveMacro(tempFile);
            
            Assert.That(File.Exists(tempFile), Is.True);
            var lines = File.ReadAllLines(tempFile);
            Assert.That(lines[0], Does.Contain("SaveTest"));
            
            File.Delete(tempFile);
        }

        [Test]
        public void GetMacroSummary_ListsAllCommands()
        {
            _macroRecorder.StartRecording("SummaryTest");
            _invokerWithMacro.ExecuteCommand(new ValidateOrderCommand(_order));
            _invokerWithMacro.ExecuteCommand(new ProcessPaymentCommand(_order));
            _macroRecorder.StopRecording();

            var summary = _macroRecorder.GetMacroSummary();
            Assert.That(summary.Count, Is.EqualTo(2));
            Assert.That(summary, Does.Contain("ValidateOrder"));
            Assert.That(summary, Does.Contain("ProcessPayment"));
        }

        [Test]
        public void ClearMacro_RemovesAllCommands()
        {
            _macroRecorder.StartRecording("ClearTest");
            _invokerWithMacro.ExecuteCommand(new ValidateOrderCommand(_order));
            _macroRecorder.StopRecording();
            
            _macroRecorder.ClearMacro();
            Assert.That(_macroRecorder.GetCommandCount(), Is.EqualTo(0));
        }

        [Test]
        public void PlayMacroStep_ExecutesSingleCommand()
        {
            _macroRecorder.StartRecording("StepTest");
            _invokerWithMacro.ExecuteCommand(new ValidateOrderCommand(_order));
            _invokerWithMacro.ExecuteCommand(new ProcessPaymentCommand(_order));
            _macroRecorder.StopRecording();

            var newInvoker = new OrderInvokerWithMacro();
            var newOrder = new Order { OrderId = "ORD-004", CustomerId = "CUST-004", Total = 100m, AccountBalance = 500m };
            
            Assert.That(_macroRecorder.PlayMacroStep(newInvoker, 0), Is.True);
            Assert.That(newOrder.IsValid, Is.False); // New order not validated yet
        }

        [Test]
        public void ReplayMacro_MultipleTimes()
        {
            _macroRecorder.StartRecording("ReplayTest");
            _invokerWithMacro.ExecuteCommand(new ValidateOrderCommand(_order));
            _macroRecorder.StopRecording();

            int playCount = 0;
            for (int i = 0; i < 3; i++)
            {
                if (_macroRecorder.PlayMacro(_invokerWithMacro))
                    playCount++;
            }

            Assert.That(playCount, Is.EqualTo(3));
        }

        [Test]
        public void MacroRecording_AutomaticWithInvoker()
        {
            _macroRecorder.StartRecording("AutoRecordTest");
            
            // Commands executed through invoker should be auto-recorded
            _invokerWithMacro.ExecuteCommand(new ValidateOrderCommand(_order));
            _invokerWithMacro.ExecuteCommand(new ProcessPaymentCommand(_order));
            _invokerWithMacro.ExecuteCommand(new ReserveInventoryCommand(_order));
            
            _macroRecorder.StopRecording();
            
            Assert.That(_macroRecorder.GetCommandCount(), Is.EqualTo(3));
            var summary = _macroRecorder.GetMacroSummary();
            Assert.That(summary.Count, Is.EqualTo(3));
        }

        [Test]
        public void MacroWithComplexWorkflow()
        {
            // Record: validate, payment, reserve, ship
            _macroRecorder.StartRecording("CompleteOrderMacro");
            _invokerWithMacro.ExecuteCommand(new ValidateOrderCommand(_order));
            _invokerWithMacro.ExecuteCommand(new ProcessPaymentCommand(_order));
            _invokerWithMacro.ExecuteCommand(new ReserveInventoryCommand(_order));
            _invokerWithMacro.ExecuteCommand(new ShipOrderCommand(_order));
            _macroRecorder.StopRecording();

            // Verify macro captured all steps
            Assert.That(_macroRecorder.GetCommandCount(), Is.EqualTo(4));
            Assert.That(_order.Shipped, Is.True);
            Assert.That(_order.AccountBalance, Is.EqualTo(400m));
        }

        [Test]
        public void DifferentMacroNames()
        {
            _macroRecorder.StartRecording("Macro1");
            Assert.That(_macroRecorder.GetMacroName(), Is.EqualTo("Macro1"));
            
            _macroRecorder.StartRecording("Macro2");
            Assert.That(_macroRecorder.GetMacroName(), Is.EqualTo("Macro2"));
        }
    }
}
