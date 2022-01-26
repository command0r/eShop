using System.ComponentModel.DataAnnotations;

namespace Core.Entities.OrderAggregate;

public class Address
{
    // EF also needs parameterless constructor
    public Address()
    { }
    
    public Address(string firstName, string lastName, string street, string city, string state, string zipCode)
    {
        FirstName = firstName;
        LastName = lastName;
        Street = street;
        City = city;
        State = state;
        ZipCode = zipCode;
    }
    
    // In EF Core 6 at least one property must be marked as 'Required'
    [Required]
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }
}