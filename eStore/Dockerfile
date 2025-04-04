# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY eStore/eStore.csproj ./eStore/
COPY BusinessObject/BusinessObject.csproj ./BusinessObject/
COPY Core/Core.csproj ./Core/
COPY DataAccess/DataAccess.csproj ./DataAccess/
COPY Services/Services.csproj ./Services/


# Restore
RUN dotnet restore eStore/eStore.csproj

# Copy all source code
COPY . .

# Publish app
RUN dotnet publish eStore/eStore.csproj -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "eStore.dll"]
