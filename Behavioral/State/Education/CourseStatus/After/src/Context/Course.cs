using System;

namespace CourseStatus.After.Context
{
    /// <summary>
    /// Course Context: Enrollment → InProgress → Completed
    /// </summary>
    public class Course
    {
        public string CourseId { get; set; }
        public string CourseName { get; set; }
        public ICourseState CurrentState { get; private set; }
        public int StudentCount { get; set; }
        public int CompletedCount { get; set; }

        public Course(string courseId, string courseName)
        {
            CourseId = courseId;
            CourseName = courseName;
            CurrentState = new EnrollmentState();
            Console.WriteLine($"[Course {courseId}] Created - State: Enrollment");
        }

        public void TransitionTo(ICourseState newState)
        {
            string oldState = CurrentState.GetStateName();
            CurrentState = newState;
            Console.WriteLine($"[Course {CourseId}] {oldState} → {newState.GetStateName()}");
        }

        public bool Enroll(int studentCount) => CurrentState.Enroll(this, studentCount);
        public bool Start() => CurrentState.Start(this);
        public bool RecordCompletion(int count) => CurrentState.RecordCompletion(this, count);
        public bool Complete() => CurrentState.Complete(this);

        public string GetCurrentStateName() => CurrentState.GetStateName();
    }

    public interface ICourseState
    {
        string GetStateName();
        bool Enroll(Course course, int studentCount);
        bool Start(Course course);
        bool RecordCompletion(Course course, int count);
        bool Complete(Course course);
    }

    public class EnrollmentState : ICourseState
    {
        public string GetStateName() => "Enrollment";

        public bool Enroll(Course course, int studentCount)
        {
            if (studentCount <= 0) return false;
            course.StudentCount += studentCount;
            Console.WriteLine($"✓ {studentCount} students enrolled. Total: {course.StudentCount}");
            return true;
        }

        public bool Start(Course course)
        {
            if (course.StudentCount == 0) return false;
            course.TransitionTo(new InProgressState());
            Console.WriteLine($"✓ Course started with {course.StudentCount} students");
            return true;
        }

        public bool RecordCompletion(Course course, int count) => false;
        public bool Complete(Course course) => false;
    }

    public class InProgressState : ICourseState
    {
        public string GetStateName() => "InProgress";

        public bool Enroll(Course course, int studentCount) => false; // Cannot enroll after start

        public bool Start(Course course) => false;

        public bool RecordCompletion(Course course, int count)
        {
            if (count <= 0 || count > course.StudentCount) return false;
            course.CompletedCount += count;
            Console.WriteLine($"✓ {count} students completed. Total: {course.CompletedCount}/{course.StudentCount}");
            return true;
        }

        public bool Complete(Course course)
        {
            if (course.CompletedCount < course.StudentCount) return false;
            course.TransitionTo(new CompletedState());
            Console.WriteLine($"✓ Course completed");
            return true;
        }
    }

    public class CompletedState : ICourseState
    {
        public string GetStateName() => "Completed";

        public bool Enroll(Course course, int studentCount) => false;
        public bool Start(Course course) => false;
        public bool RecordCompletion(Course course, int count) => false;
        public bool Complete(Course course) => false;
    }
}
