/**
 * MOY File Image Extractor
 * Extracts PNG images from MOY file for validation
 */

import { readFileSync, writeFileSync, mkdirSync, existsSync } from 'fs';
import { join, basename } from 'path';

/**
 * Extract images from MOY file
 */
function extractImagesFromMoy(moyFilePath: string, outputDir: string): void {
  console.log('=== MOY Image Extractor ===\n');
  console.log(`Reading MOY file: ${moyFilePath}`);
  
  // Read the entire MOY file as buffer
  const moyBuffer = readFileSync(moyFilePath);
  console.log(`Total file size: ${moyBuffer.length} bytes\n`);
  
  // Create output directory
  if (!existsSync(outputDir)) {
    mkdirSync(outputDir, { recursive: true });
  }
  
  // Find the first MOYEND marker (end of JSON)
  const moyendMarker = Buffer.from('MOYEND', 'binary');
  const firstMoyendIndex = moyBuffer.indexOf(moyendMarker);
  
  if (firstMoyendIndex === -1) {
    throw new Error('Could not find first MOYEND marker');
  }
  
  console.log(`JSON section ends at: ${firstMoyendIndex}`);
  console.log(`JSON length: ${firstMoyendIndex} bytes\n`);
  
  // Extract and parse JSON
  const jsonBuffer = moyBuffer.subarray(0, firstMoyendIndex);
  const jsonString = jsonBuffer.toString('binary');
  const moyData = JSON.parse(jsonString);
  
  console.log(`Watch name: ${moyData.faceName}`);
  console.log(`Layer groups: ${moyData.layerGroups?.length || 0}\n`);
  
  // Start searching for images after first MOYEND
  let currentPos = firstMoyendIndex + moyendMarker.length;
  const imgendMarker = Buffer.from('IMGEND', 'binary');
  let imageCount = 0;
  
  console.log('=== Extracting Images ===\n');
  
  while (currentPos < moyBuffer.length) {
    // Find next IMGEND marker
    const imgendIndex = moyBuffer.indexOf(imgendMarker, currentPos);
    
    if (imgendIndex === -1) {
      // Check if we hit the final MOYEND
      const finalMoyendIndex = moyBuffer.indexOf(moyendMarker, currentPos);
      if (finalMoyendIndex !== -1) {
        console.log(`\nReached final MOYEND at position ${finalMoyendIndex}`);
        break;
      }
      console.log(`\nNo more IMGEND markers found at position ${currentPos}`);
      break;
    }
    
    // Extract image data between current position and IMGEND
    const imageData = moyBuffer.subarray(currentPos, imgendIndex);
    
    // Validate PNG signature
    const isPNG = imageData.length >= 8 &&
      imageData[0] === 0x89 && imageData[1] === 0x50 && imageData[2] === 0x4E && imageData[3] === 0x47 &&
      imageData[4] === 0x0D && imageData[5] === 0x0A && imageData[6] === 0x1A && imageData[7] === 0x0A;
    
    imageCount++;
    const imageName = `image_${imageCount.toString().padStart(3, '0')}.png`;
    const outputPath = join(outputDir, imageName);
    
    console.log(`Image ${imageCount}:`);
    console.log(`  Position: ${currentPos} - ${imgendIndex}`);
    console.log(`  Size: ${imageData.length} bytes`);
    console.log(`  PNG signature: ${isPNG ? '✓ Valid' : '✗ Invalid'}`);
    
    if (!isPNG && imageData.length > 0) {
      console.log(`  First 16 bytes:`, Array.from(imageData.slice(0, 16)).map(b => '0x' + b.toString(16).padStart(2, '0')).join(' '));
    }
    
    // Write image to file
    writeFileSync(outputPath, imageData);
    console.log(`  Saved: ${outputPath}\n`);
    
    // Move to position after IMGEND marker
    currentPos = imgendIndex + imgendMarker.length;
  }
  
  console.log(`\n=== Extraction Complete ===`);
  console.log(`Total images extracted: ${imageCount}`);
  console.log(`Output directory: ${outputDir}`);
}

// Main execution
if (process.argv.length < 3) {
  console.log('Usage: node extract-moy-images.ts <path-to-moy-file> [output-directory]');
  console.log('Example: node extract-moy-images.ts temp/MyWatch_1234567890.moy extracted');
  process.exit(1);
}

const moyFilePath = process.argv[2];
const outputDir = process.argv[3] || join(process.cwd(), 'extracted', basename(moyFilePath, '.moy'));

try {
  extractImagesFromMoy(moyFilePath, outputDir);
} catch (error: any) {
  console.error('\n❌ Error:', error.message);
  process.exit(1);
}
