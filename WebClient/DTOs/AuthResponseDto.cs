using System.ComponentModel.DataAnnotations;

namespace WebClient.DTOs;
public record AuthResponseDto(string Token, string FullName, string Role);  
