import express, { Request, Response } from 'express';
import multer from 'multer';
import cors from 'cors';
import * as fs from 'fs-extra';
import * as path from 'path';
import dotenv from 'dotenv';
import { JieLiConverter } from './converter';
import { BinGenerator } from './binGenerator';
import { WatchFaceJSON, ConversionResult } from './types';

// Load environment variables
dotenv.config();

const app = express();
const PORT = process.env.PORT || 3000;

// Paths from environment variables
const TEMP_FOLDER = process.env.TEMP_FOLDER_PATH || './temp';
const OUTPUT_FOLDER = process.env.OUTPUT_FOLDER_PATH || './output';
const WATCHDB_GENERATOR_PATH = process.env.WATCHDB_GENERATOR_PATH || '';
const BIN_GENERATOR_PATH = process.env.JIELI_BIN_GENERATOR_PATH || '';

// Middleware
app.use(cors());
app.use(express.json({ limit: '50mb' }));
app.use(express.urlencoded({ extended: true, limit: '50mb' }));

// Configure multer for file uploads
const storage = multer.diskStorage({
  destination: async (req, file, cb) => {
    await fs.ensureDir(TEMP_FOLDER);
    cb(null, TEMP_FOLDER);
  },
  filename: (req, file, cb) => {
    const uniqueSuffix = Date.now() + '-' + Math.round(Math.random() * 1E9);
    cb(null, file.fieldname + '-' + uniqueSuffix + path.extname(file.originalname));
  }
});

const upload = multer({ 
  storage,
  limits: { fileSize: 50 * 1024 * 1024 } // 50MB limit
});

// Upload multiple files (watchface JSON + asset images)
const uploadMultiple = multer({
  storage: multer.memoryStorage(),
  limits: { fileSize: 50 * 1024 * 1024 }
}).fields([
  { name: 'watchface', maxCount: 1 },
  { name: 'assets', maxCount: 100 }
]);

// Initialize services
const converter = new JieLiConverter(TEMP_FOLDER);
const binGenerator = new BinGenerator(WATCHDB_GENERATOR_PATH, BIN_GENERATOR_PATH, OUTPUT_FOLDER);

/**
 * Helper function to resolve asset references in watchface data
 * Converts Source/ImageSource asset paths to ImageData base64
 */
async function resolveAssetReferences(watchFaceData: WatchFaceJSON, assetMap: Map<string, Buffer>): Promise<void> {
  if (!watchFaceData.WatchStyles || watchFaceData.WatchStyles.length === 0) {
    return;
  }

  for (const style of watchFaceData.WatchStyles) {
    const components = style.DragBindBases || [];
    
    for (const component of components) {
      // Handle single image Source field
      if (component.Source && component.Source.startsWith('assets/')) {
        const buffer = assetMap.get(component.Source);
        if (buffer) {
          component.ImageData = `data:image/png;base64,${buffer.toString('base64')}`;
        }
      }
      
      // Handle ImageSource array
      if (component.ImageSource && Array.isArray(component.ImageSource)) {
        const base64Images: string[] = [];
        for (const imgPath of component.ImageSource) {
          if (imgPath.startsWith('assets/')) {
            const buffer = assetMap.get(imgPath);
            if (buffer) {
              base64Images.push(`data:image/png;base64,${buffer.toString('base64')}`);
            }
          }
        }
        if (base64Images.length > 0) {
          component.ImageData = base64Images;
        }
      }
      
      // Handle EmptySource
      if (component.EmptySource && component.EmptySource.startsWith('assets/')) {
        const buffer = assetMap.get(component.EmptySource);
        if (buffer) {
          // Append to ImageData array if it exists
          if (Array.isArray(component.ImageData)) {
            component.ImageData.push(`data:image/png;base64,${buffer.toString('base64')}`);
          } else if (component.ImageData) {
            component.ImageData = [component.ImageData, `data:image/png;base64,${buffer.toString('base64')}`];
          } else {
            component.ImageData = `data:image/png;base64,${buffer.toString('base64')}`;
          }
        }
      }
    }
  }
}

/**
 * Health check endpoint
 */
app.get('/api/health', (req: Request, res: Response) => {
  res.json({
    status: 'ok',
    service: 'JieLi Middleware',
    version: '1.0.0',
    timestamp: new Date().toISOString()
  });
});

/**
 * Main conversion endpoint
 * POST /api/jieli/convert
 * Body: FormData with 'watchface' JSON file and 'assets' image files, or JSON body
 */
