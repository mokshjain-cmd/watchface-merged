# Watch Face Assembly System - Complete Startup Guide

This script starts all necessary servers for the Watch Face Assembly System.

## What Gets Started

1. **Home Page System** (Port 5173 + 3001)
   - Frontend: http://localhost:5173
   - Backend API: http://localhost:3001
   - Reads watch data from Excel file

2. **ZHJL WatchFace (ZH-JL Board)**
   - Frontend: http://localhost:5174
   - Backend: http://localhost:3000 (JieLi Middleware)

3. **ZhouHai WatchFace (ZH-SF Board)**
   - Frontend: http://localhost:5175
   - Backend: http://localhost:5000 (.NET API)

## Usage

### Start All Servers
```powershell
.\start-all-servers.ps1
```

This will open 5 separate PowerShell windows, each running a different server.

### Manual Startup (if needed)

#### 1. Home Page
```powershell
cd "C:\Users\tanmay.pawar\Downloads\merging\watch-home"
npm start
```

#### 2. ZHJL Frontend
```powershell
cd "C:\Users\tanmay.pawar\Downloads\merging\ZHJL WatchFace\watch-assembly-tool"
npm run dev -- --port 5174
```

#### 3. ZHJL Backend
```powershell
cd "C:\Users\tanmay.pawar\Downloads\merging\ZHJL WatchFace\jieli-middleware"
npm run dev
```

#### 4. ZhouHai Frontend
```powershell
cd "C:\Users\tanmay.pawar\Downloads\merging\ZhouHaiWatchFace\frontend\watch-assembly-tool"
npm run dev -- --port 5175
```

#### 5. ZhouHai Backend
```powershell
cd "C:\Users\tanmay.pawar\Downloads\merging\ZhouHaiWatchFace\WatchFaceBackendAPI"
dotnet run
```

## Port Assignments

| Service | Port | Protocol |
|---------|------|----------|
| Home Page Frontend | 5173 | HTTP |
| Home Page Backend | 3001 | HTTP |
| ZHJL Frontend | 5174 | HTTP |
| ZHJL Backend | 3000 | HTTP |
| ZhouHai Frontend | 5175 | HTTP |
| ZhouHai Backend | 5000 | HTTP |

## Stopping Servers

Press `Ctrl+C` in each PowerShell window to stop the respective server, or simply close the window.

## Workflow

1. Start all servers using `.\start-all-servers.ps1`
2. Open http://localhost:5173 in your browser
3. Select a watch from the home page
4. The home page will automatically open the correct frontend based on the watch's vendor/board
5. Each frontend can communicate with its respective backend

## Troubleshooting

If a port is already in use, you'll see an error. Either:
- Stop the process using that port
- Or modify the port in the respective configuration files
