# Инструкция для преподавателя

## 🎓 Обзор

Этот проект подходит для демонстрации следующих тем:
- **Module 8**: Containers (Docker, ACR, App Service, ACI, AKS)
- **Module 9**: Authentication (Microsoft Entra ID, OAuth, OpenID Connect)
- **AZ-900**: Azure Fundamentals (Identity, Compute, Security)

Ожидаемое время демонстрации: **30-45 минут**

---

## 📋 Подготовка перед занятием (10 минут)

### 1. Проверка окружения

```powershell
# Проверить Docker
docker --version
docker info

# Проверить .NET
dotnet --version
```

### 2. Создание Azure AD App Registration

1. Откройте [Azure Portal](https://portal.azure.com)
2. **Microsoft Entra ID** → **App registrations** → **New registration**
   - Name: `Docker Auth Demo`
   - Supported accounts: `Accounts in any organizational directory`
   - Redirect URI: `Web` → `http://localhost:5000/signin-oidc`
3. Скопируйте:
   - Application (client) ID
   - Directory (tenant) ID
4. **Certificates & secrets** → **New client secret**
   - Description: `Demo secret`
   - Expires: `180 days`
   - Скопируйте значение

### 3. Настройка проекта

```powershell
# Перейти в папку проекта
cd D:\Study\Azure\ContainersAndAuthenticationAndManagement\AzureContainersAuthSample

# Создать .env из шаблона
Copy-Item .env.example .env

# Отредактировать .env (notepad или VS Code)
notepad .env
```

Заполните в `.env`:
```env
AZURE_TENANT_ID=ваш-tenant-id
AZURE_CLIENT_ID=ваш-client-id
AZURE_CLIENT_SECRET=ваш-client-secret
```

### 4. Предварительная сборка образа (опционально)

```powershell
# Чтобы не ждать во время демо
docker-compose build
```

---

## 🎬 Сценарий демонстрации

### Часть 1: Теория (5 минут)

#### Слайд 1: Архитектура
Покажите `PRESENTATION.md` - диаграмма архитектуры:
- User → Web App (Docker) → Entra ID

**Ключевые точки:**
- Приложение в контейнере изолировано
- Аутентификация через внешний сервис (Entra ID)
- OAuth 2.0 flow: redirect → login → callback → token

#### Слайд 2: OAuth Flow
Покажите sequence диаграмму из `PRESENTATION.md`:
1. User запрашивает защищенный ресурс
2. App редиректит на Entra ID
3. User вводит credentials
4. Entra ID возвращает token
5. App валидирует token

**Ключевые точки:**
- App никогда не видит пароль пользователя
- Token содержит claims (утверждения о пользователе)
- Single Sign-On (SSO) работает автоматически

---

### Часть 2: Практика - Docker (10 минут)

#### 1. Показать структуру проекта

```powershell
# Открыть в VS
code .
```

**Объясните файлы:**
- `WebApp/Dockerfile` - как собирается образ
- `docker-compose.yml` - как запускается контейнер
- `.env.example` - шаблон конфигурации

#### 2. Разобрать Dockerfile

```dockerfile
# Stage 1: Build (SDK ~700MB)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["WebApp.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime (ASP.NET ~200MB)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WebApp.dll"]
```

**Ключевые точки:**
- Multi-stage build уменьшает размер образа (700MB → 200MB)
- В production нет инструментов разработки (безопаснее)
- Каждый `RUN` создает новый слой (кэширование)

#### 3. Запустить контейнер

```powershell
# Вариант А: Скрипт (проще)
.\start-docker.ps1

# Вариант Б: Вручную (для объяснения)
docker-compose up --build
```

**Что происходит:**
1. Читается `.env` файл
2. Собирается образ (2 этапа)
3. Создается контейнер
4. Запускается приложение на порту 8080 (внутри)
5. Пробрасывается на порт 5000 (снаружи)

#### 4. Показать Docker команды

```powershell
# Список контейнеров
docker ps

# Логи в реальном времени
docker-compose logs -f

# Информация о контейнере
docker inspect azure-containers-auth-webapp

# Статистика ресурсов
docker stats azure-containers-auth-webapp

# Вход в контейнер
docker exec -it azure-containers-auth-webapp /bin/bash
ls -la /app  # Показать файлы
exit
```

**Ключевые точки:**
- Контейнер изолирован от host системы
- Можно войти внутрь для отладки
- Логи доступны снаружи

---

### Часть 3: Практика - Azure Authentication (15 минут)

#### 1. Главная страница

Откройте http://localhost:5000

**Покажите:**
- Красивый UI с карточками
- Описание проекта
- Кнопка "Войти через Entra ID"

#### 2. Системная информация

Перейдите: http://localhost:5000/Info

**Покажите карточки:**

**Docker Status:**
- ✅ Запущено в Docker: Да
- Container Host: `linux-container-id`
- Memory: ~100 MB
- Uptime: 00:05:23

**Azure AD Configuration:**
- Tenant ID: `common` (мультитенантный)
- Client ID: `your-client-id`
- Client Secret: ✓ Настроен

**.NET Runtime:**
- Framework: .NET 8.0.x
- OS: Linux 5.x (Debian)
- Architecture: X64

**Ключевые точки:**
- Приложение знает, что оно в Docker
- Конфигурация из переменных окружения
- Мониторинг ресурсов

#### 3. Демонстрация входа

**Шаг 1:** Нажмите "Войти через Entra ID"

**Покажите что происходит:**
- Редирект на `login.microsoftonline.com`
- URL содержит: `client_id`, `redirect_uri`, `scope`

**Шаг 2:** Введите credentials

**Покажите:**
- Форма входа Microsoft
- Может попросить согласие (consent screen)

**Шаг 3:** После входа

**Покажите:**
- Редирект обратно на `http://localhost:5000/signin-oidc`
- В навигации появилось имя пользователя
- Кнопка "Выйти"

#### 4. Защищенная зона

Перейдите: http://localhost:5000/Secure

**Покажите таблицу с claims:**

| Тип | Значение |
|-----|----------|
| `name` | John Doe |
| `preferred_username` | john.doe@company.com |
| `oid` | 12345678-1234-... (уникальный ID) |
| `tid` | 87654321-4321-... (tenant ID) |
| `iat` | 1234567890 (время создания токена) |
| `exp` | 1234571490 (время истечения) |
| `roles` | (если настроены) |

**Ключевые точки:**
- Claims = утверждения о пользователе
- Приходят в JWT токене
- Можно использовать для authorization

#### 5. Показать код

**Program.cs:**
```csharp
// Регистрация аутентификации
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
	.AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

// Middleware
app.UseAuthentication();  // Кто вы?
app.UseAuthorization();   // Что вам разрешено?
```

**SecureController.cs:**
```csharp
[Authorize]  // Требует аутентификацию
public class SecureController : Controller
{
	public IActionResult Index()
	{
		var claims = User.Claims;  // Получить claims
		return View(claims);
	}
}
```

**Ключевые точки:**
- `[Authorize]` защищает контроллер
- `User.Claims` доступны после входа
- Middleware обрабатывает OAuth flow автоматически

#### 6. Выход

Нажмите "Выйти"

**Покажите:**
- Редирект на `login.microsoftonline.com/logout`
- Session очищается
- Попытка зайти в `/Secure` → редирект на вход

---

### Часть 4: Troubleshooting Live (5 минут)

Намеренно создайте проблему для демонстрации отладки:

#### Проблема 1: Неправильный Client ID

```powershell
# Остановить
docker-compose down

# Изменить .env (неправильный Client ID)
notepad .env  # Измените на 00000000-0000-0000-0000-000000000000

# Запустить
docker-compose up -d
```

**Показать ошибку:**
- При входе: "AADSTS700016: Application not found"

**Решение:**
```powershell
# Исправить .env
notepad .env  # Вернуть правильный Client ID
docker-compose restart
```

#### Проблема 2: Неправильный Redirect URI

**Показать ошибку:**
- При входе: "AADSTS50011: Reply URL does not match"

**Решение:**
- Azure Portal → App Registration → Authentication
- Добавить `http://localhost:5000/signin-oidc`

**Ключевые точки:**
- Логи помогают найти проблему
- Большинство ошибок в конфигурации
- Проверяйте URI (port, path)

---

### Часть 5: Развертывание в Azure (5 минут) - Опционально

Покажите слайды или документацию:

#### Вариант А: Azure App Service

```powershell
# 1. Создать Container Registry
az acr create --resource-group rg-demo --name myregistry --sku Basic

# 2. Загрузить образ
az acr build --registry myregistry --image azure-auth-demo:v1 ./WebApp

# 3. Создать Web App
az webapp create \
  --resource-group rg-demo \
  --plan plan-demo \
  --name my-auth-app \
  --deployment-container-image-name myregistry.azurecr.io/azure-auth-demo:v1

# 4. Настроить переменные
az webapp config appsettings set \
  --resource-group rg-demo \
  --name my-auth-app \
  --settings \
	AzureAd__TenantId="..." \
	AzureAd__ClientId="..." \
	AzureAd__ClientSecret="..."
```

⚠️ **Не забудьте** обновить Redirect URI:
- `https://my-auth-app.azurewebsites.net/signin-oidc`

#### Вариант Б: Azure Container Apps

```powershell
# Современный serverless контейнер платформ
az containerapp create \
  --name ca-auth-demo \
  --resource-group rg-demo \
  --environment env-demo \
  --image myregistry.azurecr.io/azure-auth-demo:v1 \
  --target-port 8080 \
  --ingress external
```

**Ключевые точки:**
- Container Apps = serverless Kubernetes
- Автоматическое масштабирование (0 до N)
- Встроенный HTTPS

---

## ❓ Вопросы для аудитории

### Beginner:
1. **Зачем multi-stage build?**
   - Ответ: Уменьшить размер образа, убрать SDK из production

2. **Что такое claims?**
   - Ответ: Утверждения о пользователе в токене (имя, email, роли)

3. **Где хранить секреты?**
   - Ответ: НЕ в коде! .env локально, Key Vault в production

### Intermediate:
4. **Чем отличается authentication от authorization?**
   - Authentication: "Кто вы?" (проверка личности)
   - Authorization: "Что вам разрешено?" (проверка прав)

5. **Как масштабировать Docker приложение?**
   - Локально: `docker-compose --scale webapp=3`
   - Production: Kubernetes, Container Apps

### Advanced:
6. **Как реализовать role-based authorization?**
   ```csharp
   [Authorize(Roles = "Admin")]
   public class AdminController : Controller { }
   ```

7. **Как обеспечить zero-downtime deployment?**
   - Blue-green deployment
   - Rolling updates (Kubernetes)
   - Health checks + load balancer

---

## 📝 Домашнее задание (опционально)

### Уровень 1 (Базовый):
1. Запустить проект локально
2. Настроить Azure AD
3. Протестировать вход

### Уровень 2 (Средний):
4. Добавить роли в Azure AD
5. Реализовать `[Authorize(Roles = "Admin")]`
6. Создать админ-панель

### Уровень 3 (Продвинутый):
7. Деплой в Azure App Service
8. Настроить CI/CD (GitHub Actions)
9. Добавить Application Insights
10. Использовать Managed Identity

---

## 🔗 Полезные ссылки для студентов

- [Docker Documentation](https://docs.docker.com/)
- [ASP.NET Core Security](https://docs.microsoft.com/aspnet/core/security/)
- [Microsoft Identity Platform](https://docs.microsoft.com/azure/active-directory/develop/)
- [Azure App Service](https://docs.microsoft.com/azure/app-service/)
- [AZ-900 Study Guide](https://docs.microsoft.com/certifications/azure-fundamentals/)

---

## 📞 Поддержка

Файлы в проекте:
- **QUICKSTART.md** - Быстрый старт для студентов
- **PRESENTATION.md** - Презентация с диаграммами
- **FAQ.md** - Часто задаваемые вопросы
- **DOCKER-COMMANDS.md** - Справочник команд

---

**Успешной демонстрации! 🎉**
