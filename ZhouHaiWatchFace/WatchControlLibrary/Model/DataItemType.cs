using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchControlLibrary.Model
{
    public enum DataItemType
    {
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

        healthExerciseDuration,  // Moderate to high intensity exercise duration
        healthOxygenSpO2,  // Blood oxygen 
        healthPressureIndex,  // Pressure index 
        healthMiscSevenDaysVitalityValue,  // 7-day cumulative vitality value 
        healthBloodDiastolicPressureMmhg, // Diastolic pressure
        healthBloodDiastolicPressureKpa,  // Diastolic pressure
        healthBloodSystolicPressureMmhg,  // Systolic pressure
        healthBloodSystolicPressureKpa,  // Systolic pressure
        weatherTodayTemperatureMax,  // Today's max temperature
        weatherTodayTemperatureMin,  // Today's min temperature
        healthBloodSugarValue,  // Blood sugar value
        healthSleepTargetProgress,// Sleep target progress
        healthExerciseProgress, //Moderate to high intensity exercise completion
        systemStatusDisturb,
        healthStressLevel,
        healthDistancePrecent,// Distance progress

    }

    public partial class DataItemTypeHelper
    {
        public static Dictionary<string, DataItemType> DataItemTypes => new Dictionary<string, DataItemType>
        {
            {"Year",DataItemType.dateYear },
            {"Month",DataItemType.dateMonth },
            {"Day",DataItemType.dateDay },
            {"Hour",DataItemType.timeHour },
            {"Minute",DataItemType.timeMinute },
            {"Second",DataItemType.timeSecond },
            {"Week",DataItemType.dateWeek },
            {"Month (Normal Fill)",DataItemType.dateMonth },
            {"Day (Normal Fill)",DataItemType.dateDay },
            {"Hour (Normal Fill)",DataItemType.timeHour },

            {"Step Progress",DataItemType.healthStepProgress },
            {"Calorie Progress",DataItemType.healthCalorieProgress },
            {"Sleep Goal Progress",DataItemType.healthSleepTargetProgress },
            {"Exercise Progress",DataItemType.healthExerciseProgress },
            {"Hour Tens",DataItemType.timeHourHigh},
            {"Hour Units",DataItemType.timeHourLow},
            {"Minute Tens",DataItemType.timeMinuteHigh},
            {"Minute Units",DataItemType.timeMinuteLow},
            {"Second Tens",DataItemType.timeSecondHigh},
            {"Second Units",DataItemType.timeSecondLow},
            {"Year Thousands",DataItemType.dateYearDigit4},
            {"Year Hundreds",DataItemType.dateYearDigit3},
            {"Year Tens",DataItemType.dateYearDigit2},
            {"Year Units",DataItemType.dateYearDigit1},
            {"Month Tens",DataItemType.dateMonthHigh},
            {"Month Units",DataItemType.dateMonthLow},
            {"Day Tens",DataItemType.dateDayHigh},
            {"Day Units",DataItemType.dateDayLow},
            {"Steps Ten-Thousands",DataItemType.healthStepCountDigit5},
            {"Steps Thousands",DataItemType.healthStepCountDigit4},
            {"Steps Hundreds",DataItemType.healthStepCountDigit3},
            {"Steps Tens",DataItemType.healthStepCountDigit2},
            {"Steps Units",DataItemType.healthStepCountDigit1},


            {"24H",DataItemType.miscIs24H},
            {"Blood Pressure Unit",DataItemType.healthBloodPressureUnit},
            {"Charging Status 24H",DataItemType.systemStatusCharge},
            {"Do Not Disturb",DataItemType.systemStatusDisturb},
            {"Bluetooth Status",DataItemType.systemStatusBluetooth},
            {"Temperature Unit",DataItemType.weatherTemperatureUnit},


            {"AM/PM",DataItemType.miscIsPM},

            {"Steps", DataItemType.healthStepCount },
            {"Heart Rate",DataItemType.healthHeartRate},
            {"Calories",DataItemType.healthCalorie},
            {"Battery",DataItemType.systemStatusBattery},
            {"Step Distance",DataItemType.healthStepKiloMeter},
            {"Stand Count",DataItemType.healthStandCount},
            {"Exercise Duration",DataItemType.healthExerciseDuration},
            {"Heart Rate Min",DataItemType.healthHeartRateMin},
            {"Heart Rate Max",DataItemType.healthHeartRateMax},
            {"Blood Oxygen",DataItemType.healthOxygenSpO2},
            {"Pressure Index",DataItemType.healthPressureIndex},
            {"Recovery Time",DataItemType.healthMiscRecoveryTime},
            {"7-Day Vitality",DataItemType.healthMiscSevenDaysVitalityValue},
            {"Wind Speed",DataItemType.weatherCurrentWindSpeed},
            {"Wind Direction Angle",DataItemType.weatherCurrentWindAngle},
            {"AQI",DataItemType.weatherCurrentAirQualityIndex},
            {"Visibility",DataItemType.weatherCurrentVisibility},
            {"UV Index",DataItemType.weatherCurrentUVIndex},
            {"Temperature", DataItemType.weatherCurrentTemperature},
            {"Today Max Temperature", DataItemType.weatherTodayTemperatureMax},
            {"Today Min Temperature", DataItemType.weatherTodayTemperatureMin},
            {"Feels Like Temperature", DataItemType.weatherCurrentTemperatureFeel},
            {"Humidity", DataItemType.weatherCurrentHumidity},
            {"Running Power Index", DataItemType.healthMiscRunPowerIndex},
            {"Rain Probability", DataItemType.weatherCurrentChanceOfRain},
            {"Sea Level Pressure", DataItemType.weatherCurrentPressure},
            {"Sleep Duration", DataItemType.healthSleepDuration },
            {"Blood Sugar", DataItemType.healthBloodSugarValue },
            {"Dress Index", DataItemType.weatherCurrentDressIndex },
            {"Wind Level", DataItemType.weatherCurrentWindLevel },
            {"Wind Direction", DataItemType.weatherCurrentWindDirection },
            {"Heart Rate Zone", DataItemType.healthHearRateZone },
            {"Weather", DataItemType.weatherCurrentWeather },
            {"Air Quality Level", DataItemType.weatherCurrentAirQualityIndex },
            {"Time Period", DataItemType.miscTimeSection },

            {"Stand Progress", DataItemType.healthStandProgress },

            {"Blood Oxygen Progress", DataItemType.healthOxygenSpO2 }, // todo
            {"Pressure Progress", DataItemType.healthPressureIndex },
            {"Battery Progress", DataItemType.systemStatusBattery },
            {"Pointer",DataItemType.miscTimeSection},
            {"Stress Level", DataItemType.healthStressLevel},
            {"Distance Progress",DataItemType.healthDistancePrecent }

        };

        //DragDouble
        public static IEnumerable<string> VerifyNullNum => new string[] 
        {
          "Heart Rate",
          "Blood Oxygen",
          "Stress Level",
          "Sleep Duration",
          "Sleep Goal Progress",
          "Feels Like Temperature",
          "Temperature",
          "Humidity",
          "Wind Direction Angle",
          "Wind Speed",
          "AQI",
          "Rain Probability",
          "Visibility",
          "UV Index",
          "Today Max Temperature",
          "Today Min Temperature",
          "Steps",
          "Step Distance",
          "Exercise Duration",
        };
    }

}
