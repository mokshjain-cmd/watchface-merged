/**
 * MOY File Generator for MoYoung Vendor
 * Converts watch face components to MOY format for bin generation
 */

import { WatchFaceComponent, WatchFaceProject } from '../App';

export interface MoyResolution {
  width: string;
  height: string;
  radian: string;
  thumbnail: {
    width: string;
    height: string;
  };
}

export interface MoySelectImg {
  name?: string;
  url?: string;
}

export interface MoyNodeAttr {
  size: {
    width: number;
    height: number;
  };
  position: {
    left: number;
    top: number;
  };
  numberType?: string;
  selectImg?: (MoySelectImg | {})[];
  // Date/Time specific
  showType?: number;
  language?: number;
  // Number specific
  alignment?: number;
  subscript?: number;
  skip?: number;
  composeBack?: boolean;
  spacing?: number;
  isSvg?: boolean;
  // Icon specific
  icToView?: number;
  icToViewAnimation?: number;
  // Progress/Widget specific
  batteryWrite?: {
    width: number;
    height: number;
  };
  batteryWriteOffset?: {
    left: number;
    top: number;
  };
  batteryDirection?: number;
  // Fill and stroke (optional)
  fill?: any;
  stroke?: any;
  keep?: any;
}

export interface MoyLayerGroup {
  id: string;
  path?: string;
  index: number;
  parent: string; // 'bg' | 'date' | 'time' | 'widget'
  type: string; // 'selectImg'
  code: string; // 'bg' | 'dateText' | 'timeNumber' | 'functionNumber' | 'progress' | 'icon'
  isActive?: boolean;
  isShow?: boolean;
  lock?: boolean;
  lockIcon?: string;
  leftIcon?: string;
  showIcon?: string;
  nodeAttr: MoyNodeAttr;
  imgSize?: number;
}

export interface MoyFile {
  resolution: MoyResolution;
  preview?: string;
  hdPreview?: string;
  hdfile?: string;
  thumbnail?: string;
  activeLayer?: string;
  faceName?: string;
  author?: string;
  synopsis?: string;
  series?: string;
  remark_cn?: string;
  remark_en?: string;
  platform: string;
  clockID?: number;
  tag_id?: any[];
  layerGroups: MoyLayerGroup[];
  distribution?: any;
  mutSelect?: any[];
  globalState?: any;
  base?: any;
  originFaceId?: any;
  originAutoFaceId?: any;
  originFaceNewId?: any;
  originAutoFaceNewId?: any;
  faceForm?: any;
  radian?: string;
  version?: string;
  designer?: string;
  updated_at?: string;
  created_at?: string;
  tplId?: number;
  shape?: string;
  _id?: string;
  createdAt?: string;
  updatedAt?: string;
}

export class MoyGenerator {
  private static generateMoyId(): string {
    const chars = 'ABCDEFGHJKLMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz123456789';
    let id = 'MOY_';
    for (let i = 0; i < 26; i++) {
      id += chars.charAt(Math.floor(Math.random() * chars.length));
    }
    return id;
  }

  /**
   * Map component type to MOY parent category
   */
  private static getComponentParent(component: WatchFaceComponent): string {
    if (component.type === 'image' && component.elementType === null) {
      return 'bg'; // Background image
    }
    if (component.type === 'dateTime') {
      const format = component.config.dateTimeFormat;
      if (format === 'Day' || format === 'Month' || format === 'Year' || format === 'WeekDay') {
        return 'date';
      }
      return 'time';
    }
    if (component.type === 'analog') {
      return 'time';
    }
    return 'widget'; // progress, number, etc.
  }

