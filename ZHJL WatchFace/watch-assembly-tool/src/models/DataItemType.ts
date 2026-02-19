// TypeScript equivalent of DataItemType.cs
export enum DataItemType {
  // Time related
  timeHour,
  timeHourLow,
  timeHourHigh,
  timeMinute,
  timeMinuteLow,
  timeMinuteHigh,
  timeSecond,
  timeSecondLow,
  timeSecondHigh,
  timeCentiSecond,
  timeCentiSecondLow,
  timeCentiSecondHigh,

  // Date related
  dateYear,
  dateYearDigit1,
  dateYearDigit2,
  dateYearDigit3,
  dateYearDigit4,
  dateMonth,
  dateMonthLow,
  dateMonthHigh,
  dateDay,
  dateDayLow,
  dateDayHigh,
  dateWeek,
  dateLunarYear,
  dateLunarMonth,
  dateLunarDay,

  // Miscellaneous
  miscIsAM,
  miscIsPM,
  miscTimeSection,
  miscIs24H,

  // Health related
  healthStepCount,
  healthStepCountDigit1,
  healthStepCountDigit2,
  healthStepCountDigit3,
  healthStepCountDigit4,
  healthStepCountDigit5,
  healthStepProgress,
  healthStepKiloMeter,
  healthHeartRate,
  healthHearRateZone,
  healthHeartRateMin,
  healthHeartRateMax,
  healthCalorie,
  healthCalorieProgress,
  healthStandCount,
  healthStandProgress,
  healthSleepDuration,
  healthSleepScore,
  healthMiscRecoveryTime,
  healthMiscRunPowerIndex,
  healthBloodPressureUnit,

  // Weather related
  weatherCurrentSunRiseHour,
  weatherCurrentSunRiseMinute,
  weatherCurrentSunSetHour,
  weatherCurrentSunSetMinute,
  weatherCurrentTemperature,
  weatherCurrentTemperatureFeel,
  weatherCurrentHumidity,
  weatherCurrentWeather,
  weatherCurrentWindDirection,
  weatherCurrentWindAngle,
  weatherCurrentWindSpeed,
  weatherCurrentWindLevel,
  weatherCurrentAirQualityIndex,
  weatherCurrentAirQualityLevel,
  weatherCurrentChanceOfRain,
  weatherCurrentPressure,
  weatherCurrentVisibility,
  weatherCurrentUVIndex,
  weatherCurrentUVLevel,
  weatherCurrentDressIndex,
  weatherCurrentWeatherIndex,
  weatherCurrentWeatherCode,
  weatherTemperatureUnit,

  // System status
  systemStatusBattery,
  systemStatusCharge,
  systemSensorFusionAltitude,
  systemStatusBluetooth,

  healthExerciseDuration,  // 中高强度锻炼时长 
  healthOxygenSpO2,  // 血氧 
  healthPressureIndex,  // 压力指数 
  healthMiscSevenDaysVitalityValue,  // 近7日累积元气值 
  healthBloodDiastolicPressureMmhg, //血压舒张压
  healthBloodDiastolicPressureKpa,  //血压舒张压
  healthBloodSystolicPressureMmhg,  //血压收缩压
  healthBloodSystolicPressureKpa,  //血压收缩压
  weatherTodayTemperatureMax,  //今天最高温度
  weatherTodayTemperatureMin,  //今天最高温度
  healthBloodSugarValue,  //血糖值
  healthSleepTargetProgress,//睡眠目标完成度
  healthExerciseProgress, //中高强度锻炼完成度
  systemStatusDisturb,
  healthStressLevel,
  healthDistancePrecent,//距离进度
}

