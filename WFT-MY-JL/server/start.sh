#!/bin/bash
# Start MOY Generator Backend Server

echo "╔════════════════════════════════════════╗"
echo "║   MOY Generator - Starting Backend     ║"
echo "╚════════════════════════════════════════╝"
echo ""

# Check if node_modules exists
if [ ! -d "node_modules" ]; then
    echo "📦 Installing dependencies..."
    npm install
    echo ""
fi

# Start the server
echo "🚀 Starting server on port 3001..."
npm start
