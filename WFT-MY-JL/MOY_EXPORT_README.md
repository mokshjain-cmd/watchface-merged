# MOY File Export Feature - MoYoung Vendor Integration

## Overview

This update adds support for exporting watch face designs in the **MOY file format** required by the MoYoung vendor's bin generator, while maintaining backward compatibility with the existing ZhouHai vendor's JSON format.

## What Changed

### 1. New MOY Generator Utility (`src/utils/MoyGenerator.ts`)

A comprehensive utility class that handles the conversion from the internal watch face format to MOY format:

**Key Features:**
- Converts `WatchFaceComponent` objects to MOY `layerGroups` format
- Generates proper MOY structure with resolution, metadata, and layer groups
- Handles image embedding with PNG binary data
- Maintains MOY-specific attributes like `numberType`, `parent`, `code`, etc.

**MOY File Structure:**
```
{
  "resolution": { ... },
  "platform": "杰理",
  "layerGroups": [ ... ],
  ...
}MOYEND<PNG_BINARY_DATA>IMGEND...
```

### 2. Updated Assembly View (`src/components/AssemblyView.tsx`)

Added new export functionality:

**New Function:**
- `exportMoyFile()` - Generates and downloads MOY file with embedded images

**UI Changes:**
- Added "Export MOY" button in the toolbar (cyan/info color)
- Button shows "Generating..." state during export
- Success/error alerts for user feedback

### 3. CSS Styling (`src/components/AssemblyView.css`)

Added `.btn-info` style for the MOY export button:
- Cyan/teal color scheme (#17a2b8)
- Hover effects
- Disabled state styling

## How to Use

### For Developers

1. **Import the MOY Generator:**
```typescript
import { MoyGenerator } from '../utils/MoyGenerator';
```

2. **Export MOY File:**
```typescript
const moyBlob = await MoyGenerator.exportMoyFile(project, imageData);
MoyGenerator.downloadMoyFile(moyBlob, filename);
```

### For Users

1. Design your watch face using the assembly tool
2. Click the **"Export MOY"** button in the toolbar
3. The `.moy` file will be downloaded automatically
4. Use this file with the MoYoung backend bin generator

## Format Mapping

### Component Type Mapping

| Internal Type | MOY Parent | MOY Code | Description |
|--------------|-----------|----------|-------------|
| image (bg) | bg | bg | Background image |
| dateTime (date formats) | date | dateText | Month, WeekDay display |
| dateTime (time formats) | time | timeNumber | Hour, Minute, Second |
| number | widget | functionNumber | Steps, Heart Rate, etc. |
| progress | widget | progress | Battery, progress bars |
| analog | time | analogClock | Analog clock hands |

### numberType Mapping

| Component | MOY numberType |
|-----------|---------------|
| Hour | num_hour |
| Minute | num_min |
| Second | num_sec |
| Day | num_day |
| Month | txt_month |
| WeekDay | txt_week |
| Steps | num_steps |
| Heart Rate | num_heart |
| Calories | num_cal |
| Distance | num_km |
| Battery | gra_battery |
| Blood Oxygen | num_spo2 |

## Technical Details

### MOY File Format

The MOY file is a hybrid format:
1. **JSON Section**: Contains metadata and component definitions
2. **Binary Section**: Contains PNG images embedded after the JSON
3. **Marker**: `MOYEND` separates JSON from binary data
4. **Image Markers**: Each PNG is followed by `IMGEND`

### Key MOY Attributes

- **resolution**: Watch dimensions and thumbnail size
- **platform**: Vendor platform (default: "杰理" - JieLi)
- **layerGroups**: Array of components with positioning and images
- **nodeAttr**: Component-specific attributes (size, position, images, etc.)

### Image Handling

Images are:
1. Fetched from blob URLs
2. Converted to PNG binary format
3. Embedded directly in the MOY file after the JSON section
4. Referenced by filename in the layerGroup's `selectImg` array

## Backward Compatibility

All existing functionality remains intact:
- ✅ Export JSON (ZhouHai format)
- ✅ Generate Bin File (ZhouHai backend)
- ✅ Save/Open Project
- ✅ All component types and configurations

## Testing Recommendations

1. **Test MOY Export:**
   - Create a watch face with various component types
   - Click "Export MOY"
   - Verify file downloads successfully

2. **Verify MOY Structure:**
   - Open the MOY file in a text editor
   - Check JSON structure matches example MOY files
   - Verify `MOYEND` marker exists
   - Confirm PNG binary data is present

3. **Backend Integration:**
   - Send generated MOY file to MoYoung bin generator
   - Verify bin file generates successfully
   - Test bin file on actual device

## Future Enhancements

Potential improvements:
- Add MOY validation before export
- Support for additional MOY-specific attributes
- Custom platform selection
- Preview MOY structure before export
- Batch export multiple designs

## Error Handling

The MOY exporter includes comprehensive error handling:
- ✅ Missing images are logged
- ✅ Blob fetch failures are caught
- ✅ User-friendly error messages
- ✅ Success confirmation alerts

## Files Modified

1. `src/utils/MoyGenerator.ts` - **NEW** - Core MOY generation logic
2. `src/components/AssemblyView.tsx` - Added MOY export function and button
3. `src/components/AssemblyView.css` - Added btn-info styling

## Dependencies

No new dependencies required. Uses existing:
- React
- TypeScript
- Blob API
- File Download API

---

**Note**: The MOY file format is specific to the MoYoung vendor. Always verify compatibility with your target backend bin generator version.
