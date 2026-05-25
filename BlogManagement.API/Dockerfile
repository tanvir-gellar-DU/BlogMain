FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY BlogManagement.API.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish BlogManagement.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 10000

ENTRYPOINT ["sh", "-c", "dotnet BlogManagement.API.dll --urls http://0.0.0.0:${PORT:-10000}"]