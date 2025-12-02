namespace WebClient.DTOs
{
    public class JoinedDataDto
    {
        // Поля из Measurement
        public int MeasurementId { get; set; }
        public double Intensity { get; set; }
        public DateTime MeasurementDate { get; set; }

        // Общее поле
        public int LaserId { get; set; }

        // Поля из Laser
        public double LaserX { get; set; }
        public double LaserY { get; set; }
    }
}