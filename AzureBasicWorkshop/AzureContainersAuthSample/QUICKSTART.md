# 🚀 Быстрый старт

## Шаг 1: Настройка Azure AD

1. Откройте [Azure Portal](https://portal.azure.com)
2. **Microsoft Entra ID** → **App registrations** → **New registration**
3. Заполните:
   - Name: `Docker Auth Demo`
   - Redirect URI: `Web` → `http://localhost:5000/signin-oidc`
4. После создания скопируйте:
   - **Application (client) ID**
   - **Directory (tenant) ID**
5. Перейдите в **Certificates & secrets** → **New client secret**
6. Скопируйте значение секрета (показывается один раз!)

## Шаг 2: Конфигурация проекта

Скопируйте `.env.example` в `.env` и заполните:

```env
AZURE_TENANT_ID=ваш-tenant-id-из-шага-1
AZURE_CLIENT_ID=ваш-client-id-из-шага-1
AZURE_CLIENT_SECRET=ваш-client-secret-из-шага-1
```

## Шаг 3: Запуск

### Вариант А: Через PowerShell скрипт (рекомендуется)

```powershell
.\start-docker.ps1
```

Скрипт автоматически:
- Проверит Docker
- Соберет образ
- Запустит контейнер
- Откроет браузер

### Вариант Б: Вручную через Docker Compose

```powershell
docker-compose up -d
```

Приложение: http://localhost:5000

### Вариант В: Через Docker CLI

```powershell
cd WebApp
docker build -t azure-auth-demo .
docker run -d -p 5000:8080 `
  --env-file ../.env `
  --name azure-auth-demo `
  azure-auth-demo
```

## Шаг 4: Проверка

1. Откройте http://localhost:5000
2. Нажмите **"Войти через Entra ID"**
3. Выполните вход с учетной записью Microsoft
4. Перейдите в **"Защищенная зона"** - увидите claims пользователя
5. Откройте **"Системная информация"** - проверьте конфигурацию

## 🔧 Полезные команды

```powershell
# Просмотр логов
.\view-logs.ps1
# или
docker-compose logs -f

# Остановка
.\stop-docker.ps1
# или
docker-compose down

# Перезапуск
docker-compose restart

# Пересборка после изменений кода
docker-compose up -d --build

# Проверка статуса
docker ps

# Вход в контейнер (для отладки)
docker exec -it azure-containers-auth-webapp /bin/bash
```

## ❓ Troubleshooting

### Ошибка: "Reply URL does not match"

**Решение**: Добавьте `http://localhost:5000/signin-oidc` в Redirect URIs в Azure Portal

### Контейнер не стартует

```powershell
# Проверьте логи
docker-compose logs

# Проверьте переменные окружения
docker exec azure-containers-auth-webapp printenv | Select-String Azure
```

### Docker не запускается

Убедитесь, что Docker Desktop запущен и работает корректно:
```powershell
docker info
```

### Порт 5000 занят

Измените порт в `docker-compose.yml`:
```yaml
ports:
  - "5001:8080"  # Используйте другой порт
```

И обновите Redirect URI в Azure AD на `http://localhost:5001/signin-oidc`

## 📖 Дополнительная информация

См. полную документацию в [README.md](README.md)
