using System;
using System.Collections.Generic;
using System.Linq;

namespace Composite.Organization.Chart.Component
{
    /// <summary>
    /// Component interface: Employees in organization hierarchy.
    /// Demonstrates: Composite pattern for treating individual employee same as manager with team.
    /// </summary>
    public abstract class Employee
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }

        protected Employee(string id, string name, string title, decimal salary)
        {
            Id = id;
            Name = name;
            Title = title;
            Salary = salary;
            HireDate = DateTime.UtcNow;
        }

        public abstract decimal GetTotalSalary();
        public abstract int GetTeamSize();
        public abstract void DisplayHierarchy(int level = 0);
        public abstract List<Employee> GetAllReports();
        public abstract decimal GetVacationDays();
    }

    /// <summary>
    /// Leaf: Individual contributor with no direct reports.
    /// </summary>
    public class Contributor : Employee
    {
        public string Department { get; set; }
        public string Skills { get; set; }

        public Contributor(string id, string name, string title, decimal salary) 
            : base(id, name, title, salary)
        {
            Department = "Engineering";
            Skills = "";
        }

        public override decimal GetTotalSalary() => Salary;

        public override int GetTeamSize() => 1;

        public override void DisplayHierarchy(int level = 0)
        {
            Console.WriteLine($"{new string(' ', level * 2)}{Title}: {Name} (${Salary}k)");
        }

        public override List<Employee> GetAllReports() => new List<Employee> { this };

        public override decimal GetVacationDays() => 20; // Standard vacation days

        public override string ToString() => $"{Title}({Name})";
    }

    /// <summary>
    /// Composite: Manager with direct reports.
    /// </summary>
    public class Manager : Employee
    {
        private readonly List<Employee> _directReports = new List<Employee>();
        public string Department { get; set; }
        public int BudgetAllocation { get; set; }

        public Manager(string id, string name, string title, decimal salary) 
            : base(id, name, title, salary)
        {
            Department = "Engineering";
            BudgetAllocation = 0;
        }

        public void AddReport(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));
            _directReports.Add(employee);
        }

        public void RemoveReport(Employee employee)
        {
            _directReports.Remove(employee);
        }

        public IReadOnlyList<Employee> GetDirectReports() => _directReports.AsReadOnly();

        public override decimal GetTotalSalary()
        {
            decimal total = Salary;
            foreach (var report in _directReports)
            {
                total += report.GetTotalSalary();
            }
            return total;
        }

        public override int GetTeamSize()
        {
            int size = 1;
            foreach (var report in _directReports)
            {
                size += report.GetTeamSize();
            }
            return size;
        }

        public override void DisplayHierarchy(int level = 0)
        {
            Console.WriteLine($"{new string(' ', level * 2)}Manager: {Name} (${Salary}k) - {_directReports.Count} direct reports");
            foreach (var report in _directReports)
            {
                report.DisplayHierarchy(level + 1);
            }
        }

        public override List<Employee> GetAllReports()
        {
            var reports = new List<Employee> { this };
            foreach (var report in _directReports)
            {
                reports.AddRange(report.GetAllReports());
            }
            return reports;
        }

        public override decimal GetVacationDays() => 25; // Managers get more vacation

        public override string ToString() => 
            $"Manager({Name}, {_directReports.Count} reports, ${GetTotalSalary()}k total)";
    }

    /// <summary>
    /// Composite: Executive/Director level.
    /// </summary>
    public class Director : Employee
    {
        private readonly List<Employee> _directReports = new List<Employee>();
        public string Division { get; set; }

        public Director(string id, string name, string title, decimal salary) 
            : base(id, name, title, salary)
        {
            Division = "Engineering Division";
        }

        public void AddReport(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));
            _directReports.Add(employee);
        }

        public override decimal GetTotalSalary()
        {
            decimal total = Salary;
            foreach (var report in _directReports)
            {
                total += report.GetTotalSalary();
            }
            return total;
        }

        public override int GetTeamSize()
        {
            int size = 1;
            foreach (var report in _directReports)
            {
                size += report.GetTeamSize();
            }
            return size;
        }

        public override void DisplayHierarchy(int level = 0)
        {
            Console.WriteLine($"{new string(' ', level * 2)}Director: {Name} (${Salary}k) - Team: {GetTeamSize()}");
            foreach (var report in _directReports)
            {
                report.DisplayHierarchy(level + 1);
            }
        }

        public override List<Employee> GetAllReports()
        {
            var reports = new List<Employee> { this };
            foreach (var report in _directReports)
            {
                reports.AddRange(report.GetAllReports());
            }
            return reports;
        }

        public override decimal GetVacationDays() => 30; // Executives get most vacation

        public override string ToString() => 
            $"Director({Name}, {_directReports.Count} managers, ${GetTotalSalary()}k total salary)";
    }
}
