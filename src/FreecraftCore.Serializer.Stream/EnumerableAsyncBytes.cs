#if !NET35
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer
{
	public class EnumerableAsyncBytes : IEnumerable<byte>
	{
		[NotNull]
		public Task<byte[]> AsyncBytes { get; }

		public EnumerableAsyncBytes([NotNull] Task<byte[]> asyncBytes)
		{
			if (asyncBytes == null) throw new ArgumentNullException(nameof(asyncBytes));

			AsyncBytes = asyncBytes;
		}

		/// <inheritdoc />
		public IEnumerator<byte> GetEnumerator()
		{
			//Result blocks and then returns the enumerator
			IEnumerable<byte> bytes = AsyncBytes.Result;

			return bytes.GetEnumerator();
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			//Result blocks and then returns the enumerator
			return AsyncBytes.Result.GetEnumerator();
		}
	}
}
#endif
