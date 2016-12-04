using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fasterflect;
using System.Reflection;

namespace FreecraftCore.Payload.Serializer
{
	/// <summary>
	/// Represents a complex type definition that combines multiple knowntypes or other complex types.
	/// </summary>
	/// <typeparam name="TComplexType"></typeparam>
	public class ComplexTypeSerializerStrategy<TComplexType> : ITypeSerializerStrategy<TComplexType>
		where TComplexType : new() //in .Net 4.0 > this is ok to do. Won't cause poor preformance
	{
		//TODO: Move/Refactor this
		public class MemberAndSerializerPair
		{
			public MemberInfo MemberInformation { get; }

			public ITypeSerializerStrategy TypeSerializer { get; }

			public MemberAndSerializerPair(MemberInfo memberInfo, ITypeSerializerStrategy serializer)
			{
				MemberInformation = memberInfo;
				TypeSerializer = serializer;
			}
		}

		/// <summary>
		/// Indicates the <see cref="TType"/> of the serializer.
		/// </summary>
		public Type SerializerType { get { return typeof(TComplexType); } }

		/// <summary>
		/// Ordered pairs of known serializer references and the memberinfos for wiremembers.
		/// </summary>
		IEnumerable<MemberAndSerializerPair> orderedMemberInfos { get; }

		public ComplexTypeSerializerStrategy(ISerializerFactory serializerFactory)
		{
			if (serializerFactory == null)
				throw new ArgumentNullException(nameof(serializerFactory), $"Provided service {nameof(ISerializerFactory)} was null.");

			orderedMemberInfos = typeof(TComplexType).MembersWith<WireMemberAttribute>(System.Reflection.MemberTypes.Field | System.Reflection.MemberTypes.Property, Flags.InstanceAnyDeclaredOnly)
				.OrderBy(x => x.Attribute<WireMemberAttribute>().MemberOrder).ToArray()
				.Select(x => new MemberAndSerializerPair(x, serializerFactory.Create(x.Type())));

			//TODO: pre-warm fasterflect for this type
		}

		//TODO: Error handling
		public TComplexType Read(IWireMemberReaderStrategy source)
		{
			TComplexType instance = new TComplexType();

			foreach (MemberAndSerializerPair serializerInfo in orderedMemberInfos)
			{
				if (serializerInfo.MemberInformation.MemberType == MemberTypes.Property)
					instance.SetPropertyValue(serializerInfo.MemberInformation.Name, serializerInfo.TypeSerializer.CallMethod(nameof(Read), source));
				else //it's a field
					instance.SetFieldValue(serializerInfo.MemberInformation.Name, serializerInfo.TypeSerializer.CallMethod(nameof(Read), source));
			}

			return instance;
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
					serializerInfo.TypeSerializer.CallMethod(nameof(Write), memberValue, dest);
				}
				catch (NullReferenceException e)
				{
					throw new InvalidOperationException($"Serializer failed to find serializer for member name: {serializerInfo.MemberInformation.Name} and type: {serializerInfo.MemberInformation.Type()}.", e);
				}
			}
		}
	}
}
