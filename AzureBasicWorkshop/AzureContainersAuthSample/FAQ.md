# FAQ - Часто задаваемые вопросы

## 🐳 Docker

### Q: Почему используется multi-stage build в Dockerfile?

**A:** Multi-stage build позволяет:
- **Уменьшить размер образа**: SDK (~700MB) → Runtime (~200MB)
- **Улучшить безопасность**: В production образ нет инструментов разработки
- **Ускорить деплой**: Меньший образ быстрее загружается

Пример:
```dockerfile
# Stage 1: Build (большой, с SDK)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
RUN dotnet publish ...

# Stage 2: Runtime (маленький, без SDK)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
COPY --from=build /app/publish .
```

### Q: Почему порт внутри контейнера 8080, а снаружи 5000?

**A:** Это mapping портов в docker-compose:
```yaml
ports:
  - "5000:8080"  # host:container
```
- `5000` - порт на вашем компьютере (host)
- `8080` - порт внутри контейнера

Можно использовать любой свободный порт на host.

### Q: Как изменить порт приложения?

**A:** Измените в `docker-compose.yml`:
```yaml
ports:
  - "8000:8080"  # Теперь http://localhost:8000
```

⚠️ **Не забудьте** обновить Redirect URI в Azure AD:
- Было: `http://localhost:5000/signin-oidc`
- Стало: `http://localhost:8000/signin-oidc`

### Q: Что такое healthcheck и зачем он нужен?

**A:** Healthcheck - периодическая проверка работоспособности контейнера.

В `Dockerfile`:
```dockerfile
HEALTHCHECK --interval=30s --timeout=3s \
	CMD curl --fail http://localhost:8080/health || exit 1
```

Позволяет:
- Автоматически перезапускать неработающие контейнеры
- Мониторить состояние в Kubernetes/Azure Container Apps
- Использовать в load balancer (не отправлять трафик на нездоровый контейнер)

Проверка статуса:
```powershell
docker inspect --format='{{.State.Health.Status}}' container-name
# healthy, unhealthy, starting
```

### Q: Контейнер запускается, но приложение не отвечает. Что делать?

**A:** Проверьте логи:
```powershell
docker-compose logs -f
```

Частые причины:
1. **Ошибка в коде** - смотрите exception в логах
2. **Неправильный порт** - убедитесь, что `ASPNETCORE_URLS=http://+:8080`
3. **Долгий старт** - подождите 10-15 секунд
4. **Проблемы с health check** - временно отключите его

### Q: Как очистить все контейнеры и образы?

**A:**
```powershell
# Остановка всего
docker-compose down

# Удаление неиспользуемых ресурсов
docker system prune -a

# Или полная очистка (включая volumes)
docker system prune -a --volumes
```

⚠️ **Осторожно**: удалит ВСЕ неиспользуемые образы и контейнеры!

---

## 🔐 Azure Active Directory (Entra ID)

### Q: Что такое App Registration и зачем он нужен?

**A:** App Registration - это регистрация вашего приложения в Azure AD.

Создает:
- **Application (Client) ID** - уникальный идентификатор приложения
- **Tenant ID** - идентификатор вашей организации
- **Client Secret** - секрет для аутентификации приложения

Позволяет:
- Пользователям входить через корпоративные учетные записи
- Получать информацию о пользователе (claims)
- Использовать OAuth 2.0 / OpenID Connect

### Q: Ошибка "AADSTS50011: Reply URL does not match"

**A:** Redirect URI в Azure AD не совпадает с URI в приложении.

**Решение:**
1. Откройте Azure Portal → Entra ID → App registrations → ваше приложение
2. Authentication → Add a platform → Web
3. Redirect URI: `http://localhost:5000/signin-oidc`
4. Сохраните

⚠️ URI должен **точно совпадать**, включая:
- Протокол (`http` или `https`)
- Порт (`:5000`)
- Путь (`/signin-oidc`)

### Q: Что такое Tenant ID "common"?

**A:** `TenantId: "common"` означает **multi-tenant** приложение.

