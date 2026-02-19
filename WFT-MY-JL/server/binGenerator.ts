/**
 * BIN File Generator
 * Converts MOY files to BIN files using MOYoung API
 */

import axios, { AxiosError } from 'axios';
import { createReadStream, readFileSync, writeFileSync } from 'fs';
import { join } from 'path';
import FormData from 'form-data';
import http from 'http';
import https from 'https';

// Keep-alive agents for persistent connections
const httpAgent = new http.Agent({
  keepAlive: true,
  keepAliveMsecs: 30000,
  maxSockets: 50,
  maxFreeSockets: 10,
  timeout: 120000,
});

const httpsAgent = new https.Agent({
  keepAlive: true,
  keepAliveMsecs: 30000,
  maxSockets: 50,
  maxFreeSockets: 10,
  timeout: 120000,
});

// API Configuration
const API_BASE_URL = 'http://vega.test.moyoung.com/api';
const LOGIN_ENDPOINT = `${API_BASE_URL}/login`;
const CREATE_TASK_ENDPOINT = `${API_BASE_URL}/tool/create-face-task`;
const GET_TASK_ENDPOINT = `${API_BASE_URL}/tool/get-face-task`;

// You need to provide the actual credentials
const API_CREDENTIALS = {
  type: 0,
  username: "Jagatheeswaran",
  password: "12345678",
};

interface LoginResponse {
  code: number,
  msg: string,
  data: {
    token: string;
  }
}

interface CreateTaskResponse {
  code: number,
  data: {
    task_id: string;
  };
}

interface GetTaskResponse {
  code: number,
  data: {
    moy_file: string, // URL to the moy file
    face_log: string, // MOY file contents in text format
    preview: string,
    status: number,
    face_file: string; // URL to the bin file
  };
}

/**
 * Step 1: Authenticate and get token
 */
async function authenticate(): Promise<string> {
  try {
    console.log('🔐 Authenticating with MOYoung API...');

    const response = await axios.post<LoginResponse>(LOGIN_ENDPOINT, API_CREDENTIALS);

    const token = response.data.data.token;
    console.log('✅ Authentication successful');
    console.log(`response data: ${JSON.stringify(response.data)}`);
    return token;
  } catch (error) {
    if (axios.isAxiosError(error)) {
      const axiosError = error as AxiosError;
      console.error('❌ Authentication failed:', axiosError.response?.data || axiosError.message);
      throw new Error(`Authentication failed: ${axiosError.response?.status} - ${JSON.stringify(axiosError.response?.data)}`);
    }
    throw error;
  }
}

/**
 * Step 2: Upload MOY file and create task
 */
async function createFaceTask(token: string, moyFilePath: string): Promise<string> {
  try {
    console.log('📤 Uploading MOY file to create face task...');

    // Get file info
    const fileName = moyFilePath.split(/[\\/]/).pop() || 'watchface.moy';
    const fileStats = readFileSync(moyFilePath);

    console.log(`   File size: ${fileStats.length} bytes`);
    console.log(`   File name: ${fileName}`);

    // Validate token
    if (!token) {
      throw new Error('Authentication token is missing');
    }

    // Create form data - using stream for better compatibility
    // multipart/form-data with field name 'file' and binary format
    const formData = new FormData();
    const fileStream = createReadStream(moyFilePath);
    formData.append('file', fileStream, {
      filename: fileName,
      contentType: 'application/form-data',
    });

    console.log(`   Sending to: ${CREATE_TASK_ENDPOINT}`);
    console.log(`   Token: ${token.substring(0, 20)}...`);

    // Send POST request with keep-alive agent
    const response = await axios.post<CreateTaskResponse>(
      CREATE_TASK_ENDPOINT,
      formData,
      {
        headers: {
          'accept': 'application/json',
          'token': token,
          'Connection': 'keep-alive',
          // Let form-data set the Content-Type with boundary
          ...formData.getHeaders(),
        },
        httpAgent: httpAgent,
        httpsAgent: httpsAgent,
        maxContentLength: Infinity,
        maxBodyLength: Infinity,
        timeout: 120000,
      }
    );

    console.log(`   Response status: ${response.status}`);
    console.log('   Response data:', JSON.stringify(response.data, null, 2));

    if (!response.data || response.data.code !== 0) {
      const errorMsg = response.data ? JSON.stringify(response.data) : 'No response data';
      throw new Error(`API returned error: ${errorMsg}`);
    }

    const taskId = response.data.data.task_id;
    console.log(`✅ Task created successfully: ${taskId}`);

    return taskId;
  } catch (error) {
    if (axios.isAxiosError(error)) {
      const axiosError = error as AxiosError;
      console.error('❌ Failed to create task');
      console.error('   Error:', axiosError.message);
      console.error('   Code:', axiosError.code);
      console.error('   Response status:', axiosError.response?.status);
      console.error('   Response headers:', axiosError.response?.headers);
      console.error('   Response data:', axiosError.response?.data);

      if (axiosError.code === 'ECONNABORTED' || axiosError.code === 'ETIMEDOUT') {
        throw new Error(`Request timeout - The MOYoung API server is not responding.`);
      }

      if (axiosError.code === 'ECONNRESET') {
        throw new Error(`Connection reset - Server closed the connection. Check file format and size.`);
      }

      throw new Error(`Upload failed: ${axiosError.message}`);
    }
    console.error('❌ Unexpected error:', error);
    throw error;
  }
}

