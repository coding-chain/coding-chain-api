FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY . .
RUN dotnet dev-certs https -ep CodingChainApi.WebApi.pfx
RUN dotnet dev-certs https --trust
RUN dotnet restore "CodingChainApi.WebApi/CodingChainApi.WebApi.csproj"
RUN dotnet build "CodingChainApi.WebApi/CodingChainApi.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CodingChainApi.WebApi/CodingChainApi.WebApi.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS final
EXPOSE 443 80
ENV  ASPNETCORE_ENVIRONMENT=Production ASPNETCORE_URLS=https://+:443;http://+:80
USER root
WORKDIR /app
COPY --from=publish app/publish .
COPY --from=build /src/CodingChainApi.WebApi.pfx ${HOME}/.aspnet/https/CodingChainApi.WebApi.pfx
ENTRYPOINT ["dotnet", "CodingChainApi.WebApi.dll"]