Варианты:
- **`common`** - любые Microsoft аккаунты (личные + рабочие)
- **`organizations`** - только рабочие/школьные аккаунты (любая организация)
- **`consumers`** - только личные Microsoft аккаунты
- **`<guid>`** - конкретная организация (single-tenant)

Для демо лучше использовать `common` или `organizations`.

### Q: Где хранить Client Secret безопасно?

**A:** **НЕ коммитьте секреты в Git!**

Варианты хранения:

**Локальная разработка:**
```powershell
# User Secrets (без Docker)
dotnet user-secrets set "AzureAd:ClientSecret" "your-secret"

# .env файл (с Docker)
# Добавьте .env в .gitignore!
```

**Production:**
- ✅ **Azure Key Vault** - лучший вариант
- ✅ **Environment Variables** в Azure App Service
- ✅ **Managed Identity** - без секретов вообще!
- ❌ НЕ в appsettings.json
- ❌ НЕ в коде

### Q: Что такое claims и зачем они?

**A:** Claims - утверждения о пользователе в токене.

Примеры claims:
```json
{
  "name": "John Doe",
  "preferred_username": "john.doe@company.com",
  "oid": "12345678-1234-1234-1234-123456789abc",  // уникальный ID
  "tid": "87654321-4321-4321-4321-abcdef123456",  // tenant ID
  "roles": ["Admin", "User"]  // роли пользователя
}
```

Используются для:
- Отображения имени пользователя
- Authorization (проверка ролей)
- Audit logging
- Персонализации

Доступ в коде:
```csharp
var name = User.FindFirst("name")?.Value;
var email = User.FindFirst("preferred_username")?.Value;
var isAdmin = User.IsInRole("Admin");
```

### Q: Как добавить роли пользователям?

**A:** В Azure Portal:

1. **App Registration** → **App roles** → **Create app role**
2. Настройте роль (например, "Admin", "Manager")
3. **Enterprise applications** → ваше приложение → **Users and groups**
4. Назначьте пользователям роли

В коде:
```csharp
[Authorize(Roles = "Admin")]
public class AdminController : Controller { }
```

---

## ⚙️ Конфигурация

### Q: Почему используются переменные окружения вместо appsettings.json?

**A:** Переменные окружения:
- ✅ Не коммитятся в Git (безопасность)
- ✅ Легко менять для разных сред (dev, staging, prod)
- ✅ Стандарт для Docker и облачных сред
- ✅ Поддерживаются всеми CI/CD системами

appsettings.json хорош для:
- Настроек по умолчанию
- Не-секретных значений
- Структурированной конфигурации

### Q: Как работает .env файл?

**A:** `.env` файл содержит переменные окружения:

```env
AZURE_TENANT_ID=common
AZURE_CLIENT_ID=11111111-1111-1111-1111-111111111111
AZURE_CLIENT_SECRET=my-secret
```

Docker Compose автоматически читает `.env` и подставляет в `docker-compose.yml`:

```yaml
environment:
  AzureAd__TenantId: ${AZURE_TENANT_ID}  # → "common"
```

⚠️ **Важно:**
- Добавьте `.env` в `.gitignore`
- Используйте `.env.example` как шаблон
- В production используйте Azure Key Vault

### Q: Как работает "AzureAd__TenantId" vs "AzureAd:TenantId"?

**A:** Это разные способы задания иерархической конфигурации.

**В appsettings.json:**
```json
{
  "AzureAd": {
	"TenantId": "common"
  }
}
```

**В переменных окружения (Docker, shell):**
```
AzureAd__TenantId=common  # __ (двойное подчеркивание)
```

**В User Secrets / dotnet CLI:**
```
AzureAd:TenantId  # : (двоеточие)
```

Все эти варианты создают одну и ту же конфигурацию:
```csharp
Configuration["AzureAd:TenantId"]  // → "common"
```

### Q: Как проверить, какие переменные видит контейнер?

