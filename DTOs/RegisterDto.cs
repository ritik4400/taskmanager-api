using System.ComponentModel.DataAnnotations;

namespace taskmanager_api.DTOs;

public class RegisterDto
{
    [Required, StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(72, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;
}