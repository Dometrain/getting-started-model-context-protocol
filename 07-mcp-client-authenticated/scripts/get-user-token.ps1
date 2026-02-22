<#
.SYNOPSIS
    Acquires an access token for the MCP Client API from Microsoft Entra ID.

.DESCRIPTION
    This script uses Azure CLI to authenticate and obtain an access token for the
    MCP Client API. The token can then be used to call the /api/chat endpoint.
    
    This is for testing the On-Behalf-Of (OBO) flow:
    1. You get a token for the MCP Client API (this script)
    2. The Client API exchanges it for an MCP Server token (OBO)
    3. The Client API calls the MCP Server with the exchanged token

.PARAMETER TenantId
    Your Microsoft Entra tenant ID. If not provided, uses the default tenant from Azure CLI.

.PARAMETER ClientId
    The Application (client) ID of your MCP Client API app registration.
    This is the "audience" the token is issued for.

.PARAMETER Scope
    The scope to request. Defaults to "api://<ClientId>/access_as_user".

.PARAMETER ClientApiUrl
    The base URL of your MCP Client API. Defaults to "https://localhost:5001".

.EXAMPLE
    .\get-user-token.ps1 -ClientId "00000000-0000-0000-0000-000000000000"

.EXAMPLE
    .\get-user-token.ps1 -ClientId "00000000-0000-0000-0000-000000000000" -TenantId "your-tenant-id"

.EXAMPLE
    .\get-user-token.ps1 -ClientId "00000000-0000-0000-0000-000000000000" -ClientApiUrl "https://my-api.azurewebsites.net"

.NOTES
    Prerequisites:
    1. Azure CLI installed: https://docs.microsoft.com/cli/azure/install-azure-cli
    2. Signed in: `az login`
    3. MCP Client API app registration with "access_as_user" scope exposed
    4. Consent granted for the scope (admin or user consent)
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory = $true, HelpMessage = "The Application (client) ID of your MCP Client API app registration.")]
    [string]$ClientId,

    [Parameter(Mandatory = $false, HelpMessage = "Your Microsoft Entra tenant ID.")]
    [string]$TenantId,

    [Parameter(Mandatory = $false, HelpMessage = "The scope to request.")]
    [string]$Scope,

    [Parameter(Mandatory = $false, HelpMessage = "The base URL of your MCP Client API.")]
    [string]$ClientApiUrl = "https://localhost:5001"
)

$ErrorActionPreference = "Stop"

# Build the scope if not explicitly provided
if (-not $Scope) {
    $Scope = "api://$ClientId/access_as_user"
}

Write-Host "Acquiring access token for MCP Client API..." -ForegroundColor Cyan
Write-Host "  Client ID: $ClientId" -ForegroundColor Gray
Write-Host "  Scope: $Scope" -ForegroundColor Gray
Write-Host ""

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
    Write-Host "Example curl command to call MCP Client API:" -ForegroundColor Cyan
    Write-Host @"
curl -X POST $ClientApiUrl/api/chat \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $token" \
  -d '{"message": "What tools do you have available?"}'
"@

    Write-Host ""
    Write-Host "Example PowerShell command:" -ForegroundColor Cyan
    Write-Host @"
`$headers = @{ 
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json" 
}
`$body = @{ message = "What tools do you have available?" } | ConvertTo-Json
Invoke-RestMethod -Uri "$ClientApiUrl/api/chat" -Method Post -Headers `$headers -Body `$body
"@

    Write-Host ""
    Write-Host "─" * 80 -ForegroundColor DarkGray
    Write-Host "OBO Flow Explanation:" -ForegroundColor Yellow
    Write-Host "  1. This token is for the MCP Client API (audience: api://$ClientId)"
    Write-Host "  2. When you call /api/chat, the Client API exchanges this token"
    Write-Host "     for an MCP Server token using On-Behalf-Of (OBO) flow"
    Write-Host "  3. The Client API then calls the MCP Server with the exchanged token"
    Write-Host "─" * 80 -ForegroundColor DarkGray

    return $token
}
catch {
    Write-Host "Error: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "Troubleshooting tips:" -ForegroundColor Yellow
    Write-Host "  1. Ensure Azure CLI is installed: https://docs.microsoft.com/cli/azure/install-azure-cli"
    Write-Host "  2. Sign in with: az login"
    Write-Host "  3. If using a specific tenant: az login --tenant <tenant-id>"
    Write-Host "  4. Ensure the MCP Client API app registration exposes the 'access_as_user' scope"
    Write-Host "  5. Ensure you (or an admin) have consented to the scope"
    Write-Host ""
    Write-Host "  If you get AADSTS65001 (consent required), grant consent by running:" -ForegroundColor Cyan
    Write-Host "    az logout"
    Write-Host "    az login --tenant `"<YOUR_TENANT_ID>`" --scope `"api://<CLIENT_ID>/access_as_user`""
    exit 1
}

