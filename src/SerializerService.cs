using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Fasterflect;

namespace FreecraftCore.Payload.Serializer
{
	public class SerializerService : ISerializerService
	{
		/// <summary>
		/// Known serializers (both complex/custom and library known)
		/// </summary>
		private List<ITypeSerializerStrategy> knownTypeSerializers { get; }

		/// <summary>
		/// Efficient cache map for serializers based on Type.
		/// </summary>
		private IReadOnlyDictionary<Type, ITypeSerializerStrategy> serializerMap { get; set; }

		// roslyn automatically implemented properties, in particular for get-only properties: <{Name}>k__BackingField;
		//var backingFieldName = $"<{property.Name}>k__BackingField";

		bool isCompiled { get; set; }

		public SerializerService()
		{
			//We don't inject knowns because we want end-users of the serializer to be able to easily instantiate an instance
			//of this service

			knownTypeSerializers = GetType().Assembly.GetTypes()
				.Where(t => t.GetCustomAttribute<KnownTypeSerializerAttribute>() != null)
				.Select(t => Activator.CreateInstance(t) as ITypeSerializerStrategy)
				.ToList();
		}

		public TTypeToDeserializeTo Deserialize<TTypeToDeserializeTo>(byte[] data)
			where TTypeToDeserializeTo : new()
		{
			if (!isCompiled)
				throw new InvalidOperationException($"Cannot use the service until it's compiled.");

			if (data == null)
				throw new ArgumentNullException(nameof(data), $"Provided bytes {nameof(data)} must not be null.");

			if (!isTypeRegistered(typeof(TTypeToDeserializeTo)))
				throw new InvalidOperationException($"Tried to deserialize Type: {typeof(TTypeToDeserializeTo).FullName} but the type is not known by the serializer service.");

			//TODO: Error handling
			using (var reader = new DefaultWireMemberReaderStrategy(data))
			{
				//TODO: Null checking
				return ((ITypeSerializerStrategy<TTypeToDeserializeTo>)serializerMap[typeof(TTypeToDeserializeTo)]).Read(reader);
			}
		}

		public bool isTypeRegistered(Type type)
		{
			//if not built we don't know any type
			if (serializerMap == null)
				return false;

			return serializerMap.ContainsKey(type);
		}

		private bool EnsureTypesInGraphAreKnown(Type typeToRegister)
		{
			//To register a type we must traverse the object graph and work our way up
			//This will cause hangs if there is a circular reference
			foreach (Type t in typeToRegister.MembersWith<WireMemberAttribute>(MemberTypes.Field | MemberTypes.Property, Flags.InstanceAnyVisibility).Select(mi => mi.Type()))
			{
				bool isContainedTypeKnown = false;

				foreach (var typeSerializer in knownTypeSerializers)
				{
					isContainedTypeKnown = isContainedTypeKnown || (typeSerializer.SerializerType == t);

					//Save time
					if (isContainedTypeKnown)
						break;
				}

				if (!isContainedTypeKnown)
				{
					//TODO: Error handling
					//if the type isn't known then recursively register it
					bool result = (bool)this.CallMethod(new Type[] { t }, nameof(RegisterType));

					if (!result)
						throw new InvalidOperationException($"Failed to register contained type {t.FullName} contained within Type: {typeToRegister}");
				}
			}

			return true;
		}

		public bool RegisterType<TTypeToRegister>()
			where TTypeToRegister : new()
		{
			if (isCompiled)
				throw new InvalidOperationException($"Cannot register new types after the service has been compiled");

			if (EnsureTypesInGraphAreKnown(typeof(TTypeToRegister)))
			{
				//add the complex type serializer
				ComplexTypeSerializerStrategy<TTypeToRegister> serializer = new ComplexTypeSerializerStrategy<TTypeToRegister>(knownTypeSerializers);

				//Add it to the list of known serializers
				knownTypeSerializers.Add(serializer);
			}
			else
				return false;

			return true;
		}

		public byte[] Serialize<TTypeToSerialize>(TTypeToSerialize data)
			where TTypeToSerialize : new()
		{
			if (!isCompiled)
				throw new InvalidOperationException($"Cannot use the service until it's compiled.");

			if (!isTypeRegistered(typeof(TTypeToSerialize)))
				throw new InvalidOperationException($"Tried to serialize Type: {typeof(TTypeToSerialize).FullName} but the type is not known by the serializer service.");

			//TODO: Error handling
			using (var writer = new DefaultWireMemberWriterStrategy())
			{
				((ITypeSerializerStrategy<TTypeToSerialize>)serializerMap[typeof(TTypeToSerialize)]).Write(data, writer);

				return writer.GetBytes();
			}	
		}

		public void Compile()
		{
			isCompiled = true;

			Dictionary<Type, ITypeSerializerStrategy> serializerRouteMap = new Dictionary<Type, ITypeSerializerStrategy>(knownTypeSerializers.Count);

			foreach(var serializer in knownTypeSerializers)
			{
				serializerRouteMap[serializer.SerializerType] = serializer;
			}

			serializerMap = serializerRouteMap;
		}
	}
}
