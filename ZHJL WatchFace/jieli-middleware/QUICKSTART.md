# JieLi Middleware - Quick Start Guide

## ✅ Setup Complete!

Your JieLi middleware server is successfully built and running!

**Server URL:** http://localhost:3001  
**Status:** Running ✓

---

## 🎯 What This Middleware Does

Converts watchFace.json from your React watch-assembly-tool into JieLi's folder structure and generates `.bin` files for device deployment.

**Workflow:**
```
watchFace.json → JieLi Folders → WatchJieLi.exe → .bin file
```

---

## 🚀 Quick Test

### 1. Open in Browser
Visit these URLs to verify the server:

- Health Check: http://localhost:3001/api/health
- Service Info: http://localhost:3001/api/jieli/info  
- Test Conversion: http://localhost:3001/api/jieli/test

### 2. Test with Sample JSON

A sample `sample_watchFace.json` has been created in the project folder with:
- Background image
- Hour and Minute digits
- Steps counter (5 digits)
- Battery percentage (3 digits)

### 3. Using PowerShell (Upload File)

```powershell
# Test with sample file
$file = "D:\Downloads\WatchFace\jieli-middleware\sample_watchFace.json"
$uri = "http://localhost:3001/api/jieli/convert"

# Upload and get bin file
Invoke-RestMethod -Uri $uri -Method Post -Form @{
    watchface = Get-Item -Path $file
} -OutFile "output.bin"

Write-Host "Bin file saved as output.bin"
```

### 4. Using cURL (JSON Body)

```bash
curl -X POST http://localhost:3001/api/jieli/convert \
  -H "Content-Type: application/json" \
  -d @sample_watchFace.json \
  --output watchface.bin
```

### 5. Using Postman

1. **Method:** POST
2. **URL:** http://localhost:3001/api/jieli/convert
3. **Body:** 
   - Select `form-data`
   - Key: `watchface` (type: File)
   - Value: Select your `watchFace.json` file
4. **Send** → Download response as `.bin` file

---

## 📂 Generated Folder Structure

When you convert a watchFace.json, the middleware creates:

```
temp/
└── 方形_390x450_普通_201#简约_Author_001_00/
    ├── 0101_主要#图片#背景_0_0_0/
    │   └── 0.bmp
    ├── 0601_时间#数字#时_0_100_150/
    │   ├── 0.png
    │   ├── 1.png
    │   └── ...9.png
    ├── 0603_时间#数字#分_0_200_150/
    │   └── ...
    ├── 0701_步数#数字#步_0_150_300/
    │   └── ... (5 digit folders for 5-digit number)
    ├── 0201_电量#数字#电_0_50_50/
    │   └── ...
    └── 辅助文件/
        ├── 表盘信息.txt
        ├── 效果.png
        └── 缩略.bmp
```

**Folder Naming Convention:**
```
XXXX_类别#图层类型#元素名_显示标志_X_Y
```

---

## 🔧 Configuration

Edit `.env` file to customize:

```env
# Server port (default: 3001)
PORT=3001

# Temp folder for conversions (cleaned automatically)
TEMP_FOLDER_PATH=./temp

# Output folder for bin files
OUTPUT_FOLDER_PATH=./output

# Path to WatchJieLi.exe (C# bin generator)
JIELI_BIN_GENERATOR_PATH=D:/Downloads/WatchFace/WatchJieLi/bin/Release/net6.0-windows/WatchJieLi.exe
```

---

## 📊 Component Mapping

The middleware automatically maps JSON components to JieLi codes:

| Component | JieLi Code | Folder Name Example |
|-----------|------------|---------------------|
| Background | 0101 | 0101_主要#图片#背景_0_0_0 |
| Hour | 0601, 0602 | 0601_时间#数字#时_0_X_Y |
| Minute | 0603, 0604 | 0603_时间#数字#分_0_X_Y |
| Second | 0605, 0606 | 0605_时间#数字#秒_0_X_Y |
| Steps | 0701-0709 | 0701_步数#数字#步_0_X_Y |
| Heart Rate | 0801-0803 | 0801_心率#数字#心_0_X_Y |
| Battery | 0201-0203 | 0201_电量#数字#电_0_X_Y |
| Calories | 0901-0903 | 0901_卡路里#数字#卡_0_X_Y |

