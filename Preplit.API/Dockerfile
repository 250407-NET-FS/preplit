# --- Stage 1: Build and Publish ---  

# we need to install the SDK in the container
FROM bitnami/dotnet-sdk:latest AS build 
WORKDIR /src

# copy the csproj files/sln and restore dependencies 

COPY preplit.sln ./
COPY ../Preplit.API/Preplit.API.csproj Preplit.API/
COPY ../Preplit.Domain/Preplit.Domain.csproj Preplit.Domain/
COPY ../Preplit.Services/Preplit.Services.csproj Preplit.Services/
COPY ../Preplit.Data/Preplit.Data.csproj Preplit.Data/
COPY ../Preplit.Tests/Preplit.Tests.csproj Preplit.Tests/
RUN dotnet restore

# copy everything else and then build/publish 
COPY . .
RUN dotnet publish -c Release -o /app/publish 

# --- Stage 2: building a runtime image ---
FROM bitnami/aspnet-core:latest AS runtime 
WORKDIR /app

# copy published output
COPY --from=build /app/publish ./

ENV ASPNETCORE_URLS=http://+:80

# expose a container port 
EXPOSE 80

# entrypoint
ENTRYPOINT ["dotnet", "Preplit.API.dll"]
