FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Tarifas/Tarifas.Subscriber/Tarifas.Subscriber.csproj", "src/Tarifas/Tarifas.Subscriber/"]
COPY ["src/Tarifas/Tarifas.Application/Tarifas.Application.csproj", "src/Tarifas/Tarifas.Application/"]
COPY ["src/Tarifas/Tarifas.Domain/Tarifas.Domain.csproj", "src/Tarifas/Tarifas.Domain/"]
COPY ["src/Tarifas/Tarifas.Infrastructure/Tarifas.Infrastructure.csproj", "src/Tarifas/Tarifas.Infrastructure/"]
COPY ["src/SharedKernel/SharedKernel.csproj", "src/SharedKernel/"]

RUN dotnet restore "src/Tarifas/Tarifas.Subscriber/Tarifas.Subscriber.csproj"

COPY . .
WORKDIR "/src/src/Tarifas/Tarifas.Subscriber"
RUN dotnet publish "Tarifas.Subscriber.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV LANG=C.UTF-8
ENV LC_ALL=C.UTF-8
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Tarifas.Subscriber.dll"]