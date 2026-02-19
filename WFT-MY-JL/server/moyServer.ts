/**
 * Backend Server for MOY File Generation
 * Uses Node.js fs APIs to generate MOY files matching vendor format
 */

import express from 'express';
import { openSync, writeSync, closeSync, readFileSync, mkdirSync, existsSync, writeFileSync } from 'fs';
import { join } from 'path';
import cors from 'cors';
import { fileURLToPath } from 'url';
import { dirname } from 'path';
import { generateBinFromMoy, generateBinFromMoyBuffer } from './binGenerator.js';

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

const app = express();
const PORT = parseInt(process.env.PORT || '5555', 10); // Parse as number, default 5555

// Enable CORS for frontend communication
app.use(cors());
app.use(express.json({ limit: '50mb' }));

// Temporary directory for image storage
const TEMP_DIR = join(__dirname, 'temp');
if (!existsSync(TEMP_DIR)) {
  mkdirSync(TEMP_DIR, { recursive: true });
}

interface MoyGenerationRequest {
  moyFile: any; // The MOY JSON structure
  images: Array<{ name: string; data: string; url: string }>; // Base64 encoded images
  preview?: { data: string }; // Optional preview image
}

/**
 * Extract PNG images from MOY file for verification
 */
function extractImagesFromMoy(moyPath: string, outputDir: string): string[] {
  console.log('\n🔍 Extracting images from MOY file:', moyPath);
  
  const fileData = readFileSync(moyPath);
  const extractedFiles: string[] = [];
  
  // Find the first MOYEND marker (after JSON)
  const moyendMarker = Buffer.from('MOYEND', 'binary');
  let position = fileData.indexOf(moyendMarker);
  
  if (position === -1) {
    throw new Error('Invalid MOY file: MOYEND marker not found');
  }
  
  // Move past the first MOYEND
  position += moyendMarker.length;
  
  // Extract images between MOYEND markers
  const imgendMarker = Buffer.from('IMGEND', 'binary');
  let imageIndex = 0;
  
  while (position < fileData.length) {
    // Find next IMGEND marker
    const imgendPos = fileData.indexOf(imgendMarker, position);
    
    if (imgendPos === -1) {
      // No more images, check for final MOYEND
      const finalMoyend = fileData.indexOf(moyendMarker, position);
      if (finalMoyend !== -1 && finalMoyend === position) {
        console.log('   ✓ Reached final MOYEND marker');
      }
      break;
    }
    
    // Extract image data (from current position to IMGEND)
    const imageData = fileData.slice(position, imgendPos);
    
    // Validate PNG signature
    const isPNG = imageData.length >= 8 &&
      imageData[0] === 0x89 && imageData[1] === 0x50 && imageData[2] === 0x4E && imageData[3] === 0x47 &&
      imageData[4] === 0x0D && imageData[5] === 0x0A && imageData[6] === 0x1A && imageData[7] === 0x0A;
    
    if (isPNG) {
      const outputPath = join(outputDir, `extracted_image_${imageIndex}.png`);
      writeFileSync(outputPath, imageData);
      extractedFiles.push(outputPath);
      console.log(`   ✓ Extracted: ${outputPath} (${imageData.length} bytes)`);
      imageIndex++;
    } else {
      console.log(`   ⚠ Skipped non-PNG data at position ${position} (${imageData.length} bytes)`);
    }
    
    // Move past IMGEND marker
    position = imgendPos + imgendMarker.length;
  }
  
  console.log(`\n📦 Total images extracted: ${extractedFiles.length}\n`);
  return extractedFiles;
}

/**
 * Generate MOY file using Node.js fs APIs (matching vendor implementation)
 */
/**
 * Optimized MOY File Generation
 * Aligned with vendor binary expectations
 */
function generateMoyFile(moyFile: any, images: Map<string, Buffer>, outputPath: string): void {
  // Use 'w+' to match the vendor's openSync flag
  const fileHandle = openSync(outputPath, 'w+');
  
  try {
    const jsonBuffer = Buffer.from(JSON.stringify(moyFile), 'binary');
    writeSync(fileHandle, jsonBuffer);
    writeSync(fileHandle, Buffer.from('MOYEND', 'binary'));
    
    const { layerGroups } = moyFile;
    
    layerGroups.forEach((ele: any) => {
      const { type, nodeAttr } = ele;
      if (nodeAttr && Array.isArray(nodeAttr[type])) {
        const imagesArray = nodeAttr[type].filter((imgObj: any) => imgObj.url);
        imagesArray.forEach((imgObj: any) => {
          // Skip data URLs
          if (imgObj.url.startsWith('data:image')) return;
          const imageBuffer = images.get(imgObj.name);
          
          if (imageBuffer) {
            // VENDOR REQUIREMENT: Write raw image buffer DIRECTLY.
            // Do not convert to string; write the Buffer object itself.
            writeSync(fileHandle, imageBuffer); 
            // Write the marker immediately after the image data
            writeSync(fileHandle, Buffer.from('IMGEND'));
            console.log(`Successfully attached: ${imgObj.name}`);
          } else {
            console.error(`Missing buffer for: ${imgObj.name}`);
          }
        });
      }
    });
    
    // Write thumbnail image WITHOUT IMGEND marker (last item before final MOYEND)
    const thumbnailBuffer = images.get('thumbnail');
    if (thumbnailBuffer) {
      writeSync(fileHandle, thumbnailBuffer);
      console.log(`Successfully attached thumbnail (no IMGEND for last image)`);
    }

    // writeSync(fileHandle, Buffer.from('MOYEND', 'binary'));

  } catch (err) {
    console.error("Binary generation failed:", err);
    throw err;
  } finally {
    closeSync(fileHandle);
  }
}

