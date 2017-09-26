using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Contract for types that offer one-type compile
	/// performance improvements.
	/// </summary>
	internal interface ICompilable
	{
		void Compile();
	}
}
