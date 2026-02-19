import React, { useState, useEffect } from 'react';
import { Clock, Image, Battery, Heart, Activity, TrendingUp, Calendar, Sun, Moon } from 'lucide-react';
import { DataHelper, WatchSetting } from '../utils/DataHelper';
import { DragBindBase, BindMonitorType } from '../models/DragDataBase';
import { DataItemType, DataItemTypeHelper } from '../models/DataItemType';
import './ComponentLibrary.css';

interface ComponentLibraryProps {
  onAddTimeComponent: (timeComponentType: number, dateTimeFormat: string, digitType?: 'multi' | 'single', digitPosition?: string) => void;
  onAddProgressComponent: (elementType: number) => void;
  onAddNumberComponent: (elementType: number, digitType?: 'multi' | 'single', digitPosition?: string) => void;
  onAddImageComponent: () => void;
  onAddComponent?: (component: DragBindBase) => void; // Enhanced callback for TypeScript models
  onAddAnalogComponent: (handType: 'hour' | 'minute' | 'second') => void;
  onAddTimeSeparator: () => void; // For time_dot (colon separator)
  onAddHeartIcon: () => void; // For ic_heart (heart rate icon)
  onAddStepsIcon: () => void; // For ic_steps (steps icon)
  onAddCalorieIcon: () => void; // For ic_other (calorie icon)
}

const TIME_COMPONENTS = [
  { 
    name: 'Year', 
    format: 'Year', 
    icon: Calendar, 
    timeComponent: 0,
    isMultiDigit: true,
    digitCount: 4,
    subComponents: [
      { name: 'Year (Thousands)', position: 'thousands' },
      { name: 'Year (Hundreds)', position: 'hundreds' },
      { name: 'Year (Tens)', position: 'tens' },
      { name: 'Year (Ones)', position: 'ones' }
    ]
  },
  { 
    name: 'Month', 
    format: 'Month', 
    icon: Calendar, 
    timeComponent: 1,
    isMultiDigit: true,
    digitCount: 2,
    subComponents: [
      { name: 'Month (Tens)', position: 'tens' },
      { name: 'Month (Ones)', position: 'ones' }
    ]
  },
  { 
    name: 'Day', 
    format: 'Day', 
    icon: Calendar, 
    timeComponent: 2,
    isMultiDigit: true,
    digitCount: 2,
    subComponents: [
      { name: 'Day (Tens)', position: 'tens' },
      { name: 'Day (Ones)', position: 'ones' }
    ]
  },
  { 
    name: 'Hour', 
    format: 'Hour', 
    icon: Clock, 
    timeComponent: 3,
    isMultiDigit: true,
    digitCount: 2,
    subComponents: [
      { name: 'Hour (Tens)', position: 'tens' },
      { name: 'Hour (Ones)', position: 'ones' }
    ]
  },
  { 
    name: 'Minute', 
    format: 'Minute', 
    icon: Clock, 
    timeComponent: 4,
    isMultiDigit: true,
    digitCount: 2,
    subComponents: [
      { name: 'Minute (Tens)', position: 'tens' },
      { name: 'Minute (Ones)', position: 'ones' }
    ]
  },
  { 
    name: 'Second', 
    format: 'Second', 
    icon: Clock, 
    timeComponent: 5,
    isMultiDigit: true,
    digitCount: 2,
    subComponents: [
      { name: 'Second (Tens)', position: 'tens' },
      { name: 'Second (Ones)', position: 'ones' }
    ]
  },
  { 
    name: 'AM/PM', 
    format: 'AMPM', 
    icon: Clock, 
    timeComponent: 7,
    isMultiDigit: false
  }
];

