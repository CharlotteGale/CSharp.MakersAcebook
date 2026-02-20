# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY Acebook/acebook.csproj ./Acebook/
RUN dotnet restore "Acebook/acebook.csproj"

COPY . ./
RUN dotnet publish "Acebook/acebook.csproj" -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/out ./

CMD ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT:-8080} dotnet acebook.dll"]