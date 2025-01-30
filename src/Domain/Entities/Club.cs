using System.Collections.Generic;
using System;
using System.Linq;

namespace FootballManager.Domain.Entities
{
    public class Club
    {
        private Club() { }

        public Club(string name, decimal budget)
        {
            Name = name;
            Budget = budget;
        }

        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public decimal Budget { get; private set; }

        public ICollection<Player> Players { get; private set; } = new List<Player>();
        public ICollection<Coach> Coaches { get; private set; } = new List<Coach>();

        public void HandleTransfer(Employee employee)
        {
            if (employee.ClubId != null && employee.ClubId != Id)
            {
                throw new InvalidOperationException($"Player {employee.FirstName} {employee.LastName} is already assigned to another club.");
            }
            if (employee.Salary <= 0)
            {
                throw new InvalidOperationException("Player must have a valid salary defined");
            }

            if (Budget < employee.Salary)
            {
                throw new InvalidOperationException("Insufficient budget to add player");
            }

            Budget -= employee.Salary;
        }

        public void ModifyBudget(decimal amount, decimal totalSalary)
        {
            var newBudget = Budget + amount;
            if (newBudget < totalSalary)
            {
                throw new InvalidOperationException("Budget cannot be reduced below the total salary of its players and coaches");
            }
            Budget = newBudget;
        }

        public void HandleRelease(Employee employee)
        {
            if (employee.ClubId != Id)
            {
                throw new InvalidOperationException($"Cannot release employee {employee.FirstName} {employee.LastName} as they are not assigned to this club.");
            }

            Budget += employee.Salary;
        }
    }
}
