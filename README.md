# TicketBookingSystem
Микросервисная система для управления мероприятиями и бронированиями билетов.

<p align="center">
  <img src="https://github.com/user-attachments/assets/6dcb167b-a0c0-401f-81d1-3ae5b6693381" alt="Архитектура TicketBookingSystem" width="1000">
</p>

## Установка и запуск проекта
### 1. Клонирование репозитория
```bash
git clone https://github.com/fttsr/TicketBookingSystem.git
```

### 2. Настройка переменных окружения
В корне проекта необходимо создать файл `.env`.

Пример содержимого:

```
DB_PASSWORD=your_db_password
DB_USER=your_db_user

EVENT_DB_NAME=event_service_db
BOOKING_DB_NAME=booking_service_db
AUTH_DB_NAME=auth_service_db

EMAIL_FROM=abcd@gmail.com
EMAIL_USERNAME=abcd@gmail.com
EMAIL_PASSWORD=your_app_password
EMAIL_SMTP_HOST=smtp.gmail.com
EMAIL_SMTP_PORT=587

RABBITMQ_USER=guest
RABBITMQ_PASSWORD=guest

JWT_KEY=your_jwt_key
```

### 3. Запуск системы
Из корня проекта:
```bash
docker compose up --build
```

## Стек технологий 
- C#
- .NET 10
- ASP.NET Core
- Entity Framework Core
- PostgreSQL
- RabbitMQ
- Docker
- Docker Compose
   
Полный список используемых библиотек можно посмотреть в соответствующих *.csproj* файлах проектов.

## Архитектура проекта
Проект реализован на микросервисной архитектуре.
Сервисы внутри проекта делятся на слои:
- **API** — обработчик HTTP-запросов.
- **Application** — слой сценариев использования приложения и бизнес-логики.
- **Infrastructure** — работа с внешними системами (база данных, RabbitMQ).
- **Domain** — ядро сервиса. Содержит сущности и бизнес-правила.
  
### AuthService

Отвечает за регистрацию и авторизацию пользователей.

Основные задачи:
- хэширование паролей;
- генерация JWT-токенов;
- хранение пользователей.

### EventService

Отвечает за управление мероприятиями.

Основные задачи:
- создание, изменение и удаление мероприятий;
- получение информации о мероприятиях;
- резервирование билетов на мероприятие;
- возврат билета при отмене брони.

### BookingService

Отвечает за управление бронированиями.

Основные задачи:
- создание бронирований;
- отмена бронирований;
- хранение истории бронирований;
- взаимодействие с EventService по HTTP.
  
### NotificationService

Отвечает за отправку email-уведомлений.

Сервис обрабатывает события:
- регистрации пользователя;
- создания бронирования;
- отмены бронирования.

Сообщения поступают через RabbitMQ.
  
### API Gateway

Представляет единую точку входа в систему и маршрутизует запросы к:
- AuthService;
- EventService;
- BookingService.
