FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /Acebook
EXPOSE 5287
EXPOSE 7196
# ENV ConnectionStrings__DefaultConnection="Host=dpg-d6apl1fpm1nc73dgme0g-a.frankfurt-postgres.render.com;Username=paulflieshman;Password=i3C7x0Gl57dfM56mUamCaC8BxezlxKv8;Database=acebook_csharp_development_h4uj"

RUN dotnet tool install --global dotnet-ef

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore 
# Build and publish a release
RUN dotnet publish -o out
RUN dotnet build 
# RUN dotnet run


# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /Acebook
COPY --from=build /Acebook/out .
ENV ASPNETCORE_URLS=http://+:5287
# ENV ASPNETCORE_URLS=https://+:7196
ENV ConnectionStrings__DefaultConnection="Host=dpg-d6apl1fpm1nc73dgme0g-a.frankfurt-postgres.render.com;Username=paulflieshman;Password=i3C7x0Gl57dfM56mUamCaC8BxezlxKv8;Database=acebook_csharp_development_h4uj"
ENTRYPOINT ["dotnet", "acebook.dll"]