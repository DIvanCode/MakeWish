
Создать миграцию
```shell
dotnet ef migrations add AddFriendshipsTable --startup-project ./src/MakeWish.UserService.Web --project ./src/MakeWish.UserService.Adapters.DataAccess.EntityFramework
```

Применить миграции до последней
```shell
 dotnet ef database update --startup-project ./src/MakeWish.UserService.Web --project ./src/MakeWish.UserService.Adapters.DataAccess.EntityFramework
```