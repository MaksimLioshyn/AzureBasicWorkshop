# Docker Commands Cheat Sheet

Коллекция полезных Docker команд для работы с проектом Azure Containers & Authentication Demo.

## 🚀 Основные команды

### Запуск через Docker Compose

```powershell
# Запуск контейнера в фоновом режиме
docker-compose up -d

# Запуск с пересборкой образа
docker-compose up -d --build

# Запуск с просмотром логов
docker-compose up

# Остановка и удаление контейнеров
docker-compose down

# Остановка без удаления
docker-compose stop

# Запуск остановленного контейнера
docker-compose start

# Перезапуск контейнера
docker-compose restart
```

### Ручная сборка и запуск

```powershell
# Переход в папку с Dockerfile
cd WebApp

# Сборка образа
docker build -t azure-auth-demo:latest .

# Запуск контейнера
docker run -d `
  -p 5000:8080 `
  -e AzureAd__TenantId="your-tenant-id" `
  -e AzureAd__ClientId="your-client-id" `
  -e AzureAd__ClientSecret="your-secret" `
  --name azure-auth-demo `
  azure-auth-demo:latest

# Запуск с .env файлом
docker run -d `
  -p 5000:8080 `
  --env-file ../.env `
  --name azure-auth-demo `
  azure-auth-demo:latest
```

## 📊 Мониторинг и диагностика

### Просмотр информации

```powershell
# Список запущенных контейнеров
docker ps

# Список всех контейнеров (включая остановленные)
docker ps -a

# Информация о контейнере
docker inspect azure-containers-auth-webapp

# Подробная информация в JSON
docker inspect azure-containers-auth-webapp | ConvertFrom-Json

# Статистика использования ресурсов
docker stats azure-containers-auth-webapp

# Статистика всех контейнеров
docker stats

# Информация об образе
docker images azure-auth-demo

# История создания образа
docker history azure-auth-demo
```

### Логи

```powershell
# Просмотр логов (последние 100 строк)
docker logs azure-containers-auth-webapp

# Просмотр логов в реальном времени
docker logs -f azure-containers-auth-webapp

# Последние 50 строк с временными метками
docker logs --tail 50 -t azure-containers-auth-webapp

# Логи за последние 10 минут
docker logs --since 10m azure-containers-auth-webapp

# Логи через docker-compose
docker-compose logs -f

# Последние 100 строк
docker-compose logs --tail=100
```

### Health Check

```powershell
# Проверка health status
docker inspect --format='{{json .State.Health}}' azure-containers-auth-webapp | ConvertFrom-Json

# Только статус
docker inspect --format='{{.State.Health.Status}}' azure-containers-auth-webapp

# Тест endpoint вручную
curl http://localhost:5000/health

# С подробностями
Invoke-WebRequest -Uri http://localhost:5000/health -UseBasicParsing
```

## 🔧 Отладка и troubleshooting

### Вход в контейнер

```powershell
# Вход в bash
docker exec -it azure-containers-auth-webapp /bin/bash

# Выполнение одной команды
docker exec azure-containers-auth-webapp ls -la /app

# Проверка переменных окружения
docker exec azure-containers-auth-webapp printenv

# Фильтрация переменных Azure
docker exec azure-containers-auth-webapp printenv | Select-String Azure

# Проверка процессов внутри контейнера
docker exec azure-containers-auth-webapp ps aux

# Проверка использования диска
docker exec azure-containers-auth-webapp df -h
```

### Копирование файлов

```powershell
# Копирование файла из контейнера
docker cp azure-containers-auth-webapp:/app/appsettings.json ./appsettings-from-container.json

# Копирование файла в контейнер (осторожно в production!)
docker cp custom-config.json azure-containers-auth-webapp:/app/custom-config.json

# Копирование папки
docker cp azure-containers-auth-webapp:/app/wwwroot ./wwwroot-backup
```

### Сетевая диагностика

```powershell
# Проверка сетевых настроек
docker network ls

# Информация о сети
docker network inspect bridge

# Проверка портов
docker port azure-containers-auth-webapp

# Проверка подключения изнутри контейнера
docker exec azure-containers-auth-webapp curl http://localhost:8080/health
```

## 🧹 Очистка

### Остановка и удаление

```powershell
# Остановка контейнера
docker stop azure-containers-auth-webapp

# Удаление остановленного контейнера
docker rm azure-containers-auth-webapp

# Остановка и удаление в одной команде
docker rm -f azure-containers-auth-webapp

# Остановка всех контейнеров
docker stop $(docker ps -q)

# Удаление всех остановленных контейнеров
docker container prune
```

### Удаление образов

```powershell
# Удаление образа
docker rmi azure-auth-demo

# Удаление с принудительным удалением
docker rmi -f azure-auth-demo

# Удаление неиспользуемых образов
docker image prune

# Удаление всех неиспользуемых образов
docker image prune -a
```

### Полная очистка

```powershell
# Удаление неиспользуемых контейнеров, сетей, образов и build cache
docker system prune

# С удалением всех неиспользуемых образов
docker system prune -a

# С удалением volumes
docker system prune -a --volumes

# Проверка использования диска
docker system df
```

## 📦 Работа с образами

### Сборка

