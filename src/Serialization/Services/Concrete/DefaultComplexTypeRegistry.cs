using Fasterflect;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Payload.Serializer
{
	//TODO: Doc
	public class DefaultComplexTypeRegistry : IComplexTypeRegistry
	{
		IDictionary<Type, ITypeSerializerStrategy> knownTypeSerializers { get; }

		ISerializerDecoratorService serializerDecoratorService { get; }

		ISerializerFactory serializerFactoryService { get; }

		public DefaultComplexTypeRegistry(IDictionary<Type, ITypeSerializerStrategy> knownSerializers) //this is mutable
		{
			if (knownSerializers == null)
				throw new ArgumentNullException(nameof(knownSerializers), $"Provided {nameof(knownSerializers)} collection was null.");

			knownTypeSerializers = knownSerializers;
			serializerFactoryService = new SerializerFactory(new ReadOnlyDictionary<Type, ITypeSerializerStrategy>(knownSerializers));

			serializerDecoratorService = new DefaultSerializerDecoratorService(this, serializerFactoryService);
		}

		public bool isTypeRegistered(Type type)
		{
			return knownTypeSerializers.ContainsKey(type);
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
					isContainedTypeKnown = isContainedTypeKnown || (typeSerializer.Value.SerializerType == t);

					//Save time
					if (isContainedTypeKnown)
						break;
				}

				//TODO: Cleanup
				if (!isContainedTypeKnown)
				{
					//TODO: Error handling
					//if the type isn't known then recursively register it
					bool result = false;

					if (t.Constructor(Flags.InstancePublic) != null)
					{

						result = (bool)this.CallMethod(new Type[] { t }, nameof(RegisterType));
					}
					else
					{
						result = (bool)this.CallMethod(new Type[] { t }, nameof(TryRegisterWithADecorator));
					}

					if (!result)
						throw new InvalidOperationException($"Failed to register contained type {t.FullName} contained within Type: {typeToRegister}");
				}
			}

			return true;
		}

		private bool TryRegisterWithADecorator<TTypeToRegister>()
		{
			if (EnsureTypesInGraphAreKnown(typeof(TTypeToRegister)))
			{
				//If the type is special then we need to conver it to the collection of types we need to know about
				if (serializerDecoratorService.RequiresDecorating(typeof(TTypeToRegister)))
				{
					foreach (Type innerType in serializerDecoratorService.GrabTypesThatRequiresRegister(typeof(TTypeToRegister)))
					{
						//TODO: Error handling
						//if the type isn't known then recursively register it
						bool result = (bool)this.CallMethod(new Type[] { innerType }, nameof(RegisterType));

						if (!result)
							throw new InvalidOperationException($"Failed to register contained type {innerType.FullName} contained within Type: {typeof(TTypeToRegister).FullName}.");
					}

					knownTypeSerializers[typeof(TTypeToRegister)] = serializerDecoratorService.TryProduceDecoratedSerializer(typeof(TTypeToRegister), new SerializerDecorationContext(typeof(TTypeToRegister).GetCustomAttributes<Attribute>(false), new ReadOnlyDictionary<Type, ITypeSerializerStrategy>(this.knownTypeSerializers)));

					return true;
				}
			}

			return false;
		}

		public bool RegisterType<TTypeToRegister>()
			where TTypeToRegister : new()
		{
			if (isTypeRegistered(typeof(TTypeToRegister)))
				return true;

			if (EnsureTypesInGraphAreKnown(typeof(TTypeToRegister)))
			{
				//If the type is special then we need to conver it to the collection of types we need to know about
				if (serializerDecoratorService.RequiresDecorating(typeof(TTypeToRegister)))
				{
					foreach (Type innerType in serializerDecoratorService.GrabTypesThatRequiresRegister(typeof(TTypeToRegister)))
					{
						//TODO: Error handling
						//if the type isn't known then recursively register it
						bool result = (bool)this.CallMethod(new Type[] { innerType }, nameof(RegisterType));

						if (!result)
							throw new InvalidOperationException($"Failed to register contained type {innerType.FullName} contained within Type: {typeof(TTypeToRegister).FullName}.");
					}

					knownTypeSerializers.Add(typeof(TTypeToRegister), serializerDecoratorService.TryProduceDecoratedSerializer(typeof(TTypeToRegister), new SerializerDecorationContext(typeof(TTypeToRegister).GetCustomAttributes<Attribute>(false), new ReadOnlyDictionary<Type, ITypeSerializerStrategy>(this.knownTypeSerializers))));
				}
				else
				{
					//add the complex type serializer
					ComplexTypeSerializerStrategy<TTypeToRegister> serializer = new ComplexTypeSerializerStrategy<TTypeToRegister>(serializerFactoryService);

					//Add it to the list of known serializers
					knownTypeSerializers.Add(typeof(TTypeToRegister), serializer);
				}
			}
			else
				return false;

			return true;
		}
	}
}
