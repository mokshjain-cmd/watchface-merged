using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchBin.BinAnalyze
{
    public enum Record_type_t
    {
        RECORD_TYPE_LAYOUT = 0x00, /* 0x00 */
        RECORD_TYPE_STRING, /* 0x01 */
        RECORD_TYPE_IMG, /* 0x02 */
        RECORD_TYPE_IMG_ARRAY, /* 0x03 */
        RECORD_TYPE_SPRITE, /* 0x04 */
        RECORD_TYPE_FILE, /* 0x05 */
        RECORD_TYPE_TRANSLATION, /* 0x06 */
        RECORD_TYPE_DATA, /* 0x07 */
        RECORD_TYPE_SLOT, /* 0x08 */
        RECORD_TYPE_WIDGET, /* 0x09 */
        RECORD_TYPE_TOTAL_COUNT, /* 10 */
    }
    public enum DataStyle
    {
        DATA_STYLE_TEXT = 0,
        DATA_STYLE_IMAGE_NUMBER,
        DATA_STYLE_IMAGE_VALUES,
        DATA_STYLE_POINTER,
        DATA_STYLE_ARC_PROGRESS_BAR,
        DATA_STYLE_LINE_PROGRESS_BAR,
        DATA_STYLE_IMG_OPACITY,
    }

    public enum RenderRule
    {
        ALWAYS_SHOW = 0x00,
        HIDE_WHEN_UNIT_MISMATCH = 0x01,
        HIDE_WHEN_OUT_RANGE = 0x02,
    }

    public enum Compression 
    {
       NoCompression=0x0,
       L61=0x1,
       L67=0x2,
       N65A=0x3,//ezip
    }

   


    public enum DataOriginDataSource
    {
        // Time related
        TimeHour = (1 << 12) | (1 << 8) | (1 << 3) | 0,
        TimeHourLow = (1 << 12) | (1 << 8) | (1 << 3) | 1,
        TimeHourHigh = (1 << 12) | (1 << 8) | (1 << 3) | 2,
        TimeMinute = (1 << 12) | (1 << 8) | (2 << 3) | 0,
        TimeMinuteLow = (1 << 12) | (1 << 8) | (2 << 3) | 1,
        TimeMinuteHigh = (1 << 12) | (1 << 8) | (2 << 3) | 2,
        TimeSecond = (1 << 12) | (1 << 8) | (3 << 3) | 0,
        TimeSecondLow = (1 << 12) | (1 << 8) | (3 << 3) | 1,
        TimeSecondHigh = (1 << 12) | (1 << 8) | (3 << 3) | 2,
        TimeCentiSecond = (1 << 12) | (1 << 8) | (4 << 3) | 0,
        TimeCentiSecondLow = (1 << 12) | (1 << 8) | (4 << 3) | 1,
        TimeCentiSecondHigh = (1 << 12) | (1 << 8) | (4 << 3) | 2,
        TimeHour12H = (1 << 12) | (1 << 8) | (5 << 3) | 0,
        TimeHour24H = (1 << 12) | (1 << 8) | (6 << 3) | 0,

        
        // Date related
        DateYear = (1 << 12) | (2 << 8) | (1 << 3) | 0,
        DateYearDigit1 = (1 << 12) | (2 << 8) | (1 << 3) | 1,
        DateYearDigit2 = (1 << 12) | (2 << 8) | (1 << 3) | 2,
        DateYearDigit3 = (1 << 12) | (2 << 8) | (1 << 3) | 3,
        DateYearDigit4 = (1 << 12) | (2 << 8) | (1 << 3) | 4,
        DateMonth = (1 << 12) | (2 << 8) | (2 << 3) | 0,
        DateMonthLow = (1 << 12) | (2 << 8) | (2 << 3) | 1,
        DateMonthHigh = (1 << 12) | (2 << 8) | (2 << 3) | 2,
        DateDay = (1 << 12) | (2 << 8) | (3 << 3) | 0,
        DateDayLow = (1 << 12) | (2 << 8) | (3 << 3) | 1,
        DateDayHigh = (1 << 12) | (2 << 8) | (3 << 3) | 2,
        DateWeek = (1 << 12) | (2 << 8) | (4 << 3) | 0,
        DateLunarYear = (1 << 12) | (2 << 8) | (5 << 3) | 0,
        DateLunarMonth = (1 << 12) | (2 << 8) | (6 << 3) | 0,
        DateLunarDay = (1 << 12) | (2 << 8) | (7 << 3) | 0,
        DateLunarStringYear = (1 << 12) | (2 << 8) | (8 << 3) | 0,
        DateLunarStringMonth = (1 << 12) | (2 << 8) | (9 << 3) | 0,
        DateLunarStringDay = (1 << 12) | (2 << 8) | (10 << 3) | 0,
        DateWeekStringShortCN = (1 << 12) | (2 << 8) | (11 << 3) | 0,
        DateWeekStringFullCN = (1 << 12) | (2 << 8) | (11 << 3) | 1,
        DateWeekStringFullPascalEN = (1 << 12) | (2 << 8) | (11 << 3) | 2,
        DateWeekStringFullUpperEN = (1 << 12) | (2 << 8) | (11 << 3) | 3,
        DateWeekStringFullLowerEN = (1 << 12) | (2 << 8) | (11 << 3) | 4,
        DateWeekStringShortPascalEN = (1 << 12) | (2 << 8) | (11 << 3) | 5,
        DateWeekStringShortUpperEN = (1 << 12) | (2 << 8) | (11 << 3) | 6,
        DateWeekStringShortLowerEN = (1 << 12) | (2 << 8) | (11 << 3) | 7,
        DateMonthStringShortCN = (1 << 12) | (2 << 8) | (12 << 3) | 0,
        DateMonthStringFullPascalEN = (1 << 12) | (2 << 8) | (12 << 3) | 1,
        DateMonthStringFullUpperEN = (1 << 12) | (2 << 8) | (12 << 3) | 2,
        DateMonthStringFullLowerEN = (1 << 12) | (2 << 8) | (12 << 3) | 3,
        DateMonthStringShortPascalEN = (1 << 12) | (2 << 8) | (12 << 3) | 4,
        DateMonthStringShortUpperEN = (1 << 12) | (2 << 8) | (12 << 3) | 5,
        DateMonthStringShortLowerEN = (1 << 12) | (2 << 8) | (12 << 3) | 6,
        DateSexagenaryCycle = (1 << 12) | (2 << 8) | (13 << 3) | 0,

        // Miscellaneous
        MiscIsPM = (1 << 12) | (3 << 8) | (2 << 3) | 0,
        MiscIs24H = (1 << 12) | (3 << 8) | (3 << 3) | 0,
        MiscTimeSection = (1 << 12) | (3 << 8) | (4 << 3) | 0,
        MiscDateTomorrow = (1 << 12) | (3 << 8) | (5 << 3) | 0,
        MiscDateYesterday = (1 << 12) | (3 << 8) | (6 << 3) | 0,
        MiscDateDayAfterTomorrow = (1 << 12) | (3 << 8) | (7 << 3) | 0,
        MiscDateDayBeforeYesterday = (1 << 12) | (3 << 8) | (8 << 3) | 0,
        MiscDateTomorrowWeak = (1 << 12) | (3 << 8) | (9 << 3) | 0,
        MiscDateYesterdayWeak = (1 << 12) | (3 << 8) | (10 << 3) | 0,

        // Dual time related
        DualTimeHour = (1 << 12) | (4 << 8) | (1 << 3) | 0,
        DualTimeHourLow = (1 << 12) | (4 << 8) | (1 << 3) | 1,
        DualTimeHourHigh = (1 << 12) | (4 << 8) | (1 << 3) | 2,
        DualTimeMinute = (1 << 12) | (4 << 8) | (2 << 3) | 0,
        DualTimeMinuteLow = (1 << 12) | (4 << 8) | (2 << 3) | 1,
        DualTimeMinuteHigh = (1 << 12) | (4 << 8) | (2 << 3) | 2,
        DualTimeSecond = (1 << 12) | (4 << 8) | (3 << 3) | 0,
        DualTimeSecondLow = (1 << 12) | (4 << 8) | (3 << 3) | 1,
        DualTimeSecondHigh = (1 << 12) | (4 << 8) | (3 << 3) | 2,
        DualTimeIsPM = (1 << 12) | (4 << 8) | (4 << 3) | 0,
        DualTimeHour12H = (1 << 12) | (4 << 8) | (5 << 3) | 0,
        DualTimeHour24H = (1 << 12) | (4 << 8) | (6 << 3) | 0,
        DualTimeStringCity = (1 << 12) | (4 << 8) | (7 << 3) | 0,
        DualTimeCity = (1 << 12) | (4 << 8) | (8 << 3) | 0,

        // Health related 
        HealthStepCount = (2 << 12) | (1 << 8) | (1 << 3) | 0,
        HealthStepCountDigit1 = (2 << 12) | (1 << 8) | (1 << 3) | 1,
        HealthStepCountDigit2 = (2 << 12) | (1 << 8) | (1 << 3) | 2,
        HealthStepCountDigit3 = (2 << 12) | (1 << 8) | (1 << 3) | 3,
        HealthStepCountDigit4 = (2 << 12) | (1 << 8) | (1 << 3) | 4,
        HealthStepCountDigit5 = (2 << 12) | (1 << 8) | (1 << 3) | 5,
        HealthStepProgress = (2 << 12) | (1 << 8) | (2 << 3) | 0,
        HealthStepKiloMeter = (2 << 12) | (1 << 8) | (3 << 3) | 0,
        HealthStepTarget = (2 << 12) | (1 << 8) | (4 << 3) | 0,
        HealthHeartRate = (2 << 12) | (2 << 8) | (1 << 3) | 0,
        HealthHearRateZone = (2 << 12) | (2 << 8) | (2 << 3) | 0,
        HealthHearRateMin = (2 << 12) | (2 << 8) | (3 << 3) | 0,
        HealthHearRateMax = (2 << 12) | (2 << 8) | (4 << 3) | 0,
        HealthCalorie = (2 << 12) | (3 << 8) | (1 << 3) | 0, //HealthCalorieValue 
        HealthCalorieProgress = (2 << 12) | (3 << 8) | (2 << 3) | 0,
        HealthCalorieTarget = (2 << 12) | (3 << 8) | (3 << 3) | 0,
        HealthStandCount = (2 << 12) | (4 << 8) | (1 << 3) | 0,
        HealthStandProgress = (2 << 12) | (4 << 8) | (2 << 3) | 0,
        HealthStandTarget = (2 << 12) | (4 << 8) | (3 << 3) | 0,
        HealthOxygenSpO2 = (2 << 12) | (5 << 8) | (1 << 3) | 0,
        HealthPressureIndex = (2 << 12) | (6 << 8) | (1 << 3) | 0, 
        HealthBloodDiastolicPressureMmhg = (2 << 12) | (7 << 8) | (1 << 3) | 0,
        HealthBloodSystolicPressureMmhg = (2 << 12) | (7 << 8) | (2 << 3) | 0,
        HealthBloodDiastolicPressureKpa = (2 << 12) | (7 << 8) | (3 << 3) | 0,
        HealthBloodSystolicPressureKpa = (2 << 12) | (7 << 8) | (4 << 3) | 0,
        HealthBloodPressureUnit = (2 << 12) | (7 << 8) | (5 << 3) | 0,
        HealthSleepDuration = (2 << 12) | (8 << 8) | (1 << 3) | 0,
        HealthSleepTargetProgress = (2 << 12) | (8 << 8) | (3 << 3) | 0,
        HealthSleepTarget = (2 << 12) | (8 << 8) | (5 << 3) | 0,
        HealthExerciseDuration = (2 << 12) | (9 << 8) | (1 << 3) | 0,
        HealthExerciseProgress = (2 << 12) | (9 << 8) | (2 << 3) | 0,
        HealthExerciseTarget = (2 << 12) | (9 << 8) | (3 << 3) | 0,
        HealthMiscRecoveryTime = (2 << 12) | (11 << 8) | (1 << 3) | 0,
        HealthMiscRunPowerIndex = (2 << 12) | (11 << 8) | (2 << 3) | 0,
        HealthMiscTodayVitalityValue = (2 << 12) | (11 << 8) | (3 << 3) | 0,
        HealthMiscSevenDaysVitalityValue = (2 << 12) | (11 << 8) | (4 << 3) | 0,
        HealthBloodSugarValue = (2 << 12) | (12 << 8) | (1 << 3) | 0,
        HealthBloodSugarUpdateTsString = (2 << 12) | (12 << 8) | (2 << 3) | 0,

        // Weather related
        WeatherTemperatureUnit = (3 << 12) | (1 << 8) | (1 << 3) | 0,
        WeatherCurrentTemperatureFeel = (3 << 12) | (1 << 8) | (3 << 3) | 0,
        WeatherCurrentTemperature = (3 << 12) | (1 << 8) | (4 << 3) | 0,
        WeatherCurrentHumidity = (3 << 12) | (1 << 8) | (5 << 3) | 0,
        WeatherCurrentWeather = (3 << 12) | (1 << 8) | (6 << 3) | 0,
        WeatherCurrentWindAngle = (3 << 12) | (1 << 8) | (7 << 3) | 0,
        WeatherCurrentWindSpeed = (3 << 12) | (1 << 8) | (8 << 3) | 0,
        WeatherCurrentWindLevel = (3 << 12) | (1 << 8) | (9 << 3) | 0,
        WeatherCurrentAirQualityIndex = (3 << 12) | (1 << 8) | (10 << 3) | 0,
        WeatherCurrentAirQualityLevel = (3 << 12) | (1 << 8) | (11 << 3) | 0,
        WeatherCurrentChanceOfRain = (3 << 12) | (1 << 8) | (12 << 3) | 0,
        WeatherCurrentPressure = (3 << 12) | (1 << 8) | (13 << 3) | 0,
        WeatherCurrentVisibility = (3 << 12) | (1 << 8) | (14 << 3) | 0,
        WeatherCurrentUVIndex = (3 << 12) | (1 << 8) | (15 << 3) | 0,
        WeatherCurrentDressIndex = (3 << 12) | (1 << 8) | (16 << 3) | 0,
        WeatherCurrentWindDirection = (3 << 12) | (1 << 8) | (21 << 3) | 0,
        WeatherCurrentSunRiseHour = (3 << 12) | (1 << 8) | (22 << 3) | 0,
        WeatherCurrentSunRiseMinute = (3 << 12) | (1 << 8) | (23 << 3) | 0,
        WeatherCurrentSunSetHour = (3 << 12) | (1 << 8) | (24 << 3) | 0,
        WeatherCurrentSunSetMinute = (3 << 12) | (1 << 8) | (25 << 3) | 0,
        WeatherCurrentTemperatureFahrenheit = (3 << 12) | (1 << 8) | (26 << 3) | 0,
        WeatherCurrentSunRise = (3 << 12) | (1 << 8) | (27 << 3) | 0,
        WeatherCurrentSunSet = (3 << 12) | (1 << 8) | (28 << 3) | 0,

        //新增 2024.11.21
        WeatherTodayTemperatureMax = (3 << 12) | (2 << 8) | (3 << 3) | 0,
        WeatherTodayTemperatureMin = (3 << 12) | (2 << 8) | (4 << 3) | 0,
        WeatherTodayTemperatureMaxFahrenheit = (3 << 12) | (2 << 8) | (13 << 3) | 0,
        WeatherTodayTemperatureMinFahrenheit = (3 << 12) | (2 << 8) | (14 << 3) | 0,
        WeatherTomorrowTemperatureMax = (3 << 12) | (3 << 8) | (3 << 3) | 0,
        WeatherTomorrowTemperatureMin = (3 << 12) | (3 << 8) | (4 << 3) | 0,
        WeatherTomorrowTemperatureMaxFahrenheit = (3 << 12) | (3 << 8) | (13 << 3) | 0,
        WeatherTomorrowTemperatureMinFahrenheit = (3 << 12) | (3 << 8) | (14 << 3) | 0,
        SystemStatusBattery = (4 << 12) | (1 << 8) | (1 << 3) | 0,
        SystemStatusCharge = (4 << 12) | (1 << 8) | (2 << 3) | 0,
        SystemStatusDisturb = (4 << 12) | (1 << 8) | (3 << 3) | 0,
        SystemStatusBluetooth = (4 << 12) | (1 << 8) | (4 << 3) | 0,
        SystemStatusWifi = (4 << 12) | (1 << 8) | (5 << 3) | 0,
        SystemStatusScreenLock = (4 << 12) | (1 << 8) | (6 << 3) | 0,
        SystemSensorFusionAltitude = (4 << 12) | (2 << 8) | (1 << 3) | 0,
        AppAlarmHour = (5 << 12) | (1 << 8) | (1 << 3) | 0,
        AppAlarmMinute = (5 << 12) | (1 << 8) | (2 << 3) | 0,
        SystemSensorCompass = (4 << 12) | (2 << 8) | (10 << 3) | 0,
        SystemSensorAtmosphericPressure = (4 << 12) | (2 << 8) | (11 << 3) | 0,
        AppSportNoFlyTime = (5 << 12) | (2 << 8) | (1 << 3) | 0,
        AppSportNoFlyTimeUnit = (5 << 12) | (2 << 8) | (2 << 3) | 0,
        AppSportSitTime = (5 << 12) | (2 << 8) | (3 << 3) | 0,
        AppSportSitTimeUnit = (5 << 12) | (2 << 8) | (4 << 3) | 0,
        MiscdateDayThreeDaysAgo = (1 << 12) | (3 << 8) | (11 << 3) | 0,
        MiscdateDayThreeDaysFromNow = (1 << 12) | (3 << 8) | (12 << 3) | 0,
    }

    //typedef enum {
    //    ALWAYS_SHOW = 0x00,
    //    /* Hide the record when the unit of the data source dose not match the unit
    //    of the data set by system */
    //    HIDE_WHEN_UNIT_MISMATCH = 0x01,
    //    /* Hide the record when the current value over range */
    //    HIDE_WHEN_OUT_RANGE = 0x02,
    //}
    //data_render_rule_t;
}
