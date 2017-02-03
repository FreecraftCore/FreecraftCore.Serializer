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
	public class ComplexTypeSerializerDecorator<TComplexType> : ITypeSerializerStrategy<TComplexType> //TComplex type should be a class. Can't constraint it though.
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
		private IEnumerable<IMemberSerializationMediator<TComplexType>> orderedMemberInfos { get; }

		//Complex types should NEVER require context. It should be designed to avoid context requireing complex types.
		/// <inheritdoc />
		public SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.Contextless;

		public ComplexTypeSerializerDecorator([NotNull] IEnumerable<IMemberSerializationMediator<TComplexType>> serializationDirections) //todo: create a better way to provide serialization instructions
		{
			//These can be empty. If there are no members on a type there won't be anything to serialize.
			if (serializationDirections == null)
				throw new ArgumentNullException(nameof(serializationDirections), $"Provided argument {nameof(serializationDirections)} is null.");

			if(!typeof(TComplexType).IsClass)
				throw new ArgumentException($"Provided generic Type: {typeof(TComplexType).FullName} must be a reference type.", nameof(TComplexType));

			orderedMemberInfos = serializationDirections;
		}

		//TODO: Error handling
		/// <inheritdoc />
		public TComplexType Read(IWireMemberReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			TComplexType instance = instanceGeneratorDelegate();

			foreach (IMemberSerializationMediator<TComplexType> serializerInfo in orderedMemberInfos)
			{
				serializerInfo.SetMember(instance, source);
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

			foreach(IMemberSerializationMediator<TComplexType> serializerInfo in orderedMemberInfos)
			{
				serializerInfo.ReadMember(value, dest);
			}
		}
	}
}
