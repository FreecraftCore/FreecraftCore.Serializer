using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FreecraftCore.Serializer
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public abstract class BaseGenericListAttribute : Attribute, IEnumerable<Type>
	{
		public abstract IEnumerator<Type> GetEnumerator();

		internal BaseGenericListAttribute()
		{

		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
