# Презентация: Docker & Azure Authentication

## 📊 Архитектурная диаграмма

```mermaid
graph TB
	User[👤 Пользователь<br/>Browser]

	subgraph Docker[🐳 Docker Container]
		WebApp[ASP.NET Core 8<br/>Web Application]
		IdentityWeb[Microsoft.Identity.Web<br/>OIDC Authentication]
	end

	subgraph Azure[☁️ Microsoft Azure]
		EntraID[Microsoft Entra ID<br/>Azure Active Directory]
		AppReg[App Registration<br/>Client ID + Secret]
	end

	User -->|1. HTTP Request| WebApp
	WebApp -->|2. Redirect to Login| User
	User -->|3. Sign In| EntraID
	EntraID -->|4. Validate Credentials| AppReg
	EntraID -->|5. Return Token| User
	User -->|6. Callback with Token| WebApp
	WebApp -->|7. Validate Token| IdentityWeb
	IdentityWeb -->|8. Verify with| EntraID
	WebApp -->|9. Authorized Access| User

	style Docker fill:#2496ED
	style Azure fill:#0078D4
	style User fill:#28a745
```

## 🔄 Процесс аутентификации (OAuth 2.0 / OpenID Connect)

```mermaid
sequenceDiagram
	participant U as 👤 User Browser
	participant W as 🐳 Web App<br/>(Container)
	participant E as ☁️ Entra ID

	U->>W: 1. Открыть /Secure
	W->>U: 2. 302 Redirect to Entra ID Login
	U->>E: 3. GET /authorize<br/>(client_id, redirect_uri, scope)
	E->>U: 4. Показать форму входа
	U->>E: 5. POST credentials
	E->>E: 6. Validate user
	E->>U: 7. 302 Redirect to callback<br/>с authorization code
	U->>W: 8. GET /signin-oidc?code=xxx
	W->>E: 9. POST /token<br/>(code, client_secret)
	E->>W: 10. Return id_token + access_token
	W->>W: 11. Validate token, create session
	W->>U: 12. 302 Redirect to /Secure
	U->>W: 13. GET /Secure (with cookie)
	W->>U: 14. 200 OK - Protected content
```

## 🐳 Docker Multi-Stage Build

```mermaid
graph LR
	subgraph Stage1[Build Stage - SDK 8.0 ~700MB]
		Source[Source Code] --> Restore[dotnet restore]
		Restore --> Build[dotnet build]
		Build --> Publish[dotnet publish]
	end

	subgraph Stage2[Runtime Stage - ASP.NET 8.0 ~200MB]
		Publish --> Copy[Copy artifacts]
		Copy --> Final[Final Image]
	end

	style Stage1 fill:#ff6b6b
	style Stage2 fill:#51cf66
```

## 📦 Компоненты проекта

```mermaid
graph TD
	Solution[AzureContainersAuthSample.sln]

	Solution --> WebApp[WebApp Project]
	Solution --> Docker[Docker Configuration]
	Solution --> Docs[Documentation]

	WebApp --> Controllers[Controllers]
	WebApp --> Views[Views]
	WebApp --> Config[Configuration]

	Controllers --> Home[HomeController]
	Controllers --> Secure[SecureController<br/>🔒 Requires Auth]
	Controllers --> Info[InfoController<br/>ℹ️ System Info]

	Views --> Layout[_Layout.cshtml<br/>Sign In/Out buttons]
	Views --> Index[Home/Index.cshtml]
	Views --> SecureView[Secure/Index.cshtml<br/>Display Claims]
	Views --> InfoView[Info/Index.cshtml<br/>Docker + Azure status]

	Config --> AppSettings[appsettings.json<br/>Base config]
	Config --> Env[Environment Variables<br/>Secrets from .env]

	Docker --> Dockerfile[Dockerfile<br/>Multi-stage build]
	Docker --> Compose[docker-compose.yml<br/>Orchestration]
	Docker --> EnvFile[.env<br/>🔐 Secrets]

	Docs --> README[README.md]
	Docs --> Quick[QUICKSTART.md]
	Docs --> Present[PRESENTATION.md]

	style Secure fill:#ff6b6b
	style EnvFile fill:#ffd43b
	style InfoView fill:#51cf66
```

## 🔐 Azure AD Configuration Flow

```mermaid
graph LR
	Portal[Azure Portal] --> EntraID[Microsoft Entra ID]
	EntraID --> AppReg[Create App Registration]
	AppReg --> Config[Configure]

	Config --> Name[Set Name]
	Config --> Redirect[Add Redirect URI<br/>http://localhost:5000/signin-oidc]
	Config --> Secret[Create Client Secret]

	Secret --> Copy[Copy Values]
	Copy --> TenantID[Tenant ID]
	Copy --> ClientID[Client ID]
	Copy --> ClientSecret[Client Secret]

	TenantID --> EnvVars[Store in .env file]
	ClientID --> EnvVars
	ClientSecret --> EnvVars

	EnvVars --> DockerCompose[docker-compose.yml<br/>reads from .env]
	DockerCompose --> Container[Container Environment]
	Container --> WebApp[ASP.NET Core App<br/>uses variables]

	style Secret fill:#ffd43b
	style EnvVars fill:#ffd43b
	style ClientSecret fill:#ff6b6b
```

## 🎯 Демонстрационный сценарий

### 1️⃣ Подготовка (5 минут)

