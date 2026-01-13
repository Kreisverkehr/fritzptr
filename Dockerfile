# syntax=docker/dockerfile:1.7

############################
# Build stage
############################
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG TARGETARCH
WORKDIR /src

# Copy solution / project
COPY ./src/fritzptr.slnx ./
COPY ./src/FritzPtr.Api/*.csproj ./FritzPtr.Api/
COPY ./src/FritzPtr.Core/*.csproj ./FritzPtr.Core/
RUN dotnet restore

# Copy rest of source
COPY ./src ./

WORKDIR /src/FritzPtr.Api
RUN dotnet publish FritzPtr.Api.csproj \
    -c Release \
    -o /app/publish \
    -p:UseAppHost=false

############################
# Runtime stage
############################
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

EXPOSE 80
ENV HTTP_PORTS=80

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FritzPtr.Api.dll"]
