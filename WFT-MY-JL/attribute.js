let json = {
  bg_0: {
    code: 'bg_0',
    limit: 1,
    show: 1
  },
  bg_1: {
    code: 'bg_1',
    limit: 10,
    dec: 0,
    show: 10
  },
  ani_click: {
    code: 'ani_click',
    limit: 50,
    show: 1
  },
  ani_x10: {
    code: "ani_x10",
    limit: 50,
    show: 1,
    dec: 3
  },
  ani_x20: {
    code: 'ani_x20',
    limit: 50,
    show: 1,
    dec: 2
  },
  ani_x2: {
    code: "ani_x2",
    limit: 50,
    show: 1,
    dec: 15
  },
  ani_rate: {
    code: "ani_rate",
    limit: 50,
    show: 1,
    dec: 24
  },
  ani_once: {
    code: "ani_once",
    limit: 50,
    show: 1,
    dec: 1
  },
  ani_loop: {
    code: "ani_loop",
    limit: 50,
    show: 1,
    dec: 1
  },
  ani_fireworks: {
    code: "ani_fireworks",
    limit: 50,
    show: 1
  },
  num_year: {
    code: 'num_year',
    limit: 10,
    format: 'YY',
    show: 2,
    options: ['tens_and_ones', 'ten', 'individual'],
  },
  num_year_four: {
    code: 'num_year_four',
    limit: 10,
    format: 'YYYY',
    show: 4,
    options: ['year_four', 'thousand', 'hundred', 'ten', 'individual'],
  },
  num_month: {
    code: 'num_month',
    limit: 10,
    format: 'MM',
    show: 2,
    options: ['tens_and_ones', 'ten', 'individual'],
  },
  num_date: {
    code: 'num_date',
    format: 'MM:DD',
    show: 6,
    options: ['date', 'M_ten', 'M_individual', 'unit', 'd_ten', 'd_individual'],
    limit: {"-1": 11, 0: 10, 1: 10, 2: 1, 3: 10, 4: 10, 5: 1},
  },
  num_day: {
    code: 'num_day',
    limit: 10,
    format: 'DD',
    show: 2,
    options: ['tens_and_ones', 'ten', 'individual'],
  },
  num_hour: {
    code: 'num_hour',
    limit: 10,
    format: 'HH',
    show: 2,
    options: ['tens_and_ones', 'ten', 'individual'],
  },
  num_min: {
    code: 'num_min',
    limit: 10,
    format: 'mm',
    show: 2,
    options: ['tens_and_ones', 'ten', 'individual'],
  },
  num_sec: {
    code: 'num_sec',
    limit: 10,
    format: 'ss',
    show: 2,
    options: ['tens_and_ones', 'ten', 'individual'],
  },
  txt_month: {
    code: 'txt_month',
    limit: 12,
    format: 'M',
    show: 1,
  },
  txt_week: {
    code: 'txt_week',
    limit: 7,
    format: 'd',
    show: 1,
  },
  txt_ampm: {
    show: 1,
    code: 'txt_ampm',
    format: 'A',
    limit: 2,
  },
  time_hour: {
    show: 1,
    code: 'time_hour',
    limit: 1,
    format: 'h',
    dec: 0,
  },
  time_min: {
    code: 'time_min',
    limit: 1,
    show: 1,
    format: 'm',
    dec: 1,
  },
  time_sec: {
    code: 'time_sec',
    format: 's',
    show: 1,
    limit: 1,
    dec: 2,
  },
  time_dot_top: {
    code: 'time_dot_top',
    show: 1,
    limit: 1,
  },
  time_dot_bottom: {
    code: 'time_dot_bottom',
    show: 1,
    limit: 1,
  },
  time_dot: {
    code: 'time_dot',
    show: 1,
    limit: 1,
  },
  time_24_hour: {
    code: 'time_24_hour',
    show: 1,
    limit: 60,
    max: 59,
    dec: 0
  },
  time_60_minute: {
    code: 'time_60_minute',
    show: 1,
    limit: 60,
    max: 59,
    dec: 1
  },
  time_60_seconds: {
    code: 'time_60_seconds',
    show: 1,
    limit: 60,
    max: 59,
    dec: 2
  },
  unit_steps: {
    show: 1,
    code: 'unit_steps',
    limit: 1,
  },
  unit_bpm: {
    code: 'unit_bpm',
    show: 1,
    limit: 1
  },
  unit_km: {
    code: 'unit_km',
    show: 1,
    options: ['mi', 'km'],
    limit: 2
  },
  unitk_cal: {
    code: 'unitk_cal',
    show: 1,
    limit: 1,
  },
  unit_sleeph: {
    show: 1,
    code: 'unit_sleeph',
    limit: 1,
    options: ['thousand', 'hundred', 'ten', 'individual'],
  },
  bluetooth_0: {
    code: 'bluetooth_0',
    options: ['connected', 'disconnected'],
    show: 1,
    limit: 2
  },
  ic_sleep: {
    code: 'ic_sleep',
    show: 1,
    limit: 1
  },
  ic_sleeph: {
    code: 'ic_sleeph',
    show: 1,
    limit: 1
  },
  ic_sleepm: {
    code: 'ic_sleepm',
    show: 1,
    limit: 1
  },
  ic_weather: {
    text: '天气图标',
    code: 'ic_weather',
    limit: 9,
    show: 1,
    options: ['lightning', 'cloudy', 'sunny', 'unknownWeather', 'foggy', 'Snow', 'cloudy2', 'rainy', 'thunderstorm'],
  },
  ic_batteryL: {
    code: 'ic_batteryL',
    show: 1,
    limit: 1,
    max: 1,
  },
  ic_batteryR: {
    code: 'ic_batteryR',
    show: 1,
    limit: 1,
    max: 1,
  },
  ic_battery: {
    code: 'ic_battery',
    show: 1,
    limit: 1,
    max: 1,
    dec: 209
  },
  ic_steps: {
    code: 'ic_steps',
    show: 1,
    limit: 1,
    max: 1,
  },
  ic_heart: {
    code: 'ic_heart',
    show: 1,
    limit: 1,
    max: 1,
  },
  ic_km: {
    code: 'ic_km',
    show: 1,
    limit: 1,
    max: 1,
  },
  ic_other: {
    code: 'ic_other', // we will be supporting caloiries icon using this
    show: 1,
    limit: 1,
  },
  ic_clear: {
    code: 'ic_clear',
    show: 1,
    limit: 1,
  },
  gra_steps: {
    code: 'gra_steps',
    show: 1,
    limit: 50,
    max: 49,
    dec: 0
  },
  gra_sleep: {
    code: 'gra_sleep',
    show: 1,
    limit: 50,
    max: 49,
    dec: 1
  },
  gra_kcal: {
    code: 'gra_kcal',
    show: 1,
    limit: 50,
    max: 49,
    dec: 2
  },
  gra_km: {
    code: 'gra_km',
    show: 1,
    limit: 50,
    max: 49,
    dec: 3
  },
  gra_sport_target: {
    code: 'gra_sport_target',
    show: 1,
    limit: 50,
    max: 49,
    dec: 4
  },
  gra_bpm: {
    code: 'gra_bpm',
    show: 1,
    limit: 6,
    max: 5,
    dec: 5
  },
  gra_battery: {
    code: 'gra_battery',
    show: 1,
    limit: 50,
    max: 49,
    dec: 6
  },
  num_sleep: {
    show: 6,
    code: 'num_sleep',
    limit: {"-1": 12, 0: 10, 1: 10, 2: 1, 3: 1},
    options: ['full_icon', 'hour', 'minute', 'unit_h', 'unit_m'],
  },
  num_sleeph: {
    show: 2,
    code: 'num_sleeph',
    limit: 10,
    max: 99,
    dec: 0
  },
  num_sleepm: {
    show: 2,
    code: 'num_sleepm',
    limit: 10,
    max: 99,
    dec: 1
  },
  num_sleep_target: {
    show: 6,
    code: 'num_sleep',
    max: 99,
    limit: {"-1": 12, 0: 10, 1: 10, 2: 1, 3: 1},
    options: ['full_icon', 'hour', 'minute', 'unit_h', 'unit_m'],
  },
  num_sleep_h_target: {
    show: 2,
    code: 'num_sleep_h_target',
    limit: 10,
    max: 99,
    dec: 0
  },
  num_sleep_m_target: {
    show: 2,
    code: 'num_sleep_m_target',
    limit: 10,
    max: 99,
    dec: 1
  },
  num_sport: {
    show: 6,
    code: 'num_sport',
    max: 99,
    limit: {"-1": 12, 0: 10, 1: 10, 2: 1, 3: 1},
    options: ['full_icon', 'hour', 'minute', 'unit_h', 'unit_m'],
  },
  num_sport_h: {
    show: 2,
    code: 'num_sport_h',
    limit: 10,
    max: 99,
    dec: 0
  },
  num_sport_m: {
    show: 2,
    code: 'num_sport_m',
    limit: 10,
    max: 99,
    dec: 1
  },
  num_sport_target: {
    show: 6,
    code: 'num_sport_target',
    limit: {"-1": 12, 0: 10, 1: 10, 2: 1, 3: 1},
    options: ['full_icon', 'hour', 'minute', 'unit_h', 'unit_m'],
  },
  num_sport_h_target: {
    show: 2,
    code: 'num_sport_h_target',
    limit: 10,
    max: 99,
    dec: 0
  },
  num_sport_m_target: {
    show: 2,
    code: 'num_sport_m_target',
    limit: 10,
    max: 99,
    dec: 1
  },
  num_alarm: {
    show: 6,
    code: 'num_alarm',
    limit: {"-1": 12, 0: 10, 1: 10, 2: 1, 3: 1},
    options: ['full_icon', 'hour', 'minute', 'unit_h', 'unit_m'],
  },
  num_time: {
    show: 6,
    code: 'num_time',
    format: 'HH:mm',
    limit: {"-1": 11, 0: 10, 1: 10, 2: 1, 3: 10, 4: 10, 5: 1},
    options: ['time', 'h_ten', 'h_individual', 'unit_colon', 'm_ten', 'm_individual'], // 冒号
  },
  num_time_word: {
    show: 6,
    code: 'num_time_word',
    format: 'HH:mm',
    limit: {"-1": 11, 0: 10, 1: 10, 2: 1, 3: 10, 4: 10, 5: 1},
    options: ['time_world', 'h_ten', 'h_individual', 'unit_colon', 'm_ten', 'm_individual'], // 冒号
  },
  txt_time_word_city: {
    show: 0,
    code: 'txt_time_word_city',
    limit: 0
  },
  ic_word_city: {
    code: 'ic_word_city',
    show: 1,
    limit: 4,
    max: 4,
    options: [0, 1, 2, 3]
  },
  ic_local_clock_city: {
    code: 'ic_local_clock_city',
    show: 1,
    limit: 4,
    max: 4,
    options: [0, 1, 2, 3]
  },
  num_alarm_h: {
    show: 2,
    code: 'num_sport_target',
    limit: 10,
    max: 99,
    dec: 0
  },
  num_alarm_m: {
    show: 2,
    code: 'num_alarm_target',
    limit: 10,
    max: 99,
    dec: 1
  },
  num_weather: {
    code: 'num_weather',
    limit: 13,
    show: 5,
    max: 999,
  },
  num_weather3: {
    code: 'num_weather3',
    limit: 12,
    show: 5,
    max: 999,
  },
  num_weather2: {
    code: 'num_weather2',
    limit: 13,
    show: 5,
    options: [0, 1],
    max: 999,
  },
  num_stepst: {
    show: 5,
    code: 'num_stepst',
    limit: 10,
    max: 99999,
  },
  num_steps: {
    code: 'num_steps',
    show: 5,
    limit: 12,
    max: 99999
  },
  num_steps_icon: {
    code: 'num_steps_icon',
    show: 5,
    limit: 10,
    max: 99999
  },
  num_heart: {
    show: 3,
    code: 'num_heart',
    limit: 12,
    max: 299
  },
  num_cal: {
    show: 4,
    code: 'num_cal',
    limit: 11,
    max: 9999
  },
  num_cal_target: {
    show: 4,
    code: 'num_cal_target',
    limit: 10,
    max: 9999
  },
  num_km: {
    show: 3,
    code: 'num_km',
    limit: 14,
    max: 99999
  },
  num_km_target: {
    show: 3,
    code: 'num_km_target',
    limit: 11,
    max: 99999
  },
  num_battery: {
    show: 3,
    code: 'num_battery',
    limit: 10,
    max: 100  
  },
  num_blood_pressure: {
    show: 5,
    code: 'num_blood_pressure',
    limit: 13,
    max: 9999
  },
  num_blood_oxygen: {
    show: 2,
    code: 'num_blood_oxygen',
    limit: 12,
    max: 99
  },
  num_body_temperature: {
    show: 5,
    code: 'num_body_temperature',
    limit: 14,
    max: 40
  },
  language: {
    code: 'language',
    limit: 10,
    options: ['en', 'zh'],
  },
  time_set: {
    code: 'time_set',
    limit: 1,
    options: ['12', '24'],
  },
  ic_default_dash: {
    code: 'ic_default_dash',
    show: 1,
    limit: 1,
    max: 1,
  },
}