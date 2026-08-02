using Xunit;
using Composite.Organization.Chart.Component;
using System.Linq;

namespace Composite.Organization.Chart.Tests
{
    public class OrganizationTests
    {
        [Fact]
        public void Contributor_Salary_SingleEmployee()
        {
            var contributor = new Contributor("emp1", "John", "Developer", 80000);

            Assert.Equal(80000, contributor.GetTotalSalary());
            Assert.Equal(1, contributor.GetTeamSize());
        }

        [Fact]
        public void Manager_WithReports()
        {
            var manager = new Manager("mgr1", "Alice", "Manager", 100000);
            manager.AddReport(new Contributor("emp1", "Bob", "Dev", 75000));
            manager.AddReport(new Contributor("emp2", "Carol", "Dev", 75000));

            Assert.Equal(250000, manager.GetTotalSalary());
            Assert.Equal(3, manager.GetTeamSize());
        }

        [Fact]
        public void Manager_GetDirectReports()
        {
            var manager = new Manager("mgr1", "Manager", "Manager", 100000);
            var emp1 = new Contributor("emp1", "Dev1", "Dev", 70000);
            var emp2 = new Contributor("emp2", "Dev2", "Dev", 70000);

            manager.AddReport(emp1);
            manager.AddReport(emp2);

            Assert.Equal(2, manager.GetDirectReports().Count);
        }

        [Fact]
        public void Manager_RemoveReport()
        {
            var manager = new Manager("mgr1", "Manager", "Manager", 100000);
            var emp = new Contributor("emp1", "Dev", "Dev", 70000);

            manager.AddReport(emp);
            Assert.Single(manager.GetDirectReports());

            manager.RemoveReport(emp);
            Assert.Empty(manager.GetDirectReports());
        }

        [Fact]
        public void Director_MultiLevelTeam()
        {
            var director = new Director("dir1", "CEO", "Director", 150000);
            var manager1 = new Manager("mgr1", "Manager1", "Manager", 100000);
            var manager2 = new Manager("mgr2", "Manager2", "Manager", 100000);

            manager1.AddReport(new Contributor("emp1", "Dev1", "Dev", 75000));
            manager1.AddReport(new Contributor("emp2", "Dev2", "Dev", 75000));
            manager2.AddReport(new Contributor("emp3", "Dev3", "Dev", 75000));

            director.AddReport(manager1);
            director.AddReport(manager2);

            Assert.Equal(150000 + 100000 + 75000 + 75000 + 100000 + 75000, director.GetTotalSalary());
            Assert.Equal(7, director.GetTeamSize());
        }

        [Fact]
        public void Employee_VacationDays_Contributor()
        {
            var contributor = new Contributor("emp1", "John", "Dev", 70000);

            Assert.Equal(20, contributor.GetVacationDays());
        }

        [Fact]
        public void Employee_VacationDays_Manager()
        {
            var manager = new Manager("mgr1", "Alice", "Manager", 100000);

            Assert.Equal(25, manager.GetVacationDays());
        }

        [Fact]
        public void Employee_VacationDays_Director()
        {
            var director = new Director("dir1", "Bob", "Director", 150000);

            Assert.Equal(30, director.GetVacationDays());
        }

        [Fact]
        public void Contributor_GetAllReports_OnlyItself()
        {
            var contributor = new Contributor("emp1", "John", "Dev", 70000);

            var reports = contributor.GetAllReports();

            Assert.Single(reports);
            Assert.Same(contributor, reports[0]);
        }

        [Fact]
        public void Manager_GetAllReports_IncludesAll()
        {
            var manager = new Manager("mgr1", "Manager", "Manager", 100000);
            var emp1 = new Contributor("emp1", "Dev1", "Dev", 70000);
            var emp2 = new Contributor("emp2", "Dev2", "Dev", 70000);

            manager.AddReport(emp1);
            manager.AddReport(emp2);

            var reports = manager.GetAllReports();

            Assert.Equal(3, reports.Count); // manager + 2 employees
        }

        [Fact]
        public void Director_GetAllReports_IncludesHierarchy()
        {
            var director = new Director("dir1", "Director", "Director", 150000);
            var manager = new Manager("mgr1", "Manager", "Manager", 100000);
            var emp = new Contributor("emp1", "Dev", "Dev", 70000);

            manager.AddReport(emp);
            director.AddReport(manager);

            var reports = director.GetAllReports();

            Assert.Equal(3, reports.Count); // director + manager + employee
        }

        [Fact]
        public void Manager_ToString_ContainsInfo()
        {
            var manager = new Manager("mgr1", "Alice", "Manager", 100000);
            manager.AddReport(new Contributor("emp1", "Bob", "Dev", 70000));

            var str = manager.ToString();
            Assert.Contains("Alice", str);
            Assert.Contains("1", str); // 1 direct report
            Assert.Contains("170", str); // 170k total salary
        }

        [Fact]
        public void Contributor_ToString_ContainsInfo()
        {
            var contributor = new Contributor("emp1", "John", "Developer", 80000);

            var str = contributor.ToString();
            Assert.Contains("Developer", str);
            Assert.Contains("John", str);
        }

        [Fact]
        public void Director_ToString_ContainsInfo()
        {
            var director = new Director("dir1", "Bob", "Director", 150000);
            director.AddReport(new Manager("mgr1", "Alice", "Manager", 100000));

            var str = director.ToString();
            Assert.Contains("Bob", str);
            Assert.Contains("1", str);
        }

        [Fact]
        public void Manager_AddNull_ThrowsException()
        {
            var manager = new Manager("mgr1", "Manager", "Manager", 100000);

            var exception = Assert.Throws<ArgumentNullException>(() =>
                manager.AddReport(null)
            );

            Assert.Contains("employee", exception.Message);
        }

        [Fact]
        public void Manager_MultipleTeams_SalaryCalculation()
        {
            var manager = new Manager("mgr1", "Manager", "Manager", 100000);

            for (int i = 0; i < 5; i++)
            {
                manager.AddReport(new Contributor($"emp{i}", $"Dev{i}", "Developer", 70000));
            }

            Assert.Equal(100000 + (5 * 70000), manager.GetTotalSalary());
            Assert.Equal(6, manager.GetTeamSize()); // manager + 5 devs
        }
    }
}
