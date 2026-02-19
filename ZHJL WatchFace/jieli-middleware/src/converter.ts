import * as fs from 'fs-extra';
import * as path from 'path';
import sharp from 'sharp';
import { WatchFaceJSON, DragBindComponent, JieLiFolderConfig, ConversionResult } from './types';
import { ComponentMapper } from './componentMapper';

/**
 * Converts watchFace.json to JieLi folder structure
 */
export class JieLiConverter {
  private tempFolderPath: string;
  private createdFolderCodes: Set<string> = new Set();
  
  constructor(tempFolderPath: string) {
    this.tempFolderPath = tempFolderPath;
  }

  /**
   * Main conversion method
   * @param watchFaceData - Parsed watchFace.json data
   * @returns Path to the generated folder
   */
  async convertToJieLiFormat(watchFaceData: WatchFaceJSON): Promise<string> {
    // Reset the tracking set for each conversion
    this.createdFolderCodes.clear();
    
    // Clean up old temp folders to prevent duplicate detection issues
    // Keep only the most recent folder (if any) and remove older ones
    try {
      const tempContents = await fs.readdir(this.tempFolderPath);
      for (const item of tempContents) {
        const itemPath = path.join(this.tempFolderPath, item);
        const stats = await fs.stat(itemPath);
        if (stats.isDirectory()) {
          await fs.remove(itemPath);
        }
      }
    } catch (error) {
      console.warn('Failed to clean temp folders:', error);
    }
    
    // Create project folder with naming convention
    const projectFolderName = this.generateProjectFolderName(watchFaceData);
    const projectPath = path.join(this.tempFolderPath, projectFolderName);
    
    // Ensure temp directory exists
    await fs.ensureDir(projectPath);
    
    // Collect folder codes that will be used by actual components
    const usedCodes = new Set<string>();
    if (watchFaceData.WatchStyles && watchFaceData.WatchStyles.length > 0) {
      const style = watchFaceData.WatchStyles[0];
      const components = style.DragBindBases || [];
      
      for (const component of components) {
        const codes = this.getComponentFolderCodes(component);
        codes.forEach(code => usedCodes.add(code));
      }
    }
    
    // Create all standard empty folders first (for Windows tool compatibility)
    // Skip codes that will be populated by actual components to avoid duplicates
    await this.createStandardFolderStructure(projectPath, usedCodes);
    
    // Process each component in the first watch style
    if (watchFaceData.WatchStyles && watchFaceData.WatchStyles.length > 0) {
      const style = watchFaceData.WatchStyles[0];
      const components = style.DragBindBases || [];
      
      // Group components by type for processing
      const groupedComponents = this.groupComponents(components);
      
      // Create folders for each component
      for (const component of groupedComponents) {
        await this.createComponentFolders(component, projectPath);
      }
    }
    
    // Create auxiliary files folder
    await this.createAuxiliaryFiles(watchFaceData, projectPath);
    
    // Create metadata files
    await this.createMetadataFiles(watchFaceData, projectPath);
    
    return projectPath;
  }

  /**
   * Generate project folder name following JieLi convention
   * Format: 形状_宽度x高度_类型_系列_作者_主编号_子编号
   */
  private generateProjectFolderName(data: WatchFaceJSON): string {
    const width = data.Width;
    const height = data.Height;
    
    // Determine shape based on dimensions
    const shape = width === height ? '圆形' : '方形';
    
    // Extract or default values
    const watchName = data.WatchName || 'MyWatch';
    const timestamp = new Date().toISOString().replace(/[:.]/g, '-').split('T')[0];
    
    // Format: 方形_390x450_普通_201#简约_Author_001_00
    return `方形_240x280_普通_201#简约_梁韵诗_002_00`;
  }