**A:**
```powershell
# Все переменные
docker exec azure-containers-auth-webapp printenv

# Только Azure-переменные
docker exec azure-containers-auth-webapp printenv | Select-String Azure

# Или через docker-compose
docker-compose exec webapp printenv
```

---

## 🚀 Развертывание

### Q: Как развернуть в Azure App Service?

**A:** Через Azure CLI:

```powershell
# 1. Логин
az login

# 2. Создание Resource Group
az group create --name rg-demo --location eastus

# 3. Создание Container Registry
az acr create --resource-group rg-demo --name myregistry --sku Basic

# 4. Загрузка образа
az acr build --registry myregistry --image azure-auth-demo:v1 ./WebApp

# 5. Создание App Service
az appservice plan create --name plan-demo --resource-group rg-demo --is-linux --sku B1

az webapp create `
  --resource-group rg-demo `
  --plan plan-demo `
  --name my-auth-app `
  --deployment-container-image-name myregistry.azurecr.io/azure-auth-demo:v1

# 6. Настройка переменных
az webapp config appsettings set `
  --resource-group rg-demo `
  --name my-auth-app `
  --settings `
	AzureAd__TenantId="your-tenant-id" `
	AzureAd__ClientId="your-client-id" `
	AzureAd__ClientSecret="your-secret"
```

⚠️ Обновите Redirect URI: `https://my-auth-app.azurewebsites.net/signin-oidc`

### Q: Managed Identity vs Client Secret - что лучше?

**A:**

| Аспект | Client Secret | Managed Identity |
|--------|---------------|------------------|
| Безопасность | ⚠️ Нужно хранить секрет | ✅ Нет секретов |
| Ротация | ⚠️ Вручную | ✅ Автоматически |
| Стоимость | ✅ Бесплатно | ✅ Бесплатно |
| Сложность | ✅ Просто | ⚠️ Требует настройки |
| Локальная разработка | ✅ Работает | ⚠️ Не работает |

**Рекомендация:**
- Локально: Client Secret
- Production: Managed Identity

### Q: Как использовать Azure Key Vault для секретов?

**A:** Настройка:

1. **Создать Key Vault:**
```powershell
az keyvault create --name my-keyvault --resource-group rg-demo
```

2. **Добавить секрет:**
```powershell
az keyvault secret set --vault-name my-keyvault --name AzureAdClientSecret --value "your-secret"
```

3. **Настроить App Service:**
```powershell
az webapp config appsettings set `
  --resource-group rg-demo `
  --name my-auth-app `
  --settings `
	AzureAd__ClientSecret="@Microsoft.KeyVault(SecretUri=https://my-keyvault.vault.azure.net/secrets/AzureAdClientSecret)"
```

4. **Дать доступ (через Managed Identity):**
```powershell
# Включить System-assigned identity
az webapp identity assign --resource-group rg-demo --name my-auth-app

# Дать права на Key Vault
az keyvault set-policy --name my-keyvault --object-id <identity-principal-id> --secret-permissions get
```

---

## 🐛 Troubleshooting

### Q: Контейнер постоянно перезапускается

**A:** Проверьте:

1. **Логи:**
```powershell
docker-compose logs -f
```

2. **Health check:**
```powershell
docker inspect --format='{{json .State.Health}}' container-name
```

3. **Временно отключите health check** в docker-compose.yml:
```yaml
# healthcheck:
#   test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
```

4. **Проверьте exit code:**
```powershell
docker inspect --format='{{.State.ExitCode}}' container-name
```

### Q: HTTPS редиректы не работают в Docker

**A:** По умолчанию внутри контейнера используется HTTP.

**Решение:** Отключите HTTPS редирект для Docker:

```csharp
// Program.cs
if (!app.Environment.IsDevelopment() && !IsRunningInContainer())
{
	app.UseHttpsRedirection();
}

static bool IsRunningInContainer() =>
	Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
```

Или в docker-compose:
```yaml
environment:
  ASPNETCORE_URLS: http://+:8080  # только HTTP
```

### Q: "Permission denied" при запуске Docker на Linux

**A:**

