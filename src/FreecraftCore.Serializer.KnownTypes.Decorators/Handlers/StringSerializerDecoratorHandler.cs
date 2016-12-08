using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreecraftCore.Serializer.KnownTypes
{
	public class StringSerializerDecoratorHandler : DecoratorHandler
	{
		public StringSerializerDecoratorHandler(IContextualSerializerProvider serializerProvider, IContextualSerializerLookupKeyFactory keyFactory) 
			: base(serializerProvider, keyFactory)
		{

		}

		public override bool CanHandle(ISerializableTypeContext context)
		{
			//We can handle strings. Maybe char[] but that's an odd case.
			return context.TargetType == typeof(string);
		}

		protected override ITypeSerializerStrategy<TType> TryCreateSerializer<TType>(ISerializableTypeContext context)
		{
			//It is possible that the WoW protocol expects a fixed-size string that both client and server know the length of
			//This can be seen in the first packet Auth_Challenge: uint8   gamename[4];
			//It can be seen that this field is a fixed length string (byte array)
			if (context.BuiltContextKey.Value.ContextFlags.HasFlag(ContextTypeFlags.FixedSize))
			{
				//read the key. It will have the size
				int size = context.BuiltContextKey.Value.ContextSpecificKey.Key;

				return new FixedSizeStringSerializerDecorator(new FixedSizeStringSizeStrategy(context.BuiltContextKey.Value.ContextSpecificKey.Key), new StringSerializerStrategy())
					as ITypeSerializerStrategy<TType>;
			}

			if (context.ContextRequirement == SerializationContextRequirement.Contextless)
				return new StringSerializerStrategy() as ITypeSerializerStrategy<TType>; //The caller should know what he's doing.

			throw new InvalidOperationException($"Failed to provided a serializer {GetType().FullName} failed for Context: {context.ToString()}.");
		}

		protected override IEnumerable<ISerializableTypeContext> TryGetAssociatedSerializableContexts(ISerializableTypeContext context)
		{
			//There are no interesting contexts for string serializers.
			return Enumerable.Empty<ISerializableTypeContext>();
		}
	}
}
