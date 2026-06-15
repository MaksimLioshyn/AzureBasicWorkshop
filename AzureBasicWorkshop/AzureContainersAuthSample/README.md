# AzureContainersAuthSample

Учебный .NET 8 solution, который покрывает темы:
- Module Containers: Docker, ACR, App Service for Containers, ACI, AKS
- Module Authentication: Microsoft Entra ID, OpenID Connect, B2B
- AZ-900 basics: management hierarchy, RBAC, Policy, locks, tagging

## Состав проекта

- `AzureContainersAuthSample.slnx` - solution файл
- `WebApp/` - ASP.NET Core MVC приложение с входом через Microsoft Entra ID
- `WebApp/Dockerfile` - контейнеризация приложения
- `docker-compose.yml` - локальный запуск контейнера
- `infra/main.bicep` - IaC для App Service (Linux container) + ACR + role assignment
- `infra/main.parameters.json` - пример параметров деплоя
- `deploy/aci/container-group.yaml` - пример запуска контейнера в ACI
- `deploy/aks/webapp-deployment.yaml` - минимальный манифест AKS
- `deploy/acr-webhook-sample.json` - пример webhook для auto-redeploy
- `.github/workflows/container-cd.yml` - GitHub Actions CI/CD
- `azure-pipelines.yml` - Azure DevOps pipeline пример
- `governance/` - JSON-примеры для AZ-900: policy, lock, tags

## 1) Локальный запуск приложения

```powershell
cd ContainersAndAuthenticationAndManagement/AzureContainersAuthSample/WebApp
dotnet run
```

Приложение будет доступно по адресу из вывода `dotnet run`.

## 2) Настройка Entra ID

1. Создайте `App registration` в Microsoft Entra ID.
2. Добавьте Redirect URI: `https://localhost:xxxx/signin-oidc`.
3. Получите значения `TenantId` и `ClientId`.
4. Для локальной разработки задайте секрет через user-secrets:

```powershell
cd ContainersAndAuthenticationAndManagement/AzureContainersAuthSample/WebApp
dotnet user-secrets init
dotnet user-secrets set "AzureAd:ClientSecret" "<secret>"
dotnet user-secrets set "AzureAd:TenantId" "<tenant-id>"
dotnet user-secrets set "AzureAd:ClientId" "<client-id>"
```

## 3) Запуск в Docker

```powershell
cd ContainersAndAuthenticationAndManagement/AzureContainersAuthSample
docker compose up --build
```

Проверка: `http://localhost:8080`

## 4) Деплой инфраструктуры (Bicep)

```powershell
cd ContainersAndAuthenticationAndManagement/AzureContainersAuthSample
az deployment group create \
  --resource-group <rg-name> \
  --template-file infra/main.bicep \
  --parameters @infra/main.parameters.json
```

После деплоя:
- запушьте контейнер в ACR
- в App Service будет использоваться managed identity + роль `AcrPull`

## 5) Пример деплоя в ACI и AKS

ACI:
```powershell
az container create --resource-group <rg-name> --file deploy/aci/container-group.yaml
```

AKS:
```powershell
kubectl apply -f deploy/aks/webapp-deployment.yaml
```

## 6) Важные учебные акценты

- Один и тот же контейнерный образ должен проходить через все окружения
- Секреты нельзя хранить в `appsettings*.json` в production
- Для Azure-ресурсов используйте managed identity и минимальные роли
- Для governance применяйте RBAC + Policy + locks + tags

## 7) Рекомендации для лекции

- Покажите сначала локальный запуск через `docker compose`
- Затем продемонстрируйте Entra ID login и страницу claims (`/Secure`)
- После этого объясните путь образа: local -> ACR -> App Service
- В конце закрепите AZ-900 на примерах из папки `governance/`
