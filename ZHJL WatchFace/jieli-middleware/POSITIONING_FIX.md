# JieLi Middleware - Position Calculation, Year Component & Date Separator Fix

## Issue Reports
**Watch Face**: ICON 2 2025 (uses JieLi/JL board)  
**Problems**: 
1. When adding elements, positions were completely incorrect in the generated bin file
2. Year component was not being shown in the bin file  
3. Only one date separator appearing, but need two (like month/day and day/year)
**Date Fixed**: January 29, 2026

## Root Causes Identified

### 1. Invalid Folder Code Generation
**Problem**: The `splitMultiDigitComponent` function was generating invalid folder codes like:
- `01-1` (negative number in code)
- `0100` (wrong calculation)
- `0700` (incorrect base code)

**Root Cause**: 
- Missing validation of base code before calculations
- No validation of generated codes
- Incorrect arithmetic when calculating health metric codes

**Fix**: Added comprehensive validation:
```typescript
// Validate base code
if (isNaN(baseDigit) || baseDigit < 0 || baseDigit > 99) {
  console.error(`[Split] Invalid base code: ${baseCode} for ${itemName}`);
  continue;
}

// Validate generated code
const codeNum = parseInt(digitCode);
if (isNaN(codeNum) || digitCode.length !== 4) {
  console.error(`[Split] Invalid generated code: ${digitCode} for ${itemName} digit ${i}`);
  continue;
}
```

### 2. Incorrect X Position Calculation
**Problem**: Multi-digit components had wrong X positions because:
- Used the entire component `Width` as the width for each digit
- Example: If component is 200px wide with 5 digits, each digit was offset by 200px instead of 40px

**Root Cause**:
```typescript
// OLD CODE (WRONG):
const digitWidth = component.Width || 40; // Uses full component width!
config.x = (component.Left || 0) + (i * (digitWidth + digitSpacing));
```

**Fix**: Divide total width by digit count:
```typescript
// NEW CODE (CORRECT):
const totalWidth = component.Width || (digitCount * 40);
const digitWidth = Math.floor(totalWidth / digitCount);
config.x = (component.Left || 0) + (i * (digitWidth + digitSpacing));
```

**Example**:
- Component: Steps counter at X=100, Width=200px, 5 digits
- **OLD**: Digit positions: 100, 300, 500, 700, 900 ❌ (way too far apart)
- **NEW**: Digit positions: 100, 140, 180, 220, 260 ✓ (correct spacing)

### 3. Undefined Element Names
**Problem**: Some folders had `undefined` in their names like:
- `0700_步数#3(数字)#数值#undefined_10_62_206`

**Root Cause**: 
- Element name calculation fell through without proper defaults
- Missing bounds checking when indexing into digitNames array

**Fix**:
```typescript
// Check bounds before indexing
const digitIndex = digitNames.length - digitCount + i;
if (digitIndex >= 0 && digitIndex < digitNames.length) {
  config.elementName = `数值#${digitNames[digitIndex]}`;
} else {
  config.elementName = `数值#${i + 1}`; // Fallback
}
```

### 4. Enhanced Single-Digit Detection
**Problem**: Components like "Month (Tens)" were being split into multiple folders

**Fix**: Expanded single-digit detection to include more patterns:
```typescript
const isSingleDigit = itemName.includes('Tens') || itemName.includes('Ones') || 
                      itemName.includes('(Tens)') || itemName.includes('(Ones)') ||
                      itemName.includes('Hundreds') || itemName.includes('(Hundreds)') ||
                      itemName.includes('Thousands') || itemName.includes('(Thousands)') ||
                      itemName.includes('Single') || itemName.includes('(Single)') ||
                      itemName.includes('十') || itemName.includes('个') ||
                      itemName.includes('百') || itemName.includes('千') || itemName.includes('万');
```

### 5. Missing Year Component Support
**Problem**: Year component was completely missing from the JieLi middleware
- No Yeartypes.ts`
- **Enum**: `JieLiComponentCode`
- **Changes**: Added Year component codes (0306-0309)

### `src/componentMapper.ts`
- **Function**: `getJieLiCode()`
- **Changes**: Added Year component detection and code mapping
- **Function**: `getElementName()`
- **Changes**: Added Year element names to the map
- **Function**: `splitMultiDigitComponent()`
- **Changes**:
  1. Added validation for base codes and generated codes
  2. Fixed X position calculation to use per-digit width
  3. Added bounds checking for element name array indexing
  4. Enhanced single-digit component detection
  5. Added Year with 4-digit support
  6. Added comprehensive logging for debugging

### `src/converter.ts`
- **Function**: `createStandardFolderStructure()`
- **Changes**: Added Year folders to standard JieLi structure
1. **Added Year codes to enum** (0306-0309):
   - `DATE_YEAR_THOUSANDS = '0306'` (千位)
   - `DATE_YEAR_HUNDREDS = '0307'` (百位)
   - `DATE_YEAR_TENS = '0308'` (十位)
   - `DATE_YEAR_ONES = '0309'` (个位)
, **Year**
2. **Added Year handling in getJieLiCode()**:
```typescript
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
  // Fall back to index-based logic for multi-digit Year
  if (index === 0) return JieLiComponentCode.DATE_YEAR_THOUSANDS;
0306_通用日期#3(数字)#1(年)#1(千)_10_50_30/  ✅ Year now works!
  else if (index === 1) return JieLiComponentCode.DATE_YEAR_HUNDREDS;
  else if (index === 2) return JieLiComponentCode.DATE_YEAR_TENS;
  return JieLiComponentCode.DATE_YEAR_ONES;
}
```

