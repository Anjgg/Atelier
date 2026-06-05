FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
 
COPY Api/Atelier.Api/Atelier.Api.csproj Atelier.Api/
RUN dotnet restore Atelier.Api/Atelier.Api.csproj

COPY Api/Atelier.Api/ Atelier.Api/
WORKDIR /src/Atelier.Api

RUN dotnet publish Atelier.Api.csproj -c Release -o /app/publish --no-restore 

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
 
COPY --from=build /app/publish .

USER app
 
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
 
ENTRYPOINT ["dotnet", "Atelier.Api.dll"]