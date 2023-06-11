using MinimalApiCleanArchitecture.Domain.Entities;

namespace MinimalApiCleanArchitecture.Domain.Model;
public class  Author: BaseEntity
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName => $"{FirstName} {LastName}";
    public DateTime DateOfBirth { get; set; }
    public string? Bio { get; set; }

    public void UpdateAuthor(string? firstName, string? lastName, string? bio, DateTime dateOfBirth)
    {
        FirstName = firstName;
        LastName = lastName;
        Bio = bio;
        DateOfBirth = dateOfBirth;
    }
}