
Создать миграцию
```shell
dotnet ef migrations add AddFriendshipsTable --startup-project ./src/MakeWish.UserService.Web --project ./src/MakeWish.UserService.Adapters.DataAccess.EntityFramework
```

Применить миграции до последней
```shell
 dotnet ef database update --startup-project ./src/MakeWish.UserService.Web --project ./src/MakeWish.UserService.Adapters.DataAccess.EntityFramework
```

Сгенерировать sql-скрипт всех миграций
```shell
dotnet ef migrations script --startup-project ./src/MakeWish.UserService.Web --project ./src/MakeWish.UserService.Adapters.DataAccess.EntityFramework -o ./src/MakeWish.UserService.Adapters.DataAccess.EntityFramework/Migrations/migrations.sql
```