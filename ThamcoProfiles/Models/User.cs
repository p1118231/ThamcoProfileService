using System.ComponentModel.DataAnnotations;

namespace ThamcoProfiles.Models;

public class User{
    public int Id{get;set;}

 
    [Display (Name = "First Name")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "First Name must be between 2 and 50 characters.")]
    
    public  String? FirstName{get; set;}

    
    [Display (Name = "Last Name")]
    public String? LastName{get; set;}
    
    [Required]
    [EmailAddress]
    public required String Email{get; set;}

    
    [StringLength(500, ErrorMessage = "Address must be up to 500 characters long.")]
    [Display (Name = "Payment Address")]

    public String? PaymentAddress{get; set;}
    
    [Required]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", 
    ErrorMessage = "Password must have at least one uppercase letter, one lowercase letter, one number, and one special character.")]
    [DataType(DataType.Password)]
    public required String Password{get; set;}

    [Phone]
    [RegularExpression(@"^\+?[1-9]\d{10,14}$", ErrorMessage = "The phone number must be in international format (e.g., +1234567890).")]

    [Display (Name = "Phone Number")]
    public String? PhoneNumber { get; set; }


    public String? Auth0UserId { get; set; }
}