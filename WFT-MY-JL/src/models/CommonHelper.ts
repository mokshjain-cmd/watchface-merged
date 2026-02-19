/**
 * Common helper utilities equivalent to C# CommonHelper
 * Provides path operations, serialization settings, and utility functions
 */
export class CommonHelper {
  // Path configurations
  public static readonly appPath = "/watch-faces/"; // Web equivalent
  public static readonly addBtnBackgroundPath = "/assets/AddBtnBackground.png";

  // Language codes
  public static readonly languages = [
    "zh_CN", "en_US", "zh_TW", "ja_JP", "es_ES", "fr_FR", "de_DE", "ru_RU",
    "pt_BR", "pt_PT", "it_IT", "ko_KR", "tr_TR", "nl_NL", "th_TH", "sv_SE",
    "da_DK", "vi_VN", "nb_NO", "pl_PL", "fi_FI", "in_ID", "el_GR", "ro_RO",
    "cs_CZ", "uk_UA", "sk_SK", "hu_HU", "ar_EG", "iw_IL", "zh_HK"
  ];

  // Version info
  public static readonly version = "2.0.0";

  /**
   * Get current path for watch resources
   */
  public static currentPath(watchName: string): string {
    return `${this.rootPath(watchName)}/resources`;
  }

  /**
   * Get root path for watch
   */
  public static rootPath(watchName: string): string {
    return `${this.appPath}${watchName}`;
  }

  /**
   * Get JSON path for watch face
   */
  public static getJsonPath(watchName: string): string {
    return `${this.currentPath(watchName)}/watchFace.json`;
  }

  /**
   * Get XML path for watch face
   */
  public static getXmlPath(watchName: string): string {
    return `${this.currentPath(watchName)}/watchFace.xml`;
  }

  /**
   * Get preview path
   */
  public static preview(watchName: string): string {
    return `${this.currentPath(watchName)}/_preview`;
  }

  /**
   * Get widget path
   */
  public static widget(watchName: string): string {
    return `${this.currentPath(watchName)}/_widget`;
  }

  /**
   * Get ezip path
   */
  public static ezip(watchName: string): string {
    return `${this.currentPath(watchName)}/ezip`;
  }

  /**
   * Get absolute path
   */
  public static absolutePath(relative: string): string {
    return relative.startsWith('.') ? relative.substring(1) : relative;
  }

  /**
   * Get output path
   */
  public static outputPath(watchName: string): string {
    return `${this.rootPath(watchName)}/output`;
  }

  /**
   * Get output description file path
   */
  public static outputDescriptFile(watchName: string): string {
    return `${this.outputPath(watchName)}/description.xml`;
  }

  /**
   * Get output description JSON path
   */
  public static outputDescriptJson(watchName: string): string {
    return `${this.currentPath(watchName)}/description.json`;
  }

  /**
   * Get output preview path
   */
  public static outputPathPreview(watchName: string): string {
    return `${this.rootPath(watchName)}/output/preview`;
  }

  /**
   * Get output preview cover image path
   */
  public static outputPathPreviewCoverImage(watchName: string): string {
    return `${this.outputPathPreview(watchName)}/cover.jpg`;
  }

  /**
   * Get output preview icon image path
   */
  public static outputPathPreviewIconImage(watchName: string): string {
    return `${this.outputPathPreview(watchName)}/icon_small.jpg`;
  }

  /**
   * Get watch bin path
   */
  public static watchBin(watchName: string): string {
    return `${this.rootPath(watchName)}/WatchBin`;
  }

  /**
   * Get ezip bin path
   */
  public static ezipBin(watchName: string): string {
    return `${this.rootPath(watchName)}/EzipBin`;
  }

  /**
   * Resource file name
   */
  public static readonly resource = "resource.bin";

  /**
   * Convert path format for output
   */
  public static outPath(path: string, watchName: string): string {
    return path
      .replace(/^\./, '') // Remove leading dot
      .replace(/\\\\/g, '/') // Replace double backslashes with forward slash
      .replace(/\\/g, '/') // Replace backslashes with forward slashes
      .replace(new RegExp(`/watch-faces/${watchName}/resources/`, 'g'), '') // Remove base path
      .replace(/\/+/g, '/'); // Replace multiple slashes with single slash
  }

  /**
   * JSON serialization settings equivalent to C# settings
   */
  public static readonly jsonSettings = {
    // TypeScript/JavaScript equivalent of C# TypeNameHandling settings
    preserveTypes: true,
    includeTypeInfo: true
  };

  /**
   * Generate description file (web equivalent)
   */
  public static async generateDesc(folderName: string): Promise<void> {
    try {
      const descriptionInfo = this.outputDescriptJson(folderName);
      // In web environment, this would involve fetching the JSON file
      // and converting to appropriate format for download
      console.log(`Generating description for: ${descriptionInfo}`);
    } catch (error) {
      console.error('Error generating description:', error);
    }
  }

  /**
   * Generate XML file (web equivalent)
   */
  public static async generateXml(folderName: string): Promise<void> {
    try {
      const jsonPath = this.getJsonPath(folderName);
      // In web environment, this would involve fetching and converting JSON to XML
      console.log(`Generating XML for: ${jsonPath}`);
    } catch (error) {
      console.error('Error generating XML:', error);
    }
  }

  /**
   * Validate file exists (web equivalent)
   */
  public static async fileExists(path: string): Promise<boolean> {
    try {
      const response = await fetch(path, { method: 'HEAD' });
      return response.ok;
    } catch {
      return false;
    }
  }

