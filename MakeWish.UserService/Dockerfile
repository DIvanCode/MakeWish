FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

COPY . .

RUN dotnet restore MakeWish.UserService.sln

RUN dotnet build MakeWish.UserService.sln -c Release --no-restore

RUN dotnet publish src/MakeWish.UserService.Web/MakeWish.UserService.Web.csproj -c Release -o /out --no-build

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

COPY --from=build /out .

ENV ASPNETCORE_URLS=http://+:5001
EXPOSE 5001

ENTRYPOINT ["dotnet", "MakeWish.UserService.Web.dll"]
