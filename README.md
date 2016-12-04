# FreecraftCore.Payload.Serializer

FreecraftCore is an open-source C#-based 3.3.5 World of Warcraft emulation project. It is derived from the work of Mangos and Trinitycore.

The FreecraftCore.Payload.Serializer is a metadata based serializer for WoW packets based on [JAM](https://www.youtube.com/watch?v=hCsEHYwjqVE) and inspired by the design of [Protobuf-net](https://github.com/mgravell/protobuf-net)

## Implementation

The Serializer looks at metadata marked on message classes to build a simple serializer that reads and writes from a stream. The reading and writing is based on Trinitycore's [ByteBuffer](https://github.com/TrinityCore/TrinityCore/blob/3.3.5/src/server/shared/Packets/ByteBuffer.h).

To understand how this serializer works read documentation related to Protobuf-Net [here](https://www.codeproject.com/articles/642677/protobuf-net-the-unofficial-manual). The design is similar.

## Builds

Available on a Nuget Feed: https://www.myget.org/F/hellokitty/api/v2 [![hellokitty MyGet Build Status](https://www.myget.org/BuildSource/Badge/hellokitty?identifier=a8048ae0-adcd-4997-8862-c3f5fc6adf34)](https://www.myget.org/feed/Packages/hellokitty)

##Tests

#### Linux/Mono - Unit Tests
||Debug x86|Debug x64|Release x86|Release x64|
|:--:|:--:|:--:|:--:|:--:|:--:|
|**master**| N/A | N/A | N/A | [![Build Status](https://travis-ci.org/FreecraftCore/FreecraftCore.Payload.Serializer.svg?branch=master)](https://travis-ci.org/FreecraftCore/FreecraftCore.Payload.Serializer) |
|**dev**| N/A | N/A | N/A | [![Build Status](https://travis-ci.org/FreecraftCore/FreecraftCore.Payload.Serializer.svg?branch=dev)](https://travis-ci.org/FreecraftCore/FreecraftCore.Payload.Serializer)|

#### Windows - Unit Tests

(Done locally)

##Licensing

This project is licensed under the MIT license.
