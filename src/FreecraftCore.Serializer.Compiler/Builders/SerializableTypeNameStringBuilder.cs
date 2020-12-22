using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Text;
using FreecraftCore.Serializer.Internal;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace FreecraftCore.Serializer
{
	public sealed class SerializableTypeNameStringBuilder
	{
		public INamedTypeSymbol Symbol { get; }

		public SerializableTypeNameStringBuilder([NotNull] INamedTypeSymbol symbol)
		{
			Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
		}

		public override string ToString()
		{
			return Symbol.GetFriendlyName();
		}
	}

	public sealed class SerializableTypeNameStringBuilder<TSerializableType>
	{
		public override string ToString()
		{
			return GetFriendlyName(typeof(TSerializableType));
		}

		private static string GetFriendlyName(Type type)
		{
			string friendlyName = type.Name;
			if(type.IsGenericType)
			{
				int iBacktick = friendlyName.IndexOf('`');
				if(iBacktick > 0)
				{
					friendlyName = friendlyName.Remove(iBacktick);
				}
				friendlyName += "<";
				Type[] typeParameters = type.GetGenericArguments();
				for(int i = 0; i < typeParameters.Length; ++i)
				{
					string typeParamName = GetFriendlyName(typeParameters[i]);
					friendlyName += (i == 0 ? typeParamName : "," + typeParamName);
				}
				friendlyName += ">";
			}

			return friendlyName;
		}
	}
}
