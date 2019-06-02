FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 23456 

FROM microsoft/dotnet:2.2-sdk-nanoserver-1803 AS build
WORKDIR /src
COPY ["WMSCore/WMSCore.csproj", "WMSCore/"]
RUN dotnet restore "WMSCore/WMSCore.csproj"
COPY . .
WORKDIR "/src/WMSCore"
RUN dotnet build "WMSCore.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "WMSCore.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

ENTRYPOINT ["dotnet", "WMSCore.dll"]

