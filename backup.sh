#!/bin/bash

# 1. Определяем директорию, где физически лежит скрипт
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ENV_FILE="${SCRIPT_DIR}/.env"

# 2. Подгружаем переменные из .env
if [ -f "$ENV_FILE" ]; then
    set -a
    # shellcheck disable=SC1090
    source "$ENV_FILE"
    set +a
else
    echo "[$(date)] ОШИБКА: Файл ${ENV_FILE} не найден!"
    exit 1
fi

# 3. Настройки путей бэкапа
BACKUP_DIR="${SCRIPT_DIR}/backups"
CONTAINER_NAME="${CONTAINER_DB_NAME:-flowershop-db}"
DB_NAME="${POSTGRES_DB:-flowers_db}"
DB_USER="${POSTGRES_USER:-flower_user}"

DATE=$(date +"%Y-%m-%d_%H-%M-%S")
FILENAME="backup_${DB_NAME}_${DATE}.sql.gz"
FILEPATH="${BACKUP_DIR}/${FILENAME}"

mkdir -p "$BACKUP_DIR"

# 4. Создание дампа базы данных из контейнера
docker exec "$CONTAINER_NAME" pg_dump -U "$DB_USER" "$DB_NAME" | gzip > "$FILEPATH"

# 5. Проверка целостности и отправка через Cloudflare Worker в Telegram
if [ -s "$FILEPATH" ]; then
    FILESIZE=$(du -h "$FILEPATH" | cut -f1)

    RESPONSE=$(curl -s -X POST "${CF_WORKER_URL}/bot${TG_BOT_TOKEN}/sendDocument" \
        -F chat_id="${TG_CHAT_ID}" \
        -F document=@"${FILEPATH}" \
        -F caption="📦 Бэкап БД: ${DB_NAME}%0A📅 Дата: ${DATE}%0A📊 Размер: ${FILESIZE}%0A✅ Создан успешно.")

    if echo "$RESPONSE" | grep -q '"ok":true'; then
        echo "[$(date)] Бэкап успешно отправлен в Telegram."
    else
        echo "[$(date)] Ошибка отправки: $RESPONSE"
    fi

    # Ротация: удаляем локальные дампы старше 7 дней
    find "$BACKUP_DIR" -type f -name "*.sql.gz" -mtime +7 -delete
else
    # Алерт при падении
    curl -s -X POST "${CF_WORKER_URL}/bot${TG_BOT_TOKEN}/sendMessage" \
        -d chat_id="${TG_CHAT_ID}" \
        -d text="🚨 ОШИБКА: Дамп базы ${DB_NAME} пуст или не был создан (${DATE})!" > /dev/null

    echo "[$(date)] Ошибка создания дампа базы."
fi