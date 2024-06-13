namespace Meta.Common.Contracts.Utilities.Helpers
{
    /// <summary>
    /// Helper для работы с датой и временем.
    /// </summary>
    public static class DateTimeHelper
    {
        /// <summary>
        /// Возвращает дату и время начала дня из даты.
        /// </summary>
        /// <param name="date">Даиа.</param>
        /// <returns>Дата и время начала дня.</returns>
        public static DateTime ToDateTime(this DateOnly date)
        {
            return new DateTime(date.Year, date.Month, date.Day, hour: 0, minute: 0, second: 0, DateTimeKind.Utc);
        }
    }
}