/**
 * API Endpoint: Generate MOY file
 */
app.post('/api/generate-moy', async (req, res) => {
  try {
    const { moyFile, images, preview } = req.body as MoyGenerationRequest;
    console.log('Received MOY generation request with data:', { moyFile, images: images ? `[${images.length} images]` : null, preview: preview ? '[preview data]' : null });
    if (!moyFile || !images) {
      return res.status(400).json({ 
        success: false, 
        error: 'Missing required data: moyFile and images' 
      });
    }
    
    console.log('\n📥 Received MOY generation request');
    console.log(`   Watch name: ${moyFile.faceName}`);
    console.log(`   Layer groups: ${moyFile.layerGroups?.length || 0}`);
    console.log(`   Images received: ${images.length}`);
    
    // Create temporary directory for PNG files
    const tempImageDir = join(TEMP_DIR, `images_${Date.now()}`);
    mkdirSync(tempImageDir, { recursive: true });
    
    // Convert base64 → PNG file → Binary buffer pipeline
    const imageMap = new Map<string, Buffer>();
    
    for (const img of images) {
      // Remove data URL prefix if present
      const base64Data = img.data.replace(/^data:image\/\w+;base64,/, '');
      
      // Step 1: Decode base64 to buffer
      const tempBuffer = Buffer.from(base64Data, 'base64');
      
      // Validate PNG signature before writing
      const isPNG = tempBuffer.length >= 8 &&
        tempBuffer[0] === 0x89 && tempBuffer[1] === 0x50 && tempBuffer[2] === 0x4E && tempBuffer[3] === 0x47 &&
        tempBuffer[4] === 0x0D && tempBuffer[5] === 0x0A && tempBuffer[6] === 0x1A && tempBuffer[7] === 0x0A;
      
      if (!isPNG) {
        console.error(`   ✗ Invalid PNG signature in base64 data: ${img.name}`);
        console.error(`   First 8 bytes:`, Array.from(tempBuffer.slice(0, 8)).map(b => '0x' + b.toString(16).padStart(2, '0')).join(' '));
        throw new Error(`Invalid PNG data for ${img.name}`);
      }
      
      // Step 2: Write buffer to PNG file
      const tempPngPath = join(tempImageDir, img.name);
      writeFileSync(tempPngPath, tempBuffer);
      console.log(`   ✓ Wrote PNG file: ${tempPngPath} (${tempBuffer.length} bytes)`);
      
      // Step 3: Read PNG file back as binary
      const pngBinary = readFileSync(tempPngPath);
      
      // Validate PNG signature after file round-trip
      const isPNGAfter = pngBinary.length >= 8 &&
        pngBinary[0] === 0x89 && pngBinary[1] === 0x50 && pngBinary[2] === 0x4E && pngBinary[3] === 0x47 &&
        pngBinary[4] === 0x0D && pngBinary[5] === 0x0A && pngBinary[6] === 0x1A && pngBinary[7] === 0x0A;
      
      if (!isPNGAfter) {
        console.error(`   ✗ PNG corrupted after file write: ${img.name}`);
        throw new Error(`PNG file corrupted for ${img.name}`);
      }
      
      // Map by image NAME for regular images, or 'thumbnail' for thumbnail
      if (img.url === 'thumbnail') {
        imageMap.set('thumbnail', pngBinary);
        console.log(`   ✓ Thumbnail validated from file: ${img.name} (${pngBinary.length} bytes)`);
      } else {
        imageMap.set(img.name, pngBinary);
        console.log(`   ✓ Image validated from file: ${img.name} (${pngBinary.length} bytes)`);
      }
    }
    
    // Generate output filename
    const timestamp = Date.now();
    const filename = `${moyFile.faceName || 'watchface'}_${timestamp}.moy`;
    const outputPath = join(TEMP_DIR, filename);
    
    // Generate the MOY file
    generateMoyFile(moyFile, imageMap, outputPath);
    
    // Extract images from generated MOY for verification
    const extractDir = join(TEMP_DIR, `extracted_${timestamp}`);
    mkdirSync(extractDir, { recursive: true });
    const extractedImages = extractImagesFromMoy(outputPath, extractDir);
    
    console.log(`✅ MOY file generated with ${extractedImages.length} images`);
    console.log(`   Extracted images saved to: ${extractDir}`);
    
    // Read the generated file and send as response
    const fileData = readFileSync(outputPath);
    const base64File = fileData.toString('base64');
    
    console.log(`📤 Sending MOY file: ${fileData.length} bytes\n`);
    
    res.json({
      success: true,
      filename,
      data: base64File,
      size: fileData.length,
      extractedImages: extractedImages.map((p: string) => p.replace(__dirname, ''))
    });
    
  } catch (error: any) {
    console.error('❌ Error generating MOY file:', error);
    res.status(500).json({
      success: false,
      error: error.message || 'Failed to generate MOY file'
    });
  }
});

