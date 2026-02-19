# MOY Export - Quick Reference Guide

## 🚀 Quick Start

### Export a MOY File (3 Steps)

1. **Design your watch face** using the assembly tool
2. **Click "Export MOY"** button (cyan button in toolbar)
3. **Download completes** - Your `.moy` file is ready!

---

## 🔧 For Developers

### Using the MOY Generator

```typescript
import { MoyGenerator } from '../utils/MoyGenerator';

// Collect images
const imageData = new Map<string, Blob>();
// ... fetch and add images

// Generate MOY
const moyBlob = await MoyGenerator.exportMoyFile(project, imageData);

// Download
MoyGenerator.downloadMoyFile(moyBlob, 'watchface.moy');
```

---

## 📋 Component Mapping Reference

| Your Component | MOY numberType | MOY Parent |
|---------------|---------------|-----------|
| Background Image | `bg_0` | bg |
| Hour | `num_hour` | time |
| Minute | `num_min` | time |
| Second | `num_sec` | time |
| Day | `num_day` | date |
| Month | `txt_month` | date |
| WeekDay | `txt_week` | date |
| Steps | `num_steps` | widget |
| Heart Rate | `num_heart` | widget |
| Calories | `num_cal` | widget |
| Distance | `num_km` | widget |
| Battery | `gra_battery` | widget |
| Blood Oxygen | `num_spo2` | widget |

---

## 📁 Files Reference

| File | Purpose |
|------|---------|
| `src/utils/MoyGenerator.ts` | Core generator logic |
| `src/components/AssemblyView.tsx` | UI integration |
| `MOY_EXPORT_README.md` | Full documentation |
| `IMPLEMENTATION_SUMMARY.md` | Implementation details |

---

## ⚠️ Important Notes

1. **MOY Structure Must Stay Intact**
   - The bin generator expects specific format
   - Don't manually edit generated MOY files
   - Always use the export tool

2. **Image Handling**
   - Images are embedded as PNG binary data
   - File ends with `MOYEND` marker
   - Each image ends with `IMGEND` marker

3. **Platform**
   - Default: "杰理" (JieLi)
   - Matches MoYoung requirements

---

## 🐛 Troubleshooting

### MOY Export Button Not Working
- Check browser console for errors
- Ensure all components have images
- Try with simpler design first

### Generated MOY Not Working in Backend
- Verify file size > 0
- Check MOYEND marker exists
- Compare structure with example MOY files

### Build Errors
```bash
npm run build
```
If errors appear, check TypeScript types

---

## 📞 Need Help?

1. Read `MOY_EXPORT_README.md` for details
2. Check example MOY files in parent folder
3. Review `MoyGenerator.ts` source code
4. Test with known-working designs first

---

## ✅ Verification Checklist

Before sending MOY to backend:

- [ ] File downloads successfully
- [ ] File size > 0 bytes
- [ ] Open in text editor - JSON visible
- [ ] Contains `MOYEND` marker
- [ ] Contains binary PNG data after JSON
- [ ] File extension is `.moy`

---

## 🎯 Best Practices

1. **Test incrementally:**
   - Start with simple design (bg + time)
   - Add components gradually
   - Export after each addition

2. **Keep backups:**
   - Save project as JSON too
   - Keep original images
   - Document component mappings

3. **Validate output:**
   - Compare with example MOY files
   - Check structure matches
   - Test with backend before production

---

**Quick Tip:** Export both JSON and MOY formats - JSON for editing later, MOY for bin generation!
