# Start MOY Generator Backend Server (PowerShell)

Write-Host "╔════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║   MOY Generator - Starting Backend     ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

# Check if node_modules exists
if (-not (Test-Path "node_modules")) {
    Write-Host "📦 Installing dependencies..." -ForegroundColor Yellow
    npm install
    Write-Host ""
}

# Start the server
Write-Host "🚀 Starting server on port 3001..." -ForegroundColor Green
npm start
