#!/usr/bin/env bash

set -euo pipefail

SCRIPT_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="${SCRIPT_DIR}"
ENV_FILE="${PROJECT_ROOT}/.env"

cd "${PROJECT_ROOT}"

die() {
    printf '❌ %s\n' "$1" >&2
    exit 1
}

generate_secret() {
    if command -v openssl >/dev/null 2>&1; then
        openssl rand -hex 32
    else
        od -An -N32 -tx1 /dev/urandom | tr -d ' \n'
    fi
}

get_env_value() {
    local key="$1"

    awk -F= -v key="${key}" '
        {
            line = $0
            sub(/^[[:space:]]+/, "", line)
            sub(/^export[[:space:]]+/, "", line)
            split(line, parts, "=")
            if (parts[1] == key) {
                value = substr(line, index(line, "=") + 1)
                sub(/^[[:space:]]+/, "", value)
                sub(/[[:space:]]+$/, "", value)
            }
        }
        END { print value }
    ' "${ENV_FILE}"
}

ensure_env_value() {
    local key="$1"
    local value="$2"

    [[ -n "$(get_env_value "${key}")" ]] && return

    local temp_file
    temp_file="$(mktemp "${ENV_FILE}.tmp.XXXXXX")"
    awk -v key="${key}" -v value="${value}" '
        BEGIN { replaced = 0 }
        {
            line = $0
            normalized = line
            sub(/^[[:space:]]+/, "", normalized)
            sub(/^export[[:space:]]+/, "", normalized)
            split(normalized, parts, "=")

            if (parts[1] == key) {
                if (!replaced) {
                    print key "=" value
                    replaced = 1
                }
                next
            }

            print line
        }
        END {
            if (!replaced) print key "=" value
        }
    ' "${ENV_FILE}" > "${temp_file}"
    mv "${temp_file}" "${ENV_FILE}"
    chmod 600 "${ENV_FILE}"
    env_was_completed=true
}

command -v docker >/dev/null 2>&1 || die "Docker не найден. Установите Docker Desktop или Docker Engine."
docker compose version >/dev/null 2>&1 || die "Docker Compose v2 не найден. Проверьте команду: docker compose version"

if [[ ! -f "${ENV_FILE}" ]]; then
    db_password="$(generate_secret)"
    jwt_signing_key="$(generate_secret)"

    umask 077
    {
        printf '# Локальная конфигурация, сгенерированная launch.sh\n'
        printf 'DATABASE_NAME=vsm_training\n'
        printf 'DATABASE_USERNAME=vsm_training\n'
        printf 'DATABASE_PASSWORD=%s\n' "${db_password}"
        printf '\n'
        printf 'POSTGRES_PORT=5432\n'
        printf 'BACKEND_PORT=5148\n'
        printf 'FRONTEND_PORT=5173\n'
        printf '\n'
        printf 'JWT_SIGNING_KEY=%s\n' "${jwt_signing_key}"
        printf 'JWT_ISSUER=vsm-training\n'
        printf 'JWT_AUDIENCE=vsm-training\n'
    } > "${ENV_FILE}"

    printf '✅ Создан .env с локальными значениями\n'
else
    printf 'ℹ️  Используется существующий .env\n'
fi

env_was_completed=false
umask 077

# Если .env уже существует, не перезаписываем заданные значения, а только
# автоматически добавляем отсутствующие или заполняем пустые поля.
ensure_env_value DATABASE_NAME "vsm_training"
ensure_env_value DATABASE_USERNAME "vsm_training"
ensure_env_value DATABASE_PASSWORD "$(generate_secret)"
ensure_env_value POSTGRES_PORT "5432"
ensure_env_value BACKEND_PORT "5148"
ensure_env_value FRONTEND_PORT "5173"
ensure_env_value JWT_SIGNING_KEY "$(generate_secret)"
ensure_env_value JWT_ISSUER "vsm-training"
ensure_env_value JWT_AUDIENCE "vsm-training"