  /**
   * Map component to MOY code
   */
  private static getComponentCode(component: WatchFaceComponent): string {
    if (component.type === 'image' && component.elementType === null) {
      return 'bg';
    }
    // Icons: Time separator (8), Heart rate icon (9), Steps icon (10), Calorie icon (11)
    if (component.type === 'image' && (component.elementType === 8 || component.elementType === 9 || component.elementType === 10 || component.elementType === 11)) {
      return 'icon';
    }
    if (component.type === 'dateTime') {
      const format = component.config.dateTimeFormat;
      if (format === 'WeekDay' || format === 'Month') {
        return 'dateText'; // Image-based date display
      }
      return 'timeNumber'; // Numeric time display
    }
    if (component.type === 'progress') {
      return 'progress';
    }
    if (component.type === 'number') {
      return 'functionNumber';
    }
    if (component.type === 'analog') {
      return 'analogClock';
    }
    return 'icon';
  }

  /**
   * Get numberType from component configuration
   */
  private static getNumberType(component: WatchFaceComponent): string {
    if (component.type === 'image' && component.elementType === null) {
      return 'bg_0';
    }
    
    // Icon components
    if (component.type === 'image' && component.elementType === 8) {
      return 'time_dot'; // Time separator
    }
    if (component.type === 'image' && component.elementType === 9) {
      return 'ic_heart'; // Heart rate icon
    }
    if (component.type === 'image' && component.elementType === 10) {
      return 'ic_steps'; // Steps icon
    }
    if (component.type === 'image' && component.elementType === 11) {
      return 'ic_other'; // Calorie icon
    }

    const format = component.config.dateTimeFormat;
    const itemName = component.config.itemName;

    // Time/Date formats
    if (format === 'Hour') return 'num_hour';
    if (format === 'Minute') return 'num_min';
    if (format === 'Second') return 'num_sec';
    if (format === 'Day') return 'num_day';
    if (format === 'Month') return 'num_month'; // Numeric month (01-12)
    if (format === 'Year') {
      // Use num_year_four for 4-digit year (when digitPosition is 'multi')
      if (component.config.digitPosition === 'multi') {
        return 'num_year_four';
      }
      return 'num_year';
    }
    if (format === 'WeekDay') return 'txt_week';

    // Widget types by itemName
    if (itemName === 'Steps') return 'num_steps';
    if (itemName === 'Heart Rate') return 'num_heart';
    if (itemName === 'Calories') return 'num_cal';
    if (itemName === 'Distance') return 'num_km';
    if (itemName === 'Battery') return 'gra_battery';
    if (itemName === 'Blood Oxygen') return 'num_blood_oxygen';

    // Default
    if (component.type === 'progress') return 'gra_battery';
    if (component.type === 'number') return 'num_steps';
    
    return 'ic_default_dash';
  }

  /**
   * Get expected image count for numberType
   * Based on attribute.js limit values - includes additional images for "no data" states
   */
  private static getExpectedImageCount(numberType: string): number {
    if (numberType === 'num_hour' || numberType === 'num_min' || numberType === 'num_sec' || numberType === 'num_day' || numberType === 'num_year' || numberType === 'num_year_four' || numberType === 'num_month') return 10;
    if (numberType === 'txt_month') return 12;
    if (numberType === 'txt_week') return 7;
    if (numberType === 'bg_0') return 1;
    if (numberType === 'ic_other') return 1; // Calorie icon
    if (numberType === 'time_dot') return 1; // Time separator (colon)
    if (numberType === 'ic_heart') return 1; // Heart rate icon
    if (numberType === 'ic_steps') return 1; // Steps icon
    // These support additional images beyond 10 digits (e.g., for "no data" display)
    if (numberType === 'num_steps') return 12; // 10 digits + 2 additional
    if (numberType === 'num_heart') return 12; // 10 digits + 2 additional
    if (numberType === 'num_blood_oxygen') return 12; // 10 digits + 2 additional
    if (numberType === 'num_cal') return 11; // 10 digits + 1 additional
    if (numberType === 'num_km') return 14; // According to attribute.js
    if (numberType === 'num_weather') return 13; // 10 digits + 3 additional
    if (numberType === 'ic_default_dash') return 1;
    return 1; // default
  }

