FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

COPY . .

RUN dotnet restore MakeWish.WishService.sln

RUN dotnet build MakeWish.WishService.sln -c Release --no-restore

RUN dotnet publish src/MakeWish.WishService.Web/MakeWish.WishService.Web.csproj -c Release -o /out --no-build

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

COPY --from=build /out .

ENV ASPNETCORE_URLS=http://+:5002
EXPOSE 5002

ENTRYPOINT ["dotnet", "MakeWish.WishService.Web.dll"]
