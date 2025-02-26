# MakeWish
Make Wish — сервис, в котором можно хранить список своих желаний. Каждый может посмотреть и выполнить желания своих друзей.

[Техническое задание](./docs/technical_task.md)

## Архитектура

<img src="./docs/MakeWish.Architecture.png" />

## Общая схема базы данных
Схема представлена в том виде, если бы оба сервиса имели общую реляционную базу данных.

<img src="./docs/MakeWish.Common.DB.Schema.png" />

## UserService

### Схема базы данных
<img src="./docs/MakeWish.UserService.Postgresql.png" />

### Архитектура .NET проекта
<img src="./docs/MakeWish.UserService.png" />

## WishService

### Схема базы данных
<img src="./docs/MakeWish.WishService.Neo4j.png" />

### Архитектура .NET проекта
<img src="./docs/MakeWish.WishService.png" />