app.post('/api/jieli/convert', uploadMultiple, async (req: Request, res: Response) => {
  let projectPath: string | null = null;
  
  try {
    let watchFaceData: WatchFaceJSON;
    const assetMap = new Map<string, Buffer>();

    console.log('Request received. Files:', req.files ? 'Yes' : 'No', 'Body:', req.body ? 'Yes' : 'No');

    // Handle multipart form data with files
    if (req.files && typeof req.files === 'object' && !Array.isArray(req.files)) {
      const files = req.files as { [fieldname: string]: Express.Multer.File[] };
      
      console.log('Multipart upload detected. Fields:', Object.keys(files));
      
      // Get watchface JSON
      if (files.watchface && files.watchface[0]) {
        const fileContent = files.watchface[0].buffer.toString('utf-8');
        watchFaceData = JSON.parse(fileContent);
      } else {
        res.status(400).json({
          success: false,
          error: 'No watchface JSON file provided in multipart upload.'
        });
        return;
      }
      
      // Map asset files
      if (files.assets) {
        console.log(`Received ${files.assets.length} asset files`);
        for (const assetFile of files.assets) {
          // Use originalname as the asset path (e.g., "assets/image_001.png")
          assetMap.set(assetFile.originalname, assetFile.buffer);
          console.log(`Mapped asset: ${assetFile.originalname}`);
        }
      }
      
      // Resolve asset references to base64
      console.log('Resolving asset references...');
      await resolveAssetReferences(watchFaceData, assetMap);
    }
    // Handle single file upload (legacy)
    else if (req.file) {
      console.log('Single file upload (legacy)');
      const fileContent = await fs.readFile(req.file.path, 'utf-8');
      watchFaceData = JSON.parse(fileContent);
      await fs.remove(req.file.path);
    }
    // Handle JSON body
    else if (req.body.watchFaceData) {
      console.log('JSON body detected (no asset files provided)');
      watchFaceData = typeof req.body.watchFaceData === 'string' 
        ? JSON.parse(req.body.watchFaceData)
        : req.body.watchFaceData;
      
      // Check if ImageData is provided at top level (frontend workaround)
      if (req.body.ImageData || (watchFaceData as any).ImageData) {
        const imageDataMap = req.body.ImageData || (watchFaceData as any).ImageData;
        console.log('Found ImageData map at top level, resolving to components...');
        
        // Map ImageData to components
        if (watchFaceData.WatchStyles && watchFaceData.WatchStyles.length > 0) {
          for (const style of watchFaceData.WatchStyles) {
            const components = style.DragBindBases || [];
            
            for (const component of components) {
              // Map Source field
              if (component.Source && component.Source.startsWith('assets/')) {
                const imageData = imageDataMap[component.Source];
                if (imageData) {
                  component.ImageData = imageData;
                  console.log(`Mapped ${component.Source} to ImageData`);
                }
              }
              
              // Map ImageSource array
              if (component.ImageSource && Array.isArray(component.ImageSource)) {
                const base64Images: string[] = [];
                for (const imgPath of component.ImageSource) {
                  if (imgPath.startsWith('assets/')) {
                    const imageData = imageDataMap[imgPath];
                    if (imageData) {
                      base64Images.push(imageData);
                    }
                  }
                }
                if (base64Images.length > 0) {
                  component.ImageData = base64Images;
                  console.log(`Mapped ${base64Images.length} images for ${component.DragName}`);
                }
              }
            }
          }
        }
      }
    } else {
      res.status(400).json({
        success: false,
        error: 'No watch face data provided. Send either a file or JSON in request body.'
      });
      return;
    }

    console.log('Processing watch face:', watchFaceData.WatchName);

    // Step 1: Convert JSON to JieLi folder structure
    console.log('Converting to JieLi folder structure...');
    projectPath = await converter.convertToJieLiFormat(watchFaceData);
    console.log('Conversion complete:', projectPath);

    // Step 2: Generate bin file
    console.log('Generating bin file...');
    
    // For development, use mock bin generation
    // In production, replace with: binGenerator.generateBin(projectPath)
    // const binFilePath = await binGenerator.createMockBinFile(projectPath);
    const binFilePath = await binGenerator.generateBin(projectPath);
    
    console.log('Bin file generated:', binFilePath);

    // Validate bin file
    const isValid = await binGenerator.validateBinFile(binFilePath);
    if (!isValid) {
      throw new Error('Generated bin file validation failed');
    }

    // Step 3: Send bin file as download
    const fileName = `${watchFaceData.WatchName || 'watchface'}.bin`;
    
    res.setHeader('Content-Disposition', `attachment; filename="${fileName}"`);
    res.setHeader('Content-Type', 'application/octet-stream');
    
    // Stream the file
    const fileStream = fs.createReadStream(binFilePath);
    fileStream.pipe(res);

    // Don't cleanup - keep temp folders for inspection
    // fileStream.on('end', async () => {
    //   try {
    //     if (projectPath) {
    //       await converter.cleanup(projectPath);
    //     }
    //   } catch (error) {
    //     console.error('Cleanup error:', error);
    //   }
    // });

  } catch (error: any) {
    console.error('Conversion error:', error);
    
    // Don't cleanup on error - keep temp folders for debugging
    // if (projectPath) {
    //   await converter.cleanup(projectPath).catch(console.error);
    // }

    res.status(500).json({
      success: false,
      error: error.message || 'Internal server error',
      details: process.env.NODE_ENV === 'development' ? error.stack : undefined
    });
  }
});

