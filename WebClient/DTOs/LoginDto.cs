using System.ComponentModel.DataAnnotations;

namespace WebClient.DTOs;

public record LoginDto([Required][EmailAddress] string Email, [Required] string Password);
