dotnet restore
dotnet build FreecraftCore.Serializer.sln

xbuild ./tests/FreecraftCore.Serializer.Strategy.Tests/FreecraftCore.Serializer.Strategy.Tests.csproj /p:DebugSymbols=False
xbuild ./tests/FreecraftCore.Serializer.Tests/FreecraftCore.Serializer.Tests.csproj /p:DebugSymbols=False