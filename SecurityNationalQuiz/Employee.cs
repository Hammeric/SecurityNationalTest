using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecurityNationalQuiz
{
    class Employee
    {
        readonly int PAY_WEEKS_IN_YEAR = 26;
        readonly decimal STANDARD_HOURS = 80;
        readonly decimal OVERTIME_HOURS = 90;

        readonly decimal STANDARD_RATE = 1.0m;
        readonly decimal OVERTIME_RATE = 1.5m;
        readonly decimal EXTRA_OVERTIME_RATE = 1.75m;

        private static readonly string[] LOWER_TAX_STATES = { "UT", "WY", "NV" };
        private static readonly string[] MIDDLE_TAX_STATES = { "CO", "ID", "AZ", "OR" };
        private static readonly string[] UPPER_TAX_STATES = { "WA", "NM", "TX" };

        public string EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PayType { get; set; }
        public string StateOfResidence { get; set; }

        public decimal GrossPay
        {
            get
            {
                decimal grossPay = 0;
                if (salary != 0)
                {
                    switch (PayType.ToLower())
                    {
                        case "s":
                            grossPay = salary / PAY_WEEKS_IN_YEAR;
                            break;
                        case "h":
                            if (hoursWorked <= STANDARD_HOURS)
                            {
                                grossPay = PayModifier(hoursWorked, STANDARD_RATE);
                            }
                            else
                            {
                                grossPay = PayModifier(STANDARD_HOURS, STANDARD_RATE);
                                decimal overtimeHours = 0;
                                if (hoursWorked < OVERTIME_HOURS)
                                {
                                    overtimeHours = hoursWorked - STANDARD_HOURS;
                                    grossPay += PayModifier(overtimeHours, OVERTIME_RATE);
                                }
                                else
                                {
                                    overtimeHours = OVERTIME_HOURS - STANDARD_HOURS;
                                    grossPay += PayModifier(overtimeHours, OVERTIME_RATE);

                                    overtimeHours = hoursWorked - OVERTIME_HOURS;
                                    grossPay += PayModifier(overtimeHours, EXTRA_OVERTIME_RATE);
                                }
                            }
                            break;
                        default:
                            Console.WriteLine(string.Format("Error PayType: {0} does not exist", PayType));
                            throw new Exception();
                    }
                }
                return grossPay;
            }
        }

        public decimal NetPay
        {
            get
            {
                return GrossPay - GetStateTaxWithholdings() - GetFederalTaxWithholdings();
            }
        }

        private decimal PayModifier(decimal hours, decimal modifier)
        {
            return salary * hours * modifier;
        }

        private DateTime startDate;
        public string StartDate
        {
            get
            {
                return startDate.ToString();
            }
            set
            {
                if (!DateTime.TryParse(value, out startDate))
                {
                    startDate = DateTime.MinValue;
                }
            }
        }
        public DateTime GetStartDate
        {
            get
            {
                return startDate;
            }
        }

        private decimal salary;
        public string Salary
        {
            get
            {
                return salary.ToString();
            }
            set
            {
                if (!decimal.TryParse(value, out salary))
                {
                    hoursWorked = 0;
                }
            }
        }

        private decimal hoursWorked;
        public string HoursWorked
        {
            get
            {
                return hoursWorked.ToString();
            }
            set
            {
                if (!decimal.TryParse(value, out hoursWorked))
                {
                    hoursWorked = 0;
                }
            }
        }

        public decimal GetStateTaxWithholdings()
        {
            decimal taxRate = 0.0m;

            if (LOWER_TAX_STATES.Contains(StateOfResidence))
            {
                // 5% // "UT", "WY", "NV"
                taxRate = 0.05m;
            }
            else if (MIDDLE_TAX_STATES.Contains(StateOfResidence))
            {
                // 6.5% // "CO", "ID", "AZ", "OR"
                taxRate = 0.065m;
            }
            else if (UPPER_TAX_STATES.Contains(StateOfResidence))
            {
                // 7% // "WA", "NM", "TX"
                taxRate = 0.07m;
            }
            else
            {
                Console.WriteLine(string.Format("Error State: {0} does not exist", StateOfResidence));
                throw new Exception();
            }

            return taxRate * GrossPay;
        }
        public decimal GetFederalTaxWithholdings()
        {
            // federal tax rate 15%
            decimal taxRate = 0.15m;

            return taxRate * GrossPay;
        }
    }
}
