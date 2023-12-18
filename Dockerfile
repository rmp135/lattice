FROM node:16 AS node
COPY "Web/client" "/src"
WORKDIR /src
RUN npm i
RUN npm run build

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
COPY . "/src/Lattice"
WORKDIR "/src/Lattice"
RUN dotnet restore "Web/Lattice.Web.csproj"
RUN dotnet build "Web/Lattice.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Web/Lattice.Web.csproj" -c Release -o /app/publish

FROM base AS final
EXPOSE 8888
ENV PORT=8888
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=node /src/wwwroot "wwwroot"
ENTRYPOINT ["dotnet", "Lattice.Web.dll"]