  /**
   * Get all folder codes that will be created for a component
   */
  private getComponentFolderCodes(component: DragBindComponent): string[] {
    let imageArray = component.ImageSource;
    if (!imageArray || imageArray.length === 0) {
      if (typeof component.ImageData === 'string') {
        imageArray = component.ImageData.split(',').map(s => s.trim());
      } else if (Array.isArray(component.ImageData)) {
        imageArray = component.ImageData;
      }
    }
    
    // Check if this is a specific digit component (Tens/Ones) - if so, treat as single digit
    const itemName = component.ItemName || '';
    const isSingleDigit = itemName.includes('Tens') || itemName.includes('Ones') || 
                          itemName.includes('(Tens)') || itemName.includes('(Ones)') ||
                          itemName.includes('Hundreds') || itemName.includes('(Hundreds)') ||
                          itemName.includes('Thousands') || itemName.includes('(Thousands)') ||
                          itemName.includes('Single') || itemName.includes('(Single)') ||
                          itemName.includes('十') || itemName.includes('个') ||
                          itemName.includes('百') || itemName.includes('千') || itemName.includes('万');
    
    const isMultiDigit = !isSingleDigit && 
                         (component.$type.includes('DragBindNums') || 
                          component.$type.includes('DragBindNormalDateTime')) &&
                         (imageArray?.length === 10 || imageArray?.length === 11);
    
    if (isMultiDigit) {
      const configs = ComponentMapper.splitMultiDigitComponent(component);
      return configs.map(config => config.code);
    } else {
      const config = ComponentMapper.toJieLiFolderConfig(component);
      return [config.code];
    }
  }

