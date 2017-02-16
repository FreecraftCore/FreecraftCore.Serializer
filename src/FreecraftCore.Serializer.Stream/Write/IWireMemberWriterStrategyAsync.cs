using System;
using System.IO;
using JetBrains.Annotations;

#if !NET35
using System.Threading.Tasks;
#endif


namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Contract for objects that provide wire stream writing.
	/// </summary>
	public interface IWireStreamWriterStrategyAsync : IDisposable
	{
#if !NET35
		
#endif
	}
}
