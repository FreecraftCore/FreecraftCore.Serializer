using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	internal static class TypeExtensions
	{
		/// <summary>
		/// Indicates if the provided Type is a wire message type or SHOULD be a wire message type.
		/// </summary>
		/// <typeparam name="TType">The type to check.</typeparam>
		/// <returns></returns>
		public static bool IsWireMessageType<TType>()
		{
			return IsWireMessageType(typeof(TType));
		}

		/// <summary>
		/// Indicates if the provided Type is a wire message type or SHOULD be a wire message type.
		/// </summary>
		/// <param name="type">The type to check.</param>
		/// <returns></returns>
		public static bool IsWireMessageType([NotNull] this Type type)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));

			if (type.GetCustomAttribute<WireMessageTypeAttribute>() != null)
				return true;

			if (type.GetCustomAttribute<WireDataContractAttribute>() != null)
				if (type.GetCustomAttribute<WireDataContractAttribute>().UsesSubTypeSize)
					return true; //all polymorphic serializers need Wire Message implementation
			
			return type.GetCustomAttribute<WireDataContractBaseLinkAttribute>() != null || //all polymorphic serializers need Wire Message implementation
			       typeof(ISelfSerializable).IsAssignableFrom(type);
		}

		//See: https://stackoverflow.com/questions/43634808/how-can-i-get-the-number-of-type-parameters-on-an-open-generic-in-c-sharp
		public static int GetGenericParameterCount(this Type type)
		{
			TypeInfo typeInfo = type.GetTypeInfo();
			return typeInfo.IsGenericTypeDefinition
				? typeInfo.GenericTypeParameters.Length
				: typeInfo.GenericTypeArguments.Length;
		}
	}
}
