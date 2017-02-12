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
	public class ComplexTypeSerializerDecorator<TComplexType> : ComplexTypeSerializer<TComplexType> //TComplex type should be a class. Can't constraint it though.
	{
		[NotNull]
		protected IDeserializationPrototypeFactory<TComplexType> prototypeGeneratorService { get; }

		[NotNull]
		private IEnumerable<Type> reversedInheritanceHierarchy { get; }

		public ComplexTypeSerializerDecorator([NotNull] IEnumerable<IMemberSerializationMediator<TComplexType>> serializationDirections, [NotNull] IDeserializationPrototypeFactory<TComplexType> prototypeGenerator, [NotNull] IGeneralSerializerProvider serializerProvider) //todo: create a better way to provide serialization instructions
			: base(serializationDirections, serializerProvider)
		{
			if (prototypeGenerator == null) throw new ArgumentNullException(nameof(prototypeGenerator));

			if(!typeof(TComplexType).IsClass)
				throw new ArgumentException($"Provided generic Type: {typeof(TComplexType).FullName} must be a reference type.", nameof(TComplexType));

			prototypeGeneratorService = prototypeGenerator;

			//This serializer is the finally link the chain when it comes to polymorphic serialization
			//Therefore it must deal with deserialization of all members by dispatching from top to bottom (this serializer) to read the members
			//to do so efficiently we must cache an array of the Type that represents the linear class hierarchy reversed
			List<Type> typeHierarchy = new List<Type>();

			if (typeof(TComplexType).BaseType == null || typeof(TComplexType).BaseType == typeof(object))
				reversedInheritanceHierarchy = Enumerable.Empty<Type>(); //make it an empty collection if there are no base types
			else
			{
				//add every Type to the collection (not all may have serializers or be involved in deserialization)
				Type baseType = typeof(TComplexType).BaseType;

				while (baseType != null && typeof(TComplexType).BaseType != typeof(object))
				{
					typeHierarchy.Add(baseType);

					baseType = baseType.BaseType;
				}
			}

			//reverse the collection to the proper order
			typeHierarchy.Reverse();
			reversedInheritanceHierarchy = typeHierarchy;
		}

		//TODO: Error handling
		/// <inheritdoc />
		public override TComplexType Read(IWireStreamReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			TComplexType obj = Read(prototypeGeneratorService.Create(), source);

			//invoke after deserialization if it's available
			(obj as ISerializationEventListener)?.OnAfterDeserialization();

			return obj;
		}

		private TComplexType Read(TComplexType obj, IWireStreamReaderStrategy source)
		{
			//invoke before serialization if it's available
			(obj as ISerializationEventListener)?.OnBeforeSerialization();

			object castedObj = obj;

			//read all base type members first
			//WARNING: This caused HUGE perf probleems. Several orders of magnitude slower than Protobuf
			//We MUST not check if the serializer exists and we must precache the gets.
			if(reversedInheritanceHierarchy.Count() != 0)
				foreach(Type t in reversedInheritanceHierarchy)
					if(serializerProviderService.HasSerializerFor(t))
						serializerProviderService.Get(t).ReadIntoObject(ref castedObj, source);

			SetMembersFromReaderData(obj, source);

			return obj;
		}

		//TODO: Error handling
		/// <inheritdoc />
		public override void Write(TComplexType value, IWireStreamWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//Writes base members first and goes down the inheritance line
			//WARNING: This caused HUGE perf probleems. Several orders of magnitude slower than Protobuf
			//We MUST not check if the serializer exists and we must precache the gets.
			if (reversedInheritanceHierarchy.Count() != 0)
				foreach (Type t in reversedInheritanceHierarchy)
					if (serializerProviderService.HasSerializerFor(t))
						serializerProviderService.Get(t).ObjectIntoWriter(value, dest);

			WriteMemberData(value, dest);
		}
	}
}