  /**
   * Create all standard JieLi component folders (empty with 0_0_0 coordinates)
   * This ensures Windows tool compatibility by providing the expected folder structure
   */
  private async createStandardFolderStructure(projectPath: string, excludeCodes: Set<string>): Promise<void> {
    const standardFolders = [
      // Main components (01xx)
      '0101_主要#1(图片)#1背景_0_0_0',
      '0102_主要#1(图片)#2蓝牙_10_167_164',
      
      // Battery components (02xx)
      '0201_电量#1(图片)#电量进度条_10_12_194',
      '0202_电量#2(文字)#文字_10_47_121',
      '0203_电量#1(图片)#图片_10_15_185',
      '0204_电量#1(图片)#电量百分号_0_55_158',
      '0205_电量#3(数字)#3(百)_10_13_158',
      '0206_电量#3(数字)#4(十)_0_27_158',
      '0207_电量#3(数字)#5(个)_0_41_158',
      '0208_电量#1(图片)#电量背景_10_285_11',
      '0209_电量#1(图片)#电量图片短_10_287_14',
      '0210_电量#1(图片)#电量图片长_10_288_13',
      
      // Generic date components (03xx)
      '0301_通用日期#3(数字)#2(月)#4(十)_10_14_11',
      '0302_通用日期#3(数字)#2(月)#5(个)_0_30_11',
      '0303_通用日期#1(图片)#分隔_10_46_11',
      '0304_通用日期#3(数字)#3(日)#4(十)_10_58_11',
      '0305_通用日期#3(数字)#3(日)#5(个)_0_74_11',
      '0306_通用日期#3(数字)#1(年)#1(千)_0_0_0',
      '0307_通用日期#3(数字)#1(年)#2(百)_0_0_0',
      '0308_通用日期#3(数字)#1(年)#3(十)_0_0_0',
      '0309_通用日期#3(数字)#1(年)#4(个)_0_0_0',
      
      // Chinese date components (04xx)
      '0401_中文日期#3(数字)#2(月)#4(十)_0_0_0',
      '0402_中文日期#3(数字)#2(月)#5(个)_0_0_0',
      '0403_中文日期#3(文字)#月_0_0_0',
      '0404_中文日期#3(数字)#3(日)#4(十)_0_0_0',
      '0405_中文日期#3(数字)#3(日)#5(个)_0_0_0',
      '0406_中文日期#2(文字)#日_0_0_0',
      
      // Other date components (05xx)
      '0501_其他日期#2(文字)#4(星期)_10_158_11',
      '0502_其他日期#2(文字)#4(上下午)_10_104_11',
      
      // Time components (06xx)
      '0601_时间#3(数字)#1(时)#4(十)_10_16_70',
      '0602_时间#3(数字)#1(时)#5(个)_10_70_70',
      '0603_时间#1(图片)#分隔1_10_134_21',
      '0604_时间#3(数字)#2(分)#4(十)_10_16_239',
      '0605_时间#3(数字)#2(分)#5(个)_10_70_239',
      '0606_时间#1(图片)#分隔2_10_272_54',
      '0607_时间#3(数字)#3(秒)#5(十)_10_283_54',
      '0608_时间#3(数字)#3(秒)#5(个)_10_319_54',
      
      // Steps components (07xx)
      '0701_步数#1(图片)#进度条_10_12_257',
      '0702_步数#2(文字)#文字_10_40_224',
      '0703_步数#2(文字)#单位_10_33_368',
      '0705_步数#3(数字)#数值#1(万)_10_24_430',
      '0706_步数#3(数字)#数值#2(千)_0_40_430',
      '0707_步数#3(数字)#数值#3(百)_0_56_430',
      '0708_步数#3(数字)#数值#4(十)_0_72_430',
      '0709_步数#3(数字)#数值#5(个)_0_88_430',
      '0710_步数#1(图片)#普通_10_48_276',
      
      // Heart rate components (08xx)
      '0801_心率#2(文字)#文字_10_159_223',
      '0802_心率#2(文字)#单位_10_158_368',
      '0803_心率#1(图片)#无数据_0_157_303',
      '0804_心率#3(数字)#数值#3(百)_10_157_303',
      '0805_心率#3(数字)#数值#4(十)_0_171_303',
      '0806_心率#3(数字)#数值#5(个)_0_185_303',
      '0807_心率#1(图片)#普通_10_11_141',
      
      // Calories components (09xx)
      '0901_卡路里#1(图片)#进度条_10_251_257',
      '0902_卡路里#2(文字)#文字_10_259_223',
      '0903_卡路里#2(文字)#单位_10_279_367',
      '0905_卡路里#3(数字)#数值#2(千)_10_140_430',
      '0906_卡路里#3(数字)#数值#3(百)_0_156_430',
      '0907_卡路里#3(数字)#数值#4(十)_0_172_430',
      '0908_卡路里#3(数字)#数值#5(个)_0_188_430',
      '0909_卡路里#1(图片)#普通_10_289_274',
      
      // Sleep components (10xx)
      '1001_睡眠#1(图片)#进度条_10_131_262',
      '1002_睡眠#2(文字)#单位_10_154_235',
      '1003_睡眠#1(图片)#无数据_0_159_308',
      '1004_睡眠#3(数字)#数值#3(百)_10_159_308',
      '1005_睡眠#3(数字)#数值#4(十)_0_173_308',
      '1006_睡眠#3(数字)#数值#5(个)_0_187_308',
      '1007_睡眠#1(图片)#普通_10_170_281',
      
      // Effects (11xx)
      '1101_效果#1(图片)#效果1_0_0_0',
      
      // Click areas (12xx)
      '1201_点击区域#1(图片)#步数_0_0_0',
      '1202_点击区域#1(图片)#心率_0_0_0',
      '1203_点击区域#1(图片)#卡路_0_0_0',
      '1204_点击区域#1(图片)#睡眠_0_0_0',
      '1205_点击区域#1(图片)#小爱_0_0_0',
      '1206_点击区域#1(图片)#支付_0_0_0',
      
      // Pointers (13xx)
      '1301_指针#1(图片)#时针_0_0_0',
      '1302_指针#1(图片)#分针_0_0_0',
      '1303_指针#1(图片)#秒针_0_0_0',
      
      // Progress bars - Battery (14xx)
      '1401_进度条#电量#1(图片)#BMP_0_0_0',
      '1402_进度条#电量#1(图片)#PNG_0_0_0',
      '1403_进度条#电量#1(图片)#小圆_0_0_0',
      
      // Progress bars - Steps (15xx)
      '1501_进度条#步数#1(图片)#BMP_0_0_0',
      '1502_进度条#步数#1(图片)#PNG_0_0_0',
      '1503_进度条#步数#1(图片)#小圆_0_0_0',
      
      // Progress bars - Calories (16xx)
      '1601_进度条#卡路里#1(图片)#BMP_0_0_0',
      '1602_进度条#卡路里#1(图片)#PNG_0_0_0',
      '1603_进度条#卡路里#1(图片)#小圆_0_0_0',
      
      // Progress bars - Sleep (17xx)
      '1701_进度条#睡眠#1(图片)#BMP_0_0_0',
      '1702_进度条#睡眠#1(图片)#PNG_0_0_0',
      '1703_进度条#睡眠#1(图片)#小圆_0_0_0'
    ];
    
    // Filter out folders whose codes will be used by actual components
    let createdCount = 0;
    for (const folderName of standardFolders) {
      const code = folderName.split('_')[0];
      if (excludeCodes.has(code)) {
        console.log(`[Converter] Skipping standard folder ${code} - will be populated by component`);
        continue;
      }
      const folderPath = path.join(projectPath, folderName);
      await fs.ensureDir(folderPath);
      
      // Create NoDye.txt marker in empty folders (required by Windows tool)
      await fs.writeFile(path.join(folderPath, 'NoDye.txt'), '');
      createdCount++;
    }
    
    console.log(`[Converter] Created ${createdCount}/${standardFolders.length} standard empty folders (skipped ${excludeCodes.size} that will be populated)`);
  }

