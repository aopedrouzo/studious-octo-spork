namespace FootballManager.Domain.Entities
{
    public abstract class Employee
    {
        protected Employee() { }

        protected Employee(string firstName, string lastName, DateOnly dateOfBirth, decimal salary, string email)
        {
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            Salary = salary;
            Email = email;
        }

        public int Id { get; protected set; }
        public string FirstName { get; protected set; } = string.Empty;
        public string LastName { get; protected set; } = string.Empty;
        public string Email { get; protected set; } = string.Empty;

        public DateOnly DateOfBirth { get; protected set; }
        public decimal Salary { get; protected set; }
        public int? ClubId { get; protected set; }

        public virtual void JoinClub(Club club)
        {
            ClubId = club.Id;
        }

        public virtual void LeaveClub()
        {
            ClubId = null;
        }
    }
}
