using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fasterflect;
using System.Reflection;

namespace FreecraftCore.Serializer
{

	public static class ComplexTypeSerializerDecorator
	{
		/// <summary>
		/// Creates a generic <see cref="ComplexTypeSerializerStrategy{TComplexType}"/> without access to compiletime type.
		/// </summary>
		/// <param name="complexType">Complex type for the strategy to serialize.</param>
		/// <param name="serializerProvider">Serialization provider.</param>
		/// <returns>A new instance of <see cref="ComplexTypeSerializerStrategy{TComplexType}"/>.</returns>
		public static ITypeSerializerStrategy Create(Type complexType, IEnumerable<MemberAndSerializerPair> serializationDirections)
		{
			return typeof(ComplexTypeSerializerDecorator<>).MakeGenericType(complexType)
				.CreateInstance(serializationDirections) as ITypeSerializerStrategy;
		}
	}

	/// <summary>
	/// Represents a complex type definition that combines multiple knowntypes or other complex types.
	/// </summary>
	/// <typeparam name="TComplexType"></typeparam>
	public class ComplexTypeSerializerDecorator<TComplexType> : ITypeSerializerStrategy<TComplexType>
		where TComplexType : new() //in .Net 4.0 > this is ok to do. Won't cause poor preformance
	{
		/// <summary>
		/// Indicates the <see cref="TType"/> of the serializer.
		/// </summary>
		public Type SerializerType { get { return typeof(TComplexType); } }

		/// <summary>
		/// Ordered pairs of known serializer references and the memberinfos for wiremembers.
		/// </summary>
		IEnumerable<MemberAndSerializerPair> orderedMemberInfos { get; }

		//Complex types should NEVER require context. It should be designed to avoid context requireing complex types.
		public SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.Contextless;

		public ComplexTypeSerializerDecorator(IEnumerable<MemberAndSerializerPair> serializationDirections) //todo: create a better way to provide serialization instructions
		{
			//These can be empty. If there are no members on a type there won't be anything to serialize.
			if (serializationDirections == null)
				throw new ArgumentNullException(nameof(serializationDirections), $"Provided argument {nameof(serializationDirections)} is null.");

			orderedMemberInfos = serializationDirections;
		}

		//TODO: Error handling
		public TComplexType Read(IWireMemberReaderStrategy source)
		{
			TComplexType instance = new TComplexType();

			foreach (MemberAndSerializerPair serializerInfo in orderedMemberInfos)
			{
				instance.TrySetValue(serializerInfo.MemberInformation.Name, serializerInfo.TypeSerializer.Read(source));
			}

			return instance;
		}

		void ITypeSerializerStrategy.Write(object value, IWireMemberWriterStrategy dest)
		{
			Write((TComplexType)value, dest);
		}

		object ITypeSerializerStrategy.Read(IWireMemberReaderStrategy source)
		{
			return Read(source);
		}

		//TODO: Error handling
		public void Write(TComplexType value, IWireMemberWriterStrategy dest)
		{
			foreach(MemberAndSerializerPair serializerInfo in orderedMemberInfos)
			{
				//TODO: Check how TC handles optionals or nulls.
				//Do we write nothing? Do we write 0?
				object memberValue = value.TryGetValue(serializerInfo.MemberInformation.Name);

				if (value == null || memberValue == null)
					continue;

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
