using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FreecraftCore.Serializer.KnownTypes
{
	public class StringSerializerDecoratorHandler : DecoratorHandler
	{
		public StringSerializerDecoratorHandler(IContextualSerializerProvider serializerProvider)
			: base(serializerProvider)
		{

		}

		public override bool CanHandle(ISerializableTypeContext context)
		{
			//We can handle strings. Maybe char[] but that's an odd case.
			return context.TargetType == typeof(string);
		}

		protected override ITypeSerializerStrategy<TType> TryCreateSerializer<TType>(ISerializableTypeContext context)
		{
			if (context.ContextRequirement == SerializationContextRequirement.Contextless)
				return new StringSerializerStrategy() as ITypeSerializerStrategy<TType>; //The caller should know what he's doing.

			//TODO: Throw on invalid metadata combinations

			ITypeSerializerStrategy<string> serializer = serializerProviderService.Get<string>();

			//Determine which base string serializer we want to decorate
			if (context.BuiltContextKey.Value.ContextFlags.HasFlag(ContextTypeFlags.DontTerminate))
				serializer = new DontTerminateStringSerializerDecorator(serializer);

			//It is possible that the WoW protocol expects a fixed-size string that both client and server know the length of
			//This can be seen in the first packet Auth_Challenge: uint8   gamename[4];
			//It can be seen that this field is a fixed length string (byte array)
			if (context.BuiltContextKey.Value.ContextFlags.HasFlag(ContextTypeFlags.FixedSize))
			{
				//read the key. It will have the size
				int size = context.BuiltContextKey.Value.ContextSpecificKey.Key;

				serializer = new SizeStringSerializerDecorator(new FixedSizeStringSizeStrategy(context.BuiltContextKey.Value.ContextSpecificKey.Key), serializer);
			}

			//It is also possible that the WoW protocol expects a length prefixed string
			if(context.BuiltContextKey.Value.ContextFlags.HasFlag(ContextTypeFlags.SendSize))
			{
				//This is an odd choice but if they mark it with conflicting metdata maybe we should throw?
				switch ((SendSizeAttribute.SizeType)context.BuiltContextKey.Value.ContextSpecificKey.Key)
				{
					case SendSizeAttribute.SizeType.Byte:
						serializer = new SizeStringSerializerDecorator(new SizeIncludedStringSizeStrategy<byte>(serializerProviderService.Get<byte>()), serializer);
						break;
					case SendSizeAttribute.SizeType.Int32:
						serializer = new SizeStringSerializerDecorator(new SizeIncludedStringSizeStrategy<int>(serializerProviderService.Get<int>()), serializer);
						break;
					case SendSizeAttribute.SizeType.UShort:
						serializer = new SizeStringSerializerDecorator(new SizeIncludedStringSizeStrategy<ushort>(serializerProviderService.Get<ushort>()), serializer);
						break;

					default:
						throw new InvalidOperationException($"Encountered requested {nameof(SendSizeAttribute.SizeType)} marked on Type: {context.TargetType}.");
				}
			}

			//At this point we need to check if the string should be reversed. If it should be then we need to decorate it
			if (context.BuiltContextKey.Value.ContextFlags.HasFlag(ContextTypeFlags.Reverse))
				serializer = new ReverseStringSerializerDecorator(serializer);

			return serializer as ITypeSerializerStrategy<TType>;
		}

		protected override IEnumerable<ISerializableTypeContext> TryGetAssociatedSerializableContexts(ISerializableTypeContext context)
		{
			//There are no interesting contexts for string serializers.
			return Enumerable.Empty<ISerializableTypeContext>();
		}
	}
}
