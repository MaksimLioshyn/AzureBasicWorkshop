# Script to view container logs
# PowerShell script for Windows

Write-Host "=== Azure Containers & Authentication Demo Logs ===" -ForegroundColor Cyan
Write-Host "Press Ctrl+C to exit" -ForegroundColor Yellow
Write-Host ""

docker-compose logs -f --tail=50
