using FootballManager.Domain.Enums;

namespace FootballManager.Domain.Entities;

public class Player : Employee
{
    private Player() : base() { }

    public Player(string firstName, string lastName, DateOnly dateOfBirth, Position position, decimal salary, string email)
        : base(firstName, lastName, dateOfBirth, salary, email)
    {
        Position = position;
    }

    public Position Position { get; private set; }
}
