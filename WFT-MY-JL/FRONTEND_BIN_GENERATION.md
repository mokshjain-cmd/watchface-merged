# Frontend BIN Generation - Quick Start

## Overview
The "Generate Bin File" button in the Assembly View now generates BIN files directly using the MOYoung API through your Node.js backend server.

## Prerequisites

### 1. Configure API Credentials
Create `.env` file in the `server` directory:
```bash
cd server
cp .env.example .env
```

Edit `server/.env`:
```env
MOYOUNG_USERNAME=your_actual_username
MOYOUNG_PASSWORD=your_actual_password
```

### 2. Start the Backend Server
```bash
cd server
npm install  # If not already done
npm start
```

The server should start on `http://localhost:5555`

## Usage

### Step 1: Design Your Watch Face
1. Open the watch face editor
2. Add components (images, time, progress bars, etc.)
3. Position and configure them as desired

### Step 2: Generate BIN File
1. Click the **"Generate Bin File"** button in the toolbar (green button)
2. Wait for the process to complete (10-60 seconds)
3. The BIN file will automatically download

## Process Flow

When you click "Generate Bin File":

1. **📸 Image Collection** - Collects all images from your components
2. **🔧 MOY Generation** - Creates a temporary MOY file from your design
3. **📤 Backend Upload** - Sends MOY file to Node.js backend
4. **🔄 API Conversion** - Backend uses MOYoung API to convert MOY → BIN
5. **⬇️ Download** - BIN file is automatically downloaded

## Console Output

The process provides detailed console logging:
```
╔════════════════════════════════════════╗
║   BIN File Generation Process          ║
╚════════════════════════════════════════╝

📸 Step 1/4: Collecting images from components...
  ✓ Image collection complete: 15 unique images

🔧 Step 2/4: Generating MOY file...
  ✓ MOY file generated: 245678 bytes

📤 Step 3/4: Sending to backend for BIN conversion...
  ✓ Backend conversion successful
  BIN file size: 189234 bytes

⬇️ Step 4/4: Downloading BIN file...
  ✓ BIN file downloaded: MyWatchFace.bin

╔════════════════════════════════════════╗
║   BIN Generation Complete! ✅          ║
╚════════════════════════════════════════╝
```

## Success Message

After successful generation, you'll see:
```
✅ BIN file generated successfully!

File: MyWatchFace.bin
Size: 184.80 KB

You can now transfer this file to your watch!
```

## Error Handling

### Backend Not Running
```
❌ BIN Generation Failed

Cannot connect to backend server.

Please ensure:
1. Backend server is running (npm start in server folder)
2. Server is accessible at http://localhost:5555
3. Check console for more details
```

**Solution**: Start the backend server

### Authentication Error
```
❌ BIN Generation Failed

Authentication failed: 401

Please check:
1. MOYoung API credentials in server/.env file
2. Server console for authentication errors
```

**Solution**: Update credentials in `server/.env`

### API Timeout
If the conversion takes too long (>60 seconds), the request may timeout.

**Solution**: Check MOYoung server status or retry

## Troubleshooting

### 1. Button Disabled
- **Cause**: Generation already in progress
- **Solution**: Wait for current operation to complete

### 2. No Download
- **Cause**: Browser blocked the download
- **Solution**: Check browser download settings/permissions

### 3. Invalid MOY File
- **Cause**: Missing or corrupted images
- **Solution**: Re-upload images and try again

### 4. Network Error
- **Cause**: Backend server not accessible
- **Solution**: Verify server is running on port 5555

## Differences from Export MOY

| Feature | Export MOY | Generate BIN |
|---------|-----------|--------------|
| **Output** | .moy file | .bin file |
| **Usage** | For editing/backup | For watch deployment |
| **Processing** | Client-side only | Client + Server + API |
| **Time** | Instant | 10-60 seconds |
| **Requirements** | None | Backend + API credentials |

## Tips

1. **Test MOY First**: Use "Export MOY" button to verify your design before generating BIN
2. **Check Console**: Open browser console (F12) for detailed progress
3. **Server Logs**: Check server terminal for API communication details
4. **Stable Connection**: Ensure stable internet for API calls

## File Locations

- **Downloaded BIN**: Your browser's download folder
- **Temporary MOY**: `server/temp/` directory
- **Server Logs**: Terminal where `npm start` is running

## Next Steps

After generating the BIN file:
1. Transfer the file to your smart watch
2. Use the watch's face installer
3. Apply and enjoy your custom watch face!
