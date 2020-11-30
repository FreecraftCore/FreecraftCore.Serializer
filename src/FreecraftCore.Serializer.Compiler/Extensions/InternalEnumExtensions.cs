using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace FreecraftCore.Serializer
{
	public static class InternalEnumExtensions
	{
		//This must exist because .NET Core/.NET 5 is required for generic Parse
		//See: https://docs.microsoft.com/en-us/dotnet/api/system.enum.parse?view=net-5.0#System_Enum_Parse__1_System_String_System_Boolean_
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TEnumType Parse<TEnumType>(string input, bool ignoreCase)
		{
			return (TEnumType) Enum.Parse(typeof(TEnumType), input, ignoreCase);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TEnumType ParseFull<TEnumType>(string input, bool ignoreCase)
		{
			if (input.Contains("."))
				return (TEnumType)Enum.Parse(typeof(TEnumType), input.Split('.').Last(), ignoreCase);
			else
				return (TEnumType)Enum.Parse(typeof(TEnumType), input, ignoreCase);
		}
	}
}
