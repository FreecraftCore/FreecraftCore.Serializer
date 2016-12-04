using Fasterflect;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer
{
	/// <summary>
	/// The default registeration handler for complex types.
	/// </summary>
	public class DefaultComplexTypeRegistry : IComplexTypeRegistry
	{
		/// <summary>
		/// Serializer provider service.
		/// </summary>
		ISerializerProvider serializerProviderService { get; }

		/// <summary>
		/// Event that can be subscribed to for alerts on found associated types.
		/// </summary>
		public event FoundUnknownAssociatedType OnFoundUnknownAssociatedType;

		public DefaultComplexTypeRegistry(ISerializerProvider serializerProvider) //this is mutable
		{
			if (serializerProvider == null)
				throw new ArgumentNullException(nameof(serializerProvider), $"Provided {nameof(ISerializerProvider)} service was null.");

			serializerProviderService = serializerProvider;
		}

		/// <summary>
		/// Indicates if the provided <see cref="Type"/> is registered.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public bool isTypeRegistered(Type type)
		{
			return serializerProviderService.Get(type) != null;
		}

		/// <summary>
		/// Attempts to discover the the object graph for a given <see cref="Type"/>.
		/// (Goes only 1 depth; leave the consumers of the broadcast to determine if recursive calls should be made to register entire graph)
		/// </summary>
		/// <param name="typeToParse">The <see cref="Type"/> that should be inspected.</param>
		/// <returns></returns>
		private bool DiscoverObjectGraph(Type typeToParse)
		{
			//To register a type we must traverse the object graph and work our way up
			//This will cause hangs if there is a circular reference
			foreach (MemberInfo mi in typeToParse.MembersWith<WireMemberAttribute>(MemberTypes.Field | MemberTypes.Property, Flags.InstanceAnyVisibility))
			{
				//If the provider knows about this type we should continue
				if (isTypeRegistered(mi.Type()))
					continue;

				//If not we need to broadcast that we found an unknown type
				OnFoundUnknownAssociatedType?.Invoke(mi);

				//TODO: Determine if we should check if it's known now. There are odd cases like EnumType[][] which may issues. They should never exist though.
			}

			return true;
		}

		/// <summary>
		/// Attempts to register a contract for the provided <see cref="Type"/>.
		/// </summary>
		/// <returns>The type serializer if successfully registered.</returns>
		public ITypeSerializerStrategy<TTypeToRegister> RegisterType<TTypeToRegister>()
			where TTypeToRegister : new()
		{
			if (isTypeRegistered(typeof(TTypeToRegister)))
				return serializerProviderService.Get<TTypeToRegister>(); //not really much point but return the registered/known serializer if we know it

			if (DiscoverObjectGraph(typeof(TTypeToRegister)))
			{
				//Once the object graph has been fully discovered we can focus on the Type
				return new ComplexTypeSerializerStrategy<TTypeToRegister>(serializerProviderService);
			}
			else
				throw new InvalidOperationException($"Unable to discover object graph for Type: {typeof(TTypeToRegister).FullName}.");
		}
	}
}
