using System.ComponentModel;

namespace BuildHub.Common.Utilities
{
	/// <summary>
	/// A utility functions class
	/// </summary>
	public class Utilities
	{
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
	}
}
