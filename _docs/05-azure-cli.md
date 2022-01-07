# Azure CLI

## Get info

az group list | convertfrom-json | select name
az appservice plan list | convertfrom-json | select name
az webapp list | convertfrom-json | select name

az webapp list-runtimes 
az account list-locations | convertfrom-json | select name

os-type == Windows | Linux


## Creation

`az group create --name mini-tools-rg --location southeastasia --tags project=mini-tools`

`az appservice plan create --name mini-tools-appservice-plan --resource-group mini-tools-rg --location southeastasia --sku FREE --tags project=mini-tools`

`az storage account create --name minitools --resource-group mini-tools-rg --location southeastasia --sku Standard_LRS --tags project=mini-tools`

`az webpubsub create --name minitools-wps --resource-group mini-tools-rg --location southeastasia --sku Free_F1 --tags project=mini-tools`

`az storage table create --name minitools --account-name minitools`

`az webapp create --name mini-tools --plan mini-tools-appservice-plan --resource-group mini-tools-rg --runtime DOTNET:6.0`



--DEPLOYMENT

`dotnet publish .\MiniTools.Web\ --configuration=Release`
`Compress-Archive -Path C:\src\github.com\ongzhixian\mini-tools\MiniTools.Web\bin\Release\net6.0\publish\* deploy.zip -Force`
`az webapp deploy --name mini-tools --resource-group mini-tools-rg --src-path deploy.zip --type zip`


## Zip

Compress-Archive -Path D:\src\github\mini-tools\MiniTools.Web\bin\Release\net6.0\publish\* deploy.zip -Force

## One-time create

`az webapp create --name mini-tools --plan telera-app-service-plan --resource-group telera-resource-group --runtime "DOTNET:6.0"`

## Deploy


Works!
PS> az webapp deploy --name mini-tools --resource-group telera-resource-group --src-path deploy.zip --type zip
This command is in preview and under development. Reference and support levels: https://aka.ms/CLI_refstatus


## Aside `az webapp up`

For some strange reason, deployment using `az webapp up` does not work. :-(

```cmd
DESKTOP-NJM00MP>zhixian D:\src\github\mini-tools\MiniTools.Web (main)
PS> az webapp up --sku F1 --name mini-tools-app --os-type Windows --runtime "DOTNET:6.0" --resource-group telera-resource-group --plan telera-app-service-plan --location southeastasia
```

`az webapp up --name mini-tools --os-type Windows --runtime "DOTNET:6.0" --resource-group telera-resource-group --plan telera-app-service-plan --location southeastasia`

```
{"Message":"An error has occurred.","ExceptionMessage":"No log found for 'latest'.","ExceptionType":"System.IO.FileNotFoundException","StackTrace":"   at Kudu.Core.Deployment.DeploymentManager.GetLogEntries(String id) in C:\\Kudu Files\\Private\\src\\master\\Kudu.Core\\Deployment\\DeploymentManager.cs:line 98
   at Kudu.Services.Deployment.DeploymentController.GetLogEntry(String id) in C:\\Kudu Files\\Private\\src\\master\\Kudu.Services\\Deployment\\DeploymentController.cs:line 376"}
```


## Publish script
```
dotnet publish .\MiniTools.Web\ --configuration=Release
Compress-Archive -Path D:\src\github\mini-tools\MiniTools.Web\bin\Release\net6.0\publish\* deploy.zip -Force
az webapp deploy --name mini-tools --resource-group telera-resource-group --src-path deploy.zip --type zip
```

```
Write-Host "Publishing project..."
dotnet publish .\MiniTools.Web\ --configuration=Release

if ($LASTEXITCODE)
{
    Write-Host "Publish failed"
    exit 1
}

Write-Host "Project published."
Write-Host "Zipping output to 'deploy.zip'..."

Compress-Archive -Path D:\src\github\mini-tools\MiniTools.Web\bin\Release\net6.0\publish\* deploy.zip -Force

if ($LASTEXITCODE)
{
    Write-Host "Making  zip file ('deploy.zip') failed."
    exit 1
}

Write-Host "Zip file 'deploy.zip' created."
Write-Host "Deploying to Azure..."

az webapp deploy --name mini-tools --resource-group telera-resource-group --src-path deploy.zip --type zip

if ($LASTEXITCODE)
{
    Write-Host "Failed to deploy to Azure."
    exit 1
}

Write-Host "Deployment completed."
```