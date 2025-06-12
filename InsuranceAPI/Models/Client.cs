using InsuranceAPI.Models;
using System.ComponentModel.DataAnnotations;

public class Client
{
    [Key]
    public int Id { get; set; }

    [Required]
    
    public string Name { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    [Required]
    public string Gender { get; set; } = string.Empty;

    [Required, MaxLength(15)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty; // FK to User.Username

    [Required, MaxLength(12)]
    public string AadhaarNumber { get; set; } = string.Empty;

    [Required, MaxLength(10)]
    public string PANNumber { get; set; } = string.Empty;

    [Required]
    public string Address { get; set; } = string.Empty;

    public User? User { get; set; }
    public ICollection<Vehicle>? Vehicles { get; set; }
    public ICollection<Proposal>? Proposals { get; set; }
    public IEnumerable<Insurance>? Insurances { get; set; }


}
