dotnet restore
dotnet build -c Debug src/*/project.json

xbuild ./tests/FreecraftCore.Serializer.Strategy.Tests/FreecraftCore.Serializer.Strategy.Tests.csproj /p:DebugSymbols=False
xbuild ./tests/FreecraftCore.Serializer.Tests/FreecraftCore.Serializer.Tests.csproj /p:DebugSymbols=False