  /**
   * Group and order components for proper JieLi structure
   */
  private groupComponents(components: DragBindComponent[]): DragBindComponent[] {
    // Sort by component type priority (background first, then time, health metrics, etc.)
    const priorityOrder: Record<string, number> = {
      'Background Image': 1,
      'Hour': 10,
      'Minute': 11,
      'Second': 12,
      'Month': 20,
      'Day': 21,
      'Week': 22,
      'Battery': 30,
      'Steps': 40,
      'Heart Rate': 50,
      'Calories': 60,
      'Sleep': 70,
      'Pointer': 100
    };
    
    return components.sort((a, b) => {
      const priorityA = priorityOrder[a.ItemName] || 999;
      const priorityB = priorityOrder[b.ItemName] || 999;
      return priorityA - priorityB;
    });
  }

  /**
   * Create component folders with images
   */
  private async createComponentFolders(component: DragBindComponent, projectPath: string): Promise<void> {
    // Check if this is a multi-digit component that needs splitting
    // Check both ImageSource and ImageData for the image array
    let imageArray = component.ImageSource;
    if (!imageArray || imageArray.length === 0) {
      // Try ImageData field (could be string with comma-separated values or array)
      if (typeof component.ImageData === 'string') {
        imageArray = component.ImageData.split(',').map(s => s.trim());
      } else if (Array.isArray(component.ImageData)) {
        imageArray = component.ImageData;
      }
    }
    
    // Check if this is a specific digit component (Tens/Ones) - if so, treat as single digit
    const itemName = component.ItemName || '';
    const isSingleDigit = itemName.includes('Tens') || itemName.includes('Ones') || 
                          itemName.includes('(Tens)') || itemName.includes('(Ones)') ||
                          itemName.includes('Hundreds') || itemName.includes('(Hundreds)') ||
                          itemName.includes('Thousands') || itemName.includes('(Thousands)') ||
                          itemName.includes('Single') || itemName.includes('(Single)') ||
                          itemName.includes('十') || itemName.includes('个') ||
                          itemName.includes('百') || itemName.includes('千') || itemName.includes('万');
    
    const isMultiDigit = !isSingleDigit && 
                         (component.$type.includes('DragBindNums') || 
                          component.$type.includes('DragBindNormalDateTime')) &&
                         (imageArray?.length === 10 || imageArray?.length === 11);
    
    console.log(`[Converter] ============================`);
    console.log(`[Converter] Component ItemName: "${component.ItemName}"`);
    console.log(`[Converter] isSingleDigit=${isSingleDigit}, isMultiDigit=${isMultiDigit}`);
    console.log(`[Converter] $type=${component.$type}`);
    console.log(`[Converter] imageArray.length=${imageArray?.length}`);
    console.log(`[Converter] Position: x=${component.Left}, y=${component.Top}`);
    console.log(`[Converter] ImageSource:`, component.ImageSource?.slice(0, 3));
    console.log(`[Converter] ImageData type:`, typeof component.ImageData);
    if (typeof component.ImageData === 'string') {
      console.log(`[Converter] ImageData (string) length:`, component.ImageData.length);
    } else if (Array.isArray(component.ImageData)) {
      console.log(`[Converter] ImageData (array) length:`, component.ImageData.length);
    }
    
    let configs: JieLiFolderConfig[];
    
    if (isMultiDigit) {
      // Split multi-digit components into separate folders
      console.log(`[Converter] Splitting ${component.ItemName} into multiple digit folders...`);
      configs = ComponentMapper.splitMultiDigitComponent(component);
    } else {
      // Single component
      configs = [ComponentMapper.toJieLiFolderConfig(component)];
    }
    
    // Create folder for each configuration
    for (const config of configs) {
      // Check if this code has already been created in THIS conversion
      if (this.createdFolderCodes.has(config.code)) {
        console.warn(`⚠️  Skipping duplicate folder code: ${config.code} (${component.ItemName})`);
        continue;
      }
      
      // Mark this code as created
      this.createdFolderCodes.add(config.code);
      
      const folderName = ComponentMapper.generateFolderName(config);
      const folderPath = path.join(projectPath, folderName);
      
      console.log(`[Converter] Creating folder: ${folderName}, images count: ${config.images?.length}`);
      
      // Folder may already exist from standard structure creation
      // If not, create it
      await fs.ensureDir(folderPath);
      
      // Save images in the folder (will populate empty folders or create new ones)
      await this.saveComponentImages(config, folderPath, component);
      
      // Create NoDye.txt marker if needed
      await this.createNoDyeMarker(folderPath);
    }
  }

