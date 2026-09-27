<div align="center">

<img src="projects/frontend/public/favicon.svg" alt="Логотип Симулятора проводников ВСМ" width="128">

#  Симулятор проводников ВСМ

### Интерактивный ситуационный тренажёр для подготовки проводников к нестандартным ситуациям

![React](https://img.shields.io/badge/React-19-61DAFB?style=for-the-badge&logo=react&logoColor=20232A)
![Vite](https://img.shields.io/badge/Vite-8-646CFF?style=for-the-badge&logo=vite&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-4169E1?style=for-the-badge&logo=postgresql&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?style=for-the-badge&logo=docker&logoColor=white)

## Команда

### Product Manager - Дмитриев Антон
### Design - Сысоева Олеся
### FullStack Developer - Сизов Дмитрий

</div>

---

# О проекте

**Симулятор проводников ВСМ** — обучающее приложение, в котором проводник тренируется принимать решения в сложных рабочих ситуациях без риска для пассажиров и коллег.

Пользователь проходит нелинейный сценарий, выбирает варианты действий, работает в условиях ограниченного времени и сразу видит последствия решений. По итогам система формирует результат прохождения, обновляет профиль компетенций и начисляет XP.

Проект разработан в рамках кейса **«Геймификация для ВСМ» Хакатона Московского транспорта**.

---

# Локальный запуск

Проект полностью разворачивается в Docker Compose: frontend, backend и PostgreSQL запускаются одной командой. Для локального запуска нужны только Docker Engine или Docker Desktop и Docker Compose v2 — Node.js и .NET SDK устанавливать не требуется.

## В проекте доступен автоматический запуск

Из корня репозитория выполните:

```bash
./launch.sh
```

Для Windows:

```bat
launch.bat
```

Скрипт автоматически:

- проверит наличие Docker и Docker Compose;
- создаст `.env` со случайными значениями для PostgreSQL и JWT, если файла ещё нет;
- соберёт и запустит все контейнеры;
- дождётся готовности базы данных;
- применит миграции и загрузит демо-данные;
- выведет адреса сервисов и реквизиты подключения к БД.

Существующий `.env` не перезаписывается.

### Ручной запуск через Docker Compose

Если требуется выполнить развёртывание вручную:

#### 1. Подготовить `.env`

```bash
cp .env.example .env
```

Заполните в `.env` значения базы данных и JWT:

```dotenv
DATABASE_NAME=vsm_training
DATABASE_USERNAME=vsm_training
DATABASE_PASSWORD=vsm_training_local

POSTGRES_PORT=5432
BACKEND_PORT=5148
FRONTEND_PORT=5173

JWT_SIGNING_KEY=local-development-signing-key-change-me
JWT_ISSUER=vsm-training
JWT_AUDIENCE=vsm-training
```

#### 2. Собрать и запустить всё окружение

```bash
docker compose up --build -d
```

Compose сам запустит PostgreSQL, backend и frontend, дождётся готовности базы, применит миграции и загрузит демо-сценарий.

Демо-аккаунт:

```text
Email:    demo@vsm.local
Password: demo12345
```

## Возможности MVP

<table>
<tr>
<td width="50%">

### 🎭 Сценарное обучение

- Нелинейные сценарии с ветвлением
- Демонстрационный сценарий «Нетрезвый пассажир»
- Разные исходы: успешный, неуспешный и критический
- Переходы между узлами на основе предыдущих решений

</td>
<td width="50%">

### ⏱️ Работа в условиях времени

- Таймер для критических решений
- Отдельная ветка при истечении времени
- Изменение состояния ситуации после каждого действия
- Безопасная тренировка ошибок и повторное прохождение

</td>
</tr>
<tr>
<td width="50%">

### 📊 Результаты и развитие

- Подробный экран результата
- Профиль компетенций проводника
- Начисление XP и уровни
- Таблица лидеров

</td>
<td width="50%">

### 👤 Пользовательский контур

- Регистрация и вход
- Сессия через защищённую cookie
- Восстановление пароля в development-режиме
- История прогресса и персональная статистика

</td>
</tr>
</table>

# Технологический стек

<div align="center">

![React](https://img.shields.io/badge/React-19-61DAFB?style=flat-square&logo=react&logoColor=20232A)
![Vite](https://img.shields.io/badge/Vite-8-646CFF?style=flat-square&logo=vite&logoColor=white)
![JavaScript](https://img.shields.io/badge/JavaScript-F7DF1E?style=flat-square&logo=javascript&logoColor=black)
![C%23](https://img.shields.io/badge/C%23-239120?style=flat-square&logo=csharp&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![EF Core](https://img.shields.io/badge/EF%20Core-8.0-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-4169E1?style=flat-square&logo=postgresql&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?style=flat-square&logo=docker&logoColor=white)

</div>

| Технология | Назначение |
| --- | --- |
| React 19, Vite 8, JavaScript | Пользовательский интерфейс тренажёра |
| ASP.NET Core 8, C# | HTTP API и авторизация |
| Entity Framework Core 8 | Работа с данными и миграциями |
| PostgreSQL 16 | Хранение пользователей, сценариев, попыток и прогресса |
| Docker Compose | Воспроизводимый локальный запуск всех сервисов |
| JWT в HttpOnly cookie | Аутентификация пользователя |

# Архитектура решения

Проект разделён на три запускаемых компонента:

| Компонент | Путь | Назначение |
| --- | --- | --- |
| **Frontend** | `projects/frontend` | React-приложение, игровые экраны, профиль и leaderboard |
| **Backend API** | `projects/backend/VSMTraining` | API, сценарный runtime, авторизация, результаты и XP |
| **Database** | Docker service `db` | PostgreSQL для доменных данных приложения |

Backend организован по слоям:

- `VSMTraining.Domain` — доменные сущности и перечисления;
- `VSMTraining.Application` — контракты, сценарный runtime и расчёт XP;
- `VSMTraining.Infrastructure` — EF Core, PostgreSQL, миграции и сервисы авторизации;
- `VSMTraining.API` — HTTP endpoints, Swagger и demo seed.

# Как работает сценарий

1. Пользователь регистрируется или входит в систему.
2. Frontend получает список доступных сценариев и профиль компетенций.
3. При старте создаётся попытка прохождения сценария.
4. Backend возвращает текущий узел и доступные варианты действий.
5. Выбор пользователя или истечение таймера переводит попытку в следующий узел.
6. При достижении финального узла backend рассчитывает результат, изменения компетенций и XP.
7. Frontend показывает итоговый экран и обновлённый профиль.

Контент демонстрационного сценария хранится в JSON-файле [`intoxicated_passenger_v1.json`](projects/backend/VSMTraining/VSMTraining.API/Data/intoxicated_passenger_v1.json) и валидируется при запуске backend.

---
