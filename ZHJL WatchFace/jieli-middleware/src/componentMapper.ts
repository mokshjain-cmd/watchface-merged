import { DragBindComponent, JieLiFolderConfig, JieLiComponentCode } from './types';

/**
 * Maps watchFace.json components to JieLi folder structure
 */
export class ComponentMapper {
  /**
   * Determine JieLi component code based on component type and ItemName
   */
  static getJieLiCode(component: DragBindComponent, index: number = 0): string {
    const itemName = component.ItemName;
    const type = component.$type;

    // Separator components
    if (itemName === 'Time Separator' || itemName.includes('Time Separator')) {
      return JieLiComponentCode.TIME_SEPARATOR1;
    }
    if (itemName === 'Date Separator' || itemName.includes('Date Separator')) {
      return JieLiComponentCode.DATE_SEPARATOR;
    }

    // Background/Image components
    if (type.includes('DragBindImage')) {
      if (itemName.includes('Background') || itemName === 'Background Image') {
        return JieLiComponentCode.MAIN_BACKGROUND;
      }
      if (itemName.includes('Bluetooth')) {
        return JieLiComponentCode.MAIN_BLUETOOTH;
      }
      // Default to background
      return JieLiComponentCode.MAIN_BACKGROUND;
    }

    // Analog clock pointers
    if (type.includes('DragBindPoint')) {
      if (component.ValueIndex === 0 || itemName.includes('Hour')) {
        return JieLiComponentCode.POINTER_HOUR;
      }
      if (component.ValueIndex === 1 || itemName.includes('Minute')) {
        return JieLiComponentCode.POINTER_MINUTE;
      }
      if (component.ValueIndex === 2 || itemName.includes('Second')) {
        return JieLiComponentCode.POINTER_SECOND;
      }
    }

    // Battery components
    if (itemName === 'Battery' || itemName === 'Battery Progress') {
      if (type.includes('DragBindProgress')) {
        return JieLiComponentCode.BATTERY_PROGRESS;
      }
    }

    // Time components
    if (itemName.includes('Hour') || itemName.includes('时')) {
      // Check for explicit digit position in name first
      if (itemName.includes('(Ones)') || itemName.includes('Ones)') || itemName.includes('个')) {
        return JieLiComponentCode.TIME_HOUR_ONES;
      }
      if (itemName.includes('(Tens)') || itemName.includes('Tens)') || itemName.includes('十')) {
        return JieLiComponentCode.TIME_HOUR_TENS;
      }
      // Fall back to index-based logic for components without explicit position
      if (index === 0) {
        return JieLiComponentCode.TIME_HOUR_TENS;
      }
      return JieLiComponentCode.TIME_HOUR_ONES;
    }
    if (itemName.includes('Minute') || itemName.includes('分')) {
      // Check for explicit digit position in name first
      if (itemName.includes('(Ones)') || itemName.includes('Ones)') || itemName.includes('个')) {
        return JieLiComponentCode.TIME_MINUTE_ONES;
      }
      if (itemName.includes('(Tens)') || itemName.includes('Tens)') || itemName.includes('十')) {
        return JieLiComponentCode.TIME_MINUTE_TENS;
      }
      // Fall back to index-based logic for components without explicit position
      if (index === 0) {
        return JieLiComponentCode.TIME_MINUTE_TENS;
      }
      return JieLiComponentCode.TIME_MINUTE_ONES;
    }
    if (itemName.includes('Second') || itemName.includes('秒')) {
      // Check for explicit digit position in name first
      if (itemName.includes('(Ones)') || itemName.includes('Ones)') || itemName.includes('个')) {
        return JieLiComponentCode.TIME_SECOND_ONES;
      }
      if (itemName.includes('(Tens)') || itemName.includes('Tens)') || itemName.includes('十')) {
        return JieLiComponentCode.TIME_SECOND_TENS;
      }
      // Fall back to index-based logic for components without explicit position
      if (index === 0) {
        return JieLiComponentCode.TIME_SECOND_TENS;
      }
      return JieLiComponentCode.TIME_SECOND_ONES;
    }

    // Date components
    if (itemName.includes('Month') || itemName.includes('月')) {
      // Check for explicit digit position in name first
      if (itemName.includes('(Ones)') || itemName.includes('Ones)') || itemName.includes('个')) {
        return JieLiComponentCode.DATE_MONTH_ONES;
      }
      if (itemName.includes('(Tens)') || itemName.includes('Tens)') || itemName.includes('十')) {
        return JieLiComponentCode.DATE_MONTH_TENS;
      }
      // Fall back to index-based logic for components without explicit position
      if (index === 0) {
        return JieLiComponentCode.DATE_MONTH_TENS;
      }
      return JieLiComponentCode.DATE_MONTH_ONES;
    }
    if (itemName.includes('Day') || itemName.includes('日')) {
      // Check for explicit digit position in name first
      if (itemName.includes('(Ones)') || itemName.includes('Ones)') || itemName.includes('个')) {
        return JieLiComponentCode.DATE_DAY_ONES;
      }
      if (itemName.includes('(Tens)') || itemName.includes('Tens)') || itemName.includes('十')) {
        return JieLiComponentCode.DATE_DAY_TENS;
      }
      // Fall back to index-based logic for components without explicit position
      if (index === 0) {
        return JieLiComponentCode.DATE_DAY_TENS;
      }
      return JieLiComponentCode.DATE_DAY_ONES;
    }
    if (itemName.includes('Year') || itemName.includes('年')) {
      // Check for explicit digit position in name first
      if (itemName.includes('(Ones)') || itemName.includes('Ones)') || itemName.includes('个')) {
        return JieLiComponentCode.DATE_YEAR_ONES;
      }
      if (itemName.includes('(Tens)') || itemName.includes('Tens)') || itemName.includes('十')) {
        return JieLiComponentCode.DATE_YEAR_TENS;
      }
      if (itemName.includes('(Hundreds)') || itemName.includes('Hundreds)') || itemName.includes('百')) {
        return JieLiComponentCode.DATE_YEAR_HUNDREDS;
      }
      if (itemName.includes('(Thousands)') || itemName.includes('Thousands)') || itemName.includes('千')) {
        return JieLiComponentCode.DATE_YEAR_THOUSANDS;
      }
      // Fall back to index-based logic for components without explicit position
      // Year has 4 digits: thousands, hundreds, tens, ones
      if (index === 0) {
        return JieLiComponentCode.DATE_YEAR_THOUSANDS;
      } else if (index === 1) {
        return JieLiComponentCode.DATE_YEAR_HUNDREDS;
      } else if (index === 2) {
        return JieLiComponentCode.DATE_YEAR_TENS;
      }
      return JieLiComponentCode.DATE_YEAR_ONES;
    }
    if (itemName === 'Week' || itemName.includes('星期')) {
      return JieLiComponentCode.OTHER_WEEK;
    }
    if (itemName === 'AMPM' || itemName === 'AM/PM') {
      return JieLiComponentCode.OTHER_AMPM;
    }

    // Health/Activity components
    if (itemName.includes('Steps') || itemName.includes('步数')) {
      if (type.includes('DragBindProgress')) {
        return JieLiComponentCode.STEPS_PROGRESS;
      }
      // Check for explicit digit position in name
      if (itemName.includes('(Ones)') || itemName.includes('Ones)') || itemName.includes('个')) {
        return JieLiComponentCode.STEPS_DIGIT_1; // 0709 - Ones place
      }
      if (itemName.includes('(Tens)') || itemName.includes('Tens)') || itemName.includes('十')) {
        return JieLiComponentCode.STEPS_DIGIT_10; // 0708 - Tens place
      }
      if (itemName.includes('(Hundreds)') || itemName.includes('Hundreds)') || itemName.includes('百')) {
        return JieLiComponentCode.STEPS_DIGIT_100; // 0707 - Hundreds place
      }
      if (itemName.includes('(Thousands)') || itemName.includes('Thousands)') || itemName.includes('千')) {
        return JieLiComponentCode.STEPS_DIGIT_1K; // 0706 - Thousands place
      }
      if (itemName.includes('(Ten-Thousands)') || itemName.includes('Ten-Thousands)') || itemName.includes('万')) {
        return JieLiComponentCode.STEPS_DIGIT_10K; // 0705 - Ten-thousands place
      }
      // Default for multi-digit (will be incremented)
      return JieLiComponentCode.STEPS_DIGIT_10K;
    }
    if (itemName.includes('Heart Rate') || itemName.includes('心率')) {
      if (component.EmptySource) {
        return JieLiComponentCode.HEART_NO_DATA;
      }
      // Check for explicit digit position in name
      if (itemName.includes('(Ones)') || itemName.includes('Ones)') || itemName.includes('个')) {
        return JieLiComponentCode.HEART_DIGIT_1; // 0806 - Ones place
      }
      if (itemName.includes('(Tens)') || itemName.includes('Tens)') || itemName.includes('十')) {
        return JieLiComponentCode.HEART_DIGIT_10; // 0805 - Tens place
      }
      if (itemName.includes('(Hundreds)') || itemName.includes('Hundreds)') || itemName.includes('百')) {
        return JieLiComponentCode.HEART_DIGIT_100; // 0804 - Hundreds place
      }
      // Default for multi-digit (will be incremented)
      return JieLiComponentCode.HEART_DIGIT_100;
    }
    if (itemName.includes('Calories') || itemName.includes('卡路里')) {
      if (type.includes('DragBindProgress')) {
        return JieLiComponentCode.CALORIE_PROGRESS;
      }
      // Check for explicit digit position in name
      if (itemName.includes('(Ones)') || itemName.includes('Ones)') || itemName.includes('个')) {
        return JieLiComponentCode.CALORIE_DIGIT_1; // 0908 - Ones place
      }
      if (itemName.includes('(Tens)') || itemName.includes('Tens)') || itemName.includes('十')) {
        return JieLiComponentCode.CALORIE_DIGIT_10; // 0907 - Tens place
      }
      if (itemName.includes('(Hundreds)') || itemName.includes('Hundreds)') || itemName.includes('百')) {
        return JieLiComponentCode.CALORIE_DIGIT_100; // 0906 - Hundreds place
      }
      if (itemName.includes('(Thousands)') || itemName.includes('Thousands)') || itemName.includes('千')) {
        return JieLiComponentCode.CALORIE_DIGIT_1K; // 0905 - Thousands place
      }
      // Default for multi-digit
      return JieLiComponentCode.CALORIE_DIGIT_1K;
    }
    if (itemName.includes('Sleep') || itemName.includes('睡眠')) {
      if (type.includes('DragBindProgress')) {
        return JieLiComponentCode.SLEEP_PROGRESS;
      }
      if (component.EmptySource) {
        return JieLiComponentCode.SLEEP_NO_DATA;
      }
      // Check for explicit digit position in name
      if (itemName.includes('(Ones)') || itemName.includes('Ones)') || itemName.includes('个')) {
        return JieLiComponentCode.SLEEP_DIGIT_1; // 1006 - Ones place
      }
      if (itemName.includes('(Tens)') || itemName.includes('Tens)') || itemName.includes('十')) {
        return JieLiComponentCode.SLEEP_DIGIT_10; // 1005 - Tens place
      }
      if (itemName.includes('(Hundreds)') || itemName.includes('Hundreds)') || itemName.includes('百')) {
        return JieLiComponentCode.SLEEP_DIGIT_100; // 1004 - Hundreds place
      }
      // Default for multi-digit
      return JieLiComponentCode.SLEEP_DIGIT_100;
    }

    // Default fallback
    return JieLiComponentCode.MAIN_BACKGROUND;
  }

