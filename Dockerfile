FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the .NET SDK for building the application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
RUN dotnet restore "./Up-To-Date (UTD)/Up-To-Date (UTD)/Up-To-Date (UTD).csproj"
COPY . .
RUN dotnet publish "./Up-To-Date (UTD)/Up-To-Date (UTD)/Up-To-Date (UTD).csproj" -c Release -o /app/publish

# Configure the final runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Up-To-Date (UTD).dll"]
