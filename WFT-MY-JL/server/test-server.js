// Simple test server
const express = require('express');
const cors = require('cors');

const app = express();
const PORT = 5555;

app.use(cors());
app.use(express.json({ limit: '50mb' }));

app.get('/api/health', (req, res) => {
  console.log('Health check received');
  res.json({ status: 'ok', service: 'MOY Generator Backend' });
});

app.listen(PORT, () => {
  console.log(`\n✅ Server running at http://localhost:${PORT}\n`);
});
