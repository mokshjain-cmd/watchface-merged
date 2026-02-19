# Watch Face Assembly - Home

A unified home page for selecting watches and navigating to the appropriate watch face assembly tool based on the watch's vendor and board type.

## Overview

This application provides a centralized interface for accessing different watch face assembly tools:
- **ZH-JL**: For watches with ZhouHai vendor and JieLi board
- **ZH-SF**: For watches with ZhouHai vendor and Sifli board

## Setup

1. Install dependencies:
```bash
npm install
```

2. Start both the backend server and frontend together:
```bash
npm start
```

Or start them separately:
- Backend server: `npm run server` (Port 3001)
- Frontend: `npm run dev` (Port 5173)

The home page will be available at `http://localhost:5173`

## Watch Data Management

### Adding New Watches

To add a new watch, simply update the Excel file at:
`C:\Users\tanmay.pawar\Downloads\merging\Watch List.xlsx`

The Excel file should have the following columns:
- **Watch** (or **Watch **): The name of the watch
- **Vendor**: The vendor code (e.g., ZH, MY)
- **SDK**: The board type (e.g., JL, SF)
- **Width**: The watch face width in pixels (e.g., 240, 390, 466)
- **Height**: The watch face height in pixels (e.g., 240, 280, 450, 466)
- **Shape**: The watch face shape (e.g., "Circular", "Square")
- **Watch Image (base 64)**: Optional base64 encoded image

The application will automatically read from this Excel file when it loads. No code changes needed!

When you select a watch, the correct frontend will open with the platform size and shape automatically set based on the data from the Excel file.

### Example Excel Structure

| Watch                 | Vendor | SDK | Width | Height | Shape    | Watch Image (base 64) |
|-----------------------|--------|-----|-------|--------|----------|----------------------|
| NoiseFit Twist        | ZH     | JL  | 240   | 240    | Circular |                      |
| ICON 2 2025           | ZH     | JL  | 240   | 280    | Square   |                      |
| Hive                  | ZH     | SF  | 466   | 466    | Circular |                      |
| Ultra 4               | ZH     | SF  | 390   | 450    | Square   |                      |

## Usage

### Starting the Assembly Tools

Before using the home page, make sure both assembly tool servers are running:

1. **ZH-JL Assembly Tool** (Port 5174):
```bash
cd "../ZHJL WatchFace/watch-assembly-tool"
npm install
npm run dev -- --port 5174
```

2. **ZH-SF Assembly Tool** (Port 5175):
```bash
cd "../ZhouHaiWatchFace/frontend/watch-assembly-tool"
npm install
npm run dev -- --port 5175
```

### Selecting a Watch

1. Open the home page at `http://localhost:5173`
2. Browse the available watches displayed as cards
3. Each card shows:
   - Watch name
   - Vendor badge (ZH, MY, etc.)
   - Board badge (JL, SF)
4. Click on a watch card to open the corresponding assembly tool in a new tab

## Watch Data

The watch data is **dynamically loaded** from the Excel file at:
`C:\Users\tanmay.pawar\Downloads\merging\Watch List.xlsx`

The backend server (`server.js`) reads this Excel file and serves the data via a REST API at `http://localhost:3001/api/watches`.

**To add a new watch**: Simply add a new row in the Excel file with the watch name, vendor, and SDK (board type). The changes will be reflected when you refresh the page.

## Architecture

### Backend Server (Port 3001)

A simple Express.js server that:
- Reads the Excel file using the `xlsx` library
- Exposes a REST API at `/api/watches`
- Serves watch data as JSON
- Automatically picks up changes when the page is refreshed

### Frontend (Port 5173)

A React + TypeScript application that:
- Fetches watch data from the backend API
- Displays watches in a responsive grid layout
- Routes to the correct assembly tool based on vendor/board

### Routing Configuration

The app uses a routing configuration that maps watch types to their respective frontend URLs:

```typescript
const WATCH_ROUTES = [
  {
    vendor: 'ZH',
    board: 'JL',
    path: '../ZHJL WatchFace/watch-assembly-tool/dist/index.html',
    port: 5174
  },
  {
    vendor: 'ZH',
  Add watch images by providing base64 encoded images in the Excel file
- Add search and filter functionality
- Show status indicators for running assembly tools
- Support for additional vendors (MY) and boards
- Auto-refresh when Excel file changes
- Add watch metadata (resolution, features, etc.)
```

### Components

- **App.tsx**: Main component that displays the watch grid and handles navigation
- **types.ts**: TypeScript interfaces for Watch and WatchRoute
- **App.css**: Styling for watch cards and grid layout

## Future Enhancements

- Load watch data dynamically from the Excel file or a backend API
- Add search and filter functionality
- Show status indicators for running assembly tools
- Add watch images instead of placeholder gradients
- Support for additional vendors (MY) and boards