  /**
   * Convert blob URLs to MOY image format
   */
  private static async convertImagesToMoyFormat(
    images: string[],
    baseName: string,
    numberType: string
  ): Promise<(MoySelectImg | {})[]> {
    const expectedCount = this.getExpectedImageCount(numberType);
    const moyImages: (MoySelectImg | {})[] = [];

    for (let i = 0; i < expectedCount; i++) {
      const imageUrl = images[i];
      if (imageUrl) {
        // Generate filename based on index
        const extension = '.png'; // Assume PNG for now
        let filename: string;
        if (numberType === 'bg_0') {
          filename = 'c1_bg.png';

        } else {
          filename = expectedCount > 1 
            ? `${baseName}_${i.toString().padStart(2, '0')}${extension}`
            : `${baseName}${extension}`;
        }

        // For MOY, we store the path - actual image data will be appended at the end
        moyImages.push({
          name: filename,
          url: imageUrl // Keep original URL for now, will be replaced with path in final output
        });
      } else {
        moyImages.push({});
      }
    }

    return moyImages;
  }

  /**
   * Convert WatchFaceComponent to MoyLayerGroup
   */
  private static async convertComponentToLayerGroup(
    component: WatchFaceComponent,
    index: number,
    watchName: string
  ): Promise<MoyLayerGroup> {
    const parent = this.getComponentParent(component);
    const code = this.getComponentCode(component);
    const numberType = this.getNumberType(component);
    
    // Generate base name for images
    const baseName = `${watchName}_${numberType}`;
    
    const selectImg = await this.convertImagesToMoyFormat(component.images, baseName, numberType);

    // Calculate digit count for multi-digit components
    let digitCount = 1;
    if (component.config.digitPosition === 'multi') {
      // Determine digit count based on dateTimeFormat
      if (component.config.dateTimeFormat === 'Year') {
        digitCount = 4;
      } else if (['Hour', 'Minute', 'Second', 'Day', 'Month'].includes(component.config.dateTimeFormat || '')) {
        digitCount = 2;
      } else {
        // For other multi-digit components, check expected max digits
        const itemName = component.config.itemName;
        if (itemName === 'Steps') digitCount = 5;
        else if (itemName === 'Calories') digitCount = 4;
        else if (itemName === 'Heart Rate' || itemName === 'Blood Oxygen') digitCount = 3;
        else digitCount = 2; // Default
      }
    }

    // Calculate width per digit for multi-digit components
    const totalWidth = Math.round(component.size.width);
    const widthPerDigit = component.config.digitPosition === 'multi' 
      ? Math.round(totalWidth / digitCount) 
      : totalWidth;

    // Adjust selectImg widths if multi-digit
    if (component.config.digitPosition === 'multi' && selectImg && selectImg.length > 0) {
      selectImg.forEach((img: any) => {
        img.width = widthPerDigit;
      });
    }

    const nodeAttr: MoyNodeAttr = {
      size: {
        width: totalWidth,  // Keep the full width for the container
        height: Math.round(component.size.height)
      },
      position: {
        left: Math.round(component.position.x),
        top: Math.round(component.position.y)
      },
      keep: {
        absolute: {x: 'left', y: 'top'},
        position: {top: 0, left: 0}
      },
      numberType,
      selectImg
    };

    // Add fill and stroke for non-background elements
    if (code !== 'bg') {
      nodeAttr.fill = {
        fillType: 1,
        fillColor: [{value: '#000000', pos: 0}, {value: '#000000', pos: 1}],
        fillDirection: 'horizontal',
        fillSymmetry: false
      };
      nodeAttr.stroke = {
        strokeType: 1,
        strokeColor: [{value: '#000000', pos: 0}, {value: '#000000', pos: 1}],
        strokeDirection: 'horizontal',
        strokeWidth: 0,
        strokeSymmetry: false
      };
    }

    // Add type-specific attributes based on code
    if (code === 'bg') {
      // Background images don't need extra attributes
    } else if (code === 'dateText') {
      nodeAttr.showType = 1;
      nodeAttr.language = 0;
      nodeAttr.alignment = -1;
    }

    if (code === 'timeNumber' || code === 'functionNumber') {
      // alignment: 0 = left-aligned, 1 = right-aligned/tens place
      // For multi-digit (tens_and_ones), we want left alignment (0) so the x position is the left edge
      nodeAttr.alignment = (component.config.digitPosition === 'ones' || component.config.digitPosition === 'multi') ? 0 : 1;
      nodeAttr.subscript = -1;
      // skip: 0 = hide leading zeros, 1 = show leading zeros
      // For functionNumber (Steps, Calories, etc.), use skip=1 to show all digits including when value is 0
      nodeAttr.skip = code === 'functionNumber' ? 1 : 0;
      nodeAttr.composeBack = false;
      nodeAttr.isSvg = false;
      if (component.config.digitPosition === 'multi') {
        // Spacing should be 0 since we're already accounting for it in the width calculation
        nodeAttr.spacing = 0;
      }
    }

    if (code === 'icon') {
      nodeAttr.icToView = 1;
      nodeAttr.icToViewAnimation = 1;
      nodeAttr.subscript = -1;
      nodeAttr.skip = 0;
      nodeAttr.alignment = 0;
      nodeAttr.composeBack = false;
      nodeAttr.batteryWrite = { width: 0, height: 0 };
      nodeAttr.batteryWriteOffset = { left: 2, top: 2 };
      nodeAttr.batteryDirection = 0;
    }

    // Generate path based on component type
    const componentPath = code === 'bg' 
      ? `C:\\Users\\16083\\static\\library\\${watchName}\\${numberType}`
      : `C:\\Users\\16083\\static\\library\\${watchName}\\${numberType}`;

    const layerGroup: MoyLayerGroup = {
      id: this.generateMoyId(),
      path: componentPath,
      index,
      parent,
      type: 'selectImg',
      code,
      isActive: false,
      isShow: true,
      lock: false,
      lockIcon: 'el-icon-lock',
      leftIcon: 'el-icon-s-cooperation',
      showIcon: 'el-icon-camera-solid',
      nodeAttr
    };

    return layerGroup;
  }