/**
 * Step 3: Get face task result (with polling)
 */
async function getFaceTask(
  token: string,
  taskId: string,
  maxRetries: number = 60,
  delayMs: number = 2000
): Promise<string> {
  try {
    console.log('⏳ Polling for task completion...');

    for (let attempt = 1; attempt <= maxRetries; attempt++) {
      const response = await axios.get<GetTaskResponse>(
        GET_TASK_ENDPOINT,
        {
          params: { key: taskId }, // safer than manually appending
          headers: {
            accept: 'application/json',
            token: token,
          },
          timeout: 10000,
        }
      );

      console.log(`Attempt ${attempt}/${maxRetries} - code: ${response.data.code}`);

      if (response.data.code !== 0) {
        throw new Error(`API returned error code ${response.data.code}`);
      }

      const { face_file, status, moy_file, preview } = response.data.data;

      console.log(`   Status: ${status}`);

      if (moy_file) console.log(`   MOY file: ${moy_file}`);
      if (preview) console.log(`   Preview: ${preview}`);

      // ✅ If still processing, keep retrying
      if (status === 0) {
        console.log(`   ⏳ Still processing... retrying in ${delayMs}ms`);

        if (attempt < maxRetries) {
          await new Promise(resolve => setTimeout(resolve, delayMs));
          continue;
        }
      }

      // ✅ If finished, return face_file
      if (status === 1) {
        if (face_file && face_file.includes("/bin/") && face_file.endsWith(".bin")) {
          console.log(`✅ Task completed! BIN file URL: ${face_file}`);
          return face_file;
        }

        throw new Error(`Task status=1 but face_file is invalid: ${face_file}`);
      }

      // ⚠️ Unknown status handling
      throw new Error(`Unexpected task status received: ${status}`);
    }

    throw new Error(`Task did not complete within ${maxRetries * delayMs / 1000} seconds`);
  } catch (error) {
    if (axios.isAxiosError(error)) {
      const axiosError = error as AxiosError;
      console.error('❌ Failed to get task:', axiosError.response?.data || axiosError.message);
      throw new Error(`Get task failed: ${axiosError.code || axiosError.message}`);
    }
    throw error;
  }
}


/**
 * Step 4: Download BIN file from URL
 */
async function downloadBinFile(url: string, outputPath: string): Promise<void> {
  try {
    console.log('⬇️  Downloading BIN file...');

    const response = await axios.get(url, {
      responseType: 'arraybuffer',
    });

    writeFileSync(outputPath, response.data);
    console.log(`✅ BIN file saved to: ${outputPath}`);
  } catch (error) {
    if (axios.isAxiosError(error)) {
      const axiosError = error as AxiosError;
      console.error('❌ Failed to download BIN file:', axiosError.message);
      throw new Error(`Download failed: ${axiosError.message}`);
    }
    throw error;
  }
}

/**
 * Main function: Convert MOY file to BIN file
 */
export async function generateBinFromMoy(moyFilePath: string, outputDir: string): Promise<{ binPath: string; binUrl: string }> {
  try {
    console.log('\n╔════════════════════════════════════════╗');
    console.log('║   MOY to BIN Conversion Process        ║');
    console.log('╚════════════════════════════════════════╝\n');
    console.log(`📁 Input MOY file: ${moyFilePath}\n`);

    // Step 1: Authenticate
    const token = await authenticate();
    console.log("is token present?", token)
    // Step 2: Create task
    const taskId = await createFaceTask(token, moyFilePath);

    // Step 3: Get task result (with polling)
    const binUrl = await getFaceTask(token, taskId);

    // Step 4: Download BIN file
    const moyFileName = moyFilePath.split(/[\\/]/).pop() || 'watchface.moy';
    const binFileName = moyFileName.replace('.moy', '.bin');
    const binPath = join(outputDir, binFileName);
    console.log(`\n📥 BIN file URL: ${binUrl}`);
    await downloadBinFile(binUrl, binPath);

    console.log('\n✅ MOY to BIN conversion completed successfully!\n');

    return { binPath, binUrl };
  } catch (error) {
    console.error('\n❌ MOY to BIN conversion failed:', error);
    throw error;
  }
}

/**
 * Convenience function: Generate BIN from MOY buffer
 */
export async function generateBinFromMoyBuffer(
  moyBuffer: Buffer,
  moyFileName: string,
  tempDir: string,
  outputDir: string
): Promise<{ binPath: string; binUrl: string }> {
  // Save MOY buffer to temporary file
  const tempMoyPath = join(tempDir, moyFileName);
  writeFileSync(tempMoyPath, moyBuffer);

  // Generate BIN file
  return generateBinFromMoy(tempMoyPath, outputDir);
}
