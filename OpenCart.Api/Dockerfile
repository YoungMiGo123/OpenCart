#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["OpenCart.Api/OpenCart.Api.csproj", "OpenCart.Api/"]
COPY ["OpenCart.Services/OpenCart.Services.csproj", "OpenCart.Services/"]
COPY ["OpenCart.Repositories/OpenCart.Repositories.csproj", "OpenCart.Repositories/"]
COPY ["OpenCart.Common/OpenCart.Common.csproj", "OpenCart.Common/"]
COPY ["OpenCart.Infrastructure/OpenCart.Infrastructure.csproj", "OpenCart.Infrastructure/"]
COPY ["OpenCart.Models/OpenCart.Models.csproj", "OpenCart.Models/"]
COPY ["OpenCart.Queries/OpenCart.Operations.csproj", "OpenCart.Queries/"]
RUN dotnet restore "OpenCart.Api/OpenCart.Api.csproj"
COPY . .
WORKDIR "/src/OpenCart.Api"
RUN dotnet build "OpenCart.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OpenCart.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OpenCart.Api.dll"]