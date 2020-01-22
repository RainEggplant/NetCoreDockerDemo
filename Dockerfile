FROM mcr.microsoft.com/dotnet/core/runtime:2.1
COPY NetCoreDockerDemo/bin/Release/netcoreapp2.1/publish/ app/
ENTRYPOINT ["dotnet", "app/NetCoreDockerDemo.dll"]
