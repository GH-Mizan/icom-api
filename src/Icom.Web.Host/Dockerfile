FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ["src/Icom.Web.Host/Icom.Web.Host.csproj", "src/Icom.Web.Host/"]
COPY ["src/Icom.Web.Core/Icom.Web.Core.csproj", "src/Icom.Web.Core/"]
COPY ["src/Icom.Application/Icom.Application.csproj", "src/Icom.Application/"]
COPY ["src/Icom.Core/Icom.Core.csproj", "src/Icom.Core/"]
COPY ["src/Icom.EntityFrameworkCore/Icom.EntityFrameworkCore.csproj", "src/Icom.EntityFrameworkCore/"]
WORKDIR "/src/src/Icom.Web.Host"
RUN dotnet restore 

WORKDIR /src
COPY ["src/Icom.Web.Host", "src/Icom.Web.Host"]
COPY ["src/Icom.Web.Core", "src/Icom.Web.Core"]
COPY ["src/Icom.Application", "src/Icom.Application"]
COPY ["src/Icom.Core", "src/Icom.Core"]
COPY ["src/Icom.EntityFrameworkCore", "src/Icom.EntityFrameworkCore"]
WORKDIR "/src/src/Icom.Web.Host"
RUN dotnet publish -c Release -o /publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
EXPOSE 80
WORKDIR /app
COPY --from=build /publish .
ENTRYPOINT ["dotnet", "Icom.Web.Host.dll"]
