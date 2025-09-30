FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["MedicalAppointment.Api.csproj", "."]
RUN dotnet restore "./MedicalAppointment.Api.csproj"

COPY . .
RUN dotnet build "./MedicalAppointment.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MedicalAppointment.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final
WORKDIR /app

RUN dotnet tool install --global dotnet-ef --version 8.0.10
ENV PATH="${PATH}:/root/.dotnet/tools"

COPY --from=publish /app/publish .

COPY --from=build /src /src

WORKDIR /src

ENTRYPOINT ["dotnet", "/app/MedicalAppointment.Api.dll"]