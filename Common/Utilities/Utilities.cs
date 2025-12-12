using System.ComponentModel;

namespace BuildHub.Common.Utilities
{
	/// <summary>
	/// A utility functions class
	/// </summary>
	public class Utilities
	{
		/// <summary>
		/// Default date time format
		/// </summary>
		private const string _DATE_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss.fff";

		/// <summary>
		/// Retrieves a description attribute from an enumeration
		/// </summary>
		/// <typeparam name="EnumType">Type parameter for enums</typeparam>
		/// <param name="enumeration">Value of the enum</param>
		/// <returns>string</returns>
		public static string GetEnumDescription<EnumType>(Enum enumeration)
			where EnumType : Enum
		{
			DescriptionAttribute? descriptionAttribute = enumeration.GetType()?.GetField(enumeration.ToString())
				?.GetCustomAttributes(typeof(DescriptionAttribute), false)
				.SingleOrDefault() as DescriptionAttribute;

			return descriptionAttribute?.Description ?? string.Empty;
		}

		/// <summary>
		/// Retrieves all values of the specified enumeration type.
		/// </summary>
		/// <typeparam name="EnumType">The enumeration type whose values are to be retrieved. This type must be an enumeration.</typeparam>
		/// <returns>An <see cref="IEnumerable{T}"/> containing all values of the specified enumeration type.</returns>
		public static IEnumerable<EnumType> GetEnumValues<EnumType>() => Enum.GetValues(typeof(EnumType)).Cast<EnumType>();

		/// <summary>
		/// Retrieves the system date time
		/// </summary>
		public static DateTime GetCurrentDateTime => DateTime.Now;

		/// <summary>
		/// Formats the specified <see cref="DateTime"/> value as a string using a predefined format.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime"/> value to format.</param>
		/// <returns>A string representation of the <paramref name="dateTime"/> value in the predefined format.</returns>
		public static string FormatDateTime(DateTime dateTime, string dateFormat = _DATE_TIME_FORMAT) => dateTime.ToString(dateFormat);

		/// <summary>
		/// Surrounds the given value with single quotes
		/// </summary>
		/// <param name="value"></param>
		/// <returns>The value surrounded by single quotes</returns>
		public static string Stringify(object value) => $"'{value}'";

		/// <summary>
		/// Retrieves the name of the specified type.
		/// </summary>
		/// <param name="object">The <see cref="Type"/> whose name is to be retrieved. Cannot be <see langword="null"/>.</param>
		/// <returns>The name of the specified type as a <see cref="string"/>.</returns>
		public static string GetTypeName(Type @object) => @object.Name;
	}
}
