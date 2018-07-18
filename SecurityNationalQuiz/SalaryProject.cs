using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SecurityNationalQuiz
{
    class SalaryProject
    {
        private List<Employee> employees;

        public void Run()
        {
            LoadFromFile();
            SavePayCheckResults();
            SaveTopEarners();
            SaveStateResults();
            GetEmployee();
        }

        private void GetEmployee()
        {
            string Id = string.Empty;
            while (true)
            {
                Console.WriteLine("Please enter an employee Id.  (Type 'EXIT NOW' to leave)");

                Id = Console.ReadLine();
                if (Id.ToLower().Equals("exit now"))
                {
                    break;
                }
                if (!string.IsNullOrWhiteSpace(Id))
                {
                    Console.WriteLine();
                    Console.WriteLine("Searching for Employee.");

                    var employee = GetByEmployeeId(Id);
                    if (employee != null)
                    {
                        Console.WriteLine("Employee Id: " + employee.EmployeeId);
                        Console.WriteLine("First Name: " + employee.FirstName);
                        Console.WriteLine("Last Name: " + employee.LastName);
                        Console.WriteLine("Pay Type: " + employee.PayType);
                        Console.WriteLine("Salary: " + employee.Salary);
                        Console.WriteLine("Start Date: " + employee.StartDate);
                        Console.WriteLine("State: " + employee.StateOfResidence);
                        Console.WriteLine("Hours Worked: " + employee.HoursWorked);
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine("Cannot find Employee.");
                    }
                }
            }
        }

        private void SaveStateResults()
        {
            Console.WriteLine("Saving state median data.");

            //time worked, net pay, and state taxes paid
            Dictionary<string, List<Tuple<decimal, decimal, decimal>>> states = new Dictionary<string, List<Tuple<decimal, decimal, decimal>>>();

            foreach (var employee in employees)
            {
                if (!states.ContainsKey(employee.StateOfResidence))
                {
                    states.Add(employee.StateOfResidence, new List<Tuple<decimal, decimal, decimal>>());
                }
                states[employee.StateOfResidence].Add(new Tuple<decimal, decimal, decimal>
                    (Convert.ToDecimal(employee.HoursWorked),
                    employee.NetPay,
                    employee.GetStateTaxWithholdings()));
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"..\..\..\files\StateData.txt"))
            {
                int counter = 0;
                foreach (var state in states)
                {
                    var timeWorked = state.Value.OrderBy(x => x.Item1).ToList();
                    var netPay = state.Value.OrderBy(x => x.Item2).ToList();
                    var taxesPayed = state.Value.Sum(x => x.Item3);

                    // time worked, median net pay, and total state taxes paid

                    var data = string.Format("{0},{1},{2},{3}",
                        state.Key,
                        timeWorked[timeWorked.Count / 2].Item1.ToString("0.00"),
                        netPay[netPay.Count / 2].Item2.ToString("0.00"),
                        taxesPayed.ToString("0.00"));
                    file.WriteLine(data);
                    ++counter;

                }
                Console.WriteLine(string.Format("{0} States Saved.", counter));
                file.Close();
            }

            Console.WriteLine("State median data Saved.");
        }


        private void SaveTopEarners()
        {
            Console.WriteLine("Saving Top 15% earners Data.");
            int numTopEarners = Convert.ToInt32(employees.Count * 0.15);
            var topList = employees.OrderByDescending(x => x.GrossPay).Take(numTopEarners);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"..\..\..\files\TopEarners.txt"))
            {
                int counter = 0;
                foreach (var employee in topList.OrderBy(z => z.LastName).OrderBy(y => y.FirstName).OrderBy(x => x.GetStartDate.Year))
                {
                    // first name, last name, number of years worked, gross pay

                    var data = string.Format("{0},{1},{2},{3}",
                        employee.FirstName,
                        employee.LastName,
                        DateTime.Now.Year - employee.GetStartDate.Year,
                        employee.GrossPay.ToString("0.00"));
                    file.WriteLine(data);
                    ++counter;
                }
                Console.WriteLine(string.Format("{0} Employees Saved.", counter));
                file.Close();
            }
        }

        private void SavePayCheckResults()
        {
            Console.WriteLine("Saving PayCheck Data.");

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"..\..\..\files\CalculatePayChecks.txt"))
            {
                int counter = 0;
                foreach (var employee in employees.OrderByDescending(x => x.GrossPay))
                {
                    var data = string.Format("{0},{1},{2},{3},{4},{5},{6}",
                        employee.EmployeeId,
                        employee.FirstName,
                        employee.LastName,
                        employee.GrossPay.ToString("0.00"),
                        employee.GetFederalTaxWithholdings().ToString("0.00"),
                        employee.GetStateTaxWithholdings().ToString("0.00"),
                        employee.NetPay.ToString("0.00"));
                    file.WriteLine(data);
                    ++counter;
                }
                Console.WriteLine(string.Format("{0} Employees Saved.", counter));
                file.Close();
            }
        }


        private void LoadFromFile()
        {
            Console.WriteLine("Loading Data.");
            employees = new List<Employee>();
            int counter = 0;
            string line;

            StreamReader file = new StreamReader(@"..\..\..\files\Employees.txt");
            while ((line = file.ReadLine()) != null)
            {
                int i = 0;
                Employee employee = new Employee();
                string[] words = line.Split(',');
                employee.EmployeeId = words[i++];
                employee.FirstName = words[i++];
                employee.LastName = words[i++];
                employee.PayType = words[i++];
                employee.Salary = words[i++];
                employee.StartDate = words[i++];
                employee.StateOfResidence = words[i++];
                employee.HoursWorked = words[i++];

                employees.Add(employee);
                ++counter;
            }
            Console.WriteLine(string.Format("{0} Employees Loaded.", counter));
        }

        Employee GetByEmployeeId(string employeeId)
        {
            return employees.Where(x => x.EmployeeId.ToLower().Equals(employeeId.ToLower())).FirstOrDefault();
        }
    }
}