  /**
   * Read text file (web equivalent)
   */
  public static async readTextFile(path: string): Promise<string> {
    const response = await fetch(path);
    if (!response.ok) {
      throw new Error(`Failed to read file: ${path}`);
    }
    return await response.text();
  }

  /**
   * Download file (web equivalent of file operations)
   */
  public static downloadFile(content: string, filename: string, mimeType: string = 'application/json'): void {
    const blob = new Blob([content], { type: mimeType });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = filename;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    URL.revokeObjectURL(url);
  }

  /**
   * Parse JSON with error handling
   */
  public static parseJson<T>(jsonString: string): T | null {
    try {
      return JSON.parse(jsonString) as T;
    } catch (error) {
      console.error('Error parsing JSON:', error);
      return null;
    }
  }

  /**
   * Stringify object with proper formatting
   */
  public static stringifyJson(obj: any, indent: number = 2): string {
    return JSON.stringify(obj, null, indent);
  }

  /**
   * Get file extension
   */
  public static getFileExtension(filename: string): string {
    return filename.split('.').pop()?.toLowerCase() || '';
  }

  /**
   * Check if file is image
   */
  public static isImageFile(filename: string): boolean {
    const imageExtensions = ['jpg', 'jpeg', 'png', 'gif', 'bmp', 'webp', 'svg'];
    return imageExtensions.includes(this.getFileExtension(filename));
  }

  /**
   * Generate unique ID
   */
  public static generateId(): string {
    return Math.random().toString(36).substring(2) + Date.now().toString(36);
  }

  /**
   * Deep clone object
   */
  public static deepClone<T>(obj: T): T {
    return JSON.parse(JSON.stringify(obj));
  }

  /**
   * Debounce function
   */
  public static debounce<T extends (...args: any[]) => any>(
    func: T,
    wait: number
  ): (...args: Parameters<T>) => void {
    let timeout: any = null;
    return (...args: Parameters<T>) => {
      if (timeout) clearTimeout(timeout);
      timeout = setTimeout(() => func(...args), wait);
    };
  }

  /**
   * Format file size
   */
  public static formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 B';
    const k = 1024;
    const sizes = ['B', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
  }

  /**
   * Set default values for components (equivalent to C# SetDefaultNum)
   */
  public static setDefaultNum(bindBases: any[]): void {
    for (const bindBase of bindBases) {
      // Get setting from DataHelper
      const setting = (window as any).DataHelper?.getWatchSettingByItemName?.(bindBase.itemName || '');

      // Set default values based on component type
      if (bindBase.constructor?.name === 'DragBindNums') {
        bindBase.currentNum = setting?.Default || 0;
      } else if (bindBase.constructor?.name === 'DragBindProgress') {
        bindBase.targetValue = setting?.TargetValue || 0;
        bindBase.currentNum = setting?.Default || 0;
      } else if (bindBase.constructor?.name === 'DragBindDouble') {
        bindBase.currentNum = setting?.Default || 0;
      } else if (bindBase.constructor?.name === 'DragBindKeyValue') {
        bindBase.currentNum = setting?.Default || 0;
      } else if (bindBase.constructor?.name === 'DragBindNormalDateTime') {
        bindBase.setDateTime = new Date(); // Default time
      } else if (bindBase.constructor?.name === 'DragBindMonthDay') {
        bindBase.setDateTime = new Date();
      } else if (bindBase.constructor?.name === 'DragBindWeek') {
        bindBase.setDateTime = new Date();
      } else if (bindBase.constructor?.name === 'DragBindAMPM') {
        bindBase.setDateTime = new Date();
      } else if (bindBase.constructor?.name === 'DragBindPoint') {
        // Calculate angle based on current time
        const now = new Date();
        const valueIndex = bindBase.valueIndex || 0;
        bindBase.value = this.getTimeAngle(valueIndex, now);
      } else if (bindBase.constructor?.name === 'DragBindSwitch') {
        bindBase.isOpen = bindBase.itemName === '温度单位';
      } else if (bindBase.constructor?.name === 'DragBindSingleDigit') {
        bindBase.setDateTime = new Date();
      } else if (bindBase.constructor?.name === 'DragBindWidget') {
        if (bindBase.subItems) {
          this.setDefaultNum(bindBase.subItems);
        }
      } else if (bindBase.constructor?.name === 'DragBindSlot') {
        if (bindBase.subItems) {
          this.setDefaultNum(bindBase.subItems);
        }
      }
    }
  }

  /**
   * Calculate time angle for pointers (equivalent to WatchStyle.GetTimeAngle)
   */
  private static getTimeAngle(valueIndex: number, dateTime: Date): number {
    switch (valueIndex) {
      case 0: // Hour
        return (dateTime.getHours() % 12) * 30 + dateTime.getMinutes() * 0.5;
      case 1: // Minute
        return dateTime.getMinutes() * 6;
      case 2: // Second
        return dateTime.getSeconds() * 6;
      default:
        return 0;
    }
  }

  /**
   * Generate preview images (web equivalent of GenerateImage)
   */
  public static async generateImage(folderName: string): Promise<void> {
    try {
      // This would involve canvas rendering in a web context
      console.log(`Generating preview images for: ${folderName}`);
      // Implementation would create canvas, render components, and save as images
    } catch (error) {
      console.error('Error generating image:', error);
    }
  }

  /**
   * Export canvas to PNG (web equivalent)
   */
  public static exportToPng(canvas: HTMLCanvasElement, filename: string): void {
    const link = document.createElement('a');
    link.download = filename;
    link.href = canvas.toDataURL('image/png');
    link.click();
  }
}