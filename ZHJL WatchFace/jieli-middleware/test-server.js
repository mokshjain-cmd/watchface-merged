// Simple test server to verify Express is working
const express = require('express');
const app = express();
const PORT = 3001;

app.get('/api/health', (req, res) => {
  res.json({
    status: 'ok',
    service: 'JieLi Middleware',
    version: '1.0.0',
    timestamp: new Date().toISOString()
  });
});

app.listen(PORT, () => {
  console.log(`✅ Test server running on http://localhost:${PORT}`);
  console.log(`Test: http://localhost:${PORT}/api/health`);
});
