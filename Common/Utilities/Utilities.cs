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
		private static string _DATE_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss.fff";

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
		/// Retrieves the system date time
		/// </summary>
		public static DateTime GetCurrentDateTime => DateTime.Now;

		/// <summary>
		/// Formats the specified <see cref="DateTime"/> value as a string using a predefined date and time format.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime"/> value to format.</param>
		/// <returns>A string representation of <paramref name="dateTime"/> formatted according to the predefined date and time
		/// pattern.</returns>
		public static string FormatDateTime(DateTime dateTime) => dateTime.ToString(_DATE_TIME_FORMAT);

		/// <summary>
		/// Surrounds the given value with single quotes
		/// </summary>
		/// <param name="value"></param>
		/// <returns>The value surrounded by single quotes</returns>
		public static string Stringify(object value) => $"'{value}'";
	}
}