export class DataItemTypeHelper {
  public static readonly DataItemTypes: Record<string, DataItemType> = {
    "Year": DataItemType.dateYear,
    "Month": DataItemType.dateMonth,
    "Day": DataItemType.dateDay,
    "Hour": DataItemType.timeHour,
    "Minute": DataItemType.timeMinute,
    "Second": DataItemType.timeSecond,
    "Week": DataItemType.dateWeek,
    "Month (Normal)": DataItemType.dateMonth,
    "Day (Normal)": DataItemType.dateDay,
    "Hour (Normal)": DataItemType.timeHour,

    "Step Progress": DataItemType.healthStepProgress,
    "Calorie Progress": DataItemType.healthCalorieProgress,
    "Sleep Target Progress": DataItemType.healthSleepTargetProgress,
    "Exercise Progress": DataItemType.healthExerciseProgress,
    "Hour (Tens)": DataItemType.timeHourHigh,
    "Hour (Ones)": DataItemType.timeHourLow,
    "Minute (Tens)": DataItemType.timeMinuteHigh,
    "Minute (Ones)": DataItemType.timeMinuteLow,
    "Second (Tens)": DataItemType.timeSecondHigh,
    "Second (Ones)": DataItemType.timeSecondLow,
    "Year (Thousands)": DataItemType.dateYearDigit4,
    "Year (Hundreds)": DataItemType.dateYearDigit3,
    "Year (Tens)": DataItemType.dateYearDigit2,
    "Year (Ones)": DataItemType.dateYearDigit1,
    "Month (Tens)": DataItemType.dateMonthHigh,
    "Month (Ones)": DataItemType.dateMonthLow,
    "Day (Tens)": DataItemType.dateDayHigh,
    "Day (Ones)": DataItemType.dateDayLow,
    "Steps (Ten Thousands)": DataItemType.healthStepCountDigit5,
    "Steps (Thousands)": DataItemType.healthStepCountDigit4,
    "Steps (Hundreds)": DataItemType.healthStepCountDigit3,
    "Steps (Tens)": DataItemType.healthStepCountDigit2,
    "Steps (Ones)": DataItemType.healthStepCountDigit1,

    "24H": DataItemType.miscIs24H,
    "Blood Pressure Unit": DataItemType.healthBloodPressureUnit,
    "Current Charging Status 24H": DataItemType.systemStatusCharge,
    "Do Not Disturb Mode": DataItemType.systemStatusDisturb,
    "Bluetooth Status": DataItemType.systemStatusBluetooth,
    "Temperature Unit": DataItemType.weatherTemperatureUnit,

    "AM/PM": DataItemType.miscIsPM,

    "Steps": DataItemType.healthStepCount,
    "Heart Rate": DataItemType.healthHeartRate,
    "Calories": DataItemType.healthCalorie,
    "Battery": DataItemType.systemStatusBattery,
    "Step Distance": DataItemType.healthStepKiloMeter,
    "Stand Count": DataItemType.healthStandCount,
    "Exercise Duration": DataItemType.healthExerciseDuration,
    "Min Heart Rate": DataItemType.healthHeartRateMin,
    "Max Heart Rate": DataItemType.healthHeartRateMax,
    "Blood Oxygen": DataItemType.healthOxygenSpO2,
    "Stress Index": DataItemType.healthPressureIndex,
    "Recovery Time": DataItemType.healthMiscRecoveryTime,
    "7-Day Vitality": DataItemType.healthMiscSevenDaysVitalityValue,
    "Wind Speed": DataItemType.weatherCurrentWindSpeed,
    "Wind Direction Angle": DataItemType.weatherCurrentWindAngle,
    "AQI": DataItemType.weatherCurrentAirQualityIndex,
    "Visibility": DataItemType.weatherCurrentVisibility,
    "UV Index": DataItemType.weatherCurrentUVIndex,
    "Temperature": DataItemType.weatherCurrentTemperature,
    "Today's High": DataItemType.weatherTodayTemperatureMax,
    "Today's Low": DataItemType.weatherTodayTemperatureMin,
    "Feels Like": DataItemType.weatherCurrentTemperatureFeel,
    "Humidity": DataItemType.weatherCurrentHumidity,
    "Running Power Index": DataItemType.healthMiscRunPowerIndex,
    "Rain Chance": DataItemType.weatherCurrentChanceOfRain,
    "Sea Level Pressure": DataItemType.weatherCurrentPressure,
    "Sleep Duration": DataItemType.healthSleepDuration,
    "Blood Sugar": DataItemType.healthBloodSugarValue,
    "Clothing Index": DataItemType.weatherCurrentDressIndex,
    "Wind Level": DataItemType.weatherCurrentWindLevel,
    "Wind Direction": DataItemType.weatherCurrentWindDirection,
    "Heart Rate Zone": DataItemType.healthHearRateZone,
    "Weather": DataItemType.weatherCurrentWeather,
    "Air Quality Level": DataItemType.weatherCurrentAirQualityIndex,
    "Time Section": DataItemType.miscTimeSection,

    "Stand Progress": DataItemType.healthStandProgress,

    "Blood Oxygen Progress": DataItemType.healthOxygenSpO2, // todo
    "Stress Progress": DataItemType.healthPressureIndex,
    "Battery Progress": DataItemType.systemStatusBattery,
    "Hand": DataItemType.miscTimeSection,
    "Stress Level": DataItemType.healthStressLevel,
    "Distance Progress": DataItemType.healthDistancePrecent
  };

  // DragDouble
  public static readonly VerifyNullNum: string[] = [
    "Heart Rate",
    "Blood Oxygen",
    "Stress Level",
    "Sleep Duration",
    "Sleep Target Progress",
    "Feels Like",
    "Temperature",
    "Humidity",
    "Wind Direction Angle",
    "Wind Speed",
    "AQI",
    "Rain Chance",
    "Visibility",
    "UV Index",
    "Today's High",
    "Today's Low",
    "Steps",
    "Step Distance",
    "Exercise Duration",
  ];
}