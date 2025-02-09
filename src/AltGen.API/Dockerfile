# setup stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS setup-stage
WORKDIR /app
COPY *.csproj ./
RUN dotnet restore
COPY . .

# build stage
FROM setup-stage AS build-stage
RUN dotnet publish -c Release -o dist

# production stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS production-stage
WORKDIR /app
COPY --from=build-stage /app/dist ./
EXPOSE 8080
ENTRYPOINT ["dotnet", "AltGen.API.dll"]
