# Master Startup Script for Watch Face Assembly System
# This script starts all frontend and backend servers

Write-Host "Starting Watch Face Assembly System..." -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Function to start a process in a new window
function Start-InNewWindow {
    param(
        [string]$Title,
        [string]$Command,
        [string]$WorkingDirectory
    )
    
    Write-Host "Starting $Title..." -ForegroundColor Yellow
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$WorkingDirectory'; Write-Host '$Title' -ForegroundColor Green; $Command"
}

# 1. Start Home Page (Frontend + Backend)
Start-InNewWindow -Title "Home Page (Frontend:5173 + Backend:3002)" `
    -Command "npm start" `
    -WorkingDirectory "C:\Users\tanmay.pawar\Downloads\merging\watch-home"

Start-Sleep -Seconds 2

# 2. Start ZHJL WatchFace Frontend (ZH-JL)
Start-InNewWindow -Title "ZHJL Frontend (Port 5174)" `
    -Command "npm run dev -- --port 5174" `
    -WorkingDirectory "C:\Users\tanmay.pawar\Downloads\merging\ZHJL WatchFace\watch-assembly-tool"

Start-Sleep -Seconds 1

# 3. Start ZHJL Backend (JieLi Middleware)
Start-InNewWindow -Title "ZHJL Backend (Port 3000)" `
    -Command "npm run dev" `
    -WorkingDirectory "C:\Users\tanmay.pawar\Downloads\merging\ZHJL WatchFace\jieli-middleware"

Start-Sleep -Seconds 1

# 4. Start ZhouHai WatchFace Frontend (ZH-SF)
Start-InNewWindow -Title "ZhouHai Frontend (Port 5175)" `
    -Command "npm run dev -- --port 5175" `
    -WorkingDirectory "C:\Users\tanmay.pawar\Downloads\merging\ZhouHaiWatchFace\frontend\watch-assembly-tool"

Start-Sleep -Seconds 1

# 5. Start ZhouHai Backend (.NET API)
Start-InNewWindow -Title "ZhouHai Backend (Port 5000)" `
    -Command "dotnet run" `
    -WorkingDirectory "C:\Users\tanmay.pawar\Downloads\merging\ZhouHaiWatchFace\WatchFaceBackendAPI"

Start-Sleep -Seconds 2

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "All servers are starting..." -ForegroundColor Green
Write-Host "`nServer URLs:" -ForegroundColor Cyan
Write-Host "  Home Page:           http://localhost:5173" -ForegroundColor White
Write-Host "  Home Backend:        http://localhost:3002" -ForegroundColor White
Write-Host "  ZHJL Frontend:       http://localhost:5174" -ForegroundColor White
Write-Host "  ZHJL Backend:        http://localhost:3000" -ForegroundColor White
Write-Host "  ZhouHai Frontend:    http://localhost:5175" -ForegroundColor White
Write-Host "  ZhouHai Backend:     http://localhost:5000" -ForegroundColor White
Write-Host "`nPress Ctrl+C in each window to stop the respective server" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Cyan
