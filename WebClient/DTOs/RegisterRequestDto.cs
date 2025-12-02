namespace WebClient.DTOs;

public class RegisterRequestDto
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}

// Запрос на вход
public class LoginRequestDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}
