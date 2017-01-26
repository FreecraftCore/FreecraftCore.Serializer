using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Fasterflect;
using System.Reflection;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// Represents a complex type definition that combines multiple knowntypes or other complex types.
	/// </summary>
	/// <typeparam name="TComplexType"></typeparam>
	public class ComplexTypeSerializerDecorator<TComplexType> : ITypeSerializerStrategy<TComplexType>
	{
		//New constaint is gone; this provided an efficient way to create new instances over Activator.CreateInstance.
		//Search compiled lambda and new constaint operator on google to see dicussions about it
		public static Func<TComplexType> instanceGeneratorDelegate { get; } = Expression.Lambda<Func<TComplexType>>(Expression.New(typeof(TComplexType))).Compile();

		/// <inheritdoc />
		public Type SerializerType { get; } = typeof(TComplexType);

		/// <summary>
		/// Ordered pairs of known serializer references and the memberinfos for wiremembers.
		/// </summary>
		[NotNull]
		IEnumerable<MemberAndSerializerPair<TComplexType>> orderedMemberInfos { get; }

		//Complex types should NEVER require context. It should be designed to avoid context requireing complex types.
		/// <inheritdoc />
		public SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.Contextless;

		public ComplexTypeSerializerDecorator([NotNull] IEnumerable<MemberAndSerializerPair<TComplexType>> serializationDirections) //todo: create a better way to provide serialization instructions
		{
			//These can be empty. If there are no members on a type there won't be anything to serialize.
			if (serializationDirections == null)
				throw new ArgumentNullException(nameof(serializationDirections), $"Provided argument {nameof(serializationDirections)} is null.");

			orderedMemberInfos = serializationDirections;
		}

		//TODO: Error handling
		/// <inheritdoc />
		public TComplexType Read(IWireMemberReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			TComplexType instance = instanceGeneratorDelegate();

			foreach (MemberAndSerializerPair<TComplexType> serializerInfo in orderedMemberInfos)
			{
				instance.TrySetValue(serializerInfo.MemberInformation.Name, serializerInfo.TypeSerializer.Read(source));
			}

			return instance;
		}

		/// <inheritdoc />
		void ITypeSerializerStrategy.Write(object value, IWireMemberWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			Write((TComplexType)value, dest);
		}

		/// <inheritdoc />
		object ITypeSerializerStrategy.Read(IWireMemberReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			return Read(source);
		}

		//TODO: Error handling
		/// <inheritdoc />
		public void Write(TComplexType value, IWireMemberWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			foreach(MemberAndSerializerPair<TComplexType> serializerInfo in orderedMemberInfos)
			{
				//TODO: Check how TC handles optionals or nulls.
				//Do we write nothing? Do we write 0?
				//object memberValue = value.TryGetValue(serializerInfo.MemberInformation.Name);
				object memberValue = serializerInfo.MemberGetter(value); //instead of fasterflect we use delegate to getter

				if (memberValue == null)
					throw new InvalidOperationException($"Provider FieldName: {serializerInfo.MemberInformation.Name} on Type: {serializerInfo.MemberInformation.Type()} is null. The serializer doesn't support null.");

				try
				{
					serializerInfo.TypeSerializer.Write(memberValue, dest);
				}
				catch (NullReferenceException e)
				{
					throw new InvalidOperationException($"Serializer failed to find serializer for member name: {serializerInfo.MemberInformation.Name} and type: {serializerInfo.MemberInformation.Type()}.", e);
				}
			}
		}
	}
}
