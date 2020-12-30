# FreecraftCore.Serializer

FreecraftCore is an open-source C#/.NET World of Warcraft emulation project. It is derived from the reverse engineering work done by the Mangos, Trinitycore projects as well as individuals in the emulation community.

The FreecraftCore.Serializer is an attribute-based/metadata binary serialization library. It uses [C# source generators](https://devblogs.microsoft.com/dotnet/introducing-c-source-generators/) to generate high performance serialization code for annotated DTO/Models. It's a based on Blizzard's [JAM](https://www.youtube.com/watch?v=hCsEHYwjqVE) library which was used to write the networking for World of Warcraft. It's also inspired by the design of [Protobuf-net](https://github.com/mgravell/protobuf-net) by Marc.

The library originally used introspection/reflection at runtime to build object-oriented serialization models based on the object graphs of serializable types. This changed in the recent version 4 which now utilizes C# Source Generators to emit serialization source code. Utilizing newer .NET API's such as Span\<T\> and System.Memory/Unsafe it's now able to achieve significantly fewer allocations, less buffer copies and is both highly inlined by JIT and significantly more AOT friendly.

## How to use?

1. Using the library is to include this Metadata Nuget Packaged called **[FreecraftCore.Serializer.Metadata](https://www.nuget.org/packages/FreecraftCore.Serializer.Metadata)**. This will contain the Attributes and various other metadata types required to annotate serializable DTOs/Models. See the section about creating a serializable type for more information on what to do with this part of the library. Most important attributes are \[WireMessageType\], \[WireDataContract\] and \[WireMember\].

2. Include the nuget package **[FreecraftCore.Serializer.Compiler](https://www.nuget.org/packages/FreecraftCore.Serializer.Compiler)** on any csproj that requires serialization code be generated. Usually you'll want to include both the Metadata and Compiler (C# source generator) together but for special scenarios you may not want to. Including this nuget package will insert the **serialization source code** into the compilation step in Visual Studio. It'll also generate a debug folder which should **not** be included in the compilation of the assembly. It will conflict with the hidden source generated. Include the following in your csproj to exclude this debug source from the compilation:

```
  <PropertyGroup>
    <EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
    <CompilerGeneratedFilesOutputPath>SerializerDebug</CompilerGeneratedFilesOutputPath>
  </PropertyGroup>

  <ItemGroup>
    <Compile Remove="SerializerDebug\**" />
    <None Include="SerializerDebug\**" />
  </ItemGroup>
```

3. To actually serialize objects you must include **[FreecraftCore.Serializer](https://www.nuget.org/packages/FreecraftCore.Serializer)** as it contains types capable of actually serializing \[WireMessageTypes\]. The SerializerService is the object capable of serializing objects. Simply create it and you'll be able to utilize the following APIs for all **Wire Message Types**. Polymorphic serialization of **Wire Message Types** required additional setup though.

```C#
SerializerService serializer = new SerializerService();
serializer.Read<MessageType>(buffer, ref offset);
```

## Implementation

The Serializer reflects on Types for attributes (looks at metadata) marked on wire types to build serializers at compile time. The built-in primitive datatype serialization is based on Trinitycore's [ByteBuffer](https://github.com/TrinityCore/TrinityCore/blob/3.3.5/src/server/shared/Packets/ByteBuffer.h). All serialization is like a graph/tree like structure mirroing an object/type graph with the bottom of the tree nodes essentially being basic primitive type serializers.

In short, FreecraftCore.Serializer knows how to serialize primitives in the fashion that the WoW protocol expects. However it supports common binary serialization patterns which allows it to be used for general binary DTO/Model building and serialization. This library can be and is used for binary message serialization for multiple games.

To understand how the attributes for this serializer work I'd recommend reading the documentation for a similar project called Protobuf-Net [here](https://www.codeproject.com/articles/642677/protobuf-net-the-unofficial-manual).

## Creating a Serializable Type

```C#
[WireDataContract] //mark wire messages with this attribute
public class MyMessagePayload
{
  [WireMember(1)] //Mark members you want sent over the network with [WireMember] attributes. A unique per class int key is required.
  public int Damage;
 
  [WireMember(5)] //the keys don't need to go in sequence. No benefit really.
  public string TargetName;
  
  [WireMember(7)]
  public MyCustomType Instance; //you can send custom types over the network too.
  
  //Arrays, Enums and even some basic Polymorphism works!
  
  public MyMessagePayload() //as of right now the serializer requires a public parameterless ctor. This will be changed in the future
  {
  
  }
}

[WireDataContract] //mark wire messages with this attribute
public class MyCustomType 
{
  //Properties are supported too!
  [WireMember(1)]
  public int data { get; internal set; } //At minimum property setters must be at least internal.
  
  //Even generic wire data types work!
  [WireMember(2)]
  public Vector3<int> Data2 { get; internal set; }
  
  public MyMessagePayload()
  {
  
  }
}
```

## Unsupported Serialization Scenarios

```C#
[KnownSize(5)]
[WireMember(1)]
SomeType[][] //Multidimensional arrays are not supported
```

```C#
[WireMember(1)]
IEnumerable<SomeType> //Any collection except arrays are not supported
```

```C#
[WireMember(1)]
object //polymorphic serialization without a known base-type is not supported
```

```C#
[WireMessageType]
class Generic<T> //It's currently not possible to create generic WireMessageTypes (top level types)
```

## Licensing

For regular users this repository is licensed under AGPL 3.0. Seperate from the AGPL 3.0, an additional unrestricted, non-exclusive, perpetual, and irrevocable license is also granted to Andrew Blakely for all works in this repository.