  /**
   * Generate MOY file from WatchFaceProject
   */
  public static async generateMoyFile(project: WatchFaceProject): Promise<MoyFile> {
    const watchName = project.watchCode || project.watchName || 'MyWatch';
    const timestamp = new Date();
    
    // Convert components to layer groups
    const layerGroups: MoyLayerGroup[] = [];
    
    for (let i = 0; i < project.components.length; i++) {
      const component = project.components[i];
      const layerGroup = await this.convertComponentToLayerGroup(component, i, watchName);
      layerGroups.push(layerGroup);
    }

    // Set the first layer group as active
    if (layerGroups.length > 0) {
      layerGroups[0].isActive = true;
    }

    // Calculate radian for round/rounded screens
    const radian = Math.round(Math.min(project.width, project.height) / 2);

    // Generate unique _id for the MOY file
    const moyId = this.generateMoyId();

    const moyFile: MoyFile = {
      _id: moyId,
      resolution: {
        width: project.width.toString(),
        height: project.height.toString(),
        radian: `${radian}px`,
        thumbnail: {
          width: '280',
          height: '280'
        }
      },
      preview: '',
      hdPreview: '',
      hdfile: '',
      thumbnail: '',
      activeLayer: layerGroups.length > 0 ? layerGroups[0].id : '',
      faceName: project.watchName,
      author: 'Watch Assembly Tool',
      synopsis: '',
      series: `Jieli ${project.width}*${project.height} ${project.watchShape === 'circle' ? 'Round' : 'Square'} ${radian}px`,
      remark_cn: '',
      remark_en: '',
      platform: 'Jieli',
      clockID: 129,
      tag_id: [],
      layerGroups,
      distribution: null,
      mutSelect: [],
      globalState: {
        showLoginTips: false,
        showImportTips: false,
        showBuild: false,
        showAddFaceTips: false,
        showAddAutoFaceTips: false,
        showAttrRegion: true,
        showPreviewRegion: true,
        showLibraryRegion: true,
        showImageSrc: false,
        showImageTips: '',
        path: null,
        language: 'zh'
      },
      base: {
        multiple: '1.2',
        currentDate: new Date().toISOString(),
        currentTime: new Date().toISOString(),
        txt_week: 3,
        txt_month: 10,
        click: 2,
        maxTime: 86399,
        currentSpeed: 280,
        ani_click: 389,
        ani_x10: 495,
        ani_x2: 107,
        ani_x20: 702,
        ani_once: 0,
        ani_loop: 0,
        ani_fireworks: 1301,
        path: '',
        rotate: {left: 0, top: 0},
        showBuild: false
      },
      originFaceId: null,
      originAutoFaceId: null,
      originFaceNewId: null,
      originAutoFaceNewId: null,
      faceForm: {
        name: null,
        preview: null,
        file: null,
        face_language: [],
        face_tpl: [],
        firmware_names: null,
        blacklist: null,
        tag_id: [],
        sort_in_tag: null,
        sort_in_recommend: null,
        tested: 1,
        remark: '',
        remark_en: '',
        remark_cn: '',
        uploader: ''
      },
      radian: '',
      version: '',
      designer: '',
      updated_at: timestamp.toISOString().replace('T', ' ').substring(0, 19),
      created_at: timestamp.toISOString().replace('T', ' ').substring(0, 19),
      tplId: 113,
      shape: project.watchShape === 'circle' ? 'round' : 'Square',
      createdAt: timestamp.toISOString(),
      updatedAt: timestamp.toISOString()
    };

    return moyFile;
  }