/**
 * Get conversion status/info
 * GET /api/jieli/info
 */
app.get('/api/jieli/info', (req: Request, res: Response) => {
  res.json({
    service: 'JieLi Middleware Converter',
    version: '1.0.0',
    capabilities: {
      inputFormat: 'watchFace.json (ZhouHai format)',
      outputFormat: 'JieLi folder structure → .bin file',
      supportedComponents: [
        'Background',
        'Time (Hour, Minute, Second)',
        'Date (Month, Day, Week)',
        'Battery',
        'Steps',
        'Heart Rate',
        'Calories',
        'Sleep',
        'Analog Clock Hands'
      ]
    },
    binGenerator: {
      available: fs.existsSync(BIN_GENERATOR_PATH),
      path: BIN_GENERATOR_PATH
    }
  });
});

/**
 * Test endpoint with sample data
 * GET /api/jieli/test
 */
app.get('/api/jieli/test', async (req: Request, res: Response) => {
  try {
    // Create a minimal test watch face
    const testWatchFace: WatchFaceJSON = {
      WatchName: 'Test Watch Face',
      Width: 390,
      Height: 450,
      ThumbnailWidth: 200,
      ThumbnailHeight: 230,
      Corner: 0,
      IsAlbum: false,
      FolderName: 'test_watch',
      WatchStyles: [{
        StyleName: 'default',
        DragBindBases: [],
        TemplateBinds: [],
        ScreenType: 0,
        Zh: {
          Width: 390,
          Height: 450,
          Item: {
            KwhNum: 0,
            StepNum: 5000,
            HeartRateNum: 72,
            CalorieNum: 300,
            CurrentDateTime: new Date().toISOString(),
            StrengthNum: 0,
            IsOpen: false
          },
          DragBindBases: null
        }
      }]
    };

    const projectPath = await converter.convertToJieLiFormat(testWatchFace);
    
    res.json({
      success: true,
      message: 'Test conversion successful',
      projectPath,
      files: await fs.readdir(projectPath)
    });

    // Cleanup
    setTimeout(() => converter.cleanup(projectPath), 5000);
    
  } catch (error: any) {
    res.status(500).json({
      success: false,
      error: error.message
    });
  }
});

// Error handling middleware
app.use((err: any, req: Request, res: Response, next: any) => {
  console.error('Unhandled error:', err);
  res.status(500).json({
    success: false,
    error: 'Internal server error',
    message: err.message
  });
});

// Start server
app.listen(PORT, async () => {
  // Ensure directories exist
  await fs.ensureDir(TEMP_FOLDER);
  await fs.ensureDir(OUTPUT_FOLDER);

  console.log('='.repeat(60));
  console.log('🚀 JieLi Middleware Server Started');
  console.log('='.repeat(60));
  console.log(`📡 Server running on: http://localhost:${PORT}`);
  console.log(`📁 Temp folder: ${TEMP_FOLDER}`);
  console.log(`📁 Output folder: ${OUTPUT_FOLDER}`);
  console.log(`🔧 Bin generator: ${BIN_GENERATOR_PATH || 'Not configured'}`);
  console.log('='.repeat(60));
  console.log('\nAvailable endpoints:');
  console.log(`  GET  /api/health          - Health check`);
  console.log(`  GET  /api/jieli/info      - Service information`);
  console.log(`  GET  /api/jieli/test      - Test conversion`);
  console.log(`  POST /api/jieli/convert   - Convert watchFace.json to bin`);
  console.log('='.repeat(60));
});

export default app;
