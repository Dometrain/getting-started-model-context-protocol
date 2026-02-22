<#
.SYNOPSIS
    Acquires an access token from Microsoft Entra ID for testing the MCP server.

.DESCRIPTION
    This script uses Azure CLI to authenticate and obtain an access token for the
    McpServerAzure API. You must be signed in to Azure CLI (`az login`) and have
    appropriate permissions to request tokens for the configured API.

.PARAMETER TenantId
    Your Microsoft Entra tenant ID. If not provided, uses the default tenant from Azure CLI.

.PARAMETER ClientId
    The Application (client) ID of your MCP Server app registration.
    This is the "audience" the token is issued for.

.PARAMETER Scope
    The scope to request. Defaults to "api://<ClientId>/mcp.tools".

.EXAMPLE
    .\get-token.ps1 -ClientId "00000000-0000-0000-0000-000000000000"

.EXAMPLE
    .\get-token.ps1 -ClientId "00000000-0000-0000-0000-000000000000" -TenantId "your-tenant-id"

.NOTES
    Prerequisites:
    1. Azure CLI installed: https://docs.microsoft.com/cli/azure/install-azure-cli
    2. Signed in: `az login`
    3. App registration created with "mcp.tools" scope exposed
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory = $true, HelpMessage = "The Application (client) ID of your MCP Server app registration.")]
    [string]$ClientId,

    [Parameter(Mandatory = $false, HelpMessage = "Your Microsoft Entra tenant ID.")]
    [string]$TenantId,

    [Parameter(Mandatory = $false, HelpMessage = "The scope to request.")]
    [string]$Scope
)

$ErrorActionPreference = "Stop"

# Build the scope if not explicitly provided
if (-not $Scope) {
    $Scope = "api://$ClientId/mcp.tools"
}

Write-Host "Acquiring access token..." -ForegroundColor Cyan
Write-Host "  Scope: $Scope" -ForegroundColor Gray

try {
    # Check if Azure CLI is installed
    $azCommand = Get-Command az -ErrorAction SilentlyContinue
    if (-not $azCommand) {
        Write-Host "Error: Please install the Azure CLI" -ForegroundColor Red
        Write-Host "  Install from: https://docs.microsoft.com/cli/azure/install-azure-cli" -ForegroundColor Yellow
        exit 1
    }

    # Check if signed in
    $account = az account show 2>$null | ConvertFrom-Json
    if (-not $account) {
        Write-Host "You are not signed in to Azure CLI. Running 'az login'..." -ForegroundColor Yellow
        az login
    }

    # Build the az command
    $azArgs = @("account", "get-access-token", "--scope", $Scope, "--query", "accessToken", "-o", "tsv")
    
    if ($TenantId) {
        $azArgs += @("--tenant", $TenantId)
    }

    # Get the token
    $token = & az @azArgs

    if (-not $token) {
        throw "Failed to acquire access token. Ensure you have consented to the scope."
    }

    Write-Host ""
    Write-Host "Access Token:" -ForegroundColor Green
    Write-Host "─" * 80 -ForegroundColor DarkGray
    Write-Host $token
    Write-Host "─" * 80 -ForegroundColor DarkGray
    Write-Host ""

    # Copy to clipboard if available
    if (Get-Command Set-Clipboard -ErrorAction SilentlyContinue) {
        $token | Set-Clipboard
        Write-Host "Token copied to clipboard!" -ForegroundColor Green
    }

    Write-Host ""
    Write-Host "Example curl command:" -ForegroundColor Cyan
    Write-Host @"
curl -X POST http://localhost:5000/mcp \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $token" \
  -d '{"jsonrpc":"2.0","id":1,"method":"tools/list"}'
"@

    Write-Host ""
    Write-Host "Example PowerShell command:" -ForegroundColor Cyan
    Write-Host @"
`$headers = @{ "Authorization" = "Bearer $token"; "Content-Type" = "application/json" }
Invoke-RestMethod -Uri "http://localhost:5000/mcp" -Method Post -Headers `$headers -Body '{"jsonrpc":"2.0","id":1,"method":"tools/list"}'
"@

    return $token
}
catch {
    Write-Host "Error: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "Troubleshooting tips:" -ForegroundColor Yellow
    Write-Host "  1. Ensure Azure CLI is installed: https://docs.microsoft.com/cli/azure/install-azure-cli"
    Write-Host "  2. Sign in with: az login"
    Write-Host "  3. If using a specific tenant: az login --tenant <tenant-id>"
    Write-Host "  4. Ensure the app registration exposes the 'mcp.tools' scope"
    Write-Host "  5. Ensure you (or an admin) have consented to the scope"
    exit 1
}

