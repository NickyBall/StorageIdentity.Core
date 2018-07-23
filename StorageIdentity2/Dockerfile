FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 56571
EXPOSE 44310

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["StorageIdentity2/StorageIdentity2.csproj", "StorageIdentity2/"]
RUN dotnet restore "StorageIdentity2/StorageIdentity2.csproj"
COPY . .
WORKDIR "/src/StorageIdentity2"
RUN dotnet build "StorageIdentity2.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "StorageIdentity2.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "StorageIdentity2.dll"]