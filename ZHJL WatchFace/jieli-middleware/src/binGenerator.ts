import { spawn } from 'child_process';
import * as fs from 'fs-extra';
import * as path from 'path';

/**
 * Integrates with the C# JieLi bin generator (WatchJieLi.exe)
 */
export class BinGenerator {
  private watchDBGeneratorPath: string;
  private binGeneratorPath: string;
  private outputFolderPath: string;

  constructor(watchDBGeneratorPath: string, binGeneratorPath: string, outputFolderPath: string) {
    this.watchDBGeneratorPath = watchDBGeneratorPath;
    this.binGeneratorPath = binGeneratorPath;
    this.outputFolderPath = outputFolderPath;
  }

  /**
   * Generate bin file from JieLi folder structure
   * @param projectFolderPath - Path to the JieLi format project folder
   * @returns Path to the generated bin file
   */
  async generateBin(projectFolderPath: string): Promise<string> {
    try {
      // Ensure output directory exists
      await fs.ensureDir(this.outputFolderPath);

      // Step 1: Generate watch.db database
      console.log('\n========================================');
      console.log('STEP 1: Generating watch.db database');
      console.log('========================================');
      await this.callWatchDBGenerator(projectFolderPath);

      // Step 2: Generate bin file using the example WatchFaceCLI (expects watch.db to exist)
      console.log('\n========================================');
      console.log('STEP 2: Generating bin file');
      console.log('========================================');
      await this.callWatchFaceCLI(projectFolderPath);
      
      // Find the generated bin file in the output folder
      const binFile = await this.findGeneratedBinFile(projectFolderPath);
      
      if (!binFile) {
        throw new Error('Bin file generation failed - no output file found');
      }

      return binFile;
    } catch (error) {
      console.error('Bin generation failed:', error);
      throw error;
    }
  }

  /**
   * Call the WatchDBGenerator.dll to create watch.db
   */
  private async callWatchDBGenerator(projectFolderPath: string): Promise<void> {
    return new Promise((resolve, reject) => {
      // Check if DLL exists
      if (!fs.existsSync(this.watchDBGeneratorPath)) {
        reject(new Error(`WatchDBGenerator DLL not found at: ${this.watchDBGeneratorPath}`));
        return;
      }

      // WatchDBGenerator expects: dotnet WatchDBGenerator.dll <watchfaceFolder>
      const absoluteProjectPath = path.resolve(projectFolderPath);

      const args = [this.watchDBGeneratorPath, absoluteProjectPath];
      
      console.log('EXECUTING WatchDBGenerator:');
      console.log(`dotnet "${args[0]}" "${args[1]}"`);
      console.log(`DLL Path: ${this.watchDBGeneratorPath}`);
      console.log(`Watchface folder: ${absoluteProjectPath}`);
      console.log('========================================\n');
      
      const process = spawn('dotnet', args, {
        windowsHide: true,
        stdio: ['ignore', 'pipe', 'pipe']
      });

      let stdout = '';
      let stderr = '';

      process.stdout?.on('data', (data) => {
        const output = data.toString();
        stdout += output;
        console.log(`[WatchDBGenerator] ${output.trim()}`);
      });

      process.stderr?.on('data', (data) => {
        const error = data.toString();
        stderr += error;
        console.error(`[WatchDBGenerator Error] ${error.trim()}`);
      });

      process.on('close', (code) => {
        if (code === 0) {
          console.log('watch.db generation completed successfully');
          resolve();
        } else {
          const errorMsg = `watch.db generation failed with exit code ${code}`;
          console.error(errorMsg);
          console.error(`STDERR: ${stderr}`);
          console.error(`STDOUT: ${stdout}`);
          reject(new Error(errorMsg + (stderr ? `\n${stderr}` : '')));
        }
      });

      process.on('error', (error) => {
        console.error(`Failed to start WatchDBGenerator: ${error.message}`);
        reject(new Error(`Failed to start WatchDBGenerator: ${error.message}`));
      });
    });
  }

