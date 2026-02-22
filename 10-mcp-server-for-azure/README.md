# Demo MCP Server (.NET 10) - Deploy to Azure

Builds on section 09 (authenticated server with Resources and Tools). This section covers **deploying the MCP server to Azure** using Docker and Azure Container Apps, including forwarded headers for correct URLs behind a load balancer.

## Before you run

1. **appsettings.json** and **.vscode/launch.json** (for local run / F5): Replace `<AZURE_TENANT_ID>`, `<AZURE_CLIENT_ID>`, and `<AZURE_STORAGE_CONNECTION_STRING>` with your values from Azure Portal.
2. **Docker / Azure deploy**: Replace the same placeholders in the commands below with your actual values.

---

## Instructions for deploying...


### Prerequisites

- [Azure CLI](https://docs.microsoft.com/cli/azure/install-azure-cli) installed and configured
- Azure subscription (free tier is sufficient)
- Docker installed (for local testing, optional)
- Your Entra ID app registration configured (see Quick Start section)

### Step 1: Test Docker Image Locally (Optional)

Before deploying to Azure, you can test the Docker image locally:

```powershell
# Build the Docker image
docker build -t mcp-server:latest .

# Run the container locally
docker run -p 8080:8080 `
  -e ASPNETCORE_ENVIRONMENT=Production `
  -e AzureAd__TenantId="<YOUR_TENANT_ID>" `
  -e AzureAd__ClientId="<YOUR_CLIENT_ID>" `
  -e AzureAd__Audience="api://<YOUR_CLIENT_ID>" `
  -e AzureAd__Instance="https://login.microsoftonline.com/" `
  -e AzureAd__Scopes="mcp.tools" `
  -e Storage__ConnectionString="<CONNECTION_STRING>" `
  mcp-server:latest

# With your values (replace the placeholders):
docker run -p 8080:8080 `
  -e ASPNETCORE_ENVIRONMENT=Production `
  -e AzureAd__TenantId="<AZURE_TENANT_ID>" `
  -e AzureAd__ClientId="<AZURE_CLIENT_ID>" `
  -e AzureAd__Audience="api://<AZURE_CLIENT_ID>" `
  -e AzureAd__Instance="https://login.microsoftonline.com/" `
  -e AzureAd__Scopes="mcp.tools" `
  -e Storage__ConnectionString="<AZURE_STORAGE_CONNECTION_STRING>" `
  mcp-server:latest
```

Test it at `http://localhost:8080/mcp` with a valid access token.


## Azure Setup

```powershell
az login
```

### Need to register these providers first! put it in the course

```powershell
# (Optional) sanity check you're in the right subscription
az account show
# If needed:
# az account set --subscription "<subscription-id-or-name>"

# Register required providers
az provider register --namespace Microsoft.ContainerRegistry

# You will also need these for Container Apps in your script:
az provider register --namespace Microsoft.App
az provider register --namespace Microsoft.OperationalInsights

```

...check them

```powershell
az provider show -n Microsoft.ContainerRegistry --query "registrationState" -o tsv
az provider show -n Microsoft.App --query "registrationState" -o tsv
az provider show -n Microsoft.OperationalInsights --query "registrationState" -o tsv
```


```powershell
$resourceGroup = "mcp-server-rg"
$location = "uksouth"
$containerAppName = "jc-mcp-server-demo-01"
$environmentName = "mcp-server-env"
$registryName = "mcpserver45987235"

az group create --name $resourceGroup --location $location

# Create Azure Container Registry (for storing Docker image)
az acr create --resource-group $resourceGroup --name $registryName --sku Basic --admin-enabled true

# Build and push Docker image to ACR
az acr build --registry $registryName --image mcp-server:latest --file Dockerfile .

# Create Container Apps environment
az containerapp env create `
  --name $environmentName `
  --resource-group $resourceGroup `
  --location $location

# Get registry credentials
$registryPassword = az acr credential show --name $registryName --query "passwords[0].value" -o tsv
$registryServer = "$registryName.azurecr.io"

# Create Container App
az containerapp create `
  --name $containerAppName `
  --resource-group $resourceGroup `
  --environment $environmentName `
  --image "$registryServer/mcp-server:latest" `
  --registry-server $registryServer `
  --registry-username $registryName `
  --registry-password $registryPassword `
  --target-port 8080 `
  --ingress external `
  --env-vars "ASPNETCORE_ENVIRONMENT=Production" `
  --cpu 0.25 --memory 0.5Gi `
  --min-replicas 0 --max-replicas 1


# Set the environment variables...
az containerapp update `
  --name $containerAppName `
  --resource-group $resourceGroup `
  --set-env-vars "AzureAd__TenantId=<AZURE_TENANT_ID>" `
                  "AzureAd__ClientId=<AZURE_CLIENT_ID>" `
                  "AzureAd__Audience=api://<AZURE_CLIENT_ID>" `
                  "AzureAd__Instance=https://login.microsoftonline.com/" `
                  "AzureAd__Scopes=mcp.tools" `
                  "Storage__ConnectionString=<AZURE_STORAGE_CONNECTION_STRING>"


# Get App URL
az containerapp show `
  --name $containerAppName `
  --resource-group $resourceGroup `
  --query "properties.configuration.ingress.fqdn" `
  --output tsv
```

### DELETE IT AFTER

(A "resource group" is like a Cfn stack on AWS)

```powershell
az group delete --name mcp-server-rg --yes --no-wait
```




### Tail the logs from Azure container...

```powershell
az containerapp logs show --name $containerAppName --resource-group $resourceGroup --follow
```

### View the env vars if you get errors...

```powershell
az containerapp show --name jc-mcp-server-demo-01 --resource-group $resourceGroup --query "properties.template.containers[0].env" -o table
```



### Updating the Deployment

When you make changes to your code, rebuild and redeploy:

```powershell

# Set variables (customize these)
$resourceGroup = "mcp-server-rg"
$containerAppName = "jc-mcp-server-demo-01"  # Must be globally unique
$registryName = "mcpserver45987235"
$registryServer = "$registryName.azurecr.io"

# Navigate to the repository root (if not already there)
cd C:\Code\mcp-course\10-mcp-server-for-azure  # Adjust path as needed

# Rebuild and push the Docker image to Azure Container Registry
# This builds the image from your current code and pushes it to ACR
az acr build --registry $registryName --image mcp-server:latest --file Dockerfile .

# Update the container app to use the new image
# Azure Container Apps will automatically pull the new image and restart the container
az containerapp update `
  --name $containerAppName `
  --resource-group $resourceGroup `
  --image "$registryServer/mcp-server:latest"

# Wait a minute or two for the container to restart, then verify it's running
az containerapp show --name $containerAppName --resource-group $resourceGroup --query "properties.runningStatus" -o tsv
```

**Note:** The container app will automatically restart with the new image. Environment variables and other configuration settings are preserved and don't need to be re-set unless you want to change them.
