# MedCore

Міні-проєкт для керування базовими процесами в лікарні: реєстрація пацієнтів, призначення лікарів до відділень, записи на прийом, пошук і аналітика.

## Основні можливості
- Асинхронне збереження/завантаження стану у JSON (data/medcore.json).
- Мінімум 3 завершені use cases з бізнес-правилами.
- Strategy + Composite для розширюваної валідації записів.
- LINQ-запити, фільтрація, сортування та агрегована статистика.

## Запуск
```bash
dotnet run --project src/MedCore.Console
```

## Тести
```bash
dotnet test
```

## Структура рішення
- src/MedCore.Domain — доменні сутності та контракти.
- src/MedCore.Application — бізнес-логіка, сервіси, запити.
- src/MedCore.Infrastructure — репозиторії, стратегії, JSON-сховище.
- src/MedCore.Console — консольний інтерфейс.
- tests/MedCore.Tests — юніт-тести.

## Дані
Файл збереження: data/medcore.json. При старті програми виконується спроба завантажити дані; збереження доступне через меню.

## Документація
- [USER_GUIDE.md](USER_GUIDE.md)
- [DEVELOPER_GUIDE.md](DEVELOPER_GUIDE.md)
- [TESTING.md](TESTING.md)
- [CHANGELOG.md](CHANGELOG.md)
- [DEMO.md](DEMO.md)
- [FINAL_REPORT.md](FINAL_REPORT.md)
- [docs/release-plan.md](docs/release-plan.md)
- [docs/syllabus-coverage.md](docs/syllabus-coverage.md)
- [docs/presentation.md](docs/presentation.md)
- [docs/defense-qa.md](docs/defense-qa.md)
- [docs/test-strategy.md](docs/test-strategy.md)
- [docs/test-matrix.md](docs/test-matrix.md)
