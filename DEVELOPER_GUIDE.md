# DEVELOPER GUIDE

## Архітектура
Проєкт побудовано за принципами Clean Architecture:
- **Domain**: сутності, value objects, контракти.
- **Application**: бізнес-логіка, сервіси, запити.
- **Infrastructure**: репозиторії, persistence, стратегії валідації.
- **Console**: UI та точка входу.

## Структура проєктів
- `src/MedCore.Domain`
- `src/MedCore.Application`
- `src/MedCore.Infrastructure`
- `src/MedCore.Console`
- `tests/MedCore.Tests`

## Ключові контракти
- `IRepository<T, TId>`: CRUD для сутностей.
- `IDataStore<T>`: контракт збереження/завантаження.
- `IValidationStrategy`: правила для записів на прийом.
- `IClock`: ізоляція часу для тестів.
- `IErrorReporter`: канал для фіксації помилок I/O.

## Розширення поведінки
- Додати нове правило: реалізуйте `IValidationStrategy` і підключіть у `CompositeValidationStrategy`.
- Змінити persistence: реалізуйте `IDataStore<MedCoreData>` і передайте в `PersistenceService`.
- Додати новий сценарій: створіть сервіс в `Application` і викликайте його з UI.

## Запуск тестів
```bash
dotnet test
```

## Інтеграційні тести
```bash
dotnet test --filter Category=Integration
```

## Обробка помилок
- Всі помилки I/O відловлюються у `PersistenceService`.
- Деталі можна логувати через `IErrorReporter`.
