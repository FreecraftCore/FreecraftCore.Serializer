using System;
using System.Collections.Generic;
using System.Text;

namespace FreecraftCore.Serializer
{
	public sealed class TypeNameStringBuilder
	{
		public Type ActualType { get; }

		public TypeNameStringBuilder(Type actualType)
		{
			ActualType = actualType;
		}

		public override string ToString()
		{
			return GetFriendlyName(ActualType);
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
