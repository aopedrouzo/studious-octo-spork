namespace FootballManager.Domain.Entities
{
    public class Coach : Employee
    {
        private Coach() : base() { }

        public Coach(string firstName, string lastName, DateOnly dateOfBirth, decimal salary, string email) 
            : base(firstName, lastName, dateOfBirth, salary, email)
        {
        }
    }
}
