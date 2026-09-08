# HallRental
Hall Rental Api

Коротка документація проєкту HallRental (API бронювання та оренди конференц‑залів)

1) Опис бізнес‑задачі
Компанія надає в оренду конференц‑зали для бізнесу. Потрібно реалізувати Web API, яке дозволяє:

Керувати залами (halls)
створення залу (назва, місткість, базова ціна за годину, доступні послуги)
редагування залу
видалення (soft delete)
Шукати доступні зали за заданим інтервалом часу та мінімальною місткістю.
Бронювати зал з можливістю вибрати додаткові послуги та отримати розрахунок загальної вартості.
Отримувати бізнес‑звіти/аналітику, корисну для оцінки доходу та завантаженості.
Початкові дані з ТЗ:

Зали:
Зал A: 50 осіб, 2000 грн/год
Зал B: 100 осіб, 3500 грн/год
Зал C: 30 осіб, 1500 грн/год

Послуги:
Проєктор 500 грн
Wi‑Fi 300 грн
Звук 700 грн

2) Технічні рішення та архітектура
Проєкт реалізовано на .NET 8 (ASP.NET Core Web API) з використанням MS SQL Server (LocalDB) та EF Core 8.

2.1. 3‑шарова структура
Рішення складається з трьох проєктів:

HallRental.Api (Presentation Layer):
REST Controllers
Swagger/OpenAPI
FluentValidation (валідація запитів)
ExceptionHandlingMiddleware (узгоджені HTTP‑помилки)

HallRental.BL (Business Logic Layer):
Бізнес‑сервіси: HallService, BookingManager, ReportsService, PricingService
Мапінг через Riok.Mapperly (Entities → DTO)
Правила доступності залів, цінова політика, створення бронювання

HallRental.DAL (Data Access Layer):
EF Core DbContext та Entities
Repository pattern (IBaseRepository<T>, спеціалізовані репозиторії)
Міграції, seed початкових даних
Soft delete для залів

3) Основні бізнес‑правила
3.1. Обмеження часу бронювання
Час розглядається в UTC.
Бронювання може бути на кілька днів.
Бронювання дозволене лише в проміжку 06:00–23:00 (UTC) для кожного дня.
Заборонено включати нічний інтервал 23:00–06:00 (тобто бронювання не може “перестрибувати” ніч).

3.2. Перевірка доступності залу
Зал доступний, якщо немає бронювання з перетином інтервалів:

existing.StartUtc < request.EndUtc AND request.StartUtc < existing.EndUtc

3.3. Додаткові послуги
SelectedServiceIds можуть бути порожні.

Послуги додаються до вартості 1 раз за бронювання.

Валідується:
послуги існують
послуги дозволені для конкретного залу (HallServices)

3.4. Розрахунок вартості (Pricing)
Базова ставка залу — BaseHourlyRate (грн/год).

Модифікатори за часом:
06:00–09:00: -10%
09:00–18:00: 0%
18:00–23:00: -20%
12:00–14:00: +15% (пікові години)

Алгоритм: бронювання розбивається на сегменти по тарифних межах (і по днях), сума рахується як:

Σ (hours_in_segment * baseHourlyRate * multiplier) + Σ servicesPrice

4) Модель даних (EF Core / MSSQL)
4.1. Основні таблиці
Halls — зали (soft delete через IsDeleted)
Services — додаткові послуги
HallServices — many‑to‑many (які послуги доступні в залі)
Bookings — бронювання (зберігає StartUtc, EndUtc, TotalPrice)
BookingServices — обрані послуги в бронюванні (знімок ціни PriceAtBooking)

4.2. Soft delete
Для залів застосовано HasQueryFilter(x => !x.IsDeleted)

При Update зал активується (якщо був soft-deleted) через IgnoreQueryFilters() та IsDeleted=false.

5) API (основні ендпоінти)
5.1. Halls
POST /halls — створити зал (повертає hallId)
PUT /halls/{id} — оновити зал (та активувати, якщо був soft-deleted)
DELETE /halls/{id} — soft delete
GET /halls/available?startUtc=...&endUtc=...&capacity=... — пошук доступних

GET /halls/{id} — отримати зал по id (допоміжний)

5.2. Bookings
POST /bookings
Вхід: hallId, startUtc, durationMinutes, selectedServiceIds[]
Вихід: підтвердження бронювання + розрахунок вартості (включає HallPrice, ServicesPrice, TotalPrice)

5.3. Services
GET /services — список послуг

6) Звіти та аналітика (додано згідно ТЗ)
Реалізовані звіти за період fromUtc – toUtc:
GET /reports/revenue — дохід (totalRevenue, bookingsCount, avg)
GET /reports/hall-occupancy — завантаженість залів (bookedHours, bookingsCount по кожному залу)
GET /reports/top-services — топ послуг (timesSelected, revenueFromService)

Звіти реалізовані так, щоб агрегації виконувались на SQL Server (де можливо), а мапінг в DTO виконувався після отримання агрегованих даних.

7) Обробка помилок та HTTP статуси
У HallRental.Api реалізовано ExceptionHandlingMiddleware, який повертає ProblemDetails з кодами:
400 — некоректні дані (ArgumentException, FluentValidation.ValidationException)
404 — сутність не знайдена (KeyNotFoundException)
409 — конфлікт бізнес‑правил (наприклад, перетин бронювань) (InvalidOperationException)
500 — неочікувана помилка

8) Початкові дані
При виконанні першої міграції виконується:
створення залів A/B/C
створення послуг Проєктор/Wi‑Fi/Звук

зв’язки HallServices
ID в початкових даних зроблено стабільними (fixed GUID) для зручного тестування.

9) Запуск проєкту (локально)
Переконатися, що доступний SQL Server LocalDB
Встановити connection string в appsettings.json:
Server=(localdb)\MSSQLLocalDB;Database=HallRentalDb;Trusted_Connection=True;...

Застосувати міграції:
dotnet ef migrations add InitialCreate -p HallRental.DAL -s HallRental.Api
dotnet ef database update -p HallRental.DAL -s HallRental.Api

Запустити:
dotnet run --project HallRental.Api
Відкрити Swagger: /swagger