  /**
   * Export MOY file using backend server (Node.js fs APIs)
   * Sends MOY data and images to backend for proper binary generation
   */
  public static async exportMoyFile(
    project: WatchFaceProject,
    imageData: Map<string, Blob>,
    backendUrl: string = 'http://localhost:5555'
  ): Promise<Blob> {
    const moyFile = await this.generateMoyFile(project);
    
    // Generate a unique identifier for this watch face
    const watchFaceId = Array.from({length: 16}, () => 
      'ABCDEFGHJKLMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz123456789'.charAt(
        Math.floor(Math.random() * 57)
      )
    ).join('');
    
    // Set the _id to the watchFaceId
    moyFile._id = watchFaceId;
    
    console.log('=== Preparing MOY Export (Backend Mode) ===');
    console.log(`Watch name: ${moyFile.faceName}`);
    console.log(`Layer groups: ${moyFile.layerGroups.length}`);
    console.log('Available image URLs in imageData:');
    imageData.forEach((blob, url) => {
      console.log(`  - ${url} (${blob.size} bytes)`);
    });
    
    // First, collect all images with their original blob URLs BEFORE converting URLs
    const images: Array<{ name: string; data: string; url: string }> = [];
    const urlMapping = new Map<string, string>(); // Map original URL to filename
    let backgroundImageBase64: string | null = null; // Store background image for thumbnail
    
    for (const layerGroup of moyFile.layerGroups) {
      if (layerGroup.nodeAttr.selectImg) {
        for (const img of layerGroup.nodeAttr.selectImg) {
          if (img && 'url' in img && img.url) {
            console.log(`\nProcessing image: ${img.name}`);
            console.log(`  Looking for URL: ${img.url}`);
            
            const blob = imageData.get(img.url);
            if (!blob) {
              console.error(`  ✗ Missing blob for ${img.name}`);
              console.error(`  Expected URL: ${img.url}`);
              throw new Error(`Missing image data for ${img.name}. URL: ${img.url}`);
            }
            
            // Convert blob to base64 using FileReader (browser's native binary-safe method)
            const arrayBuffer = await blob.arrayBuffer();
            const bytes = new Uint8Array(arrayBuffer);
            
            // Validate PNG signature
            const isPNG = bytes.length >= 8 && 
              bytes[0] === 0x89 && bytes[1] === 0x50 && bytes[2] === 0x4E && bytes[3] === 0x47 &&
              bytes[4] === 0x0D && bytes[5] === 0x0A && bytes[6] === 0x1A && bytes[7] === 0x0A;
            
            if (!isPNG) {
              console.error(`  ✗ Invalid PNG signature for ${img.name}`);
              console.error(`  First 8 bytes:`, Array.from(bytes.slice(0, 8)).map(b => '0x' + b.toString(16).padStart(2, '0')));
              throw new Error(`Invalid PNG file: ${img.name}`);
            }
            
            // Use browser's native base64 encoding via FileReader
            const base64 = await new Promise<string>((resolve) => {
              const reader = new FileReader();
              reader.onloadend = () => {
                const result = reader.result as string;
                // Remove data URL prefix
                const base64Data = result.split(',')[1];
                resolve(base64Data);
              };
              reader.readAsDataURL(blob);
            });
            
            console.log(`  PNG validated: ${bytes.length} bytes, base64: ${base64.length} chars`);
            
            // Store background image for thumbnail (c1_bg.png)
            if (img.name === 'c1_bg.png' || layerGroup.code === 'bg') {
              backgroundImageBase64 = base64;
              console.log(`  ✓ Captured background image for thumbnail`);
            }
            
            // Store mapping of original URL to filename
            urlMapping.set(img.url, img.name);
            
            images.push({
              name: img.name,
              data: `data:image/png;base64,${base64}`,
              url: img.url // Keep original blob URL for backend mapping
            });
            
            console.log(`  ✓ Prepared image: ${img.name} (${blob.size} bytes)`);
          }
        }
      }
    }
    
    console.log(`\nTotal images prepared: ${images.length}`);
    
    // Add thumbnail as a separate image entry (required by vendor)
    if (backgroundImageBase64) {
      const thumbnailName = `${moyFile.faceName || 'watchface'}_thumbnail.png`;
      images.push({
        name: thumbnailName,
        data: `data:image/png;base64,${backgroundImageBase64}`,
        url: 'thumbnail' // Special marker for backend
      });
      console.log(`  ✓ Added thumbnail: ${thumbnailName}`);
    }
    
    // Now convert blob URLs to proper file paths in selectImg
    // Format: C:\Users\username\static\{watchFaceId}//{filename}
    for (const layerGroup of moyFile.layerGroups) {
      if (layerGroup.nodeAttr.selectImg) {
        layerGroup.nodeAttr.selectImg = layerGroup.nodeAttr.selectImg.map(img => {
          if (img && 'name' in img) {
            return {
              name: img.name,
              url: `C:\\Users\\16083\\static\\${watchFaceId}\\${img.name}`
            };
          } else {
            return {};
          }
        });
      }
    }
    
    // Send request to backend
    console.log(`\n📤 Sending request to backend: ${backendUrl}/api/generate-moy`);
    
    try {
      const response = await fetch(`${backendUrl}/api/generate-moy`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          moyFile, //vendor schema json
          images // Thumbnail is now included in images array
        })
      });
      
      if (!response.ok) {
        const error = await response.json();
        throw new Error(error.error || `Backend error: ${response.status}`);
      }
      
      const result = await response.json();
      console.log(`\n✅ MOY file generated by backend`);
      console.log(`   Filename: ${result.filename}`);
      console.log(`   Size: ${result.size} bytes`);
      
      // Convert base64 response back to blob
      const base64Data = result.data;
      const binaryString = atob(base64Data);
      const bytes = new Uint8Array(binaryString.length);
      for (let i = 0; i < binaryString.length; i++) {
        bytes[i] = binaryString.charCodeAt(i);
      }
      
      const finalBlob = new Blob([bytes], { type: 'application/octet-stream' });
      console.log(`📦 Received MOY file blob: ${finalBlob.size} bytes\n`);
      console.log(`You can now download the MOY file using MoyGenerator.downloadMoyFile(finalBlob, '${finalBlob}')`);
      return finalBlob;
      
    } catch (error: any) {
      console.error('\n❌ Backend generation failed:', error.message);
      console.error('   Make sure the backend server is running:');
      console.error('   cd server && npm install && npm start');
      throw new Error(`Backend MOY generation failed: ${error.message}`);
    }
  }

  /**
   * Download MOY file
   */
  public static downloadMoyFile(blob: Blob, filename: string): void {
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = filename.endsWith('.moy') ? filename : `${filename}.moy`;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    URL.revokeObjectURL(url);
  }
}
