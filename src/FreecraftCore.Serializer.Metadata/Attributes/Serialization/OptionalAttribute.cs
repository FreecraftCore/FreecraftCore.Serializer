using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreecraftCore.Serializer.Internal;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Metadata marker that makes a field or property optional.
	/// Linking it to a <see cref="bool"/> field/property to determine whether it should be read or written
	/// </summary>
	[SerializationAttribute]
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public sealed class OptionalAttribute : Attribute
	{
		/// <summary>
		/// The name of the member that contains the bool
		/// indicating if the marked field/prop should be written.
		/// </summary>
		public string MemberName { get; }

		/// <summary>
		/// Creates a new attribute that makes the targeted field/prop optional.
		/// Uses the provided <see cref="MemberName"/> field/prop to determine whether it is written
		/// or read.
		/// </summary>
		/// <param name="memberName"></param>
		public OptionalAttribute([NotNull] string memberName)
		{
			if(string.IsNullOrWhiteSpace(memberName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(memberName));

			MemberName = memberName;
		}
	}
}
