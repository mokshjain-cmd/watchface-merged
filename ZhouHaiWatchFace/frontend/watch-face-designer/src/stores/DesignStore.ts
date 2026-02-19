// Design store - equivalent to CreatWatchViewModel in C#
import { makeAutoObservable, runInAction } from 'mobx';
import type { 
  WatchFace,
  WatchStyle,
  DeviceProfile,
  RecentlyWatches,
  Project,
  WatchFaceOutput
} from '@/models';
import { WatchScreenType } from '@/models';
import { 
  generateId,
  createDefaultWatchFace,
  validateWatchFace,
  convertToExportFormat
} from '@/models/ModelHelpers';

export class DesignStore {
  // Current design state
  currentWatchFace: WatchFace | null = null;
  currentStyle: WatchStyle | null = null;
  
  // Device and project context
  selectedDevice: DeviceProfile | null = null;
  currentProject: Project | null = null;
  
  // Recent files and templates
  recentWatches: RecentlyWatches[] = [];
  availableTemplates: WatchFaceTemplate[] = [];
  
  // UI state
  isLoading = false;
  isSaving = false;
  hasUnsavedChanges = false;
  
  // Preview state
  previewMode: PreviewMode = 'normal';
  previewDeviceFrame = true;
  
  // Export state
  isExporting = false;
  exportProgress = 0;

  constructor() {
    makeAutoObservable(this, {}, { autoBind: true });
    this.loadRecentWatches();
    this.loadTemplates();
  }

  // Watch face creation (equivalent to ShowItemFunc in C#)
  async createWatchFace(params: {
    name: string;
    deviceProfile: DeviceProfile;
    templateId?: string;
    projectId?: string;
  }) {
    this.setLoading(true);
    
    try {
      const { name, deviceProfile, templateId } = params;
      
      // Create new watch face based on device profile
      const watchFace = createDefaultWatchFace({
        name: deviceProfile.name,
        deviceType: deviceProfile.id,
        width: deviceProfile.specifications.screenWidth,
        height: deviceProfile.specifications.screenHeight,
        thumbnailWidth: deviceProfile.specifications.thumbnailWidth,
        thumbnailHeight: deviceProfile.specifications.thumbnailHeight,
        corner: deviceProfile.specifications.cornerRadius,
      });

      watchFace.watchName = name;
      watchFace.deviceType = deviceProfile.id;

      // Apply template if specified
      if (templateId) {
        await this.applyTemplate(watchFace, templateId);
      }

      runInAction(() => {
        this.currentWatchFace = watchFace;
        this.selectedDevice = deviceProfile;
        this.hasUnsavedChanges = true;
      });

      // Create initial style
      this.createNewStyle('Normal');

      return watchFace;
    } catch (error) {
      throw new Error(`Failed to create watch face: ${error}`);
    } finally {
      this.setLoading(false);
    }
  }

  // Load existing watch face
  async loadWatchFace(_watchFaceId: string) {
    this.setLoading(true);
    
    try {
      // In a real app, this would load from API/file system
      // For now, simulate loading
      await new Promise(resolve => setTimeout(resolve, 1000));
      
      throw new Error('Watch face loading not implemented yet');
    } catch (error) {
      throw new Error(`Failed to load watch face: ${error}`);
    } finally {
      this.setLoading(false);
    }
  }

  // Save watch face
  async saveWatchFace() {
    if (!this.currentWatchFace) {
      throw new Error('No watch face to save');
    }

    const validation = validateWatchFace(this.currentWatchFace);
    if (!validation.isValid) {
      throw new Error(`Validation failed: ${validation.errors.map(e => e.message).join(', ')}`);
    }

    this.setSaving(true);
    
    try {
      // Update timestamps
      runInAction(() => {
        if (this.currentWatchFace) {
          this.currentWatchFace.updateTime = new Date();
        }
      });

      // In a real app, this would save to API/file system
      await new Promise(resolve => setTimeout(resolve, 1000));
      
      runInAction(() => {
        this.hasUnsavedChanges = false;
      });

      // Update recent watches
      this.addToRecentWatches();
      
    } catch (error) {
      throw new Error(`Failed to save watch face: ${error}`);
    } finally {
      this.setSaving(false);
    }
  }

  // Save as new watch face
  async saveAsWatchFace(newName: string) {
    if (!this.currentWatchFace) {
      throw new Error('No watch face to save');
    }

    const newWatchFace: WatchFace = {
      ...this.currentWatchFace,
      id: generateId(),
      watchName: newName,
      watchCode: `WF_${Date.now()}`,
      createTime: new Date(),
      updateTime: new Date(),
    };

    runInAction(() => {
      this.currentWatchFace = newWatchFace;
    });

    return this.saveWatchFace();
  }

  // Style management
  createNewStyle(styleName: string, screenType: 'light' | 'dark' = 'light') {
    if (!this.currentWatchFace) return;

    const newStyle: WatchStyle = {
      id: generateId(),
      createTime: new Date(),
      updateTime: new Date(),
      styleName,
      screenType: screenType === 'light' ? WatchScreenType.LIGHT : WatchScreenType.DARK,
      dragBindBases: [],
    };

    runInAction(() => {
      this.currentWatchFace!.watchStyles.push(newStyle);
      this.currentStyle = newStyle;
      this.hasUnsavedChanges = true;
    });

    return newStyle;
  }