export const ComponentLibrary: React.FC<ComponentLibraryProps> = ({
  onAddTimeComponent,
  onAddProgressComponent,
  onAddNumberComponent,
  onAddImageComponent,
  onAddComponent,
  onAddAnalogComponent,
  onAddTimeSeparator,
  onAddHeartIcon,
  onAddStepsIcon,
  onAddCalorieIcon
}) => {
  const [showComponentList, setShowComponentList] = useState(false);
  const [selectedComponent, setSelectedComponent] = useState<any>(null);
  const [showSubComponents, setShowSubComponents] = useState(false);
  const [excelComponents, setExcelComponents] = useState<WatchSetting[]>([]);
  const [isLoading, setIsLoading] = useState(false);

  // Enhanced component creation using TypeScript models
  const createTypedComponent = (setting: WatchSetting, images: string[] = []): DragBindBase | null => {
    const component = DataHelper.getDragBindBase('', setting, setting.ItemName, images);
    if (component) {
      // Set default position and size
      component.x = 100;
      component.y = 100;
      component.width = 100;
      component.height = 50;
      
      // Validate component
      const errors = DataHelper.validateComponent(component);
      if (errors.length > 0) {
        console.warn('Component validation warnings:', errors);
      }
    }
    return component;
  };

  // Enhanced component addition with TypeScript model support
  const handleEnhancedComponentAdd = (setting: WatchSetting) => {
    if (onAddComponent) {
      const component = createTypedComponent(setting, []);
      if (component) {
        onAddComponent(component);
        setShowComponentList(false);
      }
    } else {
      // Fallback to legacy method
      handleLegacyComponentAdd(setting);
    }
  };

  // Legacy component addition method
  const handleLegacyComponentAdd = (setting: WatchSetting) => {
    const bindType = setting.BindMonitorType || BindMonitorType.Time;
    
    switch (setting.ControlType) {
      case 'DragNormalDateTime':
      case 'DragSingleDigit':
        const timeComponent = DataHelper.mapBindMonitorTypeToElementType(bindType);
        onAddTimeComponent(timeComponent, setting.ItemName || '', 'multi');
        break;
      case 'DragProgress':
      case 'DragNums':
        onAddProgressComponent(bindType);
        break;
      case 'DragImage':
        onAddImageComponent();
        break;
      default:
        console.warn(`Unsupported component type: ${setting.ControlType}`);
    }
    setShowComponentList(false);
  };

  // Load Excel data when component mounts
  useEffect(() => {
    const loadExcelData = async () => {
      setIsLoading(true);
      try {
        await DataHelper.loadWatchSettings();
        const settings = DataHelper.watchSettings;
        // Filter for only specific components we want to show
        const allowedComponents = ['Hour', 'Minute', 'Second', 'Day', 'Month', 'Year', 'AM/PM', 'AMPM'];
        const timeComponents = settings.filter(setting => 
          setting.ItemName && allowedComponents.some(allowed => 
            setting.ItemName === allowed || 
            setting.ItemName?.toLowerCase() === allowed.toLowerCase()
          )
        );
        setExcelComponents(timeComponents);
      } catch (error) {
        console.error('Failed to load Excel data:', error);
      } finally {
        setIsLoading(false);
      }
    };

    loadExcelData();
  }, []);

  const handleSelectFile = () => {
    setShowComponentList(true);
    setShowSubComponents(false);
    setSelectedComponent(null);
  };

  const handleComponentSelect = (component: any) => {
    if (component.isMultiDigit) {
      setSelectedComponent(component);
      setShowSubComponents(true);
    } else {
      // Single digit component - add directly
      const timeComponent = component.watchSetting ? 
        DataHelper.mapBindMonitorTypeToElementType(component.watchSetting.BindMonitorType || 0) :
        component.timeComponent || 0;
      
      onAddTimeComponent(timeComponent, component.format, 'single');
      setShowComponentList(false);
    }
  };

  const handleSubComponentSelect = (subComponent: any) => {
    if (selectedComponent) {
      // Check if this is a number component (Steps)
      if (selectedComponent.elementType !== undefined) {
        onAddNumberComponent(selectedComponent.elementType, 'single', subComponent.position);
      } else {
        // Add single digit time component
        const timeComponent = selectedComponent.watchSetting ? 
          DataHelper.mapBindMonitorTypeToElementType(selectedComponent.watchSetting.BindMonitorType || 0) :
          selectedComponent.timeComponent || 0;
          
        onAddTimeComponent(timeComponent, selectedComponent.format, 'single', subComponent.position);
      }
    }
    setShowComponentList(false);
    setShowSubComponents(false);
    setSelectedComponent(null);
  };

  const handleMultiDigitSelect = () => {
    if (selectedComponent) {
      // Check if this is a number component (Steps)
      if (selectedComponent.elementType !== undefined) {
        onAddNumberComponent(selectedComponent.elementType, 'multi');
      } else {
        // Add multi-digit time component
        const timeComponent = selectedComponent.watchSetting ? 
          DataHelper.mapBindMonitorTypeToElementType(selectedComponent.watchSetting.BindMonitorType || 0) :
          selectedComponent.timeComponent || 0;
          
        onAddTimeComponent(timeComponent, selectedComponent.format, 'multi');
      }
    }
    setShowComponentList(false);
    setShowSubComponents(false);
    setSelectedComponent(null);
  };

  if (showSubComponents && selectedComponent) {
    return (
      <div className="component-library">
        <div className="component-list-header">
          <h3>{selectedComponent.name} Options</h3>
          <div className="header-buttons">
            <button className="btn btn-secondary" onClick={() => setShowSubComponents(false)}>
              Back
            </button>
            <button className="close-btn" onClick={() => setShowComponentList(false)}>
              Close
            </button>
          </div>
        </div>
        
        <div className="component-list">
          <button 
            className="component-list-item multi-digit"
            onClick={handleMultiDigitSelect}
          >
            {React.createElement(selectedComponent.icon, { size: 20 })}
            <span>{selectedComponent.name} (All {selectedComponent.digitCount} digits)</span>
          </button>
          
          <div className="component-section-divider">Individual Digits</div>
          
          {selectedComponent.subComponents.map((subComp: any, index: number) => {
            const IconComponent = selectedComponent.icon;
            return (
              <button 
                key={index}
                className="component-list-item"
                onClick={() => handleSubComponentSelect(subComp)}
              >
                <IconComponent size={20} />
                <span>{subComp.name}</span>
              </button>
            );
          })}
        </div>
      </div>
    );
  }

  if (showComponentList) {
    return (
      <div className="component-library">
        <div className="component-list-header">
          <h3>Select Component Type</h3>
          <button className="close-btn" onClick={() => setShowComponentList(false)}>
            Close
          </button>
        </div>
        
        <div className="component-list">
          {isLoading ? (
            <div style={{ textAlign: 'center', padding: '20px', color: '#fff' }}>
              Loading components from Excel...
            </div>
          ) : (
            <>
              {/* Excel-based components first */}
              {excelComponents.map((setting, index) => {
                const isMultiDigit = setting.ItemName?.includes('Hour') || 
                                   setting.ItemName?.includes('Minute') || 
                                   setting.ItemName?.includes('Second') ||
                                   setting.ItemName?.includes('Day') ||
                                   setting.ItemName?.includes('Month') ||
                                   setting.ItemName?.includes('Year');
                
                const component = {
                  name: setting.ItemName || '',
                  format: setting.ItemName || '',
                  icon: setting.ItemName?.includes('Hour') || setting.ItemName?.includes('Minute') || setting.ItemName?.includes('Second') ? Clock : Calendar,
                  timeComponent: DataHelper.mapBindMonitorTypeToElementType(setting.BindMonitorType || 0),
                  isMultiDigit,
                  digitCount: setting.ItemName?.includes('Year') ? 4 : 2,
                  subComponents: isMultiDigit ? (setting.ItemName?.includes('Year') ? [
                    { name: `${setting.ItemName} (Thousands)`, position: 'thousands' },
                    { name: `${setting.ItemName} (Hundreds)`, position: 'hundreds' },
                    { name: `${setting.ItemName} (Tens)`, position: 'tens' },
                    { name: `${setting.ItemName} (Ones)`, position: 'ones' }
                  ] : [
                    { name: `${setting.ItemName} (Tens)`, position: 'tens' },
                    { name: `${setting.ItemName} (Ones)`, position: 'ones' }
                  ]) : undefined,
                  watchSetting: setting
                };
                
                const IconComponent = component.icon;
                return (
                  <button 
                    key={`excel-${index}`}
                    className="component-list-item"
                    onClick={() => handleComponentSelect(component)}
                  >
                    <IconComponent size={20} />
                    <span>{component.name}</span>
                  </button>
                );
              })}
              
              {/* Fallback to hardcoded components if Excel has no data */}
              {excelComponents.length === 0 && TIME_COMPONENTS.map((component, index) => {
                const IconComponent = component.icon;
                return (
                  <button 
                    key={`fallback-${index}`}
                    className="component-list-item"
                    onClick={() => handleComponentSelect(component)}
                  >
                    <IconComponent size={20} />
                    <span>{component.name}</span>
                  </button>
                );
              })}
            </>
          )}
          
          <div className="component-section-divider">Time Separator</div>
          
          <button className="component-list-item" onClick={() => { onAddTimeSeparator(); setShowComponentList(false); }}>
            <Clock size={20} />
            <span>Time Separator (:)</span>
          </button>
          
          <div className="component-section-divider">Icons</div>
          
          <button className="component-list-item" onClick={() => { onAddHeartIcon(); setShowComponentList(false); }}>
            <Heart size={20} />
            <span>Heart Rate Icon</span>
          </button>
          
          <button className="component-list-item" onClick={() => { onAddStepsIcon(); setShowComponentList(false); }}>
            <Activity size={20} />
            <span>Steps Icon</span>
          </button>
          
          <button className="component-list-item" onClick={() => { onAddCalorieIcon(); setShowComponentList(false); }}>
            <TrendingUp size={20} />
            <span>Calorie Icon</span>
          </button>
          
          <div className="component-section-divider">Health & Fitness</div>
          
          <button className="component-list-item" onClick={() => handleComponentSelect({
            name: 'Blood Oxygen',
            format: 'Blood Oxygen',
            icon: Heart,
            elementType: 1,
            isMultiDigit: true,
            digitCount: 3,
            subComponents: [
              { name: 'Blood Oxygen (Hundreds)', position: 'hundreds' },
              { name: 'Blood Oxygen (Tens)', position: 'tens' },
              { name: 'Blood Oxygen (Ones)', position: 'ones' }
            ]
          })}>
            <Heart size={20} />
            <span>Blood Oxygen</span>
          </button>
          
          <button className="component-list-item" onClick={() => { onAddProgressComponent(2); setShowComponentList(false); }}>
            <Battery size={20} />
            <span>Battery Progress</span>
          </button>
          
          <button className="component-list-item" onClick={() => handleComponentSelect({
            name: 'Steps',
            format: 'Steps',
            icon: Activity,
            elementType: 3,
            isMultiDigit: true,
            digitCount: 5,
            subComponents: [
              { name: 'Steps (Ten Thousands)', position: 'ten-thousands' },
              { name: 'Steps (Thousands)', position: 'thousands' },
              { name: 'Steps (Hundreds)', position: 'hundreds' },
              { name: 'Steps (Tens)', position: 'tens' },
              { name: 'Steps (Ones)', position: 'ones' }
            ]
          })}>
            <Activity size={20} />
            <span>Steps</span>
          </button>
          
          <button className="component-list-item" onClick={() => handleComponentSelect({
            name: 'Heart Rate',
            format: 'Heart Rate',
            icon: TrendingUp,
            elementType: 4,
            isMultiDigit: true,
            digitCount: 3,
            subComponents: [
              { name: 'Heart Rate (Hundreds)', position: 'hundreds' },
              { name: 'Heart Rate (Tens)', position: 'tens' },
              { name: 'Heart Rate (Ones)', position: 'ones' }
            ]
          })}>
            <TrendingUp size={20} />
            <span>Heart Rate</span>
          </button>
          
          <button className="component-list-item" onClick={() => handleComponentSelect({
            name: 'Calories',
            format: 'Calories',
            icon: Activity,
            elementType: 5,
            isMultiDigit: true,
            digitCount: 4,
            subComponents: [
              { name: 'Calories (Thousands)', position: 'thousands' },
              { name: 'Calories (Hundreds)', position: 'hundreds' },
              { name: 'Calories (Tens)', position: 'tens' },
              { name: 'Calories (Ones)', position: 'ones' }
            ]
          })}>
            <Activity size={20} />
            <span>Calories</span>
          </button>
          
          <div className="component-section-divider">Analog Time</div>
          
          <button className="component-list-item" onClick={() => { onAddAnalogComponent('hour'); setShowComponentList(false); }}>
            <Clock size={20} />
            <span>Hour Hand</span>
          </button>
          
          <button className="component-list-item" onClick={() => { onAddAnalogComponent('minute'); setShowComponentList(false); }}>
            <Clock size={20} />
            <span>Minute Hand</span>
          </button>
          
          <button className="component-list-item" onClick={() => { onAddAnalogComponent('second'); setShowComponentList(false); }}>
            <Clock size={20} />
            <span>Second Hand</span>
          </button>
          
          <div className="component-section-divider">Graphics</div>
          
          <button className="component-list-item" onClick={() => { onAddImageComponent(); setShowComponentList(false); }}>
            <Image size={20} />
            <span>Background Image</span>
          </button>
        </div>
        
        <div className="component-list-footer">
          <button className="btn btn-secondary" onClick={() => setShowComponentList(false)}>
            Close
          </button>
          <button className="btn btn-primary" onClick={() => setShowComponentList(false)}>
            Add
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="component-library">
      <div className="component-section">
        <h4>Component Selection</h4>
        <button className="select-file-btn" onClick={handleSelectFile}>
          Select File
        </button>
        <p className="help-text">
          Click "Select File" to choose from available component types, 
          just like in the Windows app.
        </p>
      </div>

      <div className="component-section">
        <h4>Instructions</h4>
        <div className="instruction-text">
          <p><strong>1. Select File:</strong> Choose component type from the list</p>
          <p><strong>2. Upload Images:</strong> For digits upload 10 images (0-9)</p>
          <p><strong>3. Position:</strong> Drag elements on the canvas</p>
          <p><strong>4. Preview:</strong> Use sliders to test your watch face</p>
        </div>
      </div>
    </div>
  );
};