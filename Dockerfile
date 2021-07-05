FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "CodingChainApi.WebApi/CodingChainApi.WebApi.csproj"
RUN dotnet build "CodingChainApi.WebApi/CodingChainApi.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CodingChainApi.WebApi/CodingChainApi.WebApi.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS final
EXPOSE 443 80
ENV  ASPNETCORE_ENVIRONMENT=Production ASPNETCORE_URLS=http://+:443;http://+:80 APPLICATION_USER=app_user APPLICATION_GROUP=app_users 
RUN groupadd -g 1010 $APPLICATION_GROUP \
    && useradd --create-home -g $APPLICATION_GROUP $APPLICATION_USER 
USER $APPLICATION_USER
WORKDIR /home/$APPLICATION_USER
COPY --from=publish app/publish .
ENTRYPOINT ["dotnet", "CodingChainApi.WebApi.dll"]