  /**
   * Call the C# WatchFaceCLI.dll to generate bin files using dotnet command
   */
  private async callWatchFaceCLI(projectFolderPath: string): Promise<void> {
    return new Promise((resolve, reject) => {
      // Check if DLL exists
      if (!fs.existsSync(this.binGeneratorPath)) {
        reject(new Error(`Bin generator DLL not found at: ${this.binGeneratorPath}`));
        return;
      }

      // WatchFaceCLI expects: dotnet WatchFaceCLI.dll <inputFolder> <outputFolder>
      // It will create a timestamped subfolder and hor.bin inside
      // Convert to absolute paths to avoid encoding issues with Chinese characters
      const absoluteProjectPath = path.resolve(projectFolderPath);
      const absoluteOutputFolder = path.resolve(this.outputFolderPath);

      // Build the command arguments array
      const args = [this.binGeneratorPath, absoluteProjectPath, absoluteOutputFolder];
      
      // Log the full command that will be executed
      console.log('\n========================================');
      console.log('EXECUTING CLI COMMAND:');
      console.log(`dotnet "${args[0]}" "${args[1]}" "${args[2]}"`);
      console.log('========================================');
      console.log(`DLL Path: ${this.binGeneratorPath}`);
      console.log(`Input folder: ${absoluteProjectPath}`);
      console.log(`Output folder: ${absoluteOutputFolder}`);
      console.log('========================================\n');
      
      // Call the CLI tool with absolute paths using dotnet command
      // WatchFaceCLI should be invoked as: dotnet WatchFaceCLI.dll <inputFolder> <outputFolder>
      // Don't use shell:true to avoid encoding issues with Chinese characters
      const process = spawn('dotnet', args, {
        windowsHide: true,
        stdio: ['ignore', 'pipe', 'pipe']
      });

      let stdout = '';
      let stderr = '';

      process.stdout?.on('data', (data) => {
        const output = data.toString();
        stdout += output;
        console.log(`[JieLiBinGenerator] ${output.trim()}`);
      });

      process.stderr?.on('data', (data) => {
        const error = data.toString();
        stderr += error;
        console.error(`[JieLiBinGenerator Error] ${error.trim()}`);
      });

      process.on('close', async (code) => {
        if (code === 0) {
          console.log('Bin generation completed successfully');
          resolve();
        } else {
          const errorMsg = `Bin generation failed with exit code ${code}`;
          console.error(errorMsg);
          console.error(`STDERR: ${stderr}`);
          console.error(`STDOUT: ${stdout}`);
          reject(new Error(errorMsg + (stderr ? `\n${stderr}` : '')));
        }
      });

      process.on('error', (error) => {
        console.error(`Failed to start bin generator: ${error.message}`);
        reject(new Error(`Failed to start bin generator: ${error.message}`));
      });
    });
  }

  /**
   * Find the generated bin file
   * WatchFaceCLI creates a subfolder named after the watchface, then timestamped folders inside that
   */
  private async findGeneratedBinFile(projectFolderPath: string): Promise<string | null> {
    try {
      // WatchFaceCLI creates structure: output/<watchface-name>/<timestamp>/hor.bin
      // First, get the watchface folder name from the project path
      const watchfaceName = path.basename(projectFolderPath);
      // Convert to absolute path to ensure proper file system access
      const absoluteOutputFolder = path.resolve(this.outputFolderPath);
      const watchfaceOutputFolder = path.join(absoluteOutputFolder, watchfaceName);
      
      console.log(`Looking for bin file in: ${watchfaceOutputFolder}`);
      
      // Check if the watchface output folder exists
      if (!await fs.pathExists(watchfaceOutputFolder)) {
        console.error(`Watchface output folder not found: ${watchfaceOutputFolder}`);
        return null;
      }
      
      // Look for the most recent timestamped folder inside the watchface folder
      const items = await fs.readdir(watchfaceOutputFolder);
      console.log(`Items in output folder: ${items.join(', ')}`);
      
      // Find timestamped folders (format: YYYY-MM-DD_HH-mm-ss or YYYY-MM-DD_HH_mm_ss)
      const timestampFolders = items.filter(item => {
        const folderPath = path.join(watchfaceOutputFolder, item);
        try {
          const isDir = fs.statSync(folderPath).isDirectory();
          // Match both formats: YYYY-MM-DD_HH-mm-ss and YYYY-MM-DD_HH_mm_ss
          const matchesPattern = item.match(/^\d{4}-\d{2}-\d{2}_\d{2}[-_]\d{2}[-_]\d{2}$/);
          console.log(`Checking item "${item}": isDir=${isDir}, matchesPattern=${!!matchesPattern}`);
          return isDir && matchesPattern;
        } catch (err) {
          console.error(`Error checking item "${item}":`, err);
          return false;
        }
      });
      
      console.log(`Found ${timestampFolders.length} timestamped folders: ${timestampFolders.join(', ')}`);

      if (timestampFolders.length > 0) {
        // Get the most recent folder
        const latestFolder = timestampFolders.sort().reverse()[0];
        const binFolder = path.join(watchfaceOutputFolder, latestFolder);
        console.log(`Checking most recent folder: ${binFolder}`);

        // Check for hor.bin
        const horBinPath = path.join(binFolder, 'hor.bin');
        console.log(`Checking for hor.bin at: ${horBinPath}`);
        const horExists = await fs.pathExists(horBinPath);
        console.log(`hor.bin exists: ${horExists}`);
        
        if (horExists) {
          console.log(`Found bin file at: ${horBinPath}`);
          return horBinPath;
        }

        // Check for ver.bin
        const verBinPath = path.join(binFolder, 'ver.bin');
        console.log(`Checking for ver.bin at: ${verBinPath}`);
        const verExists = await fs.pathExists(verBinPath);
        console.log(`ver.bin exists: ${verExists}`);
        
        if (verExists) {
          console.log(`Found bin file at: ${verBinPath}`);
          return verBinPath;
        }
      }

      console.error(`No bin file found in output folder: ${watchfaceOutputFolder}`);
      return null;
    } catch (error) {
      console.error('Error finding bin file:', error);
      return null;
    }
  }

  /**
   * Validate bin file
   */
  async validateBinFile(binPath: string): Promise<boolean> {
    try {
      const stats = await fs.stat(binPath);
      // Bin files should be reasonably sized (not empty, not too large)
      // JieLi bins can be quite small when compressed, so accept files >= 100 bytes
      return stats.size >= 100 && stats.size < 10 * 1024 * 1024; // Between 100 bytes and 10MB
    } catch {
      return false;
    }
  }
}
