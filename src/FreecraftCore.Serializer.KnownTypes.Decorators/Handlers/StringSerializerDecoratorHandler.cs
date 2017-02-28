using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;


namespace FreecraftCore.Serializer.KnownTypes
{
	public class StringSerializerDecoratorHandler : DecoratorHandler
	{
		public StringSerializerDecoratorHandler([NotNull] IContextualSerializerProvider serializerProvider)
			: base(serializerProvider)
		{

		}

		/// <inheritdoc />
		public override bool CanHandle(ISerializableTypeContext context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			//We can handle strings. Maybe char[] but that's an odd case.
			return context.TargetType == typeof(string);
		}

		//TODO: Refactor
		/// <inheritdoc />
		protected override ITypeSerializerStrategy<TType> TryCreateSerializer<TType>(ISerializableTypeContext context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			if(typeof(TType) != typeof(string))
				throw new InvalidOperationException($"{nameof(StringSerializerDecoratorHandler)} cannot handle creation of serializer decorators involves {typeof(string).FullName}.");

			if(!context.HasContextualKey())
				throw new ArgumentException($"Provided context {nameof(context)} did not contain a valid {nameof(context.BuiltContextKey)}.");

			if (context.ContextRequirement == SerializationContextRequirement.Contextless)
				return (ITypeSerializerStrategy<TType>) serializerProviderService.Get<string>(); //The caller should know what he's doing.

			//TODO: Throw on invalid metadata combinations

			ITypeSerializerStrategy<string> serializer = serializerProviderService.Get<string>();

			bool shouldTerminate = !context.BuiltContextKey.Value.ContextFlags.HasFlag(ContextTypeFlags.DontTerminate);

			//Determine which base string serializer we want to decorate
			if (!shouldTerminate)
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
						serializer = new SizeStringSerializerDecorator(new SizeIncludedStringSizeStrategy<byte>(serializerProviderService.Get<byte>(), shouldTerminate), serializer);
						break;
					case SendSizeAttribute.SizeType.Int32:
						serializer = new SizeStringSerializerDecorator(new SizeIncludedStringSizeStrategy<int>(serializerProviderService.Get<int>(), shouldTerminate), serializer);
						break;
					case SendSizeAttribute.SizeType.UShort:
						serializer = new SizeStringSerializerDecorator(new SizeIncludedStringSizeStrategy<ushort>(serializerProviderService.Get<ushort>(), shouldTerminate), serializer);
						break;

					default:
						throw new InvalidOperationException($"Encountered requested {nameof(SendSizeAttribute.SizeType)} marked on Type: {context.TargetType}.");
				}
			}

			//At this point we need to check if the string should be reversed. If it should be then we need to decorate it
			if (context.BuiltContextKey.Value.ContextFlags.HasFlag(ContextTypeFlags.Reverse))
				serializer = new ReverseStringSerializerDecorator(serializer);

			return (ITypeSerializerStrategy<TType>) serializer;
		}

		protected override IEnumerable<ISerializableTypeContext> TryGetAssociatedSerializableContexts(ISerializableTypeContext context)
		{
			//There are no interesting contexts for string serializers.
			return Enumerable.Empty<ISerializableTypeContext>();
		}
	}
}