```powershell
# Обычная сборка
docker build -t azure-auth-demo:latest ./WebApp

# С тегом версии
docker build -t azure-auth-demo:1.0 ./WebApp

# Без использования кэша
docker build --no-cache -t azure-auth-demo:latest ./WebApp

# С build arguments
docker build --build-arg ASPNETCORE_ENVIRONMENT=Production -t azure-auth-demo:latest ./WebApp

# Просмотр процесса сборки
docker build --progress=plain -t azure-auth-demo:latest ./WebApp
```

### Теги и push

```powershell
# Добавление тега
docker tag azure-auth-demo:latest myregistry.azurecr.io/azure-auth-demo:v1

# Push в Azure Container Registry
az acr login --name myregistry
docker push myregistry.azurecr.io/azure-auth-demo:v1

# Pull образа
docker pull myregistry.azurecr.io/azure-auth-demo:v1

# Экспорт образа в файл
docker save azure-auth-demo:latest -o azure-auth-demo.tar

# Импорт образа из файла
docker load -i azure-auth-demo.tar
```

## 🔄 Docker Compose специфичные команды

```powershell
# Просмотр конфигурации
docker-compose config

# Проверка синтаксиса
docker-compose config --quiet

# Список сервисов
docker-compose ps

# Запуск конкретного сервиса
docker-compose up webapp

# Масштабирование (несколько инстансов)
docker-compose up -d --scale webapp=3

# Пересборка сервиса
docker-compose build webapp

# Просмотр событий
docker-compose events

# Выполнение команды в сервисе
docker-compose exec webapp /bin/bash
```

## 🎯 Полезные скрипты

### Автоматическая очистка старых контейнеров

```powershell
# Удаление контейнеров старше 24 часов
docker ps -a --filter "status=exited" --format "{{.ID}}\t{{.CreatedAt}}" | 
	ForEach-Object {
		$id, $created = $_ -split '\t'
		if ((Get-Date) - [DateTime]::Parse($created) -gt [TimeSpan]::FromHours(24)) {
			docker rm $id
		}
	}
```

### Проверка работоспособности

```powershell
# Скрипт для проверки статуса
$containerName = "azure-containers-auth-webapp"
$status = docker inspect --format='{{.State.Status}}' $containerName 2>$null

if ($status -eq "running") {
	Write-Host "✓ Контейнер работает" -ForegroundColor Green

	$health = docker inspect --format='{{.State.Health.Status}}' $containerName 2>$null
	if ($health -eq "healthy") {
		Write-Host "✓ Health check: OK" -ForegroundColor Green
	} else {
		Write-Host "⚠ Health check: $health" -ForegroundColor Yellow
	}
} else {
	Write-Host "✗ Контейнер не работает: $status" -ForegroundColor Red
}
```

### Backup конфигурации

```powershell
# Backup docker-compose конфигурации
$timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
Copy-Item docker-compose.yml "docker-compose.backup.$timestamp.yml"
Copy-Item .env ".env.backup.$timestamp" -ErrorAction SilentlyContinue

Write-Host "Backup создан: docker-compose.backup.$timestamp.yml"
```

## 📝 Примеры для демонстрации

### Демо 1: Первый запуск

```powershell
# 1. Сборка
cd WebApp
docker build -t azure-auth-demo .

# 2. Проверка образа
docker images azure-auth-demo

# 3. Запуск
docker run -d -p 5000:8080 --name demo azure-auth-demo

# 4. Проверка
docker ps
curl http://localhost:5000/health

# 5. Просмотр логов
docker logs demo

# 6. Очистка
docker stop demo
docker rm demo
```

### Демо 2: С переменными окружения

```powershell
# Запуск с настройками Azure AD
docker run -d -p 5000:8080 `
  -e AzureAd__TenantId="common" `
  -e AzureAd__ClientId="your-client-id" `
  -e AzureAd__ClientSecret="your-secret" `
  --name demo `
  azure-auth-demo

# Проверка переменных
docker exec demo printenv | Select-String AzureAd

# Тест в браузере
Start-Process http://localhost:5000
```

### Демо 3: Docker Compose workflow

```powershell
# Полный цикл разработки
docker-compose up -d --build   # Сборка и запуск
docker-compose logs -f         # Просмотр логов
# ... тестирование ...
docker-compose restart         # Перезапуск
docker-compose down           # Остановка
```

## 🐛 Troubleshooting команды

```powershell
# Контейнер не запускается
docker logs azure-containers-auth-webapp
docker inspect azure-containers-auth-webapp

# Проблемы с сетью
docker network inspect bridge
docker port azure-containers-auth-webapp

# Проблемы с портами
netstat -ano | findstr :5000
# Или в PowerShell
Get-NetTCPConnection -LocalPort 5000

# Проверка образа
docker history azure-auth-demo
docker inspect azure-auth-demo

# Проверка Docker daemon
docker info
docker version

# Очистка всего при зависании
docker-compose down -v
docker system prune -a -f --volumes
```

## 📚 Дополнительные ресурсы

- [Docker CLI Documentation](https://docs.docker.com/engine/reference/commandline/cli/)
- [Docker Compose Reference](https://docs.docker.com/compose/compose-file/)
- [Dockerfile Best Practices](https://docs.docker.com/develop/develop-images/dockerfile_best-practices/)
