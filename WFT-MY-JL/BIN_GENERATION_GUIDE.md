# BIN File Generation Guide

This guide explains how to generate BIN files directly from MOY files using the MOYoung API.

## Overview

The system now supports automatic conversion of MOY files to BIN files through the MOYoung API. The process involves:

1. **Authentication** - Get an API token from MOYoung
2. **Upload MOY** - Send the MOY file to create a conversion task
3. **Poll for completion** - Wait for the conversion to complete
4. **Download BIN** - Retrieve the generated BIN file

## Setup

### 1. Install Dependencies

The required packages are already installed:
- `axios` - HTTP client for API calls
- `form-data` - For multipart file uploads

### 2. Configure API Credentials

Create a `.env` file in the `server` directory:

```bash
cd server
cp .env.example .env
```

Edit `.env` and add your MOYoung API credentials:

```env
MOYOUNG_USERNAME=your_actual_username
MOYOUNG_PASSWORD=your_actual_password
```

## API Endpoints

### 1. Generate BIN from existing MOY file

**Endpoint:** `POST http://localhost:5555/api/generate-bin`

**Request Body:**
```json
{
  "moyFilePath": "/path/to/watchface.moy"
}
```

Or with buffer:
```json
{
  "moyBuffer": "base64_encoded_moy_file",
  "moyFileName": "watchface.moy"
}
```

**Response:**
```json
{
  "success": true,
  "binPath": "/path/to/watchface.bin",
  "binUrl": "http://cdn.moyoung.com/...",
  "data": "base64_encoded_bin_file",
  "size": 123456
}
```

### 2. Generate MOY and BIN together

**Endpoint:** `POST http://localhost:5555/api/generate-moy-and-bin`

**Request Body:**
```json
{
  "moyFile": { /* MOY JSON structure */ },
  "images": [
    {
      "name": "image1.png",
      "data": "base64_encoded_image",
      "url": "image_url"
    }
  ],
  "preview": {
    "data": "base64_encoded_preview"
  }
}
```

**Response:**
```json
{
  "success": true,
  "moy": {
    "filename": "watchface_123456.moy",
    "data": "base64_encoded_moy",
    "size": 123456,
    "extractedImages": ["/extracted/image1.png"]
  },
  "bin": {
    "filename": "watchface_123456.bin",
    "data": "base64_encoded_bin",
    "size": 234567,
    "url": "http://cdn.moyoung.com/...",
    "path": "/path/to/watchface.bin"
  }
}
```

## Implementation Details

### BIN Generation Flow

The `binGenerator.ts` module handles the conversion:

```typescript
import { generateBinFromMoy } from './binGenerator.js';

// Convert MOY file to BIN
const result = await generateBinFromMoy(moyFilePath, outputDir);
console.log('BIN file:', result.binPath);
console.log('BIN URL:', result.binUrl);
```

### API Flow

1. **`authenticate()`**
   - POST to `http://vega.test.moyoung.com/api/login`
   - Returns authentication token

2. **`createFaceTask(token, moyFilePath)`**
   - POST to `http://vega.test.moyoung.com/api/tool/create-face-task`
   - Uploads MOY file with `Authorization: Bearer ${token}`
   - Returns `task_id`

3. **`getFaceTask(token, taskId)`**
   - GET from `http://vega.test.moyoung.com/api/tool/get-face-task?task_id=${taskId}`
   - Polls every 2 seconds for up to 60 seconds
   - Returns `face_file` URL when ready

4. **`downloadBinFile(url, outputPath)`**
   - Downloads BIN file from the URL
   - Saves to local filesystem

### Error Handling

The system includes comprehensive error handling:
- Authentication failures
- Upload errors
- Task creation failures
- Timeout after 60 seconds
- Download errors

If BIN generation fails, the MOY file is still available.

## Usage Examples

### Frontend Integration

```typescript
// Generate both MOY and BIN files
async function generateWatchface(watchfaceData) {
  const response = await fetch('http://localhost:5555/api/generate-moy-and-bin', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({
      moyFile: watchfaceData.moyFile,
      images: watchfaceData.images,
      preview: watchfaceData.preview,
    }),
  });
  
  const result = await response.json();
  
  if (result.success) {
    // Download MOY file
    downloadBase64File(result.moy.data, result.moy.filename);
    
    // Download BIN file
    if (result.bin.data) {
      downloadBase64File(result.bin.data, result.bin.filename);
    }
  }
}

function downloadBase64File(base64Data, filename) {
  const blob = base64ToBlob(base64Data);
  const url = URL.createObjectURL(blob);
  const a = document.createElement('a');
  a.href = url;
  a.download = filename;
  a.click();
  URL.revokeObjectURL(url);
}
```

### Command Line Testing

```bash
# Test with curl
curl -X POST http://localhost:5555/api/generate-bin \
  -H "Content-Type: application/json" \
  -d '{"moyFilePath": "server/temp/My_Watch_Face_123456.moy"}'
```

## Configuration

### Polling Settings

Adjust polling behavior in `binGenerator.ts`:

```typescript
// Poll every 2 seconds for up to 30 attempts (60 seconds total)
const binUrl = await getFaceTask(token, taskId, 30, 2000);
```

### API Endpoints

The API endpoints are configured in `binGenerator.ts`:

```typescript
const API_BASE_URL = 'http://vega.test.moyoung.com/api';
```

## Troubleshooting

### Authentication Errors

- Verify credentials in `.env` file
- Check if the API endpoint is accessible
- Ensure network connectivity

### Task Creation Errors

- Verify MOY file is valid
- Check file size limits
- Ensure proper file format

### Timeout Issues

- Increase polling attempts in `getFaceTask()`
- Check MOYoung server status
- Verify task_id is correct

### Download Errors

- Check if URL is accessible
- Verify network connectivity
- Ensure sufficient disk space

## Notes

- BIN files are saved in the `server/temp` directory
- MOY files must be valid and properly formatted
- The process typically takes 10-30 seconds
- Both files are returned as base64 for easy download

## Security Considerations

- Never commit `.env` file to version control
- Store API credentials securely
- Consider rate limiting for production use
- Validate file sizes and types before processing
