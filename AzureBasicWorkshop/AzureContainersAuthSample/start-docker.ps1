# Script to start Docker container with Azure Authentication Demo
# PowerShell script for Windows

Write-Host "=== Azure Containers & Authentication Demo ===" -ForegroundColor Cyan
Write-Host ""

# Check if .env file exists
if (-Not (Test-Path ".env")) {
	Write-Host "WARNING: File .env not found!" -ForegroundColor Yellow
	Write-Host "Creating .env from .env.example..." -ForegroundColor Yellow
	Copy-Item ".env.example" ".env"
	Write-Host ""
	Write-Host "SUCCESS: File .env created. Please edit it and add:" -ForegroundColor Green
	Write-Host "  - AZURE_TENANT_ID" -ForegroundColor White
	Write-Host "  - AZURE_CLIENT_ID" -ForegroundColor White
	Write-Host "  - AZURE_CLIENT_SECRET" -ForegroundColor White
	Write-Host ""
	Write-Host "After editing .env, run this script again." -ForegroundColor Cyan
	Write-Host ""

	# Open .env in notepad
	notepad.exe ".env"

	Write-Host "Press any key to exit..." -ForegroundColor Gray
	$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
	exit 0
}

Write-Host "1. Checking Docker..." -ForegroundColor Cyan
$dockerRunning = docker info 2>&1 | Select-String "Server Version"
if (-Not $dockerRunning) {
	Write-Host "ERROR: Docker is not running! Please start Docker Desktop." -ForegroundColor Red
	Write-Host ""
	Write-Host "Press any key to exit..." -ForegroundColor Gray
	$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
	exit 1
}
Write-Host "SUCCESS: Docker is running" -ForegroundColor Green
Write-Host ""

Write-Host "2. Stopping old containers (if any)..." -ForegroundColor Cyan
docker-compose down 2>$null
Write-Host "SUCCESS: Done" -ForegroundColor Green
Write-Host ""

Write-Host "3. Building image..." -ForegroundColor Cyan
docker-compose build
if ($LASTEXITCODE -ne 0) {
	Write-Host ""
	Write-Host "ERROR: Error building image" -ForegroundColor Red
	Write-Host "See output above for error details" -ForegroundColor Yellow
	Write-Host ""
	Write-Host "Press any key to exit..." -ForegroundColor Gray
	$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
	exit 1
}
Write-Host "SUCCESS: Image built successfully" -ForegroundColor Green
Write-Host ""

Write-Host "4. Starting container..." -ForegroundColor Cyan
docker-compose up -d
if ($LASTEXITCODE -ne 0) {
	Write-Host ""
	Write-Host "ERROR: Error starting container" -ForegroundColor Red
	Write-Host "See output above for error details" -ForegroundColor Yellow
	Write-Host ""
	Write-Host "Press any key to exit..." -ForegroundColor Gray
	$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
	exit 1
}
Write-Host "SUCCESS: Container started" -ForegroundColor Green
Write-Host ""

Write-Host "5. Waiting for application to be ready..." -ForegroundColor Cyan
Start-Sleep -Seconds 5

$maxAttempts = 10
$attempt = 0
$isReady = $false

while ($attempt -lt $maxAttempts -and -not $isReady) {
	try {
		$response = Invoke-WebRequest -Uri "http://localhost:5000" -TimeoutSec 2 -UseBasicParsing -ErrorAction SilentlyContinue
		if ($response.StatusCode -eq 200) {
			$isReady = $true
		}
	}
	catch {
		$attempt++
		Write-Host "  Attempt $attempt of $maxAttempts..." -ForegroundColor Yellow
		Start-Sleep -Seconds 3
	}
}

Write-Host ""
if ($isReady) {
	Write-Host "SUCCESS: Application is ready!" -ForegroundColor Green
	Write-Host ""
	Write-Host "Open in browser: http://localhost:5000" -ForegroundColor Cyan
	Write-Host ""
	Write-Host "Available pages:" -ForegroundColor White
	Write-Host "  - Home:              http://localhost:5000" -ForegroundColor White
	Write-Host "  - System Info:       http://localhost:5000/Info" -ForegroundColor White
	Write-Host "  - Protected Area:    http://localhost:5000/Secure" -ForegroundColor White
	Write-Host ""
	Write-Host "Useful commands:" -ForegroundColor White
	Write-Host "  - View logs:   docker-compose logs -f" -ForegroundColor Gray
	Write-Host "  - Stop:        docker-compose down" -ForegroundColor Gray
	Write-Host "  - Restart:     docker-compose restart" -ForegroundColor Gray
	Write-Host ""

	# Open browser
	Start-Process "http://localhost:5000"
}
else {
	Write-Host "WARNING: Application did not respond in time" -ForegroundColor Yellow
	Write-Host "Check logs: docker-compose logs -f" -ForegroundColor Yellow
	Write-Host ""
}

Write-Host "Press any key to exit..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
