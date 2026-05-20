# Testing

## Запуск усіх тестів
```bash
dotnet test
```

## Покриття коду (coverlet)
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

## HTML-звіт
```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:**/coverage.opencover.xml -targetdir:coverage-report
```

## Інтеграційні тести
```bash
dotnet test --filter Category=Integration
```

## Примітки
- Інтеграційні тести використовують тимчасові файли та не потребують ручної підготовки даних.
- Для CI використовується збір покриття у форматі opencover.
