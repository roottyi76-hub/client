namespace WebClient.DTOs.Measurement;

public class MeasurementDto
{
    public int MeasurementId { get; set; }
    public double Intensity { get; set; }
    public DateTime MeasurementDate { get; set; }
    public int LaserId { get; set; }
}