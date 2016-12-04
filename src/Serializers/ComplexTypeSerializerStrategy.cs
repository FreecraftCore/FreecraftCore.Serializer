using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fasterflect;
using System.Reflection;

namespace FreecraftCore.Payload.Serializer
{
	public class ComplexTypeSerializerStrategy<TComplexType> : ITypeSerializerStrategy<TComplexType>
		where TComplexType : new() //in .Net 4.0 > this is ok to do. Won't cause poor preformance
	{
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

		public ComplexTypeSerializerStrategy(IEnumerable<ITypeSerializerStrategy> knownTypeSerializers)
		{
			orderedMemberInfos = typeof(TComplexType).MembersWith<WireMemberAttribute>(System.Reflection.MemberTypes.Field | System.Reflection.MemberTypes.Property, Flags.InstanceAnyVisibility)
				.OrderBy(x => x.Attribute<WireMemberAttribute>().MemberOrder).ToArray()
				.Select(x =>
				{
					ITypeSerializerStrategy strategy = knownTypeSerializers.First(s => x.Type().IsEnum ? s.SerializerType == x.Type().GetEnumUnderlyingType() : s.SerializerType == x.Type());
					return new MemberAndSerializerPair(x, !x.Type().IsEnum ? strategy : typeof(EnumSerializerDecorator<,>).MakeGenericType(x.Type(), strategy.SerializerType).CreateInstance(strategy) as ITypeSerializerStrategy);
				});

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
				if (serializerInfo.MemberInformation.MemberType == MemberTypes.Property)
					serializerInfo.TypeSerializer.CallMethod(nameof(Write), value.GetPropertyValue(serializerInfo.MemberInformation.Name), dest);
				else //it's a field
					serializerInfo.TypeSerializer.CallMethod(nameof(Write), value.GetFieldValue(serializerInfo.MemberInformation.Name), dest);
			}
		}
	}
}