**Multi-Digit Components:**
- Steps with 5 digits → Creates 5 folders (0701, 0702, 0703, 0704, 0705)
- Each folder contains digit images 0-9.png

---

## 🐛 Troubleshooting

### Server won't start
```powershell
# Check if port 3001 is already in use
netstat -ano | findstr :3001

# Kill the process if needed
taskkill /PID <PID> /F

# Restart server
npm run dev
```

### "Cannot find module" errors
```powershell
# Reinstall dependencies
npm install

# Rebuild TypeScript
npm run build
```

### Bin file generation fails
- Ensure WatchJieLi.exe path in `.env` is correct
- Check if .NET 6.0 Runtime is installed
- Currently using mock bin generation for testing
- To enable real bin generation, update `server.ts`:
  ```typescript
  // Change from:
  const binFilePath = await binGenerator.createMockBinFile(projectPath);
  
  // To:
  const binFilePath = await binGenerator.generateBin(projectPath);
  ```

### Images not converting properly
- Ensure ImageData contains base64 encoded images
- Check that image paths are valid
- BMP format may render as PNG for compatibility (Sharp library limitation)

---

## 🔄 Integration with Your Frontend

### Option 1: Direct API Call from React

```typescript
// In your React watch-assembly-tool
const exportToJieLi = async (watchFaceData: WatchFaceJSON) => {
  const response = await fetch('http://localhost:3001/api/jieli/convert', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ watchFaceData })
  });
  
  const blob = await response.blob();
  const url = window.URL.createObjectURL(blob);
  const a = document.createElement('a');
  a.href = url;
  a.download = `${watchFaceData.WatchName}.bin`;
  a.click();
};
```

### Option 2: FormData Upload

```typescript
const exportToJieLi = async (jsonFile: File) => {
  const formData = new FormData();
  formData.append('watchface', jsonFile);
  
  const response = await fetch('http://localhost:3001/api/jieli/convert', {
    method: 'POST',
    body: formData
  });
  
  const blob = await response.blob();
  // Download as .bin file
};
```

---

## 📝 Development Commands

```powershell
# Install dependencies
npm install

# Build TypeScript
npm run build

# Start server (development with auto-reload)
npm run dev

# Start server (production)
npm start

# Run in background
Start-Process powershell -ArgumentList "cd 'D:\Downloads\WatchFace\jieli-middleware'; npm run dev" -WindowStyle Hidden
```

---

## 📦 Project Structure

```
jieli-middleware/
├── src/
│   ├── types.ts              # TypeScript type definitions
│   ├── componentMapper.ts    # Maps JSON → JieLi codes
│   ├── converter.ts          # Creates folder structure
│   ├── binGenerator.ts       # Calls WatchJieLi.exe
│   └── server.ts             # Express API server
├── dist/                     # Compiled JavaScript
├── temp/                     # Temporary conversion folders
├── output/                   # Generated bin files
├── .env                      # Configuration
├── package.json
├── tsconfig.json
├── README.md                 # Full documentation
└── sample_watchFace.json     # Test data
```

---

## ✨ Next Steps

1. **Test with your actual watchFace.json** from the React tool
2. **Integrate the API endpoint** into your frontend
3. **Enable real bin generation** by uncommenting binGenerator.generateBin()
4. **Deploy to a server** if needed (update CORS settings in server.ts)

---

## 🎉 Success!

Your middleware is ready to convert watch faces! 

**Current Status:**
- ✅ Server running on http://localhost:3001
- ✅ All conversion logic implemented
- ✅ JieLi folder structure generation working
- ✅ C# WatchJieLi.exe integration ready
- ⚠️ Using mock bin generation (change to real when ready)

**Test it now:**
Visit http://localhost:3001/api/jieli/test in your browser!
