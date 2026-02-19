import express from 'express';
import XLSX from 'xlsx';
import cors from 'cors';
import path from 'path';
import fs from 'fs';
import { fileURLToPath } from 'url';

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

const app = express();
const PORT = 3002;

// Enable CORS for frontend
app.use(cors());

// Serve static images
app.use('/images', express.static(path.join(__dirname, 'images')));

// Excel file path
const EXCEL_FILE_PATH = path.join(__dirname, '..', 'Watch List.xlsx');

// Endpoint to get all watches
app.get('/api/watches', (req, res) => {
  try {
    // Check if file exists
    if (!fs.existsSync(EXCEL_FILE_PATH)) {
      return res.status(404).json({ error: 'Watch List.xlsx not found' });
    }

    // Read the Excel file
    const workbook = XLSX.readFile(EXCEL_FILE_PATH);
    const sheetName = workbook.SheetNames[0];
    const worksheet = workbook.Sheets[sheetName];
    
    // Convert to JSON
    const data = XLSX.utils.sheet_to_json(worksheet);
    
    // Transform the data to match our interface
    const watches = data.map(row => {
      const imageName = row['Image'] || row['image'] || '';
      return {
        name: (row['Watch '] || row['Watch'] || '').trim(),
        vendor: (row['Vendor'] || '').trim(),
        board: (row['SDK'] || '').trim(),
        width: row['Width'] || null,
        height: row['Height'] || null,
        shape: (row['Shape'] || '').trim(),
        image: imageName ? `/images/${imageName.trim()}` : null
      };
    }).filter(watch => watch.name && watch.vendor && watch.board);
    
    res.json({ watches, count: watches.length });
  } catch (error) {
    console.error('Error reading Excel file:', error);
    res.status(500).json({ error: 'Failed to read watch data', message: error.message });
  }
});

// Health check endpoint
app.get('/api/health', (req, res) => {
  res.json({ status: 'OK', timestamp: new Date().toISOString() });
});

app.listen(PORT, () => {
  console.log(`Watch data server running on http://localhost:${PORT}`);
  console.log(`Excel file path: ${EXCEL_FILE_PATH}`);
});
