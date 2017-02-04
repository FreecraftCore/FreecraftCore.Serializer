using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	public abstract class ComplexTypeSerializer<TType> : ITypeSerializerStrategy<TType>
	{
		/// <inheritdoc />
		public virtual Type SerializerType { get; } = typeof(TType);

		/// <inheritdoc />
		public SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.Contextless;

		/// <summary>
		/// Ordered pairs of known serializer references and the memberinfos for wiremembers.
		/// </summary>
		[NotNull]
		protected IEnumerable<IMemberSerializationMediator<TType>> orderedMemberInfos { get; }

		protected ComplexTypeSerializer([NotNull] IEnumerable<IMemberSerializationMediator<TType>> serializationDirections)
		{
			//These can be empty. If there are no members on a type there won't be anything to serialize.
			if (serializationDirections == null)
				throw new ArgumentNullException(nameof(serializationDirections), $"Provided argument {nameof(serializationDirections)} is null.");

			orderedMemberInfos = serializationDirections;
		}

		/// <inheritdoc />
		public abstract TType Read(IWireMemberReaderStrategy source);

		/// <inheritdoc />
		public abstract TType Read(ref TType obj, IWireMemberReaderStrategy source);

		/// <inheritdoc />
		public abstract void Write(TType value, IWireMemberWriterStrategy dest);

		/// <inheritdoc />
		void ITypeSerializerStrategy.Write(object value, IWireMemberWriterStrategy dest)
		{
			Write((TType)value, dest);
		}

		/// <inheritdoc />
		object ITypeSerializerStrategy.Read(IWireMemberReaderStrategy source)
		{
			return Read(source);
		}

		protected void SetMembersFromReaderData(TType obj, IWireMemberReaderStrategy source)
		{
			foreach (IMemberSerializationMediator<TType> serializerInfo in orderedMemberInfos)
			{
				serializerInfo.SetMember(obj, source);
			}
		}

		protected void WriteMemberData(TType obj, IWireMemberWriterStrategy dest)
		{
			foreach (IMemberSerializationMediator<TType> serializerInfo in orderedMemberInfos)
			{
				serializerInfo.WriteMember(obj, dest);
			}
		}
	}
}