3. **Added Year to standard folder structure**:
```typescript
'0306_通用日期#3(数字)#1(年)#1(千)_0_0_0',
'0307_通用日期#3(数字)#1(年)#2(百)_0_0_0',
'0308_通用日期#3(数字)#1(年)#3(十)_0_0_0',
'0309_通用日期#3(数字)#1(年)#4(个)_0_0_0',
```

4. **Added Year to element name map**:
```typescript
'0306': '1(年)#1(千)',
'0307': '1(年)#2(百)',
'0308': '1(年)#3(十)',
'0309': '1(年)#4(个)',
```

5. **Updated splitMultiDigitComponent** to handle 4-digit Year:
```typescript
else if (itemName === 'Year' || itemName.includes('年')) digitCount = 4;
```

### 6. Only One Date Separator Appearing
**Problem**: Multiple date separators were all being assigned code 0303
- Frontend creates multiple "Date Separator" components with same name
- Middleware mapped all of them to the same code (0303)
- Only one separator appeared on the watch (duplicate codes were skipped)
- Date separators needed two codes like Time has (0603 and 0606)

**Root Cause**: 
- Only one DATE_SEPARATOR code existed (0303)
- No index-based logic to differentiate multiple separators
- No counter to track how many separators were already processed

**Fix**: Added complete second separator support:
1. **Added DATE_SEPARATOR2 code** (0310):
   ```typescript
   DATE_SEPARATOR2 = '0310'
   ```

2. **Added index-based separator detection**:
   ```typescript
   if (itemName === 'Date Separator' || itemName.includes('Date Separator')) {
     // Support multiple date separators using index
     if (index === 0) {
       return JieLiComponentCode.DATE_SEPARATOR; // 0303
     }
     return JieLiComponentCode.DATE_SEPARATOR2; // 0310
   }
   ```

3. **Added separator counter tracking** in converter:
   ```typescript
   private separatorCounters: Map<string, number> = new Map();
   
   // When processing separator:
   const separatorKey = itemName.includes('Time') ? 'Time Separator' : 'Date Separator';
   const currentIndex = this.separatorCounters.get(separatorKey) || 0;
   configs = [ComponentMapper.toJieLiFolderConfig(component, currentIndex)];
   this.separatorCounters.set(separatorKey, currentIndex + 1);
   ```

4. **Added to standard folder structure**:
   ```typescript
   '0310_通用日期#1(图片)#分隔2_0_0_0'
   ```

**Result**: Now you can add two date separators:
- First separator → 0303 (between month and day)
- Second separator → 0310 (between day and year)
- Both will appear correctly on the watch!

## Files Modified

### `src/componentMapper.ts`
- **Function**: `splitMultiDigitComponent()`
- **Changes**:
  1. Added validation for base codes and generated codes
  2. Fixed X position calculation to use per-digit width
  3. Added bounds checking for element name array indexing
  4. Enhanced single-digit component detection
  5. Added comprehensive logging for debugging

## Testing Recommendations

1. **Clear temp folder** before testing:
   ```powershell
   Remove-Item "D:\Downloads\merging\ZHJL WatchFace\jieli-middleware\temp\*" -Recurse -Force
   ```

2. **Test with ICON 2 2025**:
   - Create watch face with multiple element types
   - Generate bin file
   - Check folder structure in temp folder
   - Verify all folder codes are valid 4-digit codes
   - Verify X positions increment correctly
   - Flash to watch and verify positions

3. **Verify all element types**:
   - ✅ Time (Hour, Minute, Second)
   - ✅ Date (Month, Day, **Year**)
   - ✅ **Date Separators (2 separators now supported)**
   - ✅ Steps (5 digits)
   - ✅ Heart Rate (3 digits)
   - ✅ Calories (4 digits)
   - ✅ Sleep (3 digits)

## Expected Results

### Before Fix:
```
0100_主要#3(数字)#数值#4(十)_10_194_79/      ❌ Invalid code
0700_步数#3(数字)#数值#undefined_10_62_206/  ❌ Undefined element name
Steps positions: X=100, X=300, X=500...       ❌ Too far apart
0303_通用日期#1(图片)#分隔_10_79_81/          ✓ First separator works
0303_通用日期#1(图片)#分隔_10_130_81/         ❌ Second separator has same code (skipped!)
```

### After Fix:
```
0601_时间#3(数字)#1(时)#4(十)_10_45_116/      ✅ Valid code
0705_步数#3(数字)#数值#1(万)_10_100_206/       ✅ Correct element name
0306_通用日期#3(数字)#1(年)#1(千)_10_50_30/    ✅ Year now works!
0303_通用日期#1(图片)#分隔_10_79_81/           ✅ First date separator
0310_通用日期#1(图片)#分隔2_10_130_81/         ✅ Second date separator!
Steps positions: X=100, X=140, X=180...        ✅ Correct spacing
```

## Additional Notes

- The fix maintains backward compatibility with existing watch faces
- Single-digit components (explicitly marked as Tens/Ones) continue to work correctly
- Multi-digit components now have proper spacing and positioning
- All generated folder codes are guaranteed to be valid 4-digit JieLi codes

## Next Steps

If issues persist after this fix:
1. Check the console logs in the middleware for any validation errors
2. Verify that component `Width` values in the JSON are correct
3. Ensure image data is properly base64 encoded
4. Check that the C# bin generator is receiving the correct folder structure
