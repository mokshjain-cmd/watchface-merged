# Test script for JieLi Middleware

Write-Host "==================================" -ForegroundColor Cyan
Write-Host "JieLi Middleware - Test Suite" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""

$baseUrl = "http://localhost:3001"

# Test 1: Health Check
Write-Host "Test 1: Health Check" -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/health" -Method Get
    Write-Host "✓ Status: $($response.status)" -ForegroundColor Green
    Write-Host "✓ Service: $($response.service)" -ForegroundColor Green
} catch {
    Write-Host "✗ Health check failed: $_" -ForegroundColor Red
}

Write-Host ""

# Test 2: Service Info
Write-Host "Test 2: Service Information" -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/jieli/info" -Method Get
    Write-Host "✓ Service: $($response.service)" -ForegroundColor Green
    Write-Host "✓ Input Format: $($response.capabilities.inputFormat)" -ForegroundColor Green
    Write-Host "✓ Output Format: $($response.capabilities.outputFormat)" -ForegroundColor Green
    Write-Host "✓ Bin Generator Available: $($response.binGenerator.available)" -ForegroundColor Green
} catch {
    Write-Host "✗ Service info failed: $_" -ForegroundColor Red
}

Write-Host ""

# Test 3: Test Conversion
Write-Host "Test 3: Test Conversion (Minimal Watch Face)" -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/jieli/test" -Method Get
    Write-Host "✓ Success: $($response.success)" -ForegroundColor Green
    Write-Host "✓ Project Path: $($response.projectPath)" -ForegroundColor Green
    Write-Host "✓ Files Created:" -ForegroundColor Green
    $response.files | ForEach-Object { Write-Host "  - $_" -ForegroundColor Gray }
} catch {
    Write-Host "✗ Test conversion failed: $_" -ForegroundColor Red
}

Write-Host ""
Write-Host "==================================" -ForegroundColor Cyan
Write-Host "Test Suite Complete" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
