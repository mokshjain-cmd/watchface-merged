# JieLi Middleware — README (concise handoff)

This repository provides a Node.js/TypeScript middleware to convert `watchFace.json` (produced by `watch-assembly-tool`) into the JieLi folder structure and produce device-ready `.bin` files using the C# bin generator.

For a full, detailed handover (per-file deep-dive, progress, issues, and next steps) see: `PROJECT_COMPLETE.md` in this folder.

Quick Start
-----------
1. Install dependencies and build:

```powershell
cd jieli-middleware
npm install
npm run build
```

2. Configure environment variables in `.env` (example):

```env
PORT=3001
TEMP_FOLDER_PATH=./temp
OUTPUT_FOLDER_PATH=./output
JIELI_BIN_GENERATOR_PATH=C:/Users/<you>/Downloads/WatchFace/WatchFaceCLI/bin/Release/net6.0-windows7.0/WatchFaceCLI.exe
```

3. Run the server:

```powershell
node dist/server.js
```

API Endpoints (summary)
-----------------------
- `GET /api/health` — Health check
- `GET /api/jieli/info` — Service capabilities and configured bin generator path
- `GET /api/jieli/test` — Minimal test conversion (creates a small temp folder)
- `POST /api/jieli/convert` — Convert a watchFace JSON into a `.bin` file

How to call `/api/jieli/convert`
--------------------------------
Option A — Send JSON with embedded ImageData map (recommended for the web client):

```powershell
curl -X POST http://localhost:3001/api/jieli/convert \
  -H "Content-Type: application/json" \
  -d @watchFace.json --output mywatch.bin
```

Where `watchFace.json` contains either `ImageData` inside components or a top-level `ImageData` map keyed by asset path.

Option B — Multipart upload (files + JSON)

```powershell
curl -X POST http://localhost:3001/api/jieli/convert \
  -F "watchface=@watchFace.json" \
  -F "assets=@assets/image_001.png" \
  --output mywatch.bin
```

Notes on integration
--------------------
- Prefer sending an `ImageData` map (base64 data URLs or raw base64 strings) in the JSON body to avoid filename mapping issues.
- For multi-digit components, frontend must provide digit images in the expected order (0..9) and ideally include `size.width` that represents per-digit width. Middleware falls back to `Width` or 40px.
- The middleware calls the configured C# CLI (`WatchFaceCLI.exe` or `WatchJieLi.exe`) with two args: inputFolder and outputFolder. The CLI writes `hor.bin` under a timestamped subfolder in the output folder.

Common commands
---------------
Start dev server (with watch / nodemon in your environment):

```powershell
npm run dev
```

Generate bin manually (PowerShell):

```powershell
& "C:\Users\<you>\Downloads\WatchFace\WatchFaceCLI\bin\Release\net6.0-windows7.0\WatchFaceCLI.exe" "C:\path\to\project_folder" "C:\path\to\output"
```

Concise Known Issues & Next Steps
--------------------------------
- Digit offsets: middleware currently computes per-digit X using `component.Width` or a 40px default. To match the Windows GUI exactly, either frontend should export `digitWidth`/`digitSpacing` or middleware should compute the actual image width via `sharp(buffer).metadata()` before placing digits (recommended).
- MaxNum mismatch: previously caused 3-digit detection for time components (MaxNum=100). Fixed by forcing 2 digits for time/date.
- BMP handling: BMP buffers are written directly to disk to avoid `sharp` parsing issues.
- Encoding: If you see path/encoding problems with Chinese characters, run the CLI manually from PowerShell to inspect output.

