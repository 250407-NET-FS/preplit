# --- Stage 1: Build and Publish ---   

# we need to install the SDK in the container
FROM mcr.Microsoft.com/dotnet/sdk:9.0 AS build 
WORKDIR /src 

# copy the csproj files/sln and restore dependencies 

COPY preplit.sln ./
COPY ../Preplit.API/Preplit.API.csproj Preplit.API/
COPY ../Preplit.Domain/Preplit.Domain.csproj Preplit.Domain/
COPY ../Preplit.Service/Preplit.Service.csproj Preplit.Service/
COPY ../Preplit.Data/Preplit.Data.csproj Preplit.Data/
COPY ../Preplit.Tests/Preplit.Tests.csproj Preplit.Tests/
RUN dotnet restore

# copy everything else and then build/publish 
COPY . .
RUN dotnet publish Preplit.API/Preplit.API.csproj -c Release -o /app/publish 

# --- Stage 2: building a runtime image ---
FROM mcr.Microsoft.com/dotnet/aspnet:9.0 AS runtime 
WORKDIR /app

# copy published output
COPY --from=build /app/publish ./

ENV ASPNETCORE_URLS=http://+:80

# expose a container port 
EXPOSE 80

# entrypoint
ENTRYPOINT ["dotnet", "Preplit.API.dll"]