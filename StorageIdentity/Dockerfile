FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 56403
EXPOSE 44337

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["StorageIdentity/StorageIdentity.csproj", "StorageIdentity/"]
RUN dotnet restore "StorageIdentity/StorageIdentity.csproj"
COPY . .
WORKDIR "/src/StorageIdentity"
RUN dotnet build "StorageIdentity.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "StorageIdentity.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "StorageIdentity.dll"]