/**
 * Health check endpoint
 */
app.get('/api/health', (req, res) => {
  res.json({ status: 'ok', service: 'MOY Generator Backend' });
});

/**
 * API Endpoint: Generate BIN file from MOY file
 * This endpoint takes a MOY file and converts it to a BIN file using MOYoung API
 */
app.post('/api/generate-bin', async (req, res) => {
  try {
    const { moyFilePath, moyBuffer, moyFileName } = req.body;
    console.log('Received BIN generation request with data:', { moyFilePath, moyBuffer: moyBuffer ? '[buffer data]' : null, moyFileName });
    
    if (!moyFilePath && !moyBuffer) {
      return res.status(400).json({
        success: false,
        error: 'Either moyFilePath or moyBuffer must be provided',
      });
    }
    
    console.log('\n📥 Received BIN generation request');

    
    
    let binPath: string;
    let binUrl: string;
    
    if (moyBuffer) {
      // Convert from buffer (base64)
      const fileName = moyFileName || `watchface_${Date.now()}.moy`;
      const buffer = Buffer.from(moyBuffer, 'base64');
      
      console.log(`   Converting from buffer: ${fileName} (${buffer.length} bytes)`);
      
      const result = await generateBinFromMoyBuffer(buffer, fileName, TEMP_DIR, TEMP_DIR);
      binPath = result.binPath;
      binUrl = result.binUrl;
    } else {
      // Convert from file path
      console.log(`   Converting from file: ${moyFilePath}`);
      
      const result = await generateBinFromMoy(moyFilePath, TEMP_DIR);
      binPath = result.binPath;
      binUrl = result.binUrl;
    }
    
    // Read the generated BIN file
    const binData = readFileSync(binPath);
    const base64Bin = binData.toString('base64');
    
    console.log(`📤 Sending BIN file: ${binData.length} bytes\n`);
    
    res.json({
      success: true,
      binPath,
      binUrl,
      data: base64Bin,
      size: binData.length,
    });
    
  } catch (error: any) {
    console.error('❌ Error generating BIN file:', error);
    res.status(500).json({
      success: false,
      error: error.message || 'Failed to generate BIN file',
    });
  }
});

/**
 * API Endpoint: Generate MOY and BIN files together
 * This endpoint generates both MOY and BIN files in one request
 */
