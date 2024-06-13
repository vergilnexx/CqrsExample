using Meta.Common.Contracts.Utilities.Helpers;

namespace Meta.Common.Test.UnitTests.Contracts.Utilities.Helpers
{
    /// <summary>
    /// Тесты helper'а работы с датами и временем.
    /// </summary>
    internal class DateTimeHelperTests
    {
        [Test(Description = "Если дата валиджная, то результат должен быть определенным.")]
        public void ToDateTime_ValidDate_DateTimeIsNotNull()
        {
            // Arrange
            DateOnly date = new (2000, 1, 2);

            // Act
            var result = DateTimeHelper.ToDateTime(date);

            // Assert
            var emptyDate = new DateTime(0, DateTimeKind.Unspecified);
            Assert.That(result, Is.Not.EqualTo(emptyDate));
        }

        [Test(Description = "Если дата валиджная, то результат должен быть равен дате.")]
        public void ToDateTime_ValidDate_ResultEqualsOriginData()
        {
            // Arrange
            DateOnly date = new (2000, 1, 2);

            // Act
            var result = DateTimeHelper.ToDateTime(date);

            // Assert
            Assert.That(result.Year, Is.EqualTo(date.Year));
            Assert.That(result.Month, Is.EqualTo(date.Month));
            Assert.That(result.Day, Is.EqualTo(date.Day));
        }

        [Test(Description = "Если дата валиджная, то время у результата должно быть равно полночи.")]
        public void ToDateTime_ValidDate_DateTimeInMidnight()
        {
            // Arrange
            DateOnly date = new(2000, 1, 2);

            // Act
            var result = DateTimeHelper.ToDateTime(date);

            // Assert
            Assert.That(result.Hour, Is.EqualTo(0));
            Assert.That(result.Minute, Is.EqualTo(0));
            Assert.That(result.Second, Is.EqualTo(0));
        }

        [Test(Description = "Если дата валиджная, то время у результата должно быть в UTC.")]
        public void ToDateTime_ValidDate_DateTimeHasUtcKind()
        {
            // Arrange
            DateOnly date = new(2000, 1, 2);

            // Act
            var result = DateTimeHelper.ToDateTime(date);

            // Assert
            Assert.That(result.Kind, Is.EqualTo(DateTimeKind.Utc));
        }
    }
}