  /**
   * Get Chinese category name
   */
  static getCategory(code: string): string {
    const codeNum = parseInt(code.substring(0, 2));
    
    switch (codeNum) {
      case 1: return '主要';
      case 2: return '电量';
      case 3: return '通用日期';
      case 4: return '中文日期';
      case 5: return '其他日期';
      case 6: return '时间';
      case 7: return '步数';
      case 8: return '心率';
      case 9: return '卡路里';
      case 10: return '睡眠';
      case 11: return '效果';
      case 12: return '点击区域';
      case 13: return '指针';
      case 14: return '进度条#电量';
      case 15: return '进度条#步数';
      case 16: return '进度条#卡路里';
      case 17: return '进度条#睡眠';
      default: return '主要';
    }
  }

  /**
   * Get layer type identifier
   */
  static getLayerType(component: DragBindComponent): string {
    // 1 = 图片 (Image)
    // 2 = 文字 (Text)
    // 3 = 数字 (Number/Digit)
    
    if (component.$type.includes('DragBindImage') || 
        component.$type.includes('DragBindPoint') ||
        component.$type.includes('DragBindProgress')) {
      return '1(图片)';
    }
    
    if (component.$type.includes('DragBindNormalDateTime') ||
        component.$type.includes('DragBindNums')) {
      return '3(数字)';
    }
    
    return '1(图片)';
  }

