@echo off
setlocal EnableExtensions EnableDelayedExpansion
chcp 65001 >nul

cd /d "%~dp0"
set "ENV_FILE=%CD%\.env"

where docker >nul 2>&1
if errorlevel 1 (
    echo [ERROR] Docker не найден. Установите Docker Desktop.
    exit /b 1
)

docker compose version >nul 2>&1
if errorlevel 1 (
    echo [ERROR] Docker Compose v2 не найден. Проверьте команду: docker compose version
    exit /b 1
)

where powershell >nul 2>&1
if errorlevel 1 (
    echo [ERROR] PowerShell не найден. Он необходим для генерации секретов.
    exit /b 1
)

if not exist "%ENV_FILE%" (
    for /f "delims=" %%A in ('powershell -NoProfile -Command "$b=New-Object byte[] 32; $r=[Security.Cryptography.RandomNumberGenerator]::Create(); $r.GetBytes($b); [BitConverter]::ToString($b).Replace('-','').ToLowerInvariant()"') do set "DB_PASSWORD=%%A"
    for /f "delims=" %%A in ('powershell -NoProfile -Command "$b=New-Object byte[] 32; $r=[Security.Cryptography.RandomNumberGenerator]::Create(); $r.GetBytes($b); [BitConverter]::ToString($b).Replace('-','').ToLowerInvariant()"') do set "JWT_SIGNING_KEY=%%A"

    (
        echo # Локальная конфигурация, сгенерированная setup-local.bat
        echo DATABASE_NAME=vsm_training
        echo DATABASE_USERNAME=vsm_training
        echo DATABASE_PASSWORD=!DB_PASSWORD!
        echo.
        echo POSTGRES_PORT=5432
        echo BACKEND_PORT=5148
        echo FRONTEND_PORT=5173
        echo.
        echo JWT_SIGNING_KEY=!JWT_SIGNING_KEY!
        echo JWT_ISSUER=vsm-training
        echo JWT_AUDIENCE=vsm-training
    ) > "%ENV_FILE%"

    echo [OK] Создан .env с локальными значениями
) else (
    echo [INFO] Используется существующий .env
)

for /f "usebackq tokens=1,* delims==" %%A in ("%ENV_FILE%") do (
    if "%%A"=="DATABASE_NAME" set "DATABASE_NAME=%%B"
    if "%%A"=="DATABASE_USERNAME" set "DATABASE_USERNAME=%%B"
    if "%%A"=="DATABASE_PASSWORD" set "DATABASE_PASSWORD=%%B"
    if "%%A"=="POSTGRES_PORT" set "POSTGRES_PORT=%%B"
    if "%%A"=="BACKEND_PORT" set "BACKEND_PORT=%%B"
    if "%%A"=="FRONTEND_PORT" set "FRONTEND_PORT=%%B"
    if "%%A"=="JWT_SIGNING_KEY" set "JWT_SIGNING_KEY=%%B"
)

if not defined DATABASE_NAME (
    echo [ERROR] В .env не задано DATABASE_NAME
    exit /b 1
)
if not defined DATABASE_USERNAME (
    echo [ERROR] В .env не задано DATABASE_USERNAME
    exit /b 1
)
if not defined DATABASE_PASSWORD (
    echo [ERROR] В .env не задано DATABASE_PASSWORD
    exit /b 1
)
if not defined JWT_SIGNING_KEY (
    echo [ERROR] В .env не задано JWT_SIGNING_KEY
    exit /b 1
)
if not defined POSTGRES_PORT set "POSTGRES_PORT=5432"
if not defined BACKEND_PORT set "BACKEND_PORT=5148"
if not defined FRONTEND_PORT set "FRONTEND_PORT=5173"

echo [INFO] Запускаю сервисы...
docker compose --env-file "%ENV_FILE%" up --build -d
if errorlevel 1 (
    echo [ERROR] Не удалось запустить Docker Compose.
    exit /b 1
)

echo [INFO] Жду готовности PostgreSQL...
set "DB_READY=false"
for /l %%I in (1,1,30) do (
    set "CONTAINER_ID="
    for /f "delims=" %%A in ('docker compose --env-file "%ENV_FILE%" ps -q db 2^>nul') do set "CONTAINER_ID=%%A"

    if defined CONTAINER_ID (
        set "HEALTH="
        for /f "delims=" %%A in ('docker inspect --format "{{if .State.Health}}{{.State.Health.Status}}{{else}}starting{{end}}" "!CONTAINER_ID!" 2^>nul') do set "HEALTH=%%A"
        if /i "!HEALTH!"=="healthy" (
            set "DB_READY=true"
            goto :db_ready
        )
    )

    <nul set /p "=."
    timeout /t 2 /nobreak >nul
)

:db_ready
echo.
if /i not "!DB_READY!"=="true" (
    echo [ERROR] PostgreSQL не перешёл в состояние healthy.
    echo Проверьте логи командой: docker compose logs db
    exit /b 1
)

echo.
echo ========================================
echo [OK] Локальное окружение запущено
echo ========================================
echo.
echo Frontend:  http://localhost:!FRONTEND_PORT!
echo Swagger:   http://localhost:!BACKEND_PORT!/swagger
echo.
echo Database:
echo   Host:     localhost
echo   Port:     !POSTGRES_PORT!
echo   Database: !DATABASE_NAME!
echo   User:     !DATABASE_USERNAME!
echo   Password: !DATABASE_PASSWORD!
echo.
echo Demo account:
echo   Email:    demo@vsm.local
echo   Password: demo12345
echo.
echo Остановить окружение: docker compose down

endlocal
