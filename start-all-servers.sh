#!/bin/bash

# =========================================
# Master Startup Script for Watch Face Assembly System
# =========================================

echo "========================================="
echo "Starting Watch Face Assembly System..."
echo "========================================="

SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
MY_VENDOR_PATH="/Users/mokshjain/Desktop/WFT-MY-JL"

# -----------------------------------------
# Utility: Ensure Node Project Is Healthy
# -----------------------------------------
ensure_node_project() {
    local project_dir=$1
    local project_name=$2

    if [ ! -d "$project_dir" ]; then
        return
    fi

    echo "Checking $project_name..."

    cd "$project_dir" || exit

    # If node_modules missing OR vite not executable → reinstall
    if [ ! -d "node_modules" ] || [ -f "package.json" ] && [ -d "node_modules/.bin" ] && [ ! -x "node_modules/.bin/vite" ]; then
        echo "Installing dependencies for $project_name..."
        rm -rf node_modules package-lock.json
        npm install
    fi

    # Fix binary permissions
    if [ -d "node_modules/.bin" ]; then
        chmod -R +x node_modules/.bin 2>/dev/null
    fi
}

# -----------------------------------------
# Utility: Start Server in New Terminal Tab
# -----------------------------------------
start_server() {
    local title=$1
    local dir=$2
    local command=$3

    echo "Starting $title..."

    osascript <<EOF
tell application "Terminal"
    do script "cd '$dir' && echo '=== $title ===' && $command"
    activate
end tell
EOF
}

# =========================================
# WATCH HOME
# =========================================

WATCH_HOME="$SCRIPT_DIR/watch-home"
ensure_node_project "$WATCH_HOME" "Watch Home"

start_server "Home Page (Frontend:5173 + Backend:3002)" \
    "$WATCH_HOME" \
    "npm start"

sleep 2

# =========================================
# ZHJL WATCHFACE
# =========================================

ZHJL_FRONTEND="$SCRIPT_DIR/ZHJL WatchFace/watch-assembly-tool"
ZHJL_BACKEND="$SCRIPT_DIR/ZHJL WatchFace/jieli-middleware"

ensure_node_project "$ZHJL_FRONTEND" "ZHJL Frontend"
ensure_node_project "$ZHJL_BACKEND" "ZHJL Backend"

if [ -d "$ZHJL_FRONTEND" ]; then
    start_server "ZHJL Frontend (Port 5174)" \
        "$ZHJL_FRONTEND" \
        "npm run dev -- --port 5174"
    sleep 1
fi

if [ -d "$ZHJL_BACKEND" ]; then
    start_server "ZHJL Backend (Port 3000)" \
        "$ZHJL_BACKEND" \
        "npm run dev"
    sleep 1
fi

# =========================================
# ZHOUHAI WATCHFACE
# =========================================

ZH_FRONTEND="$SCRIPT_DIR/ZhouHaiWatchFace/frontend/watch-assembly-tool"
ZH_BACKEND="$SCRIPT_DIR/ZhouHaiWatchFace/WatchFaceBackendAPI"

ensure_node_project "$ZH_FRONTEND" "ZhouHai Frontend"

if [ -d "$ZH_FRONTEND" ]; then
    start_server "ZhouHai Frontend (Port 5175)" \
        "$ZH_FRONTEND" \
        "npm run dev -- --port 5175"
    sleep 1
fi

if [ -d "$ZH_BACKEND" ]; then
    start_server "ZhouHai Backend (Port 5000)" \
        "$ZH_BACKEND" \
        "dotnet run"
    sleep 1
fi

# =========================================
# MY VENDOR (WFT-MY-JL)
# =========================================

MY_FRONTEND="$MY_VENDOR_PATH"
MY_BACKEND="$MY_VENDOR_PATH/server"

ensure_node_project "$MY_FRONTEND" "MY Frontend"
ensure_node_project "$MY_BACKEND" "MY Backend"

if [ -d "$MY_FRONTEND" ]; then
    start_server "MY Frontend (Port 5176)" \
        "$MY_FRONTEND" \
        "npm run dev -- --port 5176"
    sleep 1
fi

if [ -d "$MY_BACKEND" ]; then
    start_server "MY Backend (Port 3001)" \
        "$MY_BACKEND" \
        "npm run dev"
    sleep 1
fi

# =========================================
# Final Output
# =========================================

echo ""
echo "========================================="
echo "Servers are starting in new Terminal tabs..."
echo ""
echo "Server URLs:"
echo "  Home Page:           http://localhost:5173"
echo "  Home Backend:        http://localhost:3002"
echo "  ZHJL Frontend:       http://localhost:5174"
echo "  ZHJL Backend:        http://localhost:3000"
echo "  ZhouHai Frontend:    http://localhost:5175"
echo "  ZhouHai Backend:     http://localhost:5000"
echo "  MY Frontend:         http://localhost:5176"
echo "  MY Backend:          http://localhost:3001"
echo ""
echo "Press Ctrl+C in each Terminal tab to stop the respective server"
echo "========================================="
