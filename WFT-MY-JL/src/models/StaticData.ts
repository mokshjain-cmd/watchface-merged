/**
 * Static data and constants equivalent to C# StaticData
 * Contains weather data, heart rate zones, and other lookup tables
 */

export interface Weather {
  id: number;
  content?: string;
  content2?: string;
  content_CN?: string;
  groupId: number;
}

export class MonitorItem {
  public static readonly DefaultTime = new Date('2024-01-01T12:00:00');
}

export class StaticData {
  // Weather data - in web app, this would be loaded from a JSON file or API
  public static get weathers(): Weather[] {
    return [
      { id: 1, content: 'Sunny', content_CN: '晴天', groupId: 1 },
      { id: 2, content: 'Cloudy', content_CN: '多云', groupId: 2 },
      { id: 3, content: 'Rainy', content_CN: '雨天', groupId: 3 },
      { id: 4, content: 'Snowy', content_CN: '雪天', groupId: 4 },
      // Add more weather data as needed
    ];
  }

  // Heart rate zones data
  public static readonly heartZonesData: Record<number, string> = {
    0: 'Quiet',
    1: 'Warm-up',
    2: 'Fat Burn',
    3: 'Aerobic',
    4: 'Anaerobic',
    5: 'Maximum'
  };

  // Pressure level data
  public static readonly pressLevelData: Record<number, string> = {
    0: 'Relaxed',
    1: 'Normal',
    2: 'Medium',
    3: 'High'
  };

  // Weather data grouped by category
  public static get weatherData(): Record<number, string> {
    const grouped = this.weathers.reduce((acc, weather) => {
      if (weather.content_CN && !acc[weather.groupId]) {
        acc[weather.groupId] = weather.content_CN;
      }
      return acc;
    }, {} as Record<number, string>);
    return grouped;
  }

  // Wind direction data
  public static readonly windDirectionData: Record<number, string> = {
    0: 'No Persistent Wind',
    1: 'Variable Wind',
    2: 'North Wind',
    3: 'Northeast Wind',
    4: 'East Wind',
    5: 'Southeast Wind',
    6: 'South Wind',
    7: 'Southwest Wind',
    8: 'West Wind',
    9: 'Northwest Wind'
  };

  // Wind level data
  public static readonly windLevelData: Record<number, string> = {
    0: 'Light Breeze',
    1: 'Level 1',
    2: 'Level 2',
    3: 'Level 3',
    4: 'Level 4',
    5: 'Level 5',
    6: 'Level 6',
    7: 'Level 7',
    8: 'Level 8',
    9: 'Level 9',
    10: 'Level 10',
    11: 'Level 11',
    12: 'Level 12 and above'
  };

  // AQI level data
  public static readonly aqiLevelData: Record<number, string> = {
    0: 'Excellent',
    1: 'Good',
    2: 'Light Pollution',
    3: 'Moderate Pollution',
    4: 'Heavy Pollution',
    5: 'Severe Pollution'
  };

  // Dress level data
  public static readonly dressLevelData: Record<number, string> = {
    0: 'Suitable for T-shirt',
    1: 'Suitable for Shirt',
    2: 'Suitable for Light Jacket',
    3: 'Suitable for Jacket',
    4: 'Suitable for Windbreaker',
    5: 'Suitable for Cotton Coat',
    6: 'Suitable for Winter Coat',
    7: 'Suitable for Down Jacket'
  };

  // Time span data
  public static readonly timeSpan: Record<number, string> = {
    0: 'Late Night',
    1: 'Dawn',
    2: 'Early Morning',
    3: 'Morning',
    4: 'Noon',
    5: 'Afternoon',
    6: 'Evening',
    7: 'Dusk',
    8: 'Night'
  };

  /**
   * Factory method for creating key-value data based on type
   */
  public static toolKeyValuesFactory(createName: string): Record<number, string> {
    switch (createName) {
      case 'Wind Direction':
        return this.windDirectionData;
      case 'Heart Rate Zone':
        return this.heartZonesData;
      case 'Weather':
        return this.weatherData;
      case 'Wind Level':
        return this.windLevelData;
      case 'Air Quality Index':
        return this.aqiLevelData;
      case 'Dress Index':
        return this.dressLevelData;
      case 'Time Period':
        return this.timeSpan;
      case 'Pressure Level':
        return this.pressLevelData;
      default:
        throw new Error(`Unsupported type: ${createName}`);
    }
  }

  /**
   * Get weather data by group ID
   */
  public static getWeatherByGroupId(groupId: number): Weather | undefined {
    return this.weathers.find(w => w.groupId === groupId);
  }

  /**
   * Get all weather groups
   */
  public static getWeatherGroups(): Record<number, string> {
    return this.weatherData;
  }

  /**
   * Load weather data from external source (in web app)
   */
  public static async loadWeatherData(): Promise<Weather[]> {
    try {
      // In a real web app, this would fetch from an API or JSON file
      // For now, return the static data
      return this.weathers;
    } catch (error) {
      console.error('Error loading weather data:', error);
      return [];
    }
  }
}