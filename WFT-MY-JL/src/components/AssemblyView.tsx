import React, { useState, useRef, useEffect } from 'react';
import { Plus, Upload, Save, Download, Settings, Grid } from 'lucide-react';
import { WatchFaceProject, WatchFaceComponent } from '../App';
import { ComponentLibrary } from './ComponentLibrary';
import { ComponentProperties } from './ComponentProperties';
import { DataHelper } from '../utils/DataHelper';
import { DragBindBase, BindMonitorType } from '../models/DragDataBase';
import { DataItemType, DataItemTypeHelper } from '../models/DataItemType';
import { MoyGenerator } from '../utils/MoyGenerator';
import JSZip from 'jszip';
import { v4 as uuidv4 } from 'uuid';
import { Canvas, Rect, Line } from 'fabric';
import './AssemblyView.css';

interface AssemblyViewProps {
  project: WatchFaceProject;
  onProjectUpdate: (project: WatchFaceProject) => void;
}


export const AssemblyView: React.FC<AssemblyViewProps> = ({ project, onProjectUpdate }) => {
  const [selectedComponent, setSelectedComponent] = useState<WatchFaceComponent | null>(null);
  const [draggedComponent, setDraggedComponent] = useState<WatchFaceComponent | null>(null);
  const [isGenerating, setIsGenerating] = useState(false);
  const [showGrid, setShowGrid] = useState(false);
  const [showFilePickerDialog, setShowFilePickerDialog] = useState(false);
  const [pendingFileUpload, setPendingFileUpload] = useState<{
    componentType: string;
    elementType: number | null;
    dateTimeFormat?: string;
    timeComponentType?: number;
    digitType?: 'multi' | 'single';
    digitPosition?: string;
    componentName?: string;
  } | null>(null);
  const [isLibraryCollapsed, setIsLibraryCollapsed] = useState(false);
  const [isPropertiesCollapsed, setIsPropertiesCollapsed] = useState(false);
  const [isLayersCollapsed, setIsLayersCollapsed] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);
  const fabricCanvasRef = useRef<HTMLCanvasElement>(null);
  const fabricCanvas = useRef<Canvas | null>(null);

    // Save/Open Project logic
    const openFileInputRef = useRef<HTMLInputElement>(null);

    const handleSaveProject = () => {
      // Download the current project as JSON
      const dataStr = "data:text/json;charset=utf-8," + encodeURIComponent(JSON.stringify(project, null, 2));
      const downloadAnchorNode = document.createElement('a');
      downloadAnchorNode.setAttribute("href", dataStr);
      downloadAnchorNode.setAttribute("download", (project.watchName || 'watchface') + ".json");
      document.body.appendChild(downloadAnchorNode);
      downloadAnchorNode.click();
      downloadAnchorNode.remove();
    };

    const handleOpenProject = () => {
      if (openFileInputRef.current) {
        openFileInputRef.current.value = '';
        openFileInputRef.current.click();
      }
    };

    const onOpenProjectFile = (e: React.ChangeEvent<HTMLInputElement>) => {
      const file = e.target.files && e.target.files[0];
      if (!file) return;
      const reader = new FileReader();
      reader.onload = (event) => {
        try {
          const json = JSON.parse(event.target?.result as string);
          // Validate minimal structure
          if (json && json.components && Array.isArray(json.components)) {
            onProjectUpdate(json);
          } else {
            alert('Invalid project file.');
          }
        } catch (err) {
          alert('Failed to load project: ' + err);
        }
      };
      reader.readAsText(file);
    };
  // Helper function to map elementType (BindMonitorType) to ItemName from Excel
  const getItemNameFromElementType = (elementType: number): string => {
    switch (elementType) {
      case BindMonitorType.Steps: return 'Steps';
      case BindMonitorType.HeartRate: return 'Heart Rate';
      case BindMonitorType.Calories: return 'Calories';
      case BindMonitorType.Battery: return 'Battery';
      case BindMonitorType.BloodOxygen: return 'Blood Oxygen';
      case BindMonitorType.Distance: return 'Step Distance';
      case BindMonitorType.Sleep: return 'Sleep Duration';
      case BindMonitorType.Weather: return 'Weather';
      default: return 'Steps'; // Default to Steps
    }
  };

  // Helper function to get default values from Excel data
  const getDefaultValuesFromExcel = (elementType: number) => {
    const itemName = getItemNameFromElementType(elementType);
    const setting = DataHelper.getWatchSettingByItemName(itemName);
    
    return {
      maxValue: setting?.MaxNum || 999999,
      minValue: setting?.MinNum || 0,
      defaultValue: setting?.Default || 0,
      targetValue: setting?.TargetValue || 0,
      itemName: itemName
    };
  };

  const handleImageUpload = (componentType: string, elementType: number | null, dateTimeFormat?: string, timeComponentType?: number, digitType?: 'multi' | 'single', digitPosition?: string, componentName?: string) => {
    // Store the pending upload info and show file picker dialog
    setPendingFileUpload({
      componentType,
      elementType,
      dateTimeFormat,
      timeComponentType,
      digitType,
      digitPosition,
      componentName
    });
    setShowFilePickerDialog(true);
  };

  const handleFileSelectFromDialog = () => {
    setShowFilePickerDialog(false);
    if (!pendingFileUpload) return;
    
    let componentType = pendingFileUpload.componentType;
    const elementType = pendingFileUpload.elementType;
    const dateTimeFormat = pendingFileUpload.dateTimeFormat;
    const timeComponentType = pendingFileUpload.timeComponentType;
    const digitType = pendingFileUpload.digitType;
    const digitPosition = pendingFileUpload.digitPosition;
    const componentName = pendingFileUpload.componentName;
    
    if (fileInputRef.current) {
      fileInputRef.current.onchange = (e) => {
        const files = Array.from((e.target as HTMLInputElement).files || []);
        if (files.length > 0) {
          
          // Process uploaded images first
          const imageUrls = files.map(file => URL.createObjectURL(file));
          
          // Validate image count for different component types
          if (componentType === 'dateTime' && dateTimeFormat) {
            // Special validation for AM/PM (needs 2 images)
            if (dateTimeFormat === 'AMPM' || dateTimeFormat === 'AM/PM') {
              if (files.length !== 2) {
                const proceed = confirm(`AM/PM component requires exactly 2 images (one for AM, one for PM). You uploaded ${files.length} images.\n\nOptions:\n• Click OK to create a simple background image instead\n• Click Cancel to go back and upload 2 images`);
                
                if (!proceed) {
                  if (fileInputRef.current) {
                    try { fileInputRef.current.value = ''; } catch { /* ignore */ }
                  }
                  return;
                }
                
                // User chose to create a background image - change component type
                componentType = 'image';
              }
            } else if (dateTimeFormat === 'Week') {
              // Week component needs 7 images (one for each day)
              if (files.length !== 7) {
                const proceed = confirm(`Week component requires 7 images (one for each day of the week). You uploaded ${files.length} images.\n\nOptions:\n• Click OK to create a simple background image instead\n• Click Cancel to go back and upload 7 images`);
                
                if (!proceed) {
                  if (fileInputRef.current) {
                    try { fileInputRef.current.value = ''; } catch { /* ignore */ }
                  }
                  return;
                }
                
                // User chose to create a background image - change component type
                componentType = 'image';
              }
            } else if (digitType === 'multi' || digitType === 'single') {
              // Numeric datetime components (Hour, Minute, etc.) need 10 images
              if (files.length < 10) {
                const proceed = confirm(`For ${dateTimeFormat || 'DateTime'} components, you typically need 10 images (one for each digit 0-9). You uploaded ${files.length} images.\n\nOptions:\n• Click OK to create a simple background image instead\n• Click Cancel to go back and upload 10 digit images (0.png to 9.png)`);
                
                if (!proceed) {
                  if (fileInputRef.current) {
                    try { fileInputRef.current.value = ''; } catch { /* ignore */ }
                  }
                  return;
                }
                
                // User chose to create a background image - change component type
                componentType = 'image';
              }
            }
          } else if (componentType === 'progress') {
            // Progress components can use any number of images for different fill levels
            // No validation needed - any number of images is fine for progress bars
          } else if (componentType === 'separator') {
            // Time separator requires exactly 1 image
            if (files.length !== 1) {
              const proceed = confirm(`Time Separator requires exactly 1 image (the colon or separator symbol). You uploaded ${files.length} images.\n\nOptions:\n• Click OK to use only the first image\n• Click Cancel to go back and upload 1 image`);
              
              if (!proceed) {
                if (fileInputRef.current) {
                  try { fileInputRef.current.value = ''; } catch { /* ignore */ }
                }
                return;
              }
              // If OK, continue with just the first image (already in imageUrls)
            }
          } else if (componentType === 'icon') {
            // Icons require exactly 1 image
            if (files.length !== 1) {
              const proceed = confirm(`Icon requires exactly 1 image. You uploaded ${files.length} images.\n\nOptions:\n• Click OK to use only the first image\n• Click Cancel to go back and upload 1 image`);
              
              if (!proceed) {
                if (fileInputRef.current) {
                  try { fileInputRef.current.value = ''; } catch { /* ignore */ }
                }
                return;
              }
            }
          } else if (componentType === 'number') {
            // Number components need digit images (0-9) plus optional "no data" image
            // Steps and Heart Rate require 12 images (10 digits + 2 additional)
            const requiresTwoNoDataImages = componentName && ['Steps', 'Heart Rate'].includes(componentName);
            const requiresOneNoDataImage = componentName && ['Blood Oxygen', 'Calories'].includes(componentName);
            const requiredImageCount = requiresTwoNoDataImages ? 12 : requiresOneNoDataImage ? 11 : 10;
            
            if (files.length < requiredImageCount) {
              const extraImagesDesc = requiresTwoNoDataImages ? '2 "no data" images' : requiresOneNoDataImage ? '1 "no data" image' : '';
              const message = (requiresTwoNoDataImages || requiresOneNoDataImage)
                ? `${componentName} component requires ${requiredImageCount} images (10 for digits 0-9${extraImagesDesc ? ', plus ' + extraImagesDesc : ''}). You uploaded ${files.length} images.\n\nOptions:\n• Click OK to create a simple background image instead\n• Click Cancel to go back and upload ${requiredImageCount} images`
                : `For number components, you typically need 10 images (one for each digit 0-9). You uploaded ${files.length} images.\n\nOptions:\n• Click OK to create a simple background image instead\n• Click Cancel to go back and upload 10 digit images (0.png to 9.png)`;
              
              const proceed = confirm(message);
              
              if (!proceed) {
                if (fileInputRef.current) {
                  try { fileInputRef.current.value = ''; } catch { /* ignore */ }
                }
                return;
              }
              
              // User chose to create a background image - change component type
              componentType = 'image';
            } else if (digitType === 'multi') {
              // Multi-digit number component (like Steps) - create ONE component showing all digits
              const excelDefaults = getDefaultValuesFromExcel(elementType || 0);
              
              const component: WatchFaceComponent = {
                id: uuidv4(),
                type: 'number',
                elementType,
                name: componentName || excelDefaults.itemName || 'Number',
                images: imageUrls,
                position: { x: 150, y: 200 },
                size: { width: 250, height: 80 },
                config: {
                  leadingZero: false, // No leading zeros for Steps
                  minValue: excelDefaults.minValue,
                  maxValue: excelDefaults.maxValue,
                  defaultValue: excelDefaults.defaultValue,
                  targetValue: excelDefaults.targetValue,
                  itemName: excelDefaults.itemName,
                  digitPosition: 'multi' as any
                }
              };
              
              const updatedProject = {
                ...project,
                components: [...project.components, component]
              };
              onProjectUpdate(updatedProject);
              if (fileInputRef.current) {
                try { fileInputRef.current.value = ''; } catch { /* ignore */ }
              }
              return;
            } else if (digitType === 'single') {
              // Single digit number component
              const excelDefaults = getDefaultValuesFromExcel(elementType || 0);
              
              const component: WatchFaceComponent = {
                id: uuidv4(),
                type: 'number',
                elementType,
                name: `${componentName || excelDefaults.itemName} ${digitPosition || 'Single'}`,
                images: imageUrls,
                position: { x: 150, y: 200 },
                size: { width: 50, height: 80 },
                config: {
                  leadingZero: false,
                  minValue: 0,
                  maxValue: 9,
                  defaultValue: 0,
                  itemName: excelDefaults.itemName,
                  digitPosition: digitPosition as any
                }
              };
              
              const updatedProject = {
                ...project,
                components: [...project.components, component]
              };
              onProjectUpdate(updatedProject);
              if (fileInputRef.current) {
                try { fileInputRef.current.value = ''; } catch { /* ignore */ }
              }
              return;
            }
          }
          
          // Get Excel settings for this component type
          const watchSetting = DataHelper.getWatchSettingByItemName(dateTimeFormat || '');
          const config = watchSetting ? DataHelper.getComponentConfig(watchSetting) : {
            dateTimeFormat,
            leadingZero: true,
            minValue: 0,
            maxValue: 100,
            defaultValue: 0
          };
          
          // For time components, create based on digitType
          if (componentType === 'dateTime' && dateTimeFormat) {
            // Special handling for AM/PM and Week components (no digitType)
            if (dateTimeFormat === 'AMPM' || dateTimeFormat === 'AM/PM') {
              const component: WatchFaceComponent = {
                id: uuidv4(),
                type: 'dateTime',
                elementType,
                timeComponent: timeComponentType,
                name: 'AM/PM',
                images: imageUrls, // 2 images: [0] = AM, [1] = PM
                position: { x: 150, y: 200 },
                size: { width: 60, height: 40 },
                config: {
                  ...config,
                  dateTimeFormat: 'AMPM'
                }
              };
              
              const updatedProject = {
                ...project,
                components: [...project.components, component]
              };
              onProjectUpdate(updatedProject);
              if (fileInputRef.current) {
                try { fileInputRef.current.value = ''; } catch { /* ignore */ }
              }
              return;
            } else if (dateTimeFormat === 'Week') {
              const component: WatchFaceComponent = {
                id: uuidv4(),
                type: 'dateTime',
                elementType,
                timeComponent: timeComponentType,
                name: 'Week',
                images: imageUrls, // 7 images: [0] = Sunday, [1] = Monday, etc.
                position: { x: 150, y: 200 },
                size: { width: 80, height: 40 },
                config: {
                  ...config,
                  dateTimeFormat: 'Week'
                }
              };
              
              const updatedProject = {
                ...project,
                components: [...project.components, component]
              };
              onProjectUpdate(updatedProject);
              if (fileInputRef.current) {
                try { fileInputRef.current.value = ''; } catch { /* ignore */ }
              }
              return;
            } else if (digitType === 'multi') {
              // Create ONE multi-digit component that shows all digits together
              const component: WatchFaceComponent = {
                id: uuidv4(),
                type: 'dateTime',
                elementType,
                timeComponent: timeComponentType,
                name: `${dateTimeFormat}`,
                images: imageUrls, // All 10 digit images (0-9)
                position: { x: 150, y: 200 }, // Better positioning
                size: { 
                  width: dateTimeFormat === 'Year' ? 200 : 100, // Wider for 4-digit year
                  height: 80 
                },
                config: {
                  ...config,
                  digitPosition: 'multi' as 'tens' | 'ones' | 'single' | 'thousands' | 'hundreds' | 'multi'
                }
              };
              
              const updatedProject = {
                ...project,
                components: [...project.components, component]
              };
              onProjectUpdate(updatedProject);
              if (fileInputRef.current) {
                // Reset file input so onchange will fire even if same files are selected next time
                try { fileInputRef.current.value = ''; } catch { /* ignore */ }
              }
              
            } else if (digitType === 'single') {
              // Create single digit component
              const component: WatchFaceComponent = {
                id: uuidv4(),
                type: 'dateTime',
                elementType,
                timeComponent: timeComponentType,
                name: `${dateTimeFormat} ${digitPosition || 'Single'}`,
                images: imageUrls,
                position: { x: 100, y: 200 },
                size: { width: 50, height: 80 },
                config: {
                  ...config,
                  digitPosition: (digitPosition as 'tens' | 'ones' | 'single' | 'thousands' | 'hundreds') || 'single'
                }
              };
              
              const updatedProject = {
                ...project,
                components: [...project.components, component]
              };
              onProjectUpdate(updatedProject);
              if (fileInputRef.current) {
                try { fileInputRef.current.value = ''; } catch { }
              }
              
            } else {
              // Legacy: Create tens and ones (for backward compatibility)
              const tensComponent: WatchFaceComponent = {
                id: uuidv4(),
                type: 'dateTime',
                elementType,
                timeComponent: timeComponentType,
                name: `${dateTimeFormat} Tens`,
                images: imageUrls,
                position: { x: 100, y: 200 },
                size: { width: 50, height: 80 },
                config: {
                  dateTimeFormat,
                  leadingZero: true,
                  minValue: 0,
                  maxValue: dateTimeFormat === 'Hour' ? 2 : 5,
                  defaultValue: 1,
                  digitPosition: 'tens'
                }
              };

              const onesComponent: WatchFaceComponent = {
                id: uuidv4(),
                type: 'dateTime',
                elementType,
                timeComponent: timeComponentType,
                name: `${dateTimeFormat} Ones`,
                images: imageUrls,
                position: { x: 160, y: 200 },
                size: { width: 50, height: 80 },
                config: {
                  dateTimeFormat,
                  leadingZero: true,
                  minValue: 0,
                  maxValue: 9,
                  defaultValue: 2,
                  digitPosition: 'ones'
                }
              };

              const updatedProject = {
                ...project,
                components: [...project.components, tensComponent, onesComponent]
              };
              onProjectUpdate(updatedProject);
              if (fileInputRef.current) {
                try { fileInputRef.current.value = ''; } catch { }
              }
            }
          } else {
            // Single component for non-time elements - Use Excel data for proper defaults
            // For image components (background), don't use Excel defaults
            const isImageComponent = componentType === 'image' && elementType === null;
            const isSeparatorComponent = componentType === 'separator';
            const isIconComponent = componentType === 'icon';
            const excelDefaults = (isImageComponent || isSeparatorComponent || isIconComponent) ? null : getDefaultValuesFromExcel(elementType || 0);
            
            const newComponent: WatchFaceComponent = {
              id: uuidv4(),
              type: (isSeparatorComponent || isIconComponent) ? 'image' : (componentType as any),
              elementType: isSeparatorComponent ? 8 : elementType,
              timeComponent: timeComponentType,
              name: isImageComponent 
                ? `Background Image ${project.components.length + 1}` 
                : isSeparatorComponent
                ? 'Time Separator'
                : isIconComponent
                ? componentName || 'Icon'
                : (excelDefaults?.itemName || `${componentType} ${project.components.length + 1}`),
              images: imageUrls,
              position: { x: 100, y: 100 }, // Start components more centered
              size: { width: isSeparatorComponent ? 20 : isIconComponent ? 30 : 100, height: isSeparatorComponent ? 40 : isIconComponent ? 30 : 100 },
              config: {
                leadingZero: true,
                minValue: excelDefaults?.minValue || 0,
                maxValue: excelDefaults?.maxValue || 100,
                defaultValue: excelDefaults?.defaultValue || 0,
                targetValue: excelDefaults?.targetValue || 0,
                itemName: isImageComponent ? 'Background Image' : isSeparatorComponent ? 'Time Separator' : isIconComponent ? (componentName || 'Icon') : (excelDefaults?.itemName || componentType)
              }
            };

            console.log('Adding new component:', newComponent); // Debug logging

            const updatedProject = {
              ...project,
              components: [...project.components, newComponent]
            };
            onProjectUpdate(updatedProject);
            setPendingFileUpload(null);
            if (fileInputRef.current) {
              try { fileInputRef.current.value = ''; } catch { }
            }
          }
        }
      };
      fileInputRef.current.click();
    }
  };

  const handleAnalogComponentAdd = (handType: 'hour' | 'minute' | 'second') => {
    if (fileInputRef.current) {
      fileInputRef.current.onchange = (e) => {
        const files = Array.from((e.target as HTMLInputElement).files || []);
        if (files.length > 0) {
          const imageUrl = URL.createObjectURL(files[0]);
          
          // ValueIndex: 0=hour, 1=minute, 2=second (based on Windows app)
          const valueIndex = handType === 'hour' ? 0 : handType === 'minute' ? 1 : 2;
          
          const newComponent: WatchFaceComponent = {
            id: uuidv4(),
            type: 'analog',
            elementType: null,
            name: `${handType.charAt(0).toUpperCase() + handType.slice(1)} Hand`,
            images: [imageUrl],
            position: { x: 220, y: 0 }, // Center position for 466x466 watch face
            size: { width: 26, height: 466 }, // Full height for the hand
            config: {
              itemName: 'Pointer',
              startAngle: 0,
              endAngle: handType === 'hour' ? 720 : 360, // Hour hand rotates twice (12-hour format)
              originPointX: 13, // Pivot point X (center of hand width)
              originPointY: 233, // Pivot point Y (center of watch face)
              valueIndex: valueIndex,
              value: 0,
              angle: 0
            }
          };

          const updatedProject = {
            ...project,
            components: [...project.components, newComponent]
          };
          onProjectUpdate(updatedProject);
          if (fileInputRef.current) {
            try { fileInputRef.current.value = ''; } catch { /* ignore */ }
          }
        }
      };
      fileInputRef.current.click();
    }
  };

  // Layer management functions
  const moveLayerUp = (index: number) => {
    if (index === 0) return; // Already at top
    const newComponents = [...project.components];
    [newComponents[index - 1], newComponents[index]] = [newComponents[index], newComponents[index - 1]];
    onProjectUpdate({ ...project, components: newComponents });
  };

  const moveLayerDown = (index: number) => {
    if (index === project.components.length - 1) return; // Already at bottom
    const newComponents = [...project.components];
    [newComponents[index], newComponents[index + 1]] = [newComponents[index + 1], newComponents[index]];
    onProjectUpdate({ ...project, components: newComponents });
  };

  const deleteLayer = (index: number) => {
    const newComponents = project.components.filter((_, i) => i !== index);
    onProjectUpdate({ ...project, components: newComponents });
    if (selectedComponent?.id === project.components[index].id) {
      setSelectedComponent(null);
    }
  };

  // Enhanced method to handle TypeScript model components
  const handleTypedComponentAdd = (component: DragBindBase) => {
    // Convert TypeScript model to React component format
    const reactComponent: WatchFaceComponent = {
      id: uuidv4(),
      type: getReactComponentType(component),
      elementType: component.elementType,
      name: component.dragName || component.itemName || 'Unnamed Component',
      images: Array.isArray(component.imageSource) ? component.imageSource : [component.imageSource].filter(Boolean),
      position: { x: component.x || 100, y: component.y || 100 },
      size: { width: component.width || 100, height: component.height || 50 },
      config: extractComponentConfig(component)
    };

    // Validate the component before adding
    const errors = DataHelper.validateComponent(component);
    if (errors.length > 0) {
      console.warn('Component validation issues:', errors);
      // Could show user notification here
    }

    // Add to project
    const updatedProject = {
      ...project,
      components: [...project.components, reactComponent]
    };
    onProjectUpdate(updatedProject);
  };

  // Helper to convert TypeScript model type to React component type
  const getReactComponentType = (component: DragBindBase): 'image' | 'dateTime' | 'progress' | 'number' => {
    const className = component.constructor.name;
    
    switch (className) {
      case 'DragBindNormalDateTime':
      case 'DragBindSingleDigit':
      case 'DragBindWeek':
      case 'DragBindMonthDay':
      case 'DragBindAMPM':
        return 'dateTime';
      case 'DragBindProgress':
        return 'progress';
      case 'DragBindNums':
        return 'number';
      case 'DragBindSwitch':
      case 'DragBindImage':
        return 'image';
      default:
        return 'dateTime';
    }
  };

  // Helper to extract configuration from TypeScript model
  const extractComponentConfig = (component: DragBindBase): any => {
    const config: any = {};

    // Common properties
    config.elementType = component.elementType;
    config.itemName = component.itemName;

    // Type-specific properties
    if ('dateTimeFormat' in component) {
      config.dateTimeFormat = (component as any).dateTimeFormat;
    }

    if ('maxNum' in component) {
      config.maxValue = (component as any).maxNum;
      config.minValue = (component as any).minNum;
      config.defaultValue = (component as any).defaultNum;
    }

    if ('targetValue' in component) {
      config.targetValue = (component as any).targetValue;
    }

    if ('fillType' in component) {
      config.fillType = (component as any).fillType;
    }

    // Switch-specific properties
    if ('openSource' in component) {
      config.openImage = (component as any).openSource;
      config.closeImage = (component as any).closeSource;
    }

    if ('amSource' in component) {
      config.amImage = (component as any).amSource;
      config.pmImage = (component as any).pmSource;
    }

    return config;
  };

  const handleComponentSelect = (component: WatchFaceComponent) => {
    setSelectedComponent(component);
  };

  const handleComponentMove = (componentId: string, newPosition: { x: number; y: number }) => {
    const updatedComponents = project.components.map(comp =>
      comp.id === componentId ? { ...comp, position: newPosition } : comp
    );
    
    onProjectUpdate({
      ...project,
      components: updatedComponents
    });
  };

  const handleComponentUpdate = (updatedComponent: WatchFaceComponent) => {
    const updatedComponents = project.components.map(comp =>
      comp.id === updatedComponent.id ? updatedComponent : comp
    );
    
    onProjectUpdate({
      ...project,
      components: updatedComponents
    });
    
    setSelectedComponent(updatedComponent);
  };

  const handleComponentDelete = (componentId: string) => {
    const updatedComponents = project.components.filter(comp => comp.id !== componentId);
    onProjectUpdate({
      ...project,
      components: updatedComponents
    });
    
    if (selectedComponent?.id === componentId) {
      setSelectedComponent(null);
    }
  };

  const exportProject = async () => {
    // Create project structure with proper file paths
    const projectName = project.watchName || 'MyWatchFace';
    const timestamp = new Date().toISOString().replace(/[:]/g, '-').split('.')[0];
    const projectFolderName = `${projectName}_${timestamp}`;
    
    // Collect all unique image URLs from components
    const imageUrls = new Set<string>();
    const imageMappings = new Map<string, string>(); // blob URL -> asset path
    
    project.components.forEach(comp => {
      comp.images.forEach(imageUrl => {
        if (imageUrl) {
          imageUrls.add(imageUrl);
        }
      });
    });

    // Convert blob URLs to actual files and create asset paths
    const imageFiles = new Map<string, File>();
    let imageIndex = 0;
    
    for (const imageUrl of imageUrls) {
      if (imageUrl.startsWith('blob:')) {
        try {
          const response = await fetch(imageUrl);
          const blob = await response.blob();
          
          // Generate a proper filename - start from 1, not 0
          const extension = blob.type.split('/')[1] || 'png';
          const filename = `image_${(imageIndex + 1).toString().padStart(3, '0')}.${extension}`;
          const assetPath = `assets/${filename}`;
          
          // Create File object
          const file = new File([blob], filename, { type: blob.type });
          imageFiles.set(assetPath, file);
          imageMappings.set(imageUrl, assetPath);
          
          imageIndex++;
        } catch (error) {
          console.warn('Failed to process image:', imageUrl, error);
          // Use original URL as fallback
          imageMappings.set(imageUrl, imageUrl);
        }
      } else {
        // For non-blob URLs, use them as-is or convert to assets path
        const filename = `external_${(imageIndex + 1).toString().padStart(3, '0')}.png`;
        const assetPath = `assets/${filename}`;
        imageMappings.set(imageUrl, assetPath);
        imageIndex++;
      }
    }

    // Generate the watch face JSON with proper asset paths
    const watchFaceData = {
      WatchName: project.watchName,
      WatchCode: project.watchCode,
      CornerX: null,
      CornerY: null,
      Width: project.width,
      Height: project.height,
      ThumbnailWidth: 46,
      ThumbnailHeight: 46,
      Corner: 0,
      DeviceType: project.deviceType,
      CreateTime: project.createTime,
      IsAlbum: false,
      ColorBit: project.colorBit,
      AlbumBackground: null,
      FolderName: projectFolderName,
      // Embed base64 images in the JSON
      ImageData: Object.fromEntries(imageFiles), // { "assets/image_001.png": "base64data..." }
      WatchStyles: [{
        StyleName: "style1",
        DragBindBases: project.components.map(comp => {
          // Convert image URLs to asset paths
          const convertedImages = comp.images.map(url => imageMappings.get(url) || url);
          
          // Debug logging - check component type
          console.log(`Processing component: "${comp.name}", type: "${comp.type}", type check: ${comp.type === 'analog'}`);

          if (comp.type === 'image') {
            return {
              $type: "WatchControlLibrary.DragBindImage, WatchControlLibrary",
              dragName: comp.name,
              height: comp.size.height,
              left: comp.position.x,
              top: comp.position.y,
              align: 0,
              elementType: comp.elementType,
              Source: convertedImages[0] || "",
              DragName: comp.name,
              Width: comp.size.width,
              DragId: comp.id,
              Height: comp.size.height,
              Left: comp.position.x,
              Top: comp.position.y,
              Align: 0,
              ElementType: comp.elementType,
              MaxNum: comp.config.maxValue || 0,
              MinNum: comp.config.minValue || 0,
              SubItems: [],
              Visable: true,
              ItemName: comp.config.itemName || comp.name.split(' ')[0],
              DefaultNum: comp.config.defaultValue || null,
              VerifyNullNum: comp.images.length === 11
            };
          } else if (comp.type === 'dateTime') {
            return {
              $type: "WatchControlLibrary.DragBindNormalDateTime, WatchControlLibrary",
              dragName: comp.name,
              height: comp.size.height,
              left: comp.position.x,
              top: comp.position.y,
              align: 0,
              elementType: comp.elementType,
              DateTimeFormat: comp.config.dateTimeFormat || "Hour",
              UnitIcon: null,
              ImageSource: convertedImages,
              SetDateTime: new Date().toISOString(),
              LeadingZero: comp.config.leadingZero || true,
              DragName: comp.name,
              Width: comp.size.width,
              DragId: comp.id,
              Height: comp.size.height,
              Left: comp.position.x,
              Top: comp.position.y,
              Align: 0,
              ElementType: comp.elementType,
              MaxNum: comp.config.maxValue || 0,
              MinNum: comp.config.minValue || 0,
              SubItems: [],
              Visable: true,
              ItemName: comp.config.itemName || comp.name.split(' ')[0],
              DefaultNum: comp.config.defaultValue || null,
              VerifyNullNum: comp.images.length === 11
            };
          } else if (comp.type === 'progress') {
            // For progress bars (like battery), use FillType: true to show as progress bar
            // For numeric progress counters, use FillType: false to show as digits
            const fillType = comp.config.fillType !== undefined ? comp.config.fillType : 
              (comp.config.itemName === 'Battery' || comp.config.itemName === 'Battery Progress') ? true : false;
            
            return {
              $type: "WatchControlLibrary.DragBindProgress, WatchControlLibrary",
              dragName: comp.name,
              height: comp.size.height,
              left: comp.position.x,
              top: comp.position.y,
              align: 0,
              elementType: comp.elementType,
              CurrentNum: 0,
              RatioNum: 0,
              TargetValue: comp.config.targetValue || comp.config.maxValue || 100,
              UnitSource: null,
              FillType: fillType,
              ImageSource: convertedImages,
              DragName: comp.name,
              Width: comp.size.width,
              DragId: comp.id,
              Height: comp.size.height,
              Left: comp.position.x,
              Top: comp.position.y,
              Align: 0,
              ElementType: comp.elementType,
              MaxNum: comp.config.maxValue || 0,
              MinNum: comp.config.minValue || 0,
              SubItems: [],
              Visable: true,
              ItemName: comp.config.itemName || comp.name.split(' ')[0],
              DefaultNum: comp.config.defaultValue || null,
              VerifyNullNum: comp.images.length === 11
            };
          } else if (comp.type === 'number') {
            // For number components, if there are 11 images, split them:
            // - First 10 images (0-9) go to ImageSource
            // - 11th image (index 10) goes to EmptySource (no data image)
            const hasEmptyImage = convertedImages.length === 11;
            const digitImages = hasEmptyImage ? convertedImages.slice(0, 10) : convertedImages;
            const emptyImage = hasEmptyImage ? convertedImages[10] : null;
            
            return {
              $type: "WatchControlLibrary.DragBindNums, WatchControlLibrary",
              dragName: comp.name,
              height: comp.size.height,
              left: comp.position.x,
              top: comp.position.y,
              align: 0,
              elementType: comp.elementType,
              CurrentNum: comp.config.defaultValue || 0,
              UnitSource: null,
              ImageSource: digitImages,
              MinusSource: null,
              EmptySource: emptyImage,
              DefaultNum: comp.config.defaultValue,
              DragName: comp.name,
              Width: comp.size.width,
              DragId: comp.id,
              Height: comp.size.height,
              Left: comp.position.x,
              Top: comp.position.y,
              Align: 0,
              ElementType: comp.elementType,
              MaxNum: comp.config.maxValue || 0,
              MinNum: comp.config.minValue || 0,
              SubItems: [],
              Visable: true,
              ItemName: comp.config.itemName || comp.name.split(' ')[0],
              VerifyNullNum: comp.images.length === 11
            };
          } else if (comp.type === 'analog') {
            // Analog clock hand (DragBindPoint) - matches Windows app format exactly
            return {
              $type: "WatchControlLibrary.DragBindPoint, WatchControlLibrary",
              dragName: comp.name,
              height: comp.size.height,
              left: comp.position.x,
              top: comp.position.y,
              align: 0,
              elementType: null,
              StartAngle: comp.config.startAngle || 0.0,
              EndAngle: comp.config.endAngle || 360.0,
              OriginPointX: comp.config.originPointX || 13.0,
              OriginPointY: comp.config.originPointY || 233.0,
              Source: convertedImages[0] || "",
              Value: comp.config.value || 0.0,
              Angle: comp.config.angle || 0.0,
              ValueIndex: comp.config.valueIndex || 0,
              DragName: comp.name,
              Width: comp.size.width,
              DragId: comp.id,
              Height: comp.size.height,
              Left: comp.position.x,
              Top: comp.position.y,
              Align: 0,
              ElementType: null,
              MaxNum: 0.0,
              MinNum: 0.0,
              SubItems: [],
              Visable: true,
              ItemName: comp.config.itemName || "Pointer",
              DefaultNum: null,
              VerifyNullNum: false
            };
          } else {
            // Fallback - should never reach here
            console.error('Unknown component type:', comp.type, comp);
            return {
              $type: "WatchControlLibrary.DragBindImage, WatchControlLibrary",
              dragName: comp.name,
              height: comp.size.height,
              left: comp.position.x,
              top: comp.position.y,
              align: 0,
              elementType: comp.elementType,
              Source: convertedImages[0] || "",
              DragName: comp.name,
              Width: comp.size.width,
              DragId: comp.id,
              Height: comp.size.height,
              Left: comp.position.x,
              Top: comp.position.y,
              Align: 0,
              ElementType: comp.elementType,
              MaxNum: comp.config.maxValue || 0,
              MinNum: comp.config.minValue || 0,
              SubItems: [],
              Visable: true,
              ItemName: comp.config.itemName || comp.name.split(' ')[0],
              DefaultNum: comp.config.defaultValue || null,
              VerifyNullNum: false
            };
          }
        }).filter(comp => comp !== null),
        TemplateBinds: [], 
        ScreenType: 0,
        Zh: {
          Width: 0,
          Height: 0,
          Item: {
            KwhNum: 0,
            StepNum: project.components.find(c => c.config.itemName === 'Steps')?.config.defaultValue || 7645,
            HeartRateNum: project.components.find(c => c.config.itemName === 'Heart Rate')?.config.defaultValue || 72,
            CalorieNum: project.components.find(c => c.config.itemName === 'Calories')?.config.defaultValue || 324,
            CurrentDateTime: new Date().toISOString(),
            StrengthNum: 0,
            IsOpen: false
          },
          DragBindBases: null
        }
      }]
    };

    // Copy TemplateBinds from DragBindBases
    watchFaceData.WatchStyles[0].TemplateBinds = [...watchFaceData.WatchStyles[0].DragBindBases];

    // Use JSZip to create a ZIP file with proper folder structure
    const zip = new JSZip();
    
    // Create project folder
    const projectFolder = zip.folder(projectFolderName);
    
    // Only add the JSON file with embedded base64 images (no separate asset files)
    const jsonString = JSON.stringify(watchFaceData, null, 2);
    projectFolder!.file(`${projectName}.json`, jsonString);
    
    // Add a readme file with instructions
    const readmeContent = `# Watch Face Project: ${projectName}

## Project Structure:
\`\`\`
${projectFolderName}/
├── ${projectName}.json          # Watch face configuration with embedded base64 images
└── README.md                    # This file
\`\`\`

## Usage for Backend Processing:
1. Extract this ZIP file to get the project folder
2. Send the entire project folder to your backend program
3. The backend will read ${projectName}.json with embedded base64 images
4. Backend will decode base64 images and generate the .bin file for the watch

## Component Details:
- Components: ${project.components.length}
- Image Assets: ${imageFiles.size} (embedded as base64)
- Watch Dimensions: ${project.width}x${project.height}
- Device Type: ${project.deviceType}

## Generated on: ${new Date().toLocaleString()}
## Export Format: Compatible with Windows WatchFace Designer backend (Base64 embedded images)
`;
    
    projectFolder!.file('README.md', readmeContent);
    
    // Generate and download the ZIP file
    try {
      const zipContent = await zip.generateAsync({ type: 'blob' });
      const url = URL.createObjectURL(zipContent);
      
      const link = document.createElement('a');
      link.href = url;
      link.download = `${projectFolderName}.zip`;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      URL.revokeObjectURL(url);
      
      console.log(`Exported project: ${projectFolderName}.zip`);
    } catch (error) {
      console.error('Export failed:', error);
      // Fallback to old method
      const blob = new Blob([jsonString], { type: 'application/json' });
      const url = URL.createObjectURL(blob);
      
      const link = document.createElement('a');
      link.href = url;
      link.download = 'watchFace.json';
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      URL.revokeObjectURL(url);
    }
  };

  const generateBinFile = async () => {
    try {
      setIsGenerating(true);
      
      console.log('\n╔════════════════════════════════════════╗');
      console.log('║   BIN File Generation Process          ║');
      console.log('╚════════════════════════════════════════╝\n');
      console.log('Current project:', project.watchName);
      
      // Validate project has components
      if (!project.components || project.components.length === 0) {
        alert('Please add components to your watch face before generating.');
        return;
      }
      
      console.log('📸 Step 1/4: Collecting images from components...');
      
      // Collect all image blobs from components (same as exportMoyFile)
      const imageData = new Map<string, Blob>();
      
      for (const component of project.components) {
        console.log(`  Component: ${component.name}, Type: ${component.type}, Images: ${component.images.length}`);
        
        for (const imageUrl of component.images) {
          if (imageUrl && imageUrl.startsWith('blob:') && !imageData.has(imageUrl)) {
            try {
              const response = await fetch(imageUrl);
              
              if (!response.ok) {
                console.error(`  ✗ Failed to fetch ${imageUrl}`);
                continue;
              }
              
              const blob = await response.blob();
              console.log(`  ✓ Fetched blob: ${blob.size} bytes, type: ${blob.type}`);
              
              // Verify it's actually a PNG
              const arrayBuffer = await blob.arrayBuffer();
              const bytes = new Uint8Array(arrayBuffer);
              const isPNG = bytes[0] === 0x89 && bytes[1] === 0x50 && bytes[2] === 0x4E && bytes[3] === 0x47;
              
              if (!isPNG) {
                console.error(`  ✗ Not a valid PNG: ${imageUrl}`);
                continue;
              }
              
              // Re-create blob from arrayBuffer to ensure it's valid
              const validBlob = new Blob([arrayBuffer], { type: 'image/png' });
              imageData.set(imageUrl, validBlob);
              
            } catch (error) {
              console.error(`  ✗ Failed to fetch image: ${imageUrl}`, error);
            }
          }
        }
      }
      
      console.log(`✓ Image collection complete: ${imageData.size} unique images\n`);
      
      console.log('🔧 Step 2/4: Generating MOY file...');
      
      // Generate MOY file (using MoyGenerator)
      const moyBlob = await MoyGenerator.exportMoyFile(project, imageData);
      console.log(`✓ MOY file generated: ${moyBlob.size} bytes\n`);
      
      console.log('📤 Step 3/4: Sending to backend for BIN conversion...');
      
      // Convert MOY blob to base64 for sending to backend
      const moyArrayBuffer = await moyBlob.arrayBuffer();
      const moyBase64 = btoa(
        new Uint8Array(moyArrayBuffer).reduce((data, byte) => data + String.fromCharCode(byte), '')
      );
      
      const moyFileName = `${project.watchName || 'MyWatchFace'}.moy`;
      
      // Send to Node.js backend
      const backendUrl = 'http://localhost:5555/api/generate-bin';
      
      const response = await fetch(backendUrl, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          moyBuffer: moyBase64,
          moyFileName: moyFileName,
        }),
      });
      
      if (!response.ok) {
        const errorData = await response.json().catch(() => ({ error: 'Unknown error' }));
        throw new Error(errorData.error || `Backend error: ${response.status}`);
      }
      
      const result = await response.json();
      
      if (!result.success) {
        throw new Error(result.error || 'BIN generation failed');
      }
      
      console.log(`✓ Backend conversion successful\n`);
      console.log(`  BIN file size: ${result.size} bytes`);
      console.log(`  BIN URL: ${result.binUrl}\n`);
      
      console.log('⬇️  Step 4/4: Downloading BIN file...');
      
      // Convert base64 back to blob and download
      const binBinaryString = atob(result.data);
      const binBytes = new Uint8Array(binBinaryString.length);
      for (let i = 0; i < binBinaryString.length; i++) {
        binBytes[i] = binBinaryString.charCodeAt(i);
      }
      const binBlob = new Blob([binBytes], { type: 'application/octet-stream' });
      
      // Download the BIN file
      const binFileName = moyFileName.replace('.moy', '.bin');
      const url = URL.createObjectURL(binBlob);
      const link = document.createElement('a');
      link.href = url;
      link.download = binFileName;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      URL.revokeObjectURL(url);
      
      console.log(`✓ BIN file downloaded: ${binFileName}\n`);
      console.log('╔════════════════════════════════════════╗');
      console.log('║   BIN Generation Complete! ✅          ║');
      console.log('╚════════════════════════════════════════╝\n');
      
      alert(`✅ BIN file generated successfully!\n\nFile: ${binFileName}\nSize: ${(result.size / 1024).toFixed(2)} KB\n\nYou can now transfer this file to your watch!`);
      
    } catch (error) {
      console.error('\n❌ Error generating BIN file:', error);
      
      const errorMessage = error.message || 'Unknown error occurred';
      
      // Provide helpful error messages
      if (errorMessage.includes('Failed to fetch') || errorMessage.includes('NetworkError')) {
        alert(`❌ BIN Generation Failed\n\nCannot connect to backend server.\n\nPlease ensure:\n1. Backend server is running (npm start in server folder)\n2. Server is accessible at http://localhost:5555\n3. Check console for more details`);
      } else if (errorMessage.includes('Authentication')) {
        alert(`❌ BIN Generation Failed\n\n${errorMessage}\n\nPlease check:\n1. MOYoung API credentials in server/.env file\n2. Server console for authentication errors`);
      } else {
        alert(`❌ BIN Generation Failed\n\n${errorMessage}\n\nPlease check the console for more details.`);
      }
    } finally {
      setIsGenerating(false);
    }
  };

  /**
   * Export MOY file for MoYoung vendor
   */
  const exportMoyFile = async () => {
    try {
      setIsGenerating(true);
      
      // Collect all image blobs from components
      const imageData = new Map<string, Blob>();
      
      console.log('=== Starting Image Collection ===');
      console.log('Total components:', project.components.length);
      
      for (const component of project.components) {
        console.log(`Component: ${component.name}, Type: ${component.type}, Images: ${component.images.length}`);
        
        for (const imageUrl of component.images) {
          if (imageUrl && imageUrl.startsWith('blob:') && !imageData.has(imageUrl)) {
            try {
              console.log(`Fetching blob: ${imageUrl}`);
              const response = await fetch(imageUrl);
              
              if (!response.ok) {
                console.error(`Failed to fetch ${imageUrl}: ${response.status} ${response.statusText}`);
                continue;
              }
              
              const blob = await response.blob();
              console.log(`✓ Fetched blob: ${blob.size} bytes, type: ${blob.type}`);
              
              // Verify it's actually a PNG
              const arrayBuffer = await blob.arrayBuffer();
              const bytes = new Uint8Array(arrayBuffer);
              const isPNG = bytes[0] === 0x89 && bytes[1] === 0x50 && bytes[2] === 0x4E && bytes[3] === 0x47;
              console.log(`  PNG signature valid: ${isPNG}`);
              
              // Re-create blob from arrayBuffer to ensure it's valid
              const validBlob = new Blob([arrayBuffer], { type: 'image/png' });
              imageData.set(imageUrl, validBlob);
              
            } catch (error) {
              console.error(`Failed to fetch image: ${imageUrl}`, error);
            }
          }
        }
      }
      
      console.log(`=== Image Collection Complete ===`);
      console.log(`Total unique images collected: ${imageData.size}`);
      console.log('Image URLs in map:', Array.from(imageData.keys()));
      
      // Generate MOY file
      const moyBlob = await MoyGenerator.exportMoyFile(project, imageData);
      
      // Download MOY file
      const filename = `${project.watchName || 'MyWatchFace'}.moy`;
      MoyGenerator.downloadMoyFile(moyBlob, filename);
      
      console.log(`Generated and downloaded MOY file: ${filename}`);
      alert(`✅ MOY file exported successfully!\n\nFile: ${filename}\n\nYou can now use this file with the MoYoung bin generator.`);
      
    } catch (error) {
      console.error('Error generating MOY file:', error);
      alert(`❌ Error generating MOY file: ${error.message || 'Unknown error occurred'}`);
    } finally {
      setIsGenerating(false);
    }
  };

  // Fabric.js Grid Functionality
  useEffect(() => {
    if (fabricCanvasRef.current && !fabricCanvas.current) {
      const canvas = new Canvas(fabricCanvasRef.current, {
        width: project.width,
        height: project.height,
        backgroundColor: 'transparent',
        selection: false,
        evented: false, // Completely disable events
        interactive: false, // Disable all interactions
        skipTargetFind: true, // Skip hit detection entirely
      });

      fabricCanvas.current = canvas;
      
      // Create initial grid if showGrid is true
      if (showGrid) {
        createGrid();
      }
    }

    return () => {
      if (fabricCanvas.current) {
        fabricCanvas.current.dispose();
        fabricCanvas.current = null;
      }
    };
  }, [project.width, project.height]); // Re-initialize if dimensions change

  // Update grid visibility when showGrid changes
  useEffect(() => {
    if (fabricCanvas.current) {
      if (showGrid) {
        createGrid();
      } else {
        removeGrid();
      }
    }
  }, [showGrid]);

  const createGrid = () => {
    if (!fabricCanvas.current) return;

    const canvas = fabricCanvas.current;
    const width = project.width;
    const height = project.height;
    const gridSize = 20; // Grid spacing in pixels
    const gridColor = '#00ff00'; // Bright green for visibility
    const gridOpacity = 0.6; // Higher opacity for visibility above components

    // Clear existing grid
    removeGrid();

    // Create vertical lines
    for (let x = 0; x <= width; x += gridSize) {
      const line = new Line([x, 0, x, height], {
        stroke: gridColor,
        strokeWidth: 1,
        opacity: gridOpacity,
        selectable: false,
        evented: false,
        data: { isGrid: true }
      });
      canvas.add(line);
    }

    // Create horizontal lines
    for (let y = 0; y <= height; y += gridSize) {
      const line = new Line([0, y, width, y], {
        stroke: gridColor,
        strokeWidth: 1,
        opacity: gridOpacity,
        selectable: false,
        evented: false,
        data: { isGrid: true }
      });
      canvas.add(line);
    }

    canvas.renderAll();
  };

  const removeGrid = () => {
    if (!fabricCanvas.current) return;

    const canvas = fabricCanvas.current;
    const objects = canvas.getObjects();
    
    // Remove all grid objects
    const gridObjects = objects.filter(obj => (obj as any).data?.isGrid);
    gridObjects.forEach(obj => canvas.remove(obj));
    
    canvas.renderAll();
  };

  const toggleGrid = () => {
    setShowGrid(!showGrid);
  };

  return (
    <div className="assembly-view view-container">
      <input 
        type="file" 
        ref={fileInputRef} 
        style={{ display: 'none' }} 
        multiple 
        accept="image/*"
      />
      

      <div className={`sidebar${isLibraryCollapsed ? ' collapsed' : ''}`}> 
        <div className="sidebar-header">
          <h3>Component Library</h3>
          <div style={{ display: 'flex', gap: 8 }}>
            <button className="btn btn-primary" onClick={() => handleImageUpload('image', null)}>
              <Upload size={16} /> Add Images
            </button>
            <button className="btn btn-secondary" style={{ minWidth: 32 }} onClick={() => setIsLibraryCollapsed(v => !v)} title={isLibraryCollapsed ? 'Expand' : 'Collapse'}>
              {isLibraryCollapsed ? '▶' : '◀'}
            </button>
          </div>
        </div>
        {!isLibraryCollapsed && (
          <ComponentLibrary 
            onAddTimeComponent={(timeComponentType, dateTimeFormat, digitType, digitPosition) => handleImageUpload('dateTime', 0, dateTimeFormat, timeComponentType, digitType, digitPosition)}
            onAddProgressComponent={(elementType) => handleImageUpload('progress', elementType)}
            onAddNumberComponent={(elementType, digitType, digitPosition) => {
              const componentName = elementType === 1 ? 'Blood Oxygen' : elementType === 3 ? 'Steps' : elementType === 4 ? 'Heart Rate' : elementType === 5 ? 'Calories' : 'Number';
              handleImageUpload('number', elementType, undefined, undefined, digitType, digitPosition, componentName);
            }}
            onAddImageComponent={() => handleImageUpload('image', null)}
            onAddComponent={handleTypedComponentAdd}
            onAddAnalogComponent={handleAnalogComponentAdd}
            onAddTimeSeparator={() => handleImageUpload('separator', 8, 'Separator', 8)}
            onAddHeartIcon={() => handleImageUpload('icon', 9, 'Heart Rate Icon', 9)}
            onAddStepsIcon={() => handleImageUpload('icon', 10, 'Steps Icon', 10)}
            onAddCalorieIcon={() => handleImageUpload('icon', 11, 'Calorie Icon', 11)}
          />
        )}
      </div>

      <div className="main-area">
        <div className="toolbar">
          <div className="toolbar-left">
            <span className="project-name">{project.watchName}</span>
            <div style={{ marginLeft: '20px', display: 'flex', alignItems: 'center', gap: '10px' }}>
              <label style={{ fontWeight: 'bold' }}>Watch Shape:</label>
              <select 
                value={`${project.watchShape}-${project.width}x${project.height}`} 
                onChange={(e) => {
                  const [shape, dimensions] = e.target.value.split('-');
                  const [width, height] = dimensions.split('x').map(Number);
                  const newProject = { 
                    ...project, 
                    watchShape: shape as 'square' | 'circle',
                    width,
                    height
                  };
                  onProjectUpdate(newProject);
                }}
                style={{ 
                  padding: '6px 12px', 
                  borderRadius: '4px', 
                  border: '1px solid #444',
                  backgroundColor: '#2a2a2a',
                  color: '#fff',
                  fontSize: '14px'
                }}
              >
                <option value="circle-466x466">Circle (466 × 466)</option>
                <option value="square-390x450">Square (390 × 450)</option>
                <option value="square-410x502">Square (410 × 502)</option>
                <option value="square-240x280">Square (240 × 280)</option>
              </select>
            </div>
          </div>
          <div className="toolbar-right">
            <button 
              className={`btn ${showGrid ? 'btn-primary' : 'btn-secondary'}`}
              onClick={toggleGrid}
              title="Toggle Guide Grid"
            >
              <Grid size={16} />
              {showGrid ? 'Hide Grid' : 'Show Grid'}
            </button>
            <button className="btn btn-secondary">
              <Settings size={16} />
              Settings
            </button>
            <button className="btn btn-primary" onClick={exportProject}>
              <Download size={16} />
              Export JSON
            </button>
            <button 
              className="btn btn-info" 
              onClick={exportMoyFile}
              disabled={isGenerating}
            >
              <Download size={16} />
              {isGenerating ? 'Generating...' : 'Export MOY'}
            </button>
            <button 
              className="btn btn-success" 
              onClick={generateBinFile}
              disabled={isGenerating}
            >
              <Download size={16} />
              {isGenerating ? 'Generating...' : 'Generate Bin File'}
            </button>
          </div>
        </div>

        <div className="canvas-area">
          <div className="watch-canvas" style={{ 
            position: 'relative',
            width: project.width,
            height: project.height,
            borderRadius: project.watchShape === 'circle' ? '50%' : '20px',
          }}>
            {/* Fabric.js Canvas for Grid Overlay */}
            <canvas
              ref={fabricCanvasRef}
              style={{
                position: 'absolute',
                top: 0,
                left: 0,
                width: '100%',
                height: '100%',
                pointerEvents: 'none', // Completely disable pointer events
                zIndex: showGrid ? 1000 : -1, // Show above everything when grid is on, hide when off
                userSelect: 'none',
                display: showGrid ? 'block' : 'none' // Completely hide when grid is off
              }}
            />
            
            {/* Component Layer */}
            <div style={{ position: 'absolute', top: 0, left: 0, width: '100%', height: '100%', zIndex: 100 }}>
              {project.components.map((component, index) => (
              <div
                key={component.id}
                className={`watch-component ${selectedComponent?.id === component.id ? 'selected' : ''}`}
                style={{
                  position: 'absolute',
                  left: component.position.x,
                  top: component.position.y,
                  width: component.size.width,
                  height: component.size.height,
                  border: selectedComponent?.id === component.id ? '2px solid #0066cc' : '1px solid transparent',
                  cursor: 'move',
                  zIndex: index // Components later in array have higher z-index (appear on top)
                }}
                onClick={() => handleComponentSelect(component)}
                onMouseDown={(e) => {
                  e.preventDefault();
                  const canvas = e.currentTarget.parentElement;
                  const rect = canvas!.getBoundingClientRect();
                  
                  // Calculate canvas-relative coordinates
                  const canvasX = e.clientX - rect.left;
                  const canvasY = e.clientY - rect.top;
                  
                  const startX = canvasX - component.position.x;
                  const startY = canvasY - component.position.y;
                  
                  const handleMouseMove = (e: MouseEvent) => {
                    const newCanvasX = e.clientX - rect.left;
                    const newCanvasY = e.clientY - rect.top;
                    
                    const newX = Math.max(0, Math.min(466 - component.size.width, newCanvasX - startX));
                    const newY = Math.max(0, Math.min(466 - component.size.height, newCanvasY - startY));
                    
                    handleComponentMove(component.id, { x: newX, y: newY });
                  };
                  
                  const handleMouseUp = () => {
                    document.removeEventListener('mousemove', handleMouseMove);
                    document.removeEventListener('mouseup', handleMouseUp);
                  };
                  
                  document.addEventListener('mousemove', handleMouseMove);
                  document.addEventListener('mouseup', handleMouseUp);
                }}
              >
                {component.images.length > 0 && (
                  <>
                    {component.config.digitPosition === 'multi' ? (
                      // For multi-digit components, show multiple digits
                      <div style={{ 
                        display: 'flex', 
                        width: '100%', 
                        height: '100%',
                        justifyContent: 'space-between'
                      }}>
                        {component.config.dateTimeFormat === 'Year' ? (
                          // Show 4 digits for year: 2025
                          <>
                            <img src={component.images[2]} alt="2" style={{ width: '22%', height: '100%', objectFit: 'contain' }} />
                            <img src={component.images[0]} alt="0" style={{ width: '22%', height: '100%', objectFit: 'contain' }} />
                            <img src={component.images[2]} alt="2" style={{ width: '22%', height: '100%', objectFit: 'contain' }} />
                            <img src={component.images[5]} alt="5" style={{ width: '22%', height: '100%', objectFit: 'contain' }} />
                          </>
                        ) : (
                          // Show 2 digits for hour/minute/etc: 12
                          <>
                            <img src={component.images[1]} alt="1" style={{ width: '45%', height: '100%', objectFit: 'contain' }} />
                            <img src={component.images[2]} alt="2" style={{ width: '45%', height: '100%', objectFit: 'contain' }} />
                          </>
                        )}
                      </div>
                    ) : (
                      // For single digit components, show one digit
                      <img 
                        src={component.images[0]} 
                        alt={component.name}
                        style={{ 
                          width: '100%', 
                          height: '100%', 
                          objectFit: 'contain',
                          pointerEvents: 'none'
                        }}
                      />
                    )}
                  </>
                )}
                <div className="component-label">
                  {component.name}
                </div>
              </div>
            ))}
            </div>
          </div>
        </div>
      </div>


      <div className={`properties-panel${isPropertiesCollapsed ? ' collapsed' : ''}`}> 
        <div className="properties-header">
          <h3>Properties</h3>
          <button className="btn btn-secondary" style={{ minWidth: 32 }} onClick={() => setIsPropertiesCollapsed(v => !v)} title={isPropertiesCollapsed ? 'Expand' : 'Collapse'}>
            {isPropertiesCollapsed ? '▶' : '◀'}
          </button>
        </div>
        {!isPropertiesCollapsed && (
          selectedComponent ? (
            <ComponentProperties 
              component={selectedComponent}
              onUpdate={handleComponentUpdate}
              onDelete={() => handleComponentDelete(selectedComponent.id)}
            />
          ) : (
            <div className="no-selection">
              <p>Select a component to edit its properties</p>
            </div>
          )
        )}
      </div>

      {/* Layer Manager Sidebar - Always visible */}

      <div className={`layer-manager-sidebar${isLayersCollapsed ? ' collapsed' : ''}`}> 
        <div className="layer-manager-header">
          <h3>Layers</h3>
          <span className="layer-count">{project.components.length}</span>
          <button className="btn btn-secondary" style={{ minWidth: 32, marginLeft: 8 }} onClick={() => setIsLayersCollapsed(v => !v)} title={isLayersCollapsed ? 'Expand' : 'Collapse'}>
            {isLayersCollapsed ? '▶' : '◀'}
          </button>
        </div>
        {!isLayersCollapsed && (
          <>
            <div className="layer-info-banner">
              <p>Top layers appear in front</p>
            </div>
            <div className="layer-list">
              {project.components.length === 0 ? (
                <div className="no-layers">
                  <p>No components yet</p>
                </div>
              ) : (
                project.components.map((component, index) => (
                  <div 
                    key={component.id} 
                    className={`layer-item ${selectedComponent?.id === component.id ? 'layer-selected' : ''}`}
                    onClick={() => setSelectedComponent(component)}
                  >
                    <div className="layer-controls">
                      <button 
                        className="layer-btn"
                        onClick={(e) => {
                          e.stopPropagation();
                          moveLayerUp(index);
                        }}
                        disabled={index === 0}
                        title="Move up (bring forward)"
                      >
                        ▲
                      </button>
                      <button 
                        className="layer-btn"
                        onClick={(e) => {
                          e.stopPropagation();
                          moveLayerDown(index);
                        }}
                        disabled={index === project.components.length - 1}
                        title="Move down (send backward)"
                      >
                        ▼
                      </button>
                    </div>
                    <div className="layer-info-text">
                      <span className="layer-index">{index + 1}</span>
                      <span className="layer-name">{component.name}</span>
                      <span className="layer-type">{component.type}</span>
                    </div>
                    <button 
                      className="layer-btn layer-delete"
                      onClick={(e) => {
                        e.stopPropagation();
                        deleteLayer(index);
                      }}
                      title="Delete layer"
                    >
                      🗑️
                    </button>
                  </div>
                ))
              )}
            </div>
          </>
        )}
      </div>

      {/* File Picker Confirmation Dialog */}
      {showFilePickerDialog && (
        <div className="modal-overlay" onClick={() => {
          setShowFilePickerDialog(false);
          setPendingFileUpload(null);
        }}>
          <div className="modal-content file-picker-dialog" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h2>Select Files</h2>
              <button className="modal-close" onClick={() => {
                setShowFilePickerDialog(false);
                setPendingFileUpload(null);
              }}>×</button>
            </div>
            
            <div className="modal-body">
              <p className="dialog-message">Click the button below to select image files for your component.</p>
              
              <div className="modal-actions">
                <button 
                  className="btn btn-secondary"
                  onClick={() => {
                    setShowFilePickerDialog(false);
                    setPendingFileUpload(null);
                  }}
                >
                  Cancel
                </button>
                <button 
                  className="btn btn-primary btn-large"
                  onClick={handleFileSelectFromDialog}
                >
                  <Upload size={16} />
                  Select Files
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};