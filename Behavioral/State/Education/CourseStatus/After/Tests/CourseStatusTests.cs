using NUnit.Framework;
using CourseStatus.After.Context;

namespace CourseStatus.After.Tests
{
    [TestFixture]
    public class CourseStatusTests
    {
        private Course _course;

        [SetUp]
        public void Setup()
        {
            _course = new Course("COURSE-001", "C# Advanced");
        }

        [Test] public void Course_Initial_Enrollment() => Assert.That(_course.GetCurrentStateName(), Is.EqualTo("Enrollment"));
        [Test] public void Course_Enrollment_CanEnroll() => Assert.That(_course.Enroll(30), Is.True);
        [Test] public void Course_StudentCount_Updated()
        {
            _course.Enroll(25);
            Assert.That(_course.StudentCount, Is.EqualTo(25));
        }
        [Test] public void Course_NegativeEnroll_Fails() => Assert.That(_course.Enroll(-5), Is.False);
        [Test] public void Course_ZeroEnroll_Fails() => Assert.That(_course.Enroll(0), Is.False);
        [Test] public void Course_MultipleEnroll()
        {
            _course.Enroll(15);
            _course.Enroll(10);
            Assert.That(_course.StudentCount, Is.EqualTo(25));
        }
        [Test] public void Course_NoStudents_CannotStart() => Assert.That(_course.Start(), Is.False);
        [Test] public void Course_WithStudents_CanStart()
        {
            _course.Enroll(20);
            Assert.That(_course.Start(), Is.True);
        }
        [Test] public void Course_Start_ChangeState()
        {
            _course.Enroll(20);
            _course.Start();
            Assert.That(_course.GetCurrentStateName(), Is.EqualTo("InProgress"));
        }
        [Test] public void Course_InProgress_CannotEnroll()
        {
            _course.Enroll(20);
            _course.Start();
            Assert.That(_course.Enroll(5), Is.False);
        }
        [Test] public void Course_RecordCompletion_Succeeds()
        {
            _course.Enroll(20);
            _course.Start();
            Assert.That(_course.RecordCompletion(10), Is.True);
        }
        [Test] public void Course_CompletedCount_Updated()
        {
            _course.Enroll(20);
            _course.Start();
            _course.RecordCompletion(15);
            Assert.That(_course.CompletedCount, Is.EqualTo(15));
        }
        [Test] public void Course_CompleteMore_ThanEnrolled_Fails()
        {
            _course.Enroll(20);
            _course.Start();
            Assert.That(_course.RecordCompletion(25), Is.False);
        }
        [Test] public void Course_AllCompleted_CanComplete()
        {
            _course.Enroll(20);
            _course.Start();
            _course.RecordCompletion(20);
            Assert.That(_course.Complete(), Is.True);
        }
        [Test] public void Course_Completed_State()
        {
            _course.Enroll(20);
            _course.Start();
            _course.RecordCompletion(20);
            _course.Complete();
            Assert.That(_course.GetCurrentStateName(), Is.EqualTo("Completed"));
        }
        [Test] public void Course_PartialCompletion_CannotComplete()
        {
            _course.Enroll(20);
            _course.Start();
            _course.RecordCompletion(10);
            Assert.That(_course.Complete(), Is.False);
        }
        [Test] public void Course_Completed_CannotEnroll()
        {
            _course.Enroll(20);
            _course.Start();
            _course.RecordCompletion(20);
            _course.Complete();
            Assert.That(_course.Enroll(5), Is.False);
        }
        [Test] public void Course_FullWorkflow()
        {
            Assert.That(_course.Enroll(25), Is.True);
            Assert.That(_course.Start(), Is.True);
            Assert.That(_course.RecordCompletion(25), Is.True);
            Assert.That(_course.Complete(), Is.True);
            Assert.That(_course.GetCurrentStateName(), Is.EqualTo("Completed"));
        }
        [Test] public void Course_MultipleCompletions()
        {
            _course.Enroll(30);
            _course.Start();
            _course.RecordCompletion(10);
            _course.RecordCompletion(10);
            _course.RecordCompletion(10);
            Assert.That(_course.CompletedCount, Is.EqualTo(30));
        }
    }
}
