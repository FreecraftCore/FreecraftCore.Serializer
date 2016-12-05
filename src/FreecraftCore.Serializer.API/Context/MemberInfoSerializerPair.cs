using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Tuple-like pair of the <see cref="MemberInfo"/> context and the corresponding
	/// <see cref="ITypeSerializerStrategy"/> for serializing the member.
	/// </summary>
	public class MemberAndSerializerPair
	{
		/// <summary>
		/// Cached <see cref="MemberInfo"/>.
		/// </summary>
		public MemberInfo MemberInformation { get; }

		/// <summary>
		/// Serializer to serialize for the <see cref="MemberInformation"/>.
		/// </summary>
		public ITypeSerializerStrategy TypeSerializer { get; }

		public MemberAndSerializerPair(MemberInfo memberInfo, ITypeSerializerStrategy serializer)
		{
			if (memberInfo == null)
				throw new ArgumentNullException(nameof(memberInfo), $"Provided argument {nameof(memberInfo)} is null.");

			if (serializer == null)
				throw new ArgumentNullException(nameof(serializer), $"Provided argument {nameof(serializer)} is null.");

			MemberInformation = memberInfo;
			TypeSerializer = serializer;
		}
	}
}
