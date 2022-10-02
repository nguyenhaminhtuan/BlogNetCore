using System.ComponentModel.DataAnnotations;

namespace BlogNetCore.Common.DTOs;

public class UserCredentialsDto
{
    [Required]
    [EmailAddress]
    public string Username { get; set; }
    
    [Required]
    [MinLength(6)]
    public string Password { get; set; }
}