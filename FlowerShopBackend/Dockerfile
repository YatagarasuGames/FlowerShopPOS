FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["FlowerShop.csproj", "./"]
RUN dotnet restore "FlowerShop.csproj"

COPY . .
RUN dotnet publish "FlowerShop.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .
RUN mkdir -p /app/wwwroot/uploads && chown -R app:app /app

USER app
EXPOSE 8080

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_UTLS=http://+:8080

ENTRYPOINT ["dotnet", "FlowerShop.dll"]
