using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
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

			//We no longer require registered types to be classes
			//if(!typeof(TComplexType).GetTypeInfo().IsClass)
			//	throw new ArgumentException($"Provided generic Type: {typeof(TComplexType).FullName} must be a erence type.", nameof(TComplexType));

			prototypeGeneratorService = prototypeGenerator;

			//This serializer is the finally link the chain when it comes to polymorphic serialization
			//Theore it must deal with deserialization of all members by dispatching from top to bottom (this serializer) to read the members
			//to do so efficiently we must cache an array of the Type that represents the linear class hierarchy reversed
			List<Type> typeHierarchy = ComputeTypeHierarchy(typeof(TComplexType).GetTypeInfo(), typeof(object));

			//reverse the collection to the proper order
			typeHierarchy.Reverse();
			reversedInheritanceHierarchy = typeHierarchy;
		}

		public List<Type> ComputeTypeHierarchy(TypeInfo info, Type finalBaseType)
		{
			if (typeof(TComplexType).GetTypeInfo().BaseType == null || typeof(TComplexType).GetTypeInfo().BaseType == finalBaseType)
				return Enumerable.Empty<Type>().ToList(); //make it an empty collection if there are no base types
			else
			{
				List<Type> tempTypeList = new List<Type>(2);

				//add every Type to the collection (not all may have serializers or be involved in deserialization)
				Type baseType = typeof(TComplexType).GetTypeInfo().BaseType;

				while (baseType != null && typeof(TComplexType).GetTypeInfo().BaseType != finalBaseType)
				{
					tempTypeList.Add(baseType);

					baseType = baseType.GetTypeInfo().BaseType;
				}

				return tempTypeList;
			}
		}

		//TODO: Error handling
		/// <inheritdoc />
		public override TComplexType Read(IWireStreamReaderStrategy source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			//We need to do this oddity for structs
			TComplexType obj = Read(prototypeGeneratorService.Create(), source);

			//invoke after deserialization if it's available
			(obj as ISerializationEventListener)?.OnAfterDeserialization();

			return obj;
		}

		private TComplexType Read(TComplexType obj, IWireStreamReaderStrategy source)
		{
			//read all base type members first
			//WARNING: This caused HUGE perf probleems. Several orders of magnitude slower than Protobuf
			//We MUST not check if the serializer exists and we must precache the gets.
			if(reversedInheritanceHierarchy.Count() != 0)
				foreach(Type t in reversedInheritanceHierarchy)
					if(serializerProviderService.HasSerializerFor(t))
						serializerProviderService.Get(t).ReadIntoObject(obj, source);

			SetMembersFromReaderData(obj, source);

			return obj;
		}

		private async Task<TComplexType> ReadAsync(TComplexType obj, IWireStreamReaderStrategyAsync source)
		{
			//read all base type members first
			//WARNING: This caused HUGE perf probleems. Several orders of magnitude slower than Protobuf
			//We MUST not check if the serializer exists and we must precache the gets.
			if (reversedInheritanceHierarchy.Count() != 0)
				foreach (Type t in reversedInheritanceHierarchy)
					if (serializerProviderService.HasSerializerFor(t))
						await serializerProviderService.Get(t).ReadIntoObjectAsync(obj, source);

			await SetMembersFromReaderDataAsync(obj, source);

			return obj;
		}

		//TODO: Error handling
		/// <inheritdoc />
		public override void Write(TComplexType value, IWireStreamWriterStrategy dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//invoke before serialization if it's available
			(value as ISerializationEventListener)?.OnBeforeSerialization();

			//Writes base members first and goes down the inheritance line
			//WARNING: This caused HUGE perf probleems. Several orders of magnitude slower than Protobuf
			//We MUST not check if the serializer exists and we must precache the gets.
			if (reversedInheritanceHierarchy.Count() != 0)
				foreach (Type t in reversedInheritanceHierarchy)
					if (serializerProviderService.HasSerializerFor(t)) //TODO: Should we remove this check for perf?
						serializerProviderService.Get(t).ObjectIntoWriter(value, dest);

			WriteMemberData(value, dest);
		}

		/// <inheritdoc />
		public override async Task WriteAsync(TComplexType value, IWireStreamWriterStrategyAsync dest)
		{
			if (dest == null) throw new ArgumentNullException(nameof(dest));

			//invoke before serialization if it's available
			(value as ISerializationEventListener)?.OnBeforeSerialization();

			//Writes base members first and goes down the inheritance line
			//WARNING: This caused HUGE perf probleems. Several orders of magnitude slower than Protobuf
			//We MUST not check if the serializer exists and we must precache the gets.
			if (reversedInheritanceHierarchy.Count() != 0)
				foreach (Type t in reversedInheritanceHierarchy)
					if (serializerProviderService.HasSerializerFor(t)) //TODO: Should we remove this check for perf?
						await serializerProviderService.Get(t).ObjectIntoWriterAsync(value, dest);

			//Write our own members now.
			await WriteMemberDataAsync(value, dest);
		}

		/// <inheritdoc />
		public override async Task<TComplexType> ReadAsync(IWireStreamReaderStrategyAsync source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			TComplexType obj = await ReadAsync(prototypeGeneratorService.Create(), source);

			//invoke after deserialization if it's available
			(obj as ISerializationEventListener)?.OnAfterDeserialization();

			return obj;
		}
	}
}
