// Enums
export enum MonthDayModeEnum {
  split = 'split',
  unit = 'unit'
}

/**
 * Helper class for date/time formatting operations
 * Equivalent to C# DragDateTimeHelper
 */
export class DragDateTimeHelper {
  public static readonly iconStr = "&";
  public static readonly monthStr = "@";
  public static readonly dayStr = "#";

  /**
   * Format number with leading zero if specified
   */
  private static getLeadingZeroStr(leadingZero: boolean, num: number): string {
    return leadingZero ? num.toString().padStart(2, '0') : num.toString();
  }

  /**
   * Get formatted date/time string based on format
   */
  public static getDateTime(
    dateTimeFormat: string, 
    leadingZero: boolean, 
    hasIcon: boolean, 
    dateTime: Date
  ): string {
    const iconSuffix = hasIcon ? this.iconStr : "";
    
    switch (dateTimeFormat) {
      case "Year":
        return `${dateTime.getFullYear()}${iconSuffix}`;
      
      case "Month":
        return `${this.getLeadingZeroStr(leadingZero, dateTime.getMonth() + 1)}${iconSuffix}`;
      
      case "Day":
        return `${this.getLeadingZeroStr(leadingZero, dateTime.getDate())}${iconSuffix}`;
      
      case "Hour":
        return `${this.getLeadingZeroStr(leadingZero, dateTime.getHours())}${iconSuffix}`;
      
      case "Minute":
        return `${this.getLeadingZeroStr(leadingZero, dateTime.getMinutes())}${iconSuffix}`;
      
      case "Second":
        return `${this.getLeadingZeroStr(leadingZero, dateTime.getSeconds())}${iconSuffix}`;
      
      case "Time":
        return `${this.getLeadingZeroStr(leadingZero, dateTime.getHours())}${iconSuffix}${dateTime.getMinutes().toString().padStart(2, '0')}`;
      
      default:
        throw new Error(`DateTime format '${dateTimeFormat}' not implemented`);
    }
  }

  /**
   * Get formatted month-day string
   */
  public static getMonthDay(
    leadingZero: boolean,
    showMonthType: MonthDayModeEnum,
    hasIcon: boolean,
    hasMonthIcon: boolean,
    hasDayIcon: boolean,
    dateTime: Date
  ): string {
    const month = this.getLeadingZeroStr(leadingZero, dateTime.getMonth() + 1);
    const day = this.getLeadingZeroStr(leadingZero, dateTime.getDate());
    
    if (showMonthType === MonthDayModeEnum.unit) {
      return `${month}${hasMonthIcon ? this.monthStr : ""}${day}${hasDayIcon ? this.dayStr : ""}`;
    } else if (showMonthType === MonthDayModeEnum.split) {
      return `${month}${hasIcon ? this.iconStr : ""}${day}`;
    }
    
    return `${month}${day}`;
  }

  /**
   * Get week day name
   */
  public static getWeekDay(dateTime: Date, format: 'short' | 'long' = 'short'): string {
    return dateTime.toLocaleDateString('en-US', { weekday: format });
  }

  /**
   * Get month name
   */
  public static getMonthName(dateTime: Date, format: 'short' | 'long' = 'short'): string {
    return dateTime.toLocaleDateString('en-US', { month: format });
  }

  /**
   * Format time in 12-hour format
   */
  public static get12HourTime(dateTime: Date, showAmPm: boolean = true): string {
    let hours = dateTime.getHours();
    const minutes = dateTime.getMinutes().toString().padStart(2, '0');
    const ampm = hours >= 12 ? 'PM' : 'AM';
    
    hours = hours % 12;
    hours = hours ? hours : 12; // 0 should be 12
    
    const timeStr = `${hours}:${minutes}`;
    return showAmPm ? `${timeStr} ${ampm}` : timeStr;
  }

  /**
   * Format time in 24-hour format
   */
  public static get24HourTime(dateTime: Date): string {
    const hours = dateTime.getHours().toString().padStart(2, '0');
    const minutes = dateTime.getMinutes().toString().padStart(2, '0');
    return `${hours}:${minutes}`;
  }

  /**
   * Get AM/PM indicator
   */
  public static getAmPm(dateTime: Date): 'AM' | 'PM' {
    return dateTime.getHours() >= 12 ? 'PM' : 'AM';
  }

  /**
   * Format date in common formats
   */
  public static getFormattedDate(dateTime: Date, format: string): string {
    switch (format.toLowerCase()) {
      case 'mm/dd/yyyy':
        return `${(dateTime.getMonth() + 1).toString().padStart(2, '0')}/${dateTime.getDate().toString().padStart(2, '0')}/${dateTime.getFullYear()}`;
      
      case 'dd/mm/yyyy':
        return `${dateTime.getDate().toString().padStart(2, '0')}/${(dateTime.getMonth() + 1).toString().padStart(2, '0')}/${dateTime.getFullYear()}`;
      
      case 'yyyy-mm-dd':
        return `${dateTime.getFullYear()}-${(dateTime.getMonth() + 1).toString().padStart(2, '0')}-${dateTime.getDate().toString().padStart(2, '0')}`;
      
      default:
        return dateTime.toLocaleDateString();
    }
  }

  /**
   * Parse time format string (equivalent to C# time parsing)
   */
  public static parseTimeFormat(format: string, dateTime: Date): string {
    return format
      .replace(/yyyy/g, dateTime.getFullYear().toString())
      .replace(/MM/g, (dateTime.getMonth() + 1).toString().padStart(2, '0'))
      .replace(/dd/g, dateTime.getDate().toString().padStart(2, '0'))
      .replace(/HH/g, dateTime.getHours().toString().padStart(2, '0'))
      .replace(/mm/g, dateTime.getMinutes().toString().padStart(2, '0'))
      .replace(/ss/g, dateTime.getSeconds().toString().padStart(2, '0'));
  }
}