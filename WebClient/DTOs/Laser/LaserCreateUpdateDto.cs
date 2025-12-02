using System.ComponentModel.DataAnnotations;

namespace WebClient.DTOs.Laser;

public class LaserCreateUpdateDto
{
    [Required]
    public double X { get; set; }

    [Required]
    public double Y { get; set; }
}