  /**
   * Save component images to folder
   */
  private async saveComponentImages(
    config: JieLiFolderConfig, 
    folderPath: string, 
    component: DragBindComponent
  ): Promise<void> {
    const images = config.images;
    
    if (!images || images.length === 0) {
      console.warn(`No images for component: ${component.ItemName}`);
      return;
    }
    
    // For pointer/background images, save as single file
    if (component.$type.includes('DragBindPoint') || component.$type.includes('DragBindImage')) {
      const imagePath = images[0];
      const fileName = component.$type.includes('DragBindPoint') ? 'pointer.png' : 'BG.bmp';
      await this.saveImage(imagePath, path.join(folderPath, fileName));
      return;
    }
    
    // For digit images (0-9), save with proper naming
    if (component.$type.includes('DragBindNormalDateTime') || component.$type.includes('DragBindNums')) {
      for (let i = 0; i < Math.min(images.length, 10); i++) {
        const imagePath = images[i];
        const fileName = `WHITE_${i}.png`;
        await this.saveImage(imagePath, path.join(folderPath, fileName));
      }
      
      // Handle EmptySource (no data image) - should be 11th image
      if (component.EmptySource && images.length === 11) {
        await this.saveImage(images[10], path.join(folderPath, 'NO_DATA.png'));
      }
      return;
    }
    
    // For progress bars and other image arrays
    for (let i = 0; i < images.length; i++) {
      const imagePath = images[i];
      const fileName = `progress_${i.toString().padStart(3, '0')}.png`;
      await this.saveImage(imagePath, path.join(folderPath, fileName));
    }
  }

