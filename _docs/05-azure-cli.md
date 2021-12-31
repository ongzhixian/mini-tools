# Azure CLI

## Get info

az group list | convertfrom-json | select name
az appservice plan list | convertfrom-json | select name
az webapp list | convertfrom-json | select name

az webapp list-runtimes 
az account list-locations | convertfrom-json | select name

os-type == Windows | Linux

## Zip

Compress-Archive -Path D:\src\github\mini-tools\MiniTools.Web\bin\Release\net6.0\publish\* deploy.zip -Force

## One-time create

`az webapp create --name mini-tools --plan telera-app-service-plan --resource-group telera-resource-group --runtime "DOTNET:6.0"`

## Deploy


Works!
PS> az webapp deploy --name mini-tools-app --resource-group telera-resource-group --src-path deploy.zip --type zip
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