  /**
   * Get element name in Chinese
   */
  static getElementName(component: DragBindComponent, code: string): string {
    const itemName = component.ItemName;
    
    // Mapping based on JieLi codes
    const elementMap: Record<string, string> = {
      '0101': '1背景',
      '0102': '2蓝牙',
      '0201': '电量进度条',
      '0202': '文字',
      '0203': '图片',
      '0204': '电量百分号',
      '0205': '3(百)',
      '0206': '4(十)',
      '0207': '5(个)',
      '0301': '2(月)#4(十)',
      '0302': '2(月)#5(个)',
      '0303': '分隔',
      '0304': '3(日)#4(十)',
      '0305': '3(日)#5(个)',
      '0306': '1(年)#1(千)',
      '0307': '1(年)#2(百)',
      '0308': '1(年)#3(十)',
      '0309': '1(年)#4(个)',
      '0501': '4(星期)',
      '0502': '4(上下午)',
      '0601': '1(时)#4(十)',
      '0602': '1(时)#5(个)',
      '0603': '分隔1',
      '0604': '2(分)#4(十)',
      '0605': '2(分)#5(个)',
      '0606': '分隔2',
      '0607': '3(秒)#5(十)',
      '0608': '3(秒)#5(个)',
      '0701': '进度条',
      '0702': '文字',
      '0703': '单位',
      '0705': '数值#1(万)',
      '0706': '数值#2(千)',
      '0707': '数值#3(百)',
      '0708': '数值#4(十)',
      '0709': '数值#5(个)',
      '0710': '普通',
      '0801': '文字',
      '0802': '单位',
      '0803': '无数据',
      '0804': '数值#3(百)',
      '0805': '数值#4(十)',
      '0806': '数值#5(个)',
      '0807': '普通',
      '0901': '进度条',
      '0902': '文字',
      '0903': '单位',
      '0905': '数值#2(千)',
      '0906': '数值#3(百)',
      '0907': '数值#4(十)',
      '0908': '数值#5(个)',
      '0909': '普通',
      '1001': '进度条',
      '1002': '单位',
      '1003': '无数据',
      '1004': '数值#3(百)',
      '1005': '数值#4(十)',
      '1006': '数值#5(个)',
      '1007': '普通',
      '1301': '时针',
      '1302': '分针',
      '1303': '秒针',
    };

    return elementMap[code] || itemName || '元素';
  }

