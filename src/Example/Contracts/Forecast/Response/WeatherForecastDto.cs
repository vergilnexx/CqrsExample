namespace Meta.Example.Contracts.Forecast.Response
{
    /// <summary>
    /// ������ � �������� ������.
    /// </summary>
    public class WeatherForecastDto
    {
        /// <summary>
        /// ����.
        /// </summary>
        public DateOnly Date { get; set; }

        /// <summary>
        /// ����������� �� �������
        /// </summary>
        public int TemperatureC { get; set; }

        /// <summary>
        /// ����������� �� ����������
        /// </summary>
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        /// <summary>
        /// ��������.
        /// </summary>
        public string? Summary { get; set; }
    }
}
