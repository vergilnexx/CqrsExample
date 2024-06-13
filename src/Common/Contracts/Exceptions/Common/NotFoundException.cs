namespace Meta.Common.Contracts.Exceptions.Common
{
    /// <summary>
    /// Исключение, вызываемое когда не найден объект.
    /// </summary>
    /// <param name="message">Сообщение об ошибке.</param>
    public class NotFoundException(string? message) : Exception(message)
    {
    }
}
