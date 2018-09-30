FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 36930
EXPOSE 44333

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["nexus-web-uploader-api/nexus-web-uploader-api.csproj", "nexus-web-uploader-api/"]
RUN dotnet restore "nexus-web-uploader-api/nexus-web-uploader-api.csproj"
COPY . .
WORKDIR "/src/nexus-web-uploader-api"
RUN dotnet build "nexus-web-uploader-api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "nexus-web-uploader-api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "nexus-web-uploader-api.dll"]