if [[ "${env_was_completed}" == true ]]; then
    printf '✅ .env автоматически дополнен недостающими значениями\n'
fi

printf '🚀 Запускаю сервисы...\n'
docker compose --env-file "${ENV_FILE}" up --build -d

printf '⏳ Жду готовности PostgreSQL'
db_ready=false
for _ in {1..30}; do
    container_id="$(docker compose --env-file "${ENV_FILE}" ps -q db 2>/dev/null || true)"
    if [[ -n "${container_id}" ]]; then
        health="$(docker inspect --format '{{if .State.Health}}{{.State.Health.Status}}{{else}}starting{{end}}' "${container_id}" 2>/dev/null || true)"
        if [[ "${health}" == "healthy" ]]; then
            db_ready=true
            break
        fi
    fi

    printf '.'
    sleep 2
done
printf '\n'

[[ "${db_ready}" == true ]] || die "PostgreSQL не перешёл в состояние healthy. Проверьте: docker compose logs db"

db_host="localhost"
db_port="$(get_env_value POSTGRES_PORT)"
db_name="$(get_env_value DATABASE_NAME)"
db_user="$(get_env_value DATABASE_USERNAME)"
db_password="$(get_env_value DATABASE_PASSWORD)"
backend_port="$(get_env_value BACKEND_PORT)"
frontend_port="$(get_env_value FRONTEND_PORT)"

printf '\n'
bold=$'\033[1m'
cyan=$'\033[36m'
green=$'\033[32m'
yellow=$'\033[33m'
dim=$'\033[2m'
reset=$'\033[0m'

# Внутренняя ширина рассчитана на стандартный терминал в 80 колонок:
# 1 символ рамки + пробел + 76 символов содержимого + пробел + 1 символ рамки.
box_content_width=76
box_inner_width=$((box_content_width + 2))
printf -v box_rule '%*s' "${box_inner_width}" ''
box_rule="${box_rule// /─}"

box_line() {
    local content="$1"
    local color="${2:-}"
    local padded

    printf -v padded '%-*s' "${box_content_width}" "${content}"
    printf '%s│%s %s%s%s %s│%s\n' \
        "${cyan}" "${reset}" "${color}" "${padded}" "${reset}" "${cyan}" "${reset}"
}

printf '%s╭%s╮%s\n' "${cyan}" "${box_rule}" "${reset}"
box_line '✓  ЛОКАЛЬНОЕ ОКРУЖЕНИЕ ЗАПУЩЕНО' "${bold}${green}"
printf '%s├%s┤%s\n' "${cyan}" "${box_rule}" "${reset}"
box_line '  ПРИЛОЖЕНИЕ' "${bold}"
box_line "    Frontend   http://localhost:${frontend_port:-5173}" "${green}"
box_line "    Swagger    http://localhost:${backend_port:-5148}/swagger" "${green}"
box_line ''
box_line '  БАЗА ДАННЫХ' "${bold}"
box_line "    Host       ${db_host}" "${yellow}"
box_line "    Port       ${db_port:-5432}" "${yellow}"
box_line "    Database   ${db_name}" "${yellow}"
box_line "    User       ${db_user}" "${yellow}"
box_line "    Password   ${db_password:0:56}" "${yellow}"
remaining_password="${db_password:56}"
while [[ -n "${remaining_password}" ]]; do
    box_line "                ${remaining_password:0:56}" "${yellow}"
    remaining_password="${remaining_password:56}"
done
box_line ''
box_line '  ДЕМО-АККАУНТ' "${bold}"
box_line '    Email      demo@vsm.local' "${yellow}"
box_line '    Password   demo12345' "${yellow}"
printf '%s├%s┤%s\n' "${cyan}" "${box_rule}" "${reset}"
box_line '  Остановить окружение: docker compose down' "${dim}${bold}"
printf '%s╰%s╯%s\n' "${cyan}" "${box_rule}" "${reset}"
