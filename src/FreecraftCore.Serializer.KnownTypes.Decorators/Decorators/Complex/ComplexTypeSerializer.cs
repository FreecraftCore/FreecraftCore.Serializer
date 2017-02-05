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

		/// <summary>
		/// General serializer provider service.
		/// </summary>
		[NotNull]
		protected IGeneralSerializerProvider serializerProviderService { get; }

		protected ComplexTypeSerializer([NotNull] IEnumerable<IMemberSerializationMediator<TType>> serializationDirections, [NotNull] IGeneralSerializerProvider serializerProvider)
		{
			//These can be empty. If there are no members on a type there won't be anything to serialize.
			if (serializationDirections == null)
				throw new ArgumentNullException(nameof(serializationDirections), $"Provided argument {nameof(serializationDirections)} is null.");

			if (serializerProvider == null)
				throw new ArgumentNullException(nameof(serializerProvider), $"Provided {nameof(serializerProvider)} service was null.");

			orderedMemberInfos = serializationDirections;
			serializerProviderService = serializerProvider;
		}

		/// <inheritdoc />
		public abstract TType Read(IWireMemberReaderStrategy source);

		/// <inheritdoc />
		public TType Read(ref TType obj, IWireMemberReaderStrategy source)
		{
			if (obj == null) throw new ArgumentNullException(nameof(obj));
			if (source == null) throw new ArgumentNullException(nameof(source));

			//Basically if someone calls this method they want us to set the members from the reader
			SetMembersFromReaderData(obj, source);

			return obj;
		}

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

		public object Read(ref object obj, IWireMemberReaderStrategy source)
		{
			TType castedObj = (TType) obj;

			SetMembersFromReaderData(castedObj, source);

			return obj;
		}
	}
}
