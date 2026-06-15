# Script to stop Docker container
# PowerShell script for Windows

Write-Host "=== Stopping Azure Containers & Authentication Demo ===" -ForegroundColor Cyan
Write-Host ""

Write-Host "Stopping and removing containers..." -ForegroundColor Yellow
docker-compose down

if ($LASTEXITCODE -eq 0) {
	Write-Host ""
	Write-Host "✓ Containers stopped and removed" -ForegroundColor Green
	Write-Host ""
	Write-Host "To restart, use: .\start-docker.ps1" -ForegroundColor Cyan
}
else {
	Write-Host ""
	Write-Host "❌ An error occurred while stopping" -ForegroundColor Red
}
