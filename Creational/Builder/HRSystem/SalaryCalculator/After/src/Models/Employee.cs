namespace SalaryCalculator.After.Models
{
    /// <summary>
    /// Employee: Core data for an employee
    /// SRP: Only responsible for storing employee identity information
    /// </summary>
    public class Employee
    {
        public string EmployeeId { get; set; } = "";
        public string EmployeeName { get; set; } = "";
        public string Role { get; set; } = "";

        public Employee(string employeeId, string employeeName, string role)
        {
            EmployeeId = employeeId;
            EmployeeName = employeeName;
            Role = role;
        }

        public override string ToString() => $"{EmployeeName} ({Role})";
    }
}
