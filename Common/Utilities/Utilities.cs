using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

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
		/// <typeparam name="TEnumType">Type parameter for enumerations</typeparam>
		/// <param name="enumeration">Value of the enum</param>
		/// <returns>string</returns>
		public static string GetEnumDescription<TEnumType>(Enum enumeration)
			where TEnumType : Enum
		{
			DescriptionAttribute? descriptionAttribute = enumeration.GetType()?.GetField(enumeration.ToString())
				?.GetCustomAttributes(typeof(DescriptionAttribute), false)
				.SingleOrDefault() as DescriptionAttribute;

			return descriptionAttribute?.Description ?? string.Empty;
		}

		/// <summary>
		/// Retrieves all values of the specified enumeration type.
		/// </summary>
		/// <typeparam name="TEnumType">The enumeration type whose values are to be retrieved. This type must be an enumeration.</typeparam>
		/// <returns>An <see cref="IEnumerable{T}"/> containing all values of the specified enumeration type.</returns>
		public static IEnumerable<TEnumType> GetEnumValues<TEnumType>() => Enum.GetValues(typeof(TEnumType)).Cast<TEnumType>();

		/// <summary>
		/// Retrieves the system date time
		/// </summary>
		public static DateTime GetCurrentDateTime => DateTime.Now;

		/// <summary>
		/// Formats the specified <see cref="DateTime"/> value as a string using a predefined format.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime"/> value to format.</param>
		/// <returns>A string representation of the <paramref name="dateTime"/> value in the predefined format.</returns>
		public static string FormatDateTime(DateTime dateTime, string dateFormat = _DATE_TIME_FORMAT) 
			=> dateTime.ToString(dateFormat);

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

		/// <summary>
		/// Retrieves the public properties of the specified type.
		/// </summary>
		/// <typeparam name="TObject">The type whose public properties are to be retrieved.</typeparam>
		/// <returns>An <see cref="IEnumerable{PropertyInfo}"/> containing the public properties of the specified type. The collection
		/// is empty if the type has no public properties.</returns>
		public static IEnumerable<PropertyInfo> GetObjectProperties<TObject>() 
			=> typeof(TObject).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToList().OrderBy(property => property.MetadataToken);

		/// <summary>
		/// Gets the value of a property using reflection
		/// </summary>
		/// <param name="object"></param>
		/// <param name="property"></param>
		/// <returns></returns>
		public static object? GetPropertyValue(object @object, PropertyInfo property)
		{
			return property.GetValue(@object);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TObject"></typeparam>
		/// <param name="expression"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		public static MemberExpression GetMemberExpression<TObject>(Expression<Func<TObject, object>> expression)
		{
			if (expression.Body is MemberExpression memberExpression)
				return memberExpression;

			if (expression.Body is UnaryExpression unaryExpression)
				return (MemberExpression)unaryExpression.Operand;

			throw new ArgumentException("Invalid expression");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TObject"></typeparam>
		/// <param name="propertyExpression"></param>
		/// <returns></returns>
		public static PropertyInfo GetPropertyInfo<TObject>(Expression<Func<TObject, object>> propertyExpression)
		{
			var memberExpression = GetMemberExpression(propertyExpression);
			var propertyInfo = (PropertyInfo)memberExpression.Member;
			
			return propertyInfo;
		}
	}
}
