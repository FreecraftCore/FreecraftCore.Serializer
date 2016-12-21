dotnet restore
dotnet build -c Debug src/*/project.json

xbuild ./src/FreecraftCore.Serializer.Strategy.Tests/FreecraftCore.Serializer.Strategy.Tests.csproj /p:DebugSymbols=False
xbuild ./src/FreecraftCore.Serializer.Tests/FreecraftCore.Serializer.Tests.csproj /p:DebugSymbols=False