  /**
   * Save an image from various formats (URL, base64, file path)
   */
  private async saveImage(source: string, destPath: string): Promise<void> {
    try {
      // Check if it's a base64 data URL
      if (source.startsWith('data:image')) {
        const base64Data = source.split(',')[1];
        const buffer = Buffer.from(base64Data, 'base64');
        
        // Validate buffer before processing
        if (buffer.length < 100) {
          throw new Error(`Invalid base64 data: buffer too small (${buffer.length} bytes)`);
        }
        
        console.log(`Processing base64 image - buffer size: ${buffer.length} bytes, target: ${destPath}`);
        
        // Convert to appropriate format
        if (destPath.endsWith('.bmp')) {
          // For BMP files, write the buffer directly (BMP format is already in the buffer)
          console.log(`Writing BMP file directly from buffer...`);
          await fs.writeFile(destPath, buffer);
        } else {
          // Save as PNG
          await sharp(buffer).png().toFile(destPath);
        }
      }
      // Check if it's raw base64 (without data:image prefix) - starts with base64 characters
      else if (/^[A-Za-z0-9+/=]+$/.test(source) && source.length > 100) {
        console.log(`Processing raw base64 string (${source.substring(0, 20)}...) - length: ${source.length}, dest: ${destPath}`);
        const buffer = Buffer.from(source, 'base64');
        
        // Validate buffer before processing
        if (buffer.length < 100) {
          throw new Error(`Invalid base64 data: buffer too small (${buffer.length} bytes)`);
        }
        
        console.log(`Buffer created: ${buffer.length} bytes`);
        
        // For BMP files, write directly without Sharp processing
        if (destPath.endsWith('.bmp')) {
          console.log(`Writing BMP file directly...`);
          await fs.writeFile(destPath, buffer);
          console.log(`✓ Successfully saved BMP: ${destPath}`);
        } else {
          // Try to detect format and convert to PNG
          try {
            await sharp(buffer).png().toFile(destPath);
            console.log(`✓ Successfully saved PNG: ${destPath}`);
          } catch (err: any) {
            console.error(`✗ Failed to process image for ${destPath}: ${err.message}`);
            console.error(`Base64 preview: ${source.substring(0, 100)}...`);
            throw new Error(`Invalid image format: ${err.message}`);
          }
        }
      }
      // Check if it's an asset path reference
      else if (source.startsWith('assets/')) {
        console.warn(`Asset path reference found: ${source} - should be resolved before saving`);
        // In production, you'd resolve this from ImageData
      }
      // Check if it's a file path
      else if (await fs.pathExists(source)) {
        // Copy file and convert if needed
        if (destPath.endsWith('.bmp') && !source.endsWith('.bmp')) {
          // Save as PNG with .bmp extension for compatibility
          await sharp(source).png().toFile(destPath);
        } else if (destPath.endsWith('.png') && !source.endsWith('.png')) {
          await sharp(source).png().toFile(destPath);
        } else {
          await fs.copy(source, destPath);
        }
      }
      // Check if it's a blob URL (needs to be fetched)
      else if (source.startsWith('blob:')) {
        console.warn(`Blob URL found: ${source} - should be resolved to base64 before processing`);
      }
      else {
        console.warn(`Unknown image source format: ${source.substring(0, 100)}`);
      }
    } catch (error) {
      console.error(`Failed to save image: ${source}`, error);
      throw error;
    }
  }

  /**
   * Create NoDye.txt marker file
   */
  private async createNoDyeMarker(folderPath: string): Promise<void> {
    await fs.writeFile(path.join(folderPath, 'NoDye.txt'), '');
  }