```bash
# Добавить пользователя в группу docker
sudo usermod -aG docker $USER

# Перелогиниться
newgrp docker

# Или использовать sudo
sudo docker-compose up
```

---

## 📚 Обучение

### Q: Какие темы покрывает этот проект?

**A:**

**Docker:**
- Контейнеризация .NET приложений
- Multi-stage builds
- Docker Compose
- Environment variables
- Health checks
- Networking

**Azure (AZ-900):**
- Microsoft Entra ID (Azure AD)
- App Registration
- OAuth 2.0 / OpenID Connect
- Claims-based authentication
- Azure App Service
- Azure Container Registry

**.NET:**
- ASP.NET Core 8
- Razor Pages / MVC
- Microsoft.Identity.Web
- Configuration management
- Middleware pipeline

### Q: Какие следующие шаги для изучения?

**A:**

**Beginner:**
1. ✅ Запустить проект локально с Docker
2. ✅ Настроить Azure AD
3. ✅ Понять OAuth flow
4. ⬜ Добавить роли пользователей

**Intermediate:**
5. ⬜ Деплой в Azure App Service
6. ⬜ Настроить CI/CD (GitHub Actions)
7. ⬜ Добавить Application Insights
8. ⬜ Использовать Managed Identity
9. ⬜ Настроить Azure Key Vault

**Advanced:**
10. ⬜ Multi-region deployment
11. ⬜ Blue-green deployments
12. ⬜ Kubernetes (AKS)
13. ⬜ Service Mesh (Dapr)
14. ⬜ Custom policies & Conditional Access

### Q: Где найти больше информации?

**A:**

**Документация:**
- [Docker Documentation](https://docs.docker.com/)
- [ASP.NET Core Docs](https://docs.microsoft.com/aspnet/core/)
- [Microsoft Identity Platform](https://docs.microsoft.com/azure/active-directory/develop/)
- [Azure App Service](https://docs.microsoft.com/azure/app-service/)

**Tutorials:**
- [Containerize .NET app](https://docs.microsoft.com/dotnet/core/docker/build-container)
- [Secure a web app with Azure AD](https://docs.microsoft.com/azure/active-directory/develop/web-app-quickstart)
- [Deploy to Azure Container Apps](https://docs.microsoft.com/azure/container-apps/)

**Сертификация:**
- [AZ-900: Azure Fundamentals](https://docs.microsoft.com/certifications/azure-fundamentals/)
- [AZ-204: Azure Developer](https://docs.microsoft.com/certifications/azure-developer/)

---

## 💡 Советы и best practices

### Q: Как ускорить сборку Docker-образа?

**A:**

1. **Оптимизируйте порядок команд** (кэширование слоев):
```dockerfile
# ✅ Хорошо - зависимости кэшируются
COPY ["WebApp.csproj", "./"]
RUN dotnet restore
COPY . .  # Код меняется чаще

# ❌ Плохо - каждый раз restore
COPY . .
RUN dotnet restore
```

2. **Используйте .dockerignore:**
```
bin/
obj/
.vs/
*.user
```

3. **BuildKit** (быстрее):
```powershell
$env:DOCKER_BUILDKIT=1
docker build -t myapp .
```

### Q: Как минимизировать размер образа?

**A:**

1. **Multi-stage build** ✅ (уже используется)
2. **Используйте Alpine** (если совместимо):
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
```
3. **Удаляйте ненужное:**
```dockerfile
RUN apt-get update && apt-get install -y curl \
	&& rm -rf /var/lib/apt/lists/*  # Очистка кэша
```

### Q: Как организовать конфигурацию для разных сред?

**A:**

Создайте отдельные файлы:

```
docker-compose.yml          # Base
docker-compose.dev.yml      # Development
docker-compose.prod.yml     # Production
```

Запуск:
```powershell
# Development
docker-compose -f docker-compose.yml -f docker-compose.dev.yml up

# Production
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up
```

---

Есть вопрос, которого нет в FAQ? Создайте [Issue на GitHub](https://github.com/yourrepo/issues) или обратитесь к документации выше!
