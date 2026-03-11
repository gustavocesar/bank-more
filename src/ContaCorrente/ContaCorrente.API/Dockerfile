FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/ContaCorrente/ContaCorrente.API/ContaCorrente.API.csproj", "src/ContaCorrente/ContaCorrente.API/"]
COPY ["src/ContaCorrente/ContaCorrente.Application/ContaCorrente.Application.csproj", "src/ContaCorrente/ContaCorrente.Application/"]
COPY ["src/ContaCorrente/ContaCorrente.Domain/ContaCorrente.Domain.csproj", "src/ContaCorrente/ContaCorrente.Domain/"]
COPY ["src/ContaCorrente/ContaCorrente.Infrastructure/ContaCorrente.Infrastructure.csproj", "src/ContaCorrente/ContaCorrente.Infrastructure/"]
COPY ["src/SharedKernel/SharedKernel.csproj", "src/SharedKernel/"]

RUN dotnet restore "src/ContaCorrente/ContaCorrente.API/ContaCorrente.API.csproj"

COPY . .
WORKDIR "/src/src/ContaCorrente/ContaCorrente.API"
RUN dotnet publish "ContaCorrente.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV LANG=C.UTF-8
ENV LC_ALL=C.UTF-8
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ContaCorrente.API.dll"]