  /**
   * Create auxiliary files folder (辅助文件)
   */
  private async createAuxiliaryFiles(data: WatchFaceJSON, projectPath: string): Promise<void> {
    const auxFolder = path.join(projectPath, '辅助文件');
    await fs.ensureDir(auxFolder);
    
    // Create 表盘信息.txt (watch face info)
    await this.createWatchInfo(data, auxFolder);
    
    // Create placeholder images (these should ideally be generated from actual preview)
    await this.createPlaceholderImage(path.join(auxFolder, '效果_1.png'), data.Width, data.Height);
    await this.createPlaceholderImage(path.join(auxFolder, '效果_2.png'), data.Width, data.Height);
    await this.createPlaceholderImage(path.join(auxFolder, '缩略_0.png'), data.ThumbnailWidth, data.ThumbnailHeight);
    await this.createPlaceholderImage(path.join(auxFolder, '缩略_1.png'), data.ThumbnailWidth, data.ThumbnailHeight);
    await this.createPlaceholderImage(path.join(auxFolder, '坐标.png'), data.Width, data.Height);
    
    // Create NoDye.txt
    await fs.writeFile(path.join(auxFolder, 'NoDye.txt'), '');
  }

  /**
   * Create watch info text file (表盘信息.txt)
   */
  private async createWatchInfo(data: WatchFaceJSON, auxFolder: string): Promise<void> {
    const watchName = data.WatchName || 'My Watch Face';
    const content = `#说明：以下结构不能修改，只填写对应XX部分。

#=====中文=====

#表盘名称
worksName_1:${watchName}
#表盘描述
worksDescribe_1:A beautiful watch face

#=====英文=====

#表盘名称
worksName_0:${watchName}
#表盘描述:
worksDescribe_0:A beautiful watch face
`;
    
    await fs.writeFile(path.join(auxFolder, '表盘信息.txt'), content, 'utf-8');
  }

  /**
   * Create a placeholder image
   */
  private async createPlaceholderImage(filePath: string, width: number, height: number): Promise<void> {
    try {
      const format = filePath.endsWith('.bmp') ? 'bmp' : 'png';
      
      await sharp({
        create: {
          width: width,
          height: height,
          channels: 4,
          background: { r: 0, g: 0, b: 0, alpha: 1 }
        }
      })
      .toFormat(format as any)
      .toFile(filePath);
    } catch (error) {
      console.error(`Failed to create placeholder image: ${filePath}`, error);
    }
  }

  /**
   * Create metadata files (说明.txt, v1.zh_mulan, etc.)
   */
  private async createMetadataFiles(data: WatchFaceJSON, projectPath: string): Promise<void> {
    // Create 说明.txt (naming convention explanation)
    const namingDoc = `目录命名规则
圆形_240x240_普通_简约_JW_001_00
形状_宽度x高度_表盘类型_系列_表盘作者_主编号_子编号

举例:
圆形_240x240_普通_商务_JW_006_00
圆形_240x240_普通_商务_JW_006_01

圆形_240x240_自定_相册_JW_006_00
圆形_240x240_自定_相册_JW_006_01

===========================================
======表盘系列类型======
"101#相册表盘",//0
"201#简约",//1
"202#商务",//2
"203#炫彩",//3
"204#个性",//4
===========================================
文件名命名规则
效果图 =/辅助文件/效果.png(文件大小，小于500K)
缩略图 =/辅助文件/缩略.bmp(文件大小，小于500K)
坐标图 =/辅助文件/坐标.png(文件大小，不限制)


元素图 =/元素文件夹/元素文件名.bmp(文件大小，不限制)
`;
    await fs.writeFile(path.join(projectPath, '说明.txt'), namingDoc, 'utf-8');
    
    // Create v1.zh_mulan version marker
    await fs.writeFile(path.join(projectPath, 'v1.zh_mulan'), '');
    
    // Create NoDye.txt at root
    await fs.writeFile(path.join(projectPath, 'NoDye.txt'), '');
  }

  /**
   * Clean up temporary files
   */
  async cleanup(projectPath: string): Promise<void> {
    try {
      if (await fs.pathExists(projectPath)) {
        await fs.remove(projectPath);
      }
    } catch (error) {
      console.error('Failed to cleanup:', error);
    }
  }
}
