using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreecraftCore.Serializer;
using NUnit.Framework;

namespace FreecraftCore
{
	[TestFixture]
	public class TestCustomTypeSerializerIncludeTests
	{
		[Test]
		public void Test_Can_Register_Custom_Type_Serializer()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<TypeWithCustomSerializer>();
			serializer.Compile();

			//act
			byte[] bytes = serializer.Serialize(new TypeWithCustomSerializer());
			TypeWithCustomSerializer obj = serializer.Deserialize<TypeWithCustomSerializer>(bytes);

			//assert
			Assert.NotNull(bytes);
			Assert.NotNull(obj);

			Assert.AreEqual(1000, bytes.Length);
			Assert.AreEqual("TEST", obj.TestString);
		}

		[IncludeCustomTypeSerializer(typeof(TypeWithCustomSerializerCustomTypeSerializer))]
		[WireDataContract]
		public class TypeWithCustomSerializer
		{
			public string TestString { get; }

			/// <inheritdoc />
			public TypeWithCustomSerializer(string testString)
			{
				TestString = testString;
			}

			public TypeWithCustomSerializer()
			{
				
			}
		}

		public sealed class TypeWithCustomSerializerCustomTypeSerializer : SimpleTypeSerializerStrategy<TypeWithCustomSerializer>
		{
			/// <inheritdoc />
			public override SerializationContextRequirement ContextRequirement { get; } = SerializationContextRequirement.Contextless;

			/// <inheritdoc />
			public override TypeWithCustomSerializer Read(IWireStreamReaderStrategy source)
			{
				return new TypeWithCustomSerializer("TEST");
			}

			/// <inheritdoc />
			public override void Write(TypeWithCustomSerializer value, IWireStreamWriterStrategy dest)
			{
				dest.Write(new byte[1000]);
			}

			/// <inheritdoc />
			public override async Task WriteAsync(TypeWithCustomSerializer value, IWireStreamWriterStrategyAsync dest)
			{
				dest.Write(new byte[1000]);
			}

			/// <inheritdoc />
			public override async Task<TypeWithCustomSerializer> ReadAsync(IWireStreamReaderStrategyAsync source)
			{
				return new TypeWithCustomSerializer("TEST");
			}
		}
	}
}
