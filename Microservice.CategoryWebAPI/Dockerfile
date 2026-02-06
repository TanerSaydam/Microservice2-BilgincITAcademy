FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
#ssl siz
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Microservice.CategoryWebAPI/Microservice.CategoryWebAPI.csproj", "Microservice.CategoryWebAPI/"]
RUN dotnet restore "Microservice.CategoryWebAPI/Microservice.CategoryWebAPI.csproj"
COPY . .
WORKDIR "/src/Microservice.CategoryWebAPI"
RUN dotnet build "Microservice.CategoryWebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Microservice.CategoryWebAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet","Microservice.CategoryWebAPI.dll"]