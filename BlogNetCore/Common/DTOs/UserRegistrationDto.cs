using System.ComponentModel.DataAnnotations;

namespace BlogNetCore.Common.DTOs;

public class UserRegistrationDto : UserCredentialsDto
{
    [Required]
    [MinLength(2)]
    public string DisplayName { get; set; }
}