  selectStyle(styleId: string) {
    if (!this.currentWatchFace) return;

    const style = this.currentWatchFace.watchStyles.find(s => s.id === styleId);
    if (style) {
      runInAction(() => {
        this.currentStyle = style;
      });
    }
  }

  deleteStyle(styleId: string) {
    if (!this.currentWatchFace || this.currentWatchFace.watchStyles.length <= 1) return;

    runInAction(() => {
      this.currentWatchFace!.watchStyles = this.currentWatchFace!.watchStyles.filter(s => s.id !== styleId);
      
      // Select another style if the current one was deleted
      if (this.currentStyle?.id === styleId) {
        this.currentStyle = this.currentWatchFace!.watchStyles[0] || null;
      }
      
      this.hasUnsavedChanges = true;
    });
  }

  duplicateStyle(styleId: string) {
    if (!this.currentWatchFace) return;

    const originalStyle = this.currentWatchFace.watchStyles.find(s => s.id === styleId);
    if (!originalStyle) return;

    const duplicatedStyle: WatchStyle = {
      ...originalStyle,
      id: generateId(),
      styleName: `${originalStyle.styleName}_copy`,
      createTime: new Date(),
      updateTime: new Date(),
      dragBindBases: [...originalStyle.dragBindBases], // Deep copy would be needed for complex objects
    };

    runInAction(() => {
      this.currentWatchFace!.watchStyles.push(duplicatedStyle);
      this.currentStyle = duplicatedStyle;
      this.hasUnsavedChanges = true;
    });

    return duplicatedStyle;
  }

  // Preview management
  setPreviewMode(mode: PreviewMode) {
    runInAction(() => {
      this.previewMode = mode;
    });
  }

  toggleDeviceFrame() {
    runInAction(() => {
      this.previewDeviceFrame = !this.previewDeviceFrame;
    });
  }

  // Export functionality (equivalent to GetWatchfaceOut in C#)
  async exportWatchFace(format: ExportFormat, settings?: ExportSettings): Promise<ExportResult> {
    if (!this.currentWatchFace) {
      throw new Error('No watch face to export');
    }

    this.setExporting(true);
    this.setExportProgress(0);

    try {
      // Convert to export format
      this.setExportProgress(20);
      const exportData = convertToExportFormat(this.currentWatchFace);
      
      // Process based on format
      this.setExportProgress(50);
      let result: ExportResult;
      
      switch (format) {
        case 'json':
          result = await this.exportAsJson(exportData, settings);
          break;
        case 'bin':
          result = await this.exportAsBinary(exportData, settings);
          break;
        case 'png':
          result = await this.exportAsImage(exportData, settings);
          break;
        case 'zip':
          result = await this.exportAsZip(exportData, settings);
          break;
        default:
          throw new Error(`Unsupported export format: ${format}`);
      }

      this.setExportProgress(100);
      return result;
      
    } catch (error) {
      throw new Error(`Export failed: ${error}`);
    } finally {
      this.setExporting(false);
      this.setExportProgress(0);
    }
  }

  // Template management
  async applyTemplate(_watchFace: WatchFace, templateId: string) {
    const template = this.availableTemplates.find(t => t.id === templateId);
    if (!template) {
      throw new Error(`Template not found: ${templateId}`);
    }

    // Apply template data to watch face
    // This would load template data and apply it
    await new Promise(resolve => setTimeout(resolve, 500));
    
    // For now, just mark as having changes
    runInAction(() => {
      this.hasUnsavedChanges = true;
    });
  }

  // Recent watches management
  async loadRecentWatches() {
    try {
      // In a real app, this would load from storage/API
      const recent: RecentlyWatches[] = [
        {
          name: 'My First Watch',
          deviceType: 'smartwatch_default',
          createTime: new Date(Date.now() - 86400000), // 1 day ago
          lastModified: new Date(Date.now() - 3600000), // 1 hour ago
        },
        {
          name: 'Fitness Tracker',
          deviceType: 'fitness_tracker',
          createTime: new Date(Date.now() - 172800000), // 2 days ago
          lastModified: new Date(Date.now() - 7200000), // 2 hours ago
        },
      ];

      runInAction(() => {
        this.recentWatches = recent;
      });
    } catch (error) {
      console.error('Failed to load recent watches:', error);
    }
  }

  private addToRecentWatches() {
    if (!this.currentWatchFace) return;

    const recentWatch: RecentlyWatches = {
      name: this.currentWatchFace.watchName || 'Untitled',
      deviceType: this.currentWatchFace.deviceType,
      createTime: this.currentWatchFace.createTime,
      lastModified: new Date(),
    };

    runInAction(() => {
      // Remove if already exists
      this.recentWatches = this.recentWatches.filter(w => w.name !== recentWatch.name);
      
      // Add to beginning
      this.recentWatches.unshift(recentWatch);
      
      // Limit to 10 recent items
      if (this.recentWatches.length > 10) {
        this.recentWatches = this.recentWatches.slice(0, 10);
      }
    });
  }