app.post('/api/generate-moy-and-bin', async (req, res) => {
  try {
    const { moyFile, images, preview } = req.body as MoyGenerationRequest;
    
    if (!moyFile || !images) {
      return res.status(400).json({ 
        success: false, 
        error: 'Missing required data: moyFile and images' 
      });
    }
    
    console.log('\n📥 Received MOY + BIN generation request');
    console.log(`   Watch name: ${moyFile.faceName}`);
    console.log(`   Layer groups: ${moyFile.layerGroups?.length || 0}`);
    console.log(`   Images received: ${images.length}`);
    
    // Create temporary directory for PNG files
    const tempImageDir = join(TEMP_DIR, `images_${Date.now()}`);
    mkdirSync(tempImageDir, { recursive: true });
    
    // Convert base64 → PNG file → Binary buffer pipeline
    const imageMap = new Map<string, Buffer>();
    
    for (const img of images) {
      // Remove data URL prefix if present
      const base64Data = img.data.replace(/^data:image\/\w+;base64,/, '');
      
      // Step 1: Decode base64 to buffer
      const tempBuffer = Buffer.from(base64Data, 'base64');
      
      // Validate PNG signature before writing
      const isPNG = tempBuffer.length >= 8 &&
        tempBuffer[0] === 0x89 && tempBuffer[1] === 0x50 && tempBuffer[2] === 0x4E && tempBuffer[3] === 0x47 &&
        tempBuffer[4] === 0x0D && tempBuffer[5] === 0x0A && tempBuffer[6] === 0x1A && tempBuffer[7] === 0x0A;
      
      if (!isPNG) {
        console.error(`   ✗ Invalid PNG signature in base64 data: ${img.name}`);
        console.error(`   First 8 bytes:`, Array.from(tempBuffer.slice(0, 8)).map(b => '0x' + b.toString(16).padStart(2, '0')).join(' '));
        throw new Error(`Invalid PNG data for ${img.name}`);
      }
      
      // Step 2: Write buffer to PNG file
      const tempPngPath = join(tempImageDir, img.name);
      writeFileSync(tempPngPath, tempBuffer);
      console.log(`   ✓ Wrote PNG file: ${tempPngPath} (${tempBuffer.length} bytes)`);
      
      // Step 3: Read PNG file back as binary
      const pngBinary = readFileSync(tempPngPath);
      
      // Validate PNG signature after file round-trip
      const isPNGAfter = pngBinary.length >= 8 &&
        pngBinary[0] === 0x89 && pngBinary[1] === 0x50 && pngBinary[2] === 0x4E && pngBinary[3] === 0x47 &&
        pngBinary[4] === 0x0D && pngBinary[5] === 0x0A && pngBinary[6] === 0x1A && pngBinary[7] === 0x0A;
      
      if (!isPNGAfter) {
        console.error(`   ✗ PNG corrupted after file write: ${img.name}`);
        throw new Error(`PNG file corrupted for ${img.name}`);
      }
      
      // Map by image NAME for regular images, or 'thumbnail' for thumbnail
      if (img.url === 'thumbnail') {
        imageMap.set('thumbnail', pngBinary);
        console.log(`   ✓ Thumbnail validated from file: ${img.name} (${pngBinary.length} bytes)`);
      } else {
        imageMap.set(img.name, pngBinary);
        console.log(`   ✓ Image validated from file: ${img.name} (${pngBinary.length} bytes)`);
      }
    }
    
    // Generate output filename
    const timestamp = Date.now();
    const filename = `${moyFile.faceName || 'watchface'}_${timestamp}.moy`;
    const outputPath = join(TEMP_DIR, filename);
    
    // Generate the MOY file
    generateMoyFile(moyFile, imageMap, outputPath);
    
    // Extract images from generated MOY for verification
    const extractDir = join(TEMP_DIR, `extracted_${timestamp}`);
    mkdirSync(extractDir, { recursive: true });
    const extractedImages = extractImagesFromMoy(outputPath, extractDir);
    
    console.log(`✅ MOY file generated with ${extractedImages.length} images`);
    console.log(`   Extracted images saved to: ${extractDir}`);
    
    // Read the generated MOY file
    const moyData = readFileSync(outputPath);
    const base64Moy = moyData.toString('base64');
    
    console.log(`📤 MOY file size: ${moyData.length} bytes`);
    
    // Now generate BIN file from the MOY file
    console.log('\n🔄 Starting BIN file generation...');
    
    try {
      const { binPath, binUrl } = await generateBinFromMoy(outputPath, TEMP_DIR);
      
      // Read the generated BIN file
      const binData = readFileSync(binPath);
      const base64Bin = binData.toString('base64');
      
      console.log(`📤 BIN file size: ${binData.length} bytes\n`);
      
      res.json({
        success: true,
        moy: {
          filename,
          data: base64Moy,
          size: moyData.length,
          extractedImages: extractedImages.map((p: string) => p.replace(__dirname, ''))
        },
        bin: {
          filename: filename.replace('.moy', '.bin'),
          data: base64Bin,
          size: binData.length,
          url: binUrl,
          path: binPath
        }
      });
    } catch (binError: any) {
      console.error('⚠️ BIN generation failed, but MOY file was created successfully');
      
      // Return MOY file with bin error
      res.json({
        success: true,
        moy: {
          filename,
          data: base64Moy,
          size: moyData.length,
          extractedImages: extractedImages.map((p: string) => p.replace(__dirname, ''))
        },
        bin: {
          error: binError.message || 'Failed to generate BIN file',
          success: false
        }
      });
    }
    
  } catch (error: any) {
    console.error('❌ Error generating files:', error);
    res.status(500).json({
      success: false,
      error: error.message || 'Failed to generate files'
    });
  }
});

// Start server
const server = app.listen(PORT, '127.0.0.1', () => {
  console.log('╔════════════════════════════════════════╗');
  console.log('║   MOY Generator Backend Server         ║');
  console.log('╚════════════════════════════════════════╝');
  console.log(`\n🚀 Server running at http://localhost:${PORT}`);
  console.log(`   Generate MOY: POST http://localhost:${PORT}/api/generate-moy\n`);
});
