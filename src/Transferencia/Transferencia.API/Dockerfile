FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Transferencia/Transferencia.API/Transferencia.API.csproj", "src/Transferencia/Transferencia.API/"]
COPY ["src/Transferencia/Transferencia.Application/Transferencia.Application.csproj", "src/Transferencia/Transferencia.Application/"]
COPY ["src/Transferencia/Transferencia.Domain/Transferencia.Domain.csproj", "src/Transferencia/Transferencia.Domain/"]
COPY ["src/Transferencia/Transferencia.Infrastructure/Transferencia.Infrastructure.csproj", "src/Transferencia/Transferencia.Infrastructure/"]
COPY ["src/SharedKernel/SharedKernel.csproj", "src/SharedKernel/"]

RUN dotnet restore "src/Transferencia/Transferencia.API/Transferencia.API.csproj"

COPY . .
WORKDIR "/src/src/Transferencia/Transferencia.API"
RUN dotnet publish "Transferencia.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV LANG=C.UTF-8
ENV LC_ALL=C.UTF-8
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Transferencia.API.dll"]