# MOY Generator Backend Setup

## Overview

The MOY file generation has been moved to a backend server using Node.js `fs` APIs, matching the vendor's implementation exactly. This ensures proper binary format handling for embedded images.

## Architecture

```
┌─────────────────┐         ┌──────────────────┐
│   Frontend      │         │   Backend        │
│   (React App)   │ ──────► │   (Express)      │
│                 │  JSON + │                  │
│  - Collects data│  Images │  - Node.js fs    │
│  - Sends to API │         │  - Binary write  │
│                 │ ◄────── │  - MOY format    │
└─────────────────┘   MOY   └──────────────────┘
                      file
```

## Setup Instructions

### 1. Install Backend Dependencies

```powershell
cd server
npm install
```

This will install:
- `express` - Web server framework
- `cors` - Enable cross-origin requests
- `multer` - Handle file uploads
- `typescript`, `ts-node` - TypeScript support

### 2. Start the Backend Server

```powershell
# From the server directory
npm start

# Or for development with auto-reload
npm run dev
```

The server will start on `http://localhost:3001`

You should see:
```
╔════════════════════════════════════════╗
║   MOY Generator Backend Server         ║
╚════════════════════════════════════════╝

🚀 Server running at http://localhost:3001
   Health check: http://localhost:3001/api/health
   Generate MOY: POST http://localhost:3001/api/generate-moy
```

### 3. Start the Frontend

In a separate terminal:

```powershell
# From the project root
npm run dev
```

The frontend will connect to the backend automatically.

## How It Works

### Frontend (MoyGenerator.ts)

1. **Collects MOY data structure** - All layer groups, settings, etc.
2. **Converts images to base64** - Each blob is converted for transmission
3. **Sends POST request** to backend with:
   ```json
   {
     "moyFile": { ...MOY JSON structure... },
     "images": [
       { "name": "bg.png", "data": "data:image/png;base64,...", "url": "blob:..." }
     ],
     "preview": { "data": "data:image/png;base64,..." }
   }
   ```
4. **Receives MOY file** as base64 response
5. **Downloads file** using browser download API

### Backend (moyServer.ts)

Matches vendor implementation exactly:

```typescript
// 1. Open file for writing
const fileHandle = openSync(outputPath, 'w+');

// 2. Write JSON data
writeSync(fileHandle, JSON.stringify(moyFile));

// 3. Write MOYEND delimiter
writeSync(fileHandle, 'MOYEND');

// 4. For each image in each layer
layerGroups.forEach(layer => {
  layer.nodeAttr.selectImg.forEach(img => {
    // Write image binary data
    writeSync(fileHandle, imageBuffer);
    
    // Write IMGEND delimiter
    writeSync(fileHandle, 'IMGEND');
  });
});

// 5. Write preview image (optional)
writeSync(fileHandle, previewBuffer);

// 6. Close file
closeSync(fileHandle);
```

## MOY File Format

```
┌────────────────────────────────────────┐
│ JSON Data (UTF-8 string)               │
├────────────────────────────────────────┤
│ "MOYEND" (6 bytes)                     │
├────────────────────────────────────────┤
│ Image 1 Binary Data (PNG)              │
├────────────────────────────────────────┤
│ "IMGEND" (6 bytes)                     │
├────────────────────────────────────────┤
│ Image 2 Binary Data (PNG)              │
├────────────────────────────────────────┤
│ "IMGEND" (6 bytes)                     │
├────────────────────────────────────────┤
│ ... (repeat for all images)            │
├────────────────────────────────────────┤
│ Preview Image Binary Data (optional)   │
└────────────────────────────────────────┘
```

## API Endpoints

### Health Check
```
GET http://localhost:3001/api/health
```

Response:
```json
{
  "status": "ok",
  "service": "MOY Generator Backend"
}
```

### Generate MOY File
```
POST http://localhost:3001/api/generate-moy
Content-Type: application/json

{
  "moyFile": { ...MOY structure... },
  "images": [ ...image data... ],
  "preview": { ...preview data... }
}
```

Response:
```json
{
  "success": true,
  "filename": "MyWatch_1736236800000.moy",
  "data": "...base64 encoded MOY file...",
  "size": 1234567
}
```

## Configuration

### Change Backend Port

Edit `server/moyServer.ts`:
```typescript
const PORT = 3001; // Change this
```

### Change Backend URL in Frontend

The frontend defaults to `http://localhost:3001`. To change it, modify the call in your component:

```typescript
const blob = await MoyGenerator.exportMoyFile(
  project, 
  imageData,
  'http://your-backend-url:port' // Optional third parameter
);
```

## Troubleshooting

### Backend not starting

**Error:** `Cannot find module 'express'`
```powershell
cd server
npm install
```

**Error:** Port 3001 already in use
```powershell
# Change PORT in moyServer.ts
# Or kill the process using the port
Get-Process -Id (Get-NetTCPConnection -LocalPort 3001).OwningProcess | Stop-Process
```

### Frontend can't connect to backend

**Error:** `Backend MOY generation failed: Failed to fetch`

1. Check backend is running: `curl http://localhost:3001/api/health`
2. Check CORS is enabled (already configured in moyServer.ts)
3. Check firewall isn't blocking port 3001

### Images corrupted in MOY file

The backend uses Node.js `fs.writeSync()` with binary buffers, matching the vendor implementation exactly. If images are still corrupted:

1. Check backend logs for image sizes
2. Verify PNG signatures in console output
3. Ensure all images are properly loaded before export

## Development

### Run backend in development mode
```powershell
cd server
npm run dev
```

This uses `nodemon` to auto-restart on file changes.

### View backend logs

All operations are logged to console:
```
📥 Received MOY generation request
   Watch name: MyWatch
   Layer groups: 5
   Images received: 12
   Converted image: bg.png (15234 bytes)
   ...
✓ Written JSON data: 5678 bytes
✓ Written MOYEND delimiter
  Writing image: bg.png (15234 bytes)
  ✓ Written image 1: bg.png
...
📤 Sending MOY file: 187456 bytes
```

## Benefits of Backend Generation

✅ **Matches vendor implementation** - Uses exact same Node.js fs APIs
✅ **Proper binary handling** - Native file I/O, no browser limitations
✅ **Reliable format** - Guaranteed binary compatibility
✅ **Server-side validation** - Can add additional checks
✅ **Scalable** - Can handle large files easily
✅ **Debuggable** - Full server-side logging

## File Structure

```
server/
├── moyServer.ts          # Express server implementation
├── package.json          # Backend dependencies
├── tsconfig.json         # TypeScript configuration
└── temp/                 # Generated MOY files (created automatically)

src/utils/
└── MoyGenerator.ts       # Updated to use backend API
```

## Next Steps

1. **Add authentication** if deploying publicly
2. **Add file cleanup** to remove temp files after download
3. **Add preview generation** on backend
4. **Add validation** for MOY structure
5. **Deploy to production** server

## Production Deployment

For production, you'll want to:

1. Build the backend:
   ```powershell
   cd server
   npm run build
   node dist/moyServer.js
   ```

2. Use environment variables:
   ```typescript
   const PORT = process.env.PORT || 3001;
   const FRONTEND_URL = process.env.FRONTEND_URL || 'http://localhost:5173';
   ```

3. Set up proper logging (Winston, Morgan, etc.)

4. Add rate limiting and security headers

5. Use a process manager (PM2, systemd, etc.)
