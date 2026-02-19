/**
 * Test script for MOY Generator Backend
 * Verifies the server is working correctly
 */

const testBackend = async () => {
  const backendUrl = process.env.BACKEND_URL || 'http://localhost:5555';
  
  console.log('🧪 Testing MOY Generator Backend\n');
  
  // Test 1: Health Check
  console.log('1️⃣  Testing health endpoint...');
  try {
    const response = await fetch(`${backendUrl}/api/health`);
    const data = await response.json();
    if (data.status === 'ok') {
      console.log('   ✅ Health check passed\n');
    } else {
      console.log('   ❌ Health check failed\n');
      return;
    }
  } catch (error) {
    console.log('   ❌ Cannot connect to backend');
    console.log('   Make sure the server is running: cd server && npm start\n');
    return;
  }
  
  // Test 2: Generate sample MOY
  console.log('2️⃣  Testing MOY generation...');
  try {
    // Create a minimal test MOY structure
    const testMoyFile = {
      _id: 'TEST123',
      faceName: 'Test Watch',
      resolution: {
        width: '454',
        height: '454',
        radian: '227px',
        thumbnail: { width: '200', height: '241' }
      },
      platform: '杰理',
      layerGroups: [
        {
          id: 'layer1',
          type: 'selectImg',
          code: 'bg',
          parent: 'bg',
          index: 0,
          nodeAttr: {
            size: { width: 454, height: 454 },
            position: { left: 0, top: 0 },
            numberType: 'bg_0',
            selectImg: [
              {
                name: 'test_bg.png',
                url: 'blob:test-url'
              }
            ]
          }
        }
      ]
    };
    
    // Create a simple test image (1x1 transparent PNG)
    const testImageBase64 = 'iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==';
    
    const requestData = {
      moyFile: testMoyFile,
      images: [
        {
          name: 'test_bg.png',
          data: `data:image/png;base64,${testImageBase64}`,
          url: 'blob:test-url'
        }
      ]
    };
    
    const response = await fetch(`${backendUrl}/api/generate-moy`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(requestData)
    });
    
    if (!response.ok) {
      const error = await response.json();
      console.log('   ❌ MOY generation failed:', error.error);
      return;
    }
    
    const result = await response.json();
    console.log('   ✅ MOY generation successful');
    console.log(`   Filename: ${result.filename}`);
    console.log(`   Size: ${result.size} bytes\n`);
    
    // Test 3: Verify MOY structure
    console.log('3️⃣  Verifying MOY file structure...');
    const moyData = Buffer.from(result.data, 'base64');
    
    // Check for MOYEND marker
    const moyendPos = moyData.indexOf('MOYEND');
    if (moyendPos > 0) {
      console.log(`   ✅ MOYEND marker found at position ${moyendPos}`);
    } else {
      console.log('   ❌ MOYEND marker not found');
      return;
    }
    
    // Check for IMGEND marker
    const imgendPos = moyData.indexOf('IMGEND');
    if (imgendPos > moyendPos) {
      console.log(`   ✅ IMGEND marker found at position ${imgendPos}`);
    } else {
      console.log('   ❌ IMGEND marker not found');
      return;
    }
    
    // Verify JSON
    try {
      const jsonStr = moyData.slice(0, moyendPos).toString('utf-8');
      const parsed = JSON.parse(jsonStr);
      console.log(`   ✅ JSON structure valid`);
      console.log(`   Watch name: ${parsed.faceName}\n`);
    } catch (error) {
      console.log('   ❌ Invalid JSON structure');
      return;
    }
    
    console.log('✅ All tests passed! Backend is working correctly.\n');
    
  } catch (error) {
    console.log('   ❌ Test failed:', error.message);
  }
};

// Run tests
testBackend();
