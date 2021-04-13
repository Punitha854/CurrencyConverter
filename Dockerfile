#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
WORKDIR /src
COPY ["CurrencyConverter.csproj", "./"]
RUN dotnet restore "CurrencyConverter.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "CurrencyConverter.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CurrencyConverter.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CurrencyConverter.dll"]