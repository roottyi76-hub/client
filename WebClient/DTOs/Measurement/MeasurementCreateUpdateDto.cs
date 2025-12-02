using System.ComponentModel.DataAnnotations;

namespace WebClient.DTOs.Measurement;

public class MeasurementCreateUpdateDto
{
    [Required]
    public double Intensity { get; set; }

    [Required]
    public DateTime MeasurementDate { get; set; }

    [Required]
    public int LaserId { get; set; }
}