FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
ENV ASPNETCORE_URLS http://*:443
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build

WORKDIR "/src"
COPY . .

## install node
RUN apt-get install curl \
    && curl -sL https://deb.nodesource.com/setup_14.x | bash \
    && apt-get install nodejs 

WORKDIR "/src/SpotSync"
RUN ls
run pwd
RUN npm install 
RUN npm run build

FROM build AS publish
RUN dotnet publish "SpotSync.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SpotSync.dll"]