Where to find more details
-------------------------
- Per-file deep dive and full handover: `PROJECT_COMPLETE.md` (this repo)
- Middleware source: `src/server.ts`, `src/converter.ts`, `src/componentMapper.ts`, `src/binGenerator.ts`
- Frontend source (editor, preview): `../watch-assembly-tool/src/components/AssemblyView.tsx`, `ComponentProperties.tsx`, `PreviewView.tsx`
- Bin generator (C#): `../WatchFaceCLI/Program.cs`, `../WatchJieLi/Common/BinHelpers.cs`

If you want me to also implement the middleware change to compute exact digit widths from decoded images and re-run tests, I can implement that next and add an end-to-end test script.

License: MIT
# JieLi Middleware

Middleware service to convert watchFace.json (ZhouHai format) to JieLi folder structure and generate binary watchface files.

## Overview

This Node.js/TypeScript service bridges the gap between:
- **Frontend**: React watch-assembly-tool that outputs `watchFace.json`
- **Backend**: JieLi's C# bin generator (`WatchJieLi.exe`)

It converts the JSON format into JieLi's specific folder structure with proper naming conventions, then generates `.bin` files for device deployment.

## Installation

```powershell
# Install dependencies
npm install

# Build TypeScript
npm run build
```

## Configuration

Update `.env` file with your paths:

```env
# Server port
PORT=3001

# Temporary folder for conversions
TEMP_FOLDER_PATH=./temp

# Output folder for bin files
OUTPUT_FOLDER_PATH=./output

# Path to WatchJieLi.exe (C# bin generator)
JIELI_BIN_GENERATOR_PATH=D:/Downloads/WatchFace/WatchJieLi/bin/Release/net6.0-windows/WatchJieLi.exe
```

## Usage

### Development Mode

```powershell
npm run dev
```

Server starts at `http://localhost:3001`

### Production Mode

```powershell
npm run build
npm start
```

## API Endpoints

### 1. Health Check
```http
GET /api/health
```

**Response:**
```json
{
  "status": "ok",
  "service": "JieLi Middleware",
  "version": "1.0.0",
  "timestamp": "2024-01-01T00:00:00.000Z"
}
```

### 2. Service Information
```http
GET /api/jieli/info
```

**Response:**
```json
{
  "service": "JieLi Middleware Converter",
  "version": "1.0.0",
  "capabilities": {
    "inputFormat": "watchFace.json (ZhouHai format)",
    "outputFormat": "JieLi folder structure → .bin file",
    "supportedComponents": [...]
  },
  "binGenerator": {
    "available": true,
    "path": "C:/Users/.../WatchJieLi.exe"
  }
}
```

### 3. Convert Watch Face
```http
POST /api/jieli/convert
Content-Type: multipart/form-data or application/json
```

**Request Option 1: File Upload**
```bash
curl -X POST http://localhost:3001/api/jieli/convert \
  -F "watchface=@watchFace.json"
```

**Request Option 2: JSON Body**
```bash
curl -X POST http://localhost:3001/api/jieli/convert \
  -H "Content-Type: application/json" \
  -d '{"watchFaceData": {...}}'
```

**Response:**
Binary file download (`watchface.bin`)

**Error Response:**
```json
{
  "success": false,
  "error": "Error message"
}
```

### 4. Test Endpoint
```http
GET /api/jieli/test
```

Runs a test conversion with minimal sample data.

## JieLi Folder Structure

The middleware generates folders following JieLi's naming convention:

```
方形_390x450_普通_201#简约_Author_001_00/
├── 0101_主要#图片#背景_0_0_0/
│   └── 0.bmp
├── 0601_时间#数字#时_0_100_150/
│   ├── 0.png
│   ├── 1.png
│   └── ...
├── 0701_步数#数字#步_0_200_300/
│   └── ...
└── 辅助文件/
    ├── 表盘信息.txt
    ├── 效果.png
    └── 缩略.bmp
```

**Folder Name Format:**
```
XXXX_类别#图层类型#元素名_显示标志_X_Y
```

**Component Codes:**
- `0101`: Background
- `02XX`: Battery
- `03XX`: Date (Month, Day, Week)
- `06XX`: Time (Hour, Minute, Second)
- `07XX`: Steps
- `08XX`: Heart Rate
- `09XX`: Calories
- `13XX`: Analog Clock Hands

## Development

### Project Structure

```
jieli-middleware/
├── src/
│   ├── types.ts              # TypeScript type definitions
│   ├── componentMapper.ts    # Component code mapping logic
│   ├── converter.ts          # JSON to folder converter
│   ├── binGenerator.ts       # Bin file generation
│   └── server.ts             # Express API server
├── temp/                     # Temporary conversion files
├── output/                   # Generated bin files
├── .env                      # Configuration
├── package.json
└── tsconfig.json
```

### Example watchFace.json

```json
{
  "WatchName": "My Watch Face",
  "Width": 390,
  "Height": 450,
  "WatchStyles": [{
    "StyleName": "default",
    "DragBindBases": [
      {
        "ItemName": "Background",
        "ElementType": "DragBindImage",
        "X": 0,
        "Y": 0,
        "ImageData": "base64_encoded_image..."
      },
      {
        "ItemName": "Hour",
        "ElementType": "DragBindNormalDateTime",
        "X": 100,
        "Y": 150,
        "ImageData": "0.png,1.png,2.png..."
      }
    ]
  }]
}
```

## Component Mapping

The middleware automatically maps JSON components to JieLi codes:

| ItemName | JieLi Code | Category | Layer Type |
|----------|------------|----------|------------|
| Background | 0101 | 主要 | 图片 |
| Hour | 0601, 0602 | 时间 | 数字 |
| Minute | 0603, 0604 | 时间 | 数字 |
| Steps | 0701-0709 | 步数 | 数字 |
| HeartRate | 0801-0803 | 心率 | 数字 |
| Battery | 0201-0203 | 电量 | 数字 |

## Error Handling

- Invalid JSON format → `400 Bad Request`
- Conversion failures → `500 Internal Server Error`
- Missing components → Logs warnings, continues conversion
- Bin generation errors → Returns error details

## Cleanup

The middleware automatically cleans up temporary folders after sending the bin file.

## License

MIT