- Показать структуру проекта в VS
- Объяснить `Dockerfile` (multi-stage build)
- Показать `docker-compose.yml`
- Показать `.env.example`

### 2️⃣ Конфигурация Azure AD (5 минут)

- Открыть Azure Portal
- Создать App Registration
- Показать Redirect URI
- Создать Client Secret
- Скопировать значения в `.env`

### 3️⃣ Запуск Docker (3 минуты)

```powershell
# Показать команду
.\start-docker.ps1

# Или вручную
docker-compose up --build
```

- Показать процесс сборки образа
- Показать запуск контейнера
- Показать логи: `docker-compose logs -f`

### 4️⃣ Демонстрация работы (10 минут)

#### A. Главная страница
- Открыть http://localhost:5000
- Показать дизайн и описание проекта

#### B. Системная информация
- Перейти на `/Info`
- Показать:
  - ✓ Запущено в Docker
  - Hostname контейнера
  - Использование памяти
  - Azure AD конфигурация
  - .NET Runtime информация

#### C. Аутентификация
- Нажать "Войти через Entra ID"
- Показать редирект на login.microsoftonline.com
- Выполнить вход
- Показать имя пользователя в навигации

#### D. Защищенная зона
- Перейти в "Защищенная зона"
- Показать claims пользователя:
  - `name` - имя
  - `preferred_username` - email
  - `oid` - уникальный ID пользователя
  - `tid` - tenant ID
  - `roles` - роли (если настроены)

#### E. Выход
- Нажать "Выйти"
- Попытаться зайти в /Secure без авторизации
- Показать редирект на вход

### 5️⃣ Docker команды (5 минут)

```powershell
# Список контейнеров
docker ps

# Информация о контейнере
docker inspect azure-containers-auth-webapp

# Логи
docker logs azure-containers-auth-webapp

# Вход в контейнер
docker exec -it azure-containers-auth-webapp /bin/bash

# Проверка переменных окружения
docker exec azure-containers-auth-webapp printenv | Select-String Azure

# Статистика ресурсов
docker stats azure-containers-auth-webapp

# Остановка
docker-compose down
```

### 6️⃣ Код (опционально, 5 минут)

#### Program.cs
```csharp
// Показать конфигурацию аутентификации
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
	.AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));
```

#### SecureController.cs
```csharp
[Authorize]  // 🔒 Требует аутентификацию
public class SecureController : Controller
{
	public IActionResult Index()
	{
		var claims = User.Claims; // Получаем claims из токена
		return View(claims);
	}
}
```

#### Dockerfile
```dockerfile
# Multi-stage build для уменьшения размера образа
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# ... сборка ...

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
# ... только runtime, без SDK
```

## 📊 Сравнение подходов

| Характеристика | Локальный запуск | Docker | Azure App Service |
|----------------|------------------|--------|-------------------|
| Размер | ~2 GB (SDK) | ~200 MB (образ) | Управляется Azure |
| Портативность | Зависит от ОС | ✅ Любая ОС | ✅ Облако |
| Скорость старта | 5-10 сек | 3-5 сек | 30-60 сек |
| Изоляция | ❌ Нет | ✅ Да | ✅ Да |
| Масштабирование | ❌ Вручную | ⚠️ Оркестратор | ✅ Автоматически |
| Стоимость | $0 | $0 (локально) | От $13/месяц |

## 🎓 Образовательные цели

### Docker концепции
- ✅ Контейнеризация приложений
- ✅ Multi-stage builds
- ✅ Docker Compose для оркестрации
- ✅ Environment variables для конфигурации
- ✅ Healthchecks

### Azure концепции (AZ-900)
- ✅ Microsoft Entra ID (Azure AD)
- ✅ App Registration
- ✅ OAuth 2.0 / OpenID Connect
- ✅ Claims-based authentication
- ✅ Managed Identity (упоминание)
- ✅ Azure Container Registry
- ✅ Azure App Service для контейнеров

### .NET концепции
- ✅ ASP.NET Core 8
- ✅ Razor Pages / MVC
- ✅ Microsoft.Identity.Web
- ✅ Configuration management
- ✅ Middleware pipeline

## ❓ Вопросы для аудитории

1. **Почему multi-stage build лучше обычного?**
   - Ответ: Уменьшает размер образа (~700MB → ~200MB), убирает инструменты разработки из production

2. **Где хранить секреты в production?**
   - Ответ: Azure Key Vault, Environment Variables Azure App Service, Managed Identity

3. **Чем отличается authentication от authorization?**
   - Authentication: "Кто вы?" (проверка личности)
   - Authorization: "Что вам разрешено?" (проверка прав)

4. **Что такое claims?**
   - Ответ: Утверждения о пользователе в токене (имя, email, роли, и т.д.)

5. **Как масштабировать Docker-приложение?**
   - Локально: Docker Compose `--scale`
   - Production: Kubernetes, Azure Container Apps, Docker Swarm

## 📝 Домашнее задание

1. Добавить роли пользователей в Azure AD
2. Реализовать role-based authorization
3. Добавить Application Insights для мониторинга
4. Настроить CI/CD с GitHub Actions
5. Деплой в Azure Container Apps

## 🔗 Полезные ссылки

- [Docker Documentation](https://docs.docker.com/)
- [ASP.NET Core Security](https://docs.microsoft.com/aspnet/core/security/)
- [Microsoft Identity Platform](https://docs.microsoft.com/azure/active-directory/develop/)
- [Azure Container Services](https://azure.microsoft.com/services/container-instances/)