  // Template loading
  private async loadTemplates() {
    try {
      // Load from templates configuration
      const templates: WatchFaceTemplate[] = [
        {
          id: 'template_analog_classic',
          name: 'Classic Analog',
          description: 'Traditional analog watch face',
          category: 'analog',
          thumbnailUrl: '/templates/analog_classic_thumb.png',
          previewUrl: '/templates/analog_classic_preview.png',
          deviceTypes: ['smartwatch_default', 'round_smartwatch'],
          isFree: true,
        },
        {
          id: 'template_digital_modern',
          name: 'Modern Digital',
          description: 'Clean digital display with weather',
          category: 'digital',
          thumbnailUrl: '/templates/digital_modern_thumb.png',
          previewUrl: '/templates/digital_modern_preview.png',
          deviceTypes: ['smartwatch_default', 'fitness_tracker'],
          isFree: true,
        },
      ];

      runInAction(() => {
        this.availableTemplates = templates;
      });
    } catch (error) {
      console.error('Failed to load templates:', error);
    }
  }

  // Export implementations
  private async exportAsJson(data: WatchFaceOutput, _settings?: ExportSettings): Promise<ExportResult> {
    const jsonString = JSON.stringify(data, null, 2);
    const blob = new Blob([jsonString], { type: 'application/json' });
    
    return {
      format: 'json',
      data: blob,
      filename: `${data.name || 'watchface'}.json`,
      size: blob.size,
    };
  }

  private async exportAsBinary(data: WatchFaceOutput, _settings?: ExportSettings): Promise<ExportResult> {
    // Binary export would require actual binary conversion logic
    // For now, return a placeholder
    const binaryData = new Uint8Array(1024); // Placeholder
    const blob = new Blob([binaryData], { type: 'application/octet-stream' });
    
    return {
      format: 'bin',
      data: blob,
      filename: `${data.name || 'watchface'}.bin`,
      size: blob.size,
    };
  }

  private async exportAsImage(data: WatchFaceOutput, _settings?: ExportSettings): Promise<ExportResult> {
    // Image export would require canvas rendering
    // For now, return a placeholder
    const canvas = document.createElement('canvas');
    canvas.width = data.width;
    canvas.height = data.height;
    
    return new Promise((resolve) => {
      canvas.toBlob((blob) => {
        resolve({
          format: 'png',
          data: blob!,
          filename: `${data.name || 'watchface'}.png`,
          size: blob!.size,
        });
      }, 'image/png');
    });
  }

  private async exportAsZip(data: WatchFaceOutput, _settings?: ExportSettings): Promise<ExportResult> {
    // ZIP export would require actual ZIP creation logic
    // For now, return a placeholder
    const zipData = new Uint8Array(2048); // Placeholder
    const blob = new Blob([zipData], { type: 'application/zip' });
    
    return {
      format: 'zip',
      data: blob,
      filename: `${data.name || 'watchface'}.zip`,
      size: blob.size,
    };
  }

  // State setters
  private setLoading(loading: boolean) {
    runInAction(() => {
      this.isLoading = loading;
    });
  }

  private setSaving(saving: boolean) {
    runInAction(() => {
      this.isSaving = saving;
    });
  }

  private setExporting(exporting: boolean) {
    runInAction(() => {
      this.isExporting = exporting;
    });
  }

  private setExportProgress(progress: number) {
    runInAction(() => {
      this.exportProgress = Math.max(0, Math.min(100, progress));
    });
  }

  // Computed properties
  get canSave() {
    return this.currentWatchFace !== null && this.hasUnsavedChanges && !this.isSaving;
  }

  get canExport() {
    return this.currentWatchFace !== null && !this.isExporting;
  }

  get hasMultipleStyles() {
    return this.currentWatchFace ? this.currentWatchFace.watchStyles.length > 1 : false;
  }

  get currentStyleIndex() {
    if (!this.currentWatchFace || !this.currentStyle) return -1;
    return this.currentWatchFace.watchStyles.findIndex(s => s.id === this.currentStyle!.id);
  }
}

// Types for the design store
export type PreviewMode = 'normal' | 'aod' | 'interactive';
export type ExportFormat = 'json' | 'bin' | 'png' | 'zip';

export interface ExportSettings {
  includeAssets?: boolean;
  compression?: boolean;
  quality?: number; // 0-1 for images
  resolution?: number; // scale factor
}

export interface ExportResult {
  format: ExportFormat;
  data: Blob;
  filename: string;
  size: number;
}

export interface WatchFaceTemplate {
  id: string;
  name: string;
  description: string;
  category: 'analog' | 'digital' | 'hybrid' | 'sports' | 'business';
  thumbnailUrl: string;
  previewUrl: string;
  deviceTypes: string[];
  isFree: boolean;
  isPremium?: boolean;
  tags?: string[];
}

// Create singleton instance
export const designStore = new DesignStore();