  /**
   * Convert component to JieLi folder configuration
   */
  static toJieLiFolderConfig(component: DragBindComponent, index: number = 0): JieLiFolderConfig {
    const code = this.getJieLiCode(component, index);
    const category = this.getCategory(code);
    const layerType = this.getLayerType(component);
    const elementName = this.getElementName(component, code);
    
    // Display flag behavior in JieLi:
    // - For time/date: 10 = show leading zeros, 0 = hide leading zeros
    // - For health metrics: Should always be 10 (visible), but check LeadingZero property
    const itemName = component.ItemName || '';
    const isHealthMetric = itemName.includes('Steps') || itemName.includes('步数') ||
                          itemName.includes('Heart Rate') || itemName.includes('心率') ||
                          itemName.includes('Calories') || itemName.includes('卡路里') ||
                          itemName.includes('Sleep') || itemName.includes('睡眠');
    
    // Check if component has LeadingZero property
    const hasLeadingZero = (component as any).LeadingZero !== false;
    
    let displayFlag = 10; // Default: visible
    if (!component.Visable) {
      displayFlag = 0; // Hidden
    } else if (!hasLeadingZero && !isHealthMetric) {
      // For time/date components with LeadingZero=false, use 0
      displayFlag = 0;
    }
    
    // Get images - prefer ImageData (base64) over Source/ImageSource (file paths)
    let images: string[] = [];
    
    // Check ImageData first (contains actual base64 data or comma-separated base64 strings)
    if (component.ImageData) {
      if (typeof component.ImageData === 'string') {
        // Check if it's a comma-separated list or single image
        if (component.ImageData.includes(',') && !component.ImageData.startsWith('data:')) {
          // Comma-separated list of base64 strings
          images = component.ImageData.split(',').map((img: string) => img.trim());
        } else {
          // Single base64 data URL
          images = [component.ImageData];
        }
      } else if (Array.isArray(component.ImageData)) {
        images = component.ImageData;
      }
    }
    // Fall back to Source/ImageSource if ImageData not available
    else if (component.Source) {
      images = [component.Source];
    } else if (component.ImageSource) {
      images = component.ImageSource;
    }
    
    // Include EmptySource (no data image) if present
    if (component.EmptySource) {
      images.push(component.EmptySource);
    }

    return {
      code,
      category,
      layerType,
      elementName,
      displayFlag,
      x: component.Left || 0,
      y: component.Top || 0,
      images
    };
  }

  /**
   * Generate folder name following JieLi convention
   * Pattern: XXXX_类别#图层类型#元素名_显示标志_X_Y
   */
  static generateFolderName(config: JieLiFolderConfig): string {
    return `${config.code}_${config.category}#${config.layerType}#${config.elementName}_${config.displayFlag}_${config.x}_${config.y}`;
  }

  /**
   * Handle multi-digit number components
   * For components like Steps with 5 digits, create 5 separate folders
   */
  static splitMultiDigitComponent(component: DragBindComponent): JieLiFolderConfig[] {
    const configs: JieLiFolderConfig[] = [];
    const itemName = component.ItemName;
    
    // Check if this is a specific digit component (Tens/Ones) - if so, treat as single digit
    const isSingleDigit = itemName.includes('Tens') || itemName.includes('Ones') || 
                          itemName.includes('(Tens)') || itemName.includes('(Ones)') ||
                          itemName.includes('Hundreds') || itemName.includes('(Hundreds)') ||
                          itemName.includes('Thousands') || itemName.includes('(Thousands)') ||
                          itemName.includes('Single') || itemName.includes('(Single)') ||
                          itemName.includes('十') || itemName.includes('个') ||
                          itemName.includes('百') || itemName.includes('千') || itemName.includes('万');
    
    if (isSingleDigit) {
      // This is a specific digit component - use exact coordinates, no auto-calculation
      console.log(`[Split] ${itemName} is a single digit component - using exact coordinates`);
      return [this.toJieLiFolderConfig(component, 0)];
    }
    
    // Determine digit count based on MaxNum
    const maxNum = component.MaxNum || 0;
    let digitCount = maxNum.toString().length;
    
    // Force digit counts for time components (override MaxNum) - only when "All" digits
    if (itemName === 'Hour' || itemName.includes('时')) digitCount = 2;
    else if (itemName === 'Minute' || itemName.includes('分')) digitCount = 2;
    else if (itemName === 'Second' || itemName.includes('秒')) digitCount = 2;
    // Force digit counts for date components
    else if (itemName === 'Month' || itemName.includes('月')) digitCount = 2;
    else if (itemName === 'Day' || itemName.includes('日')) digitCount = 2;
    else if (itemName === 'Year' || itemName.includes('年')) digitCount = 4;
    
    // Special handling for health metrics - only when "All" digits
    if (itemName === 'Steps' && digitCount < 5) digitCount = 5; // Steps can go up to 99999
    if (itemName === 'Heart Rate' && digitCount < 3) digitCount = 3; // Heart Rate: 0-999
    if (itemName === 'Calories' && digitCount < 4) digitCount = 4; // Calories: 0-9999
    if (itemName === 'Sleep' && digitCount < 3) digitCount = 3; // Sleep: 0-999
    
    // Get images from ImageSource or ImageData
    let images = component.ImageSource || [];
    if (images.length === 0 && component.ImageData) {
      if (typeof component.ImageData === 'string') {
        images = component.ImageData.split(',').map(s => s.trim());
      } else if (Array.isArray(component.ImageData)) {
        images = component.ImageData;
      }
    }
    
    console.log(`[Split] ${itemName}: digitCount=${digitCount}, maxNum=${component.MaxNum || 0}`);
    console.log(`[Split] Component Width=${component.Width}, Left=${component.Left}`);
    
    // Calculate digit width
    // IMPORTANT: component.Width represents the TOTAL width, but needs to be divided by 2 for actual spacing
    // Width of each element = Width / digitCount / 2
    const totalWidth = component.Width || (digitCount * 40);
    const digitWidth = Math.floor(totalWidth / digitCount); // Width per digit (divided by 2 as per requirement)
    const digitSpacing = 0; // No spacing between digits
    
    console.log(`[Split] ${itemName}: Total width=${totalWidth}, Width per digit=${digitWidth} (after /2), Digit count=${digitCount}`);
    
    // Create a folder for each digit position
    for (let i = 0; i < digitCount; i++) {
      const baseCode = this.getJieLiCode(component, 0);
      const codePrefix = baseCode.substring(0, 2);
      const baseDigit = parseInt(baseCode.substring(2));
      
      // Validate base code
      if (isNaN(baseDigit) || baseDigit < 0 || baseDigit > 99) {
        console.error(`[Split] Invalid base code: ${baseCode} for ${itemName}`);
        continue;
      }
      
      // Different logic for time/date vs health components:
      // - Time/Date components: getJieLiCode returns the FIRST digit (highest place value), so start there and increment
      // - Health components: getJieLiCode returns the HIGHEST digit code (but also first in sequence)
      const isTimeOrDate = itemName === 'Hour' || itemName === 'Minute' || itemName === 'Second' ||
                           itemName === 'Month' || itemName === 'Day' || itemName === 'Year' ||
                           itemName.includes('时') || itemName.includes('分') || itemName.includes('秒') ||
                           itemName.includes('月') || itemName.includes('日') || itemName.includes('年');
      
      let digitCode: string;
      if (isTimeOrDate) {
        // Time/date: base code is already the first digit, just increment
        const newDigit = baseDigit + i;
        digitCode = `${codePrefix}${newDigit.toString().padStart(2, '0')}`;
      } else {
        // Health metrics: base code is for the highest place value digit (first in folder order)
        // E.g., Steps: 0705 (10K), 0706 (1K), 0707 (100), 0708 (10), 0709 (1)
        const newDigit = baseDigit + i;
        digitCode = `${codePrefix}${newDigit.toString().padStart(2, '0')}`;
      }
      
      // Validate generated code
      const codeNum = parseInt(digitCode);
      if (isNaN(codeNum) || digitCode.length !== 4) {
        console.error(`[Split] Invalid generated code: ${digitCode} for ${itemName} digit ${i}`);
        continue;
      }
      
      const config = this.toJieLiFolderConfig(component, i);
      config.code = digitCode;
      
      // Display flag logic for multi-digit components:
      // Time components (Hour/Minute/Second): ALL digits = 10 (always show leading zeros like 09:05)
      // Date/Health components: First digit = 10, remaining = 0 (hide leading zeros)
      if (isTimeOrDate && (itemName === 'Hour' || itemName === 'Minute' || itemName === 'Second' ||
                           itemName.includes('时') || itemName.includes('分') || itemName.includes('秒'))) {
        // Time components: always show leading zeros
        config.displayFlag = 10;
      } else {
        // Date and health components: first digit visible, rest hide leading zeros
        config.displayFlag = (i === 0) ? 10 : 0;
      }
      
      // Calculate X offset for this digit position (left to right)
      config.x = (component.Left || 0) + (i * (digitWidth + digitSpacing));
      
      console.log(`[Split] Creating folder ${i+1}/${digitCount}: code=${digitCode}, x=${config.x}, width=${digitWidth}`);
      
      // Update element name for digit position from code map
      const mappedElement = this.getElementName(component, digitCode);
      if (mappedElement && mappedElement !== itemName && mappedElement !== '元素') {
        // Use the mapped element name from the standard folders
        config.elementName = mappedElement;
      } else {
        // Fall back to generic "数值#" prefix for non-standard components
        const digitNames = ['1(万)', '2(千)', '3(百)', '4(十)', '5(个)'];
        const digitIndex = digitNames.length - digitCount + i;
        if (digitIndex >= 0 && digitIndex < digitNames.length) {
          config.elementName = `数值#${digitNames[digitIndex]}`;
        } else {
          config.elementName = `数值#${i + 1}`;
        }
      }
      
      configs.push(config);
    }
    
    console.log(`[Split] Total configs created: ${configs.length}`);
    return configs.length > 0 ? configs : [this.toJieLiFolderConfig(component)];
  }
}
