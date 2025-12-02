using System.ComponentModel.DataAnnotations;

namespace WebClient.DTOs;

public record RegisterDto([Required][EmailAddress] string Email, [Required][MinLength(6)] string Password, string? FullName);
