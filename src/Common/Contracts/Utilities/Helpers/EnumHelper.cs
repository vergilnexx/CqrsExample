using System.ComponentModel;

namespace Meta.Common.Contracts.Utilities.Helpers
{
    /// <summary>
    /// Helper для работы с перечислениями.
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// Возвращает все значения перечисления.
        /// </summary>
        /// <typeparam name="TEnum">Тип перечисления.</typeparam>
        /// <returns>Все значения перечисления.</returns>
        public static string? GetEnumDescription<TEnum>(this TEnum @enum)
            where TEnum : struct
        {
            DescriptionAttribute? attribute = @enum.GetEnumAttribuite<TEnum, DescriptionAttribute>();
            return attribute == null ? @enum.ToString() : attribute.Description;
        }

        /// <summary>
        /// Возвращает атрибут перечисления.
        /// </summary>
        /// <typeparam name="TEnum">Тип перечисления.</typeparam>
        /// <typeparam name="TAttribute">Тип атрибута.</typeparam>
        /// <returns>Атрибут перечисления.</returns>
        public static TAttribute? GetEnumAttribuite<TEnum, TAttribute>(this TEnum @enum)
            where TEnum : struct
            where TAttribute: Attribute
        {
            TAttribute? attribute = @enum
                                        .GetType()
                                        ?.GetField(@enum.ToString() ?? string.Empty)
                                        ?.GetCustomAttributes(typeof(TAttribute), inherit: false)
                                        .SingleOrDefault() as TAttribute;
            return attribute;
        }
    }
}
