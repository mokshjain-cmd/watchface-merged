# Setting Up MOYoung API Credentials

This guide shows you how to configure your MOYoung API credentials for BIN file generation.

## Step 1: Locate the Server Directory

Navigate to your project's server directory:
```bash
cd "c:\Users\Kumar Aniket\WFT-MY-JL\server"
```

## Step 2: Create the .env File

Copy the example file:
```powershell
# PowerShell
Copy-Item .env.example .env
```

Or create it manually:
```powershell
# PowerShell
New-Item -Path .env -ItemType File
```

## Step 3: Edit the .env File

Open `server/.env` in any text editor and add your credentials:

```env
# MOYoung API Credentials
# Replace these with your actual credentials for the MOYoung API
MOYOUNG_USERNAME=your_actual_username_here
MOYOUNG_PASSWORD=your_actual_password_here
```

**Example:**
```env
MOYOUNG_USERNAME=john.doe@example.com
MOYOUNG_PASSWORD=MySecurePassword123!
```

## Step 4: Verify the Setup

Check that your .env file exists:
```powershell
# PowerShell
Get-Content .env
```

You should see your credentials (be careful not to share this output!).

## Step 5: Test the Connection

Start the server:
```bash
npm start
```

Try generating a BIN file from the frontend. If authentication works, you'll see:
```
🔐 Authenticating with MOYoung API...
✅ Authentication successful
```

If authentication fails, you'll see:
```
❌ Authentication failed: 401 - Invalid credentials
```

## Important Security Notes

### ⚠️ DO NOT:
- ❌ Commit `.env` file to Git
- ❌ Share `.env` file publicly
- ❌ Include credentials in screenshots
- ❌ Hard-code credentials in source files

### ✅ DO:
- ✅ Keep `.env` file local only
- ✅ Use `.gitignore` to exclude `.env`
- ✅ Use environment variables for production
- ✅ Rotate credentials periodically

## Git Configuration

Make sure `.env` is in your `.gitignore`:

```gitignore
# Environment variables
.env
.env.local
.env.*.local

# Server temp files
server/temp/
```

Check if `.env` is ignored:
```bash
git status
```

`.env` should NOT appear in the output.

## Production Deployment

For production environments, use proper secret management:

### Option 1: Environment Variables
```bash
export MOYOUNG_USERNAME="your_username"
export MOYOUNG_PASSWORD="your_password"
```

### Option 2: Cloud Secret Manager
- AWS Secrets Manager
- Azure Key Vault
- Google Cloud Secret Manager

### Option 3: Container Secrets
```dockerfile
# Docker
docker run -e MOYOUNG_USERNAME=xxx -e MOYOUNG_PASSWORD=xxx ...
```

## Troubleshooting

### Issue: .env file not found
**Solution**: Make sure you're in the `server` directory when creating the file.

### Issue: Credentials not loading
**Solution**: Restart the Node.js server after editing `.env`

### Issue: Authentication still failing
**Solution**: 
1. Verify credentials are correct
2. Check for extra spaces in `.env` file
3. Ensure no quotes around values (unless needed)
4. Test credentials directly on MOYoung website

### Issue: Permission denied
**Solution**: Check file permissions:
```powershell
# PowerShell
icacls .env
```

## Getting MOYoung Credentials

If you don't have MOYoung API credentials:

1. **Contact MOYoung Support**:
   - Visit: https://moyoung.com (or appropriate URL)
   - Request API access
   - Provide your use case

2. **Developer Portal**:
   - Sign up for developer account
   - Create API credentials
   - Note the username and password

3. **Existing Account**:
   - Log in to MOYoung platform
   - Navigate to API settings
   - Generate new credentials

## Testing Without Credentials

If you don't have credentials yet:

1. Use the **"Export MOY"** button instead
2. This generates .moy files without API calls
3. Later, when you have credentials:
   - Set up `.env`
   - Use **"Generate Bin File"** for direct BIN generation

## Support

If you need help:
- Check server console for detailed error messages
- Review [BIN_GENERATION_GUIDE.md](BIN_GENERATION_GUIDE.md)
- Contact MOYoung support for API access
