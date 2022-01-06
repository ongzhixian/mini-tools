# Azure Functions

## Create

func init MiniTools.FuncApp --dotnet

--(In `./MiniTools.FuncApp`)

func new --name HttpExample --template "HTTP trigger" --authlevel "anonymous"

az functionapp create --resource-group AzureFunctionsQuickstart-rg --consumption-plan-location <REGION> --runtime dotnet --functions-version 3 --name <APP_NAME> --storage-account <STORAGE_NAME>


func azure functionapp publish <APP_NAME>


## tldr;

func init MiniTools.FuncApp --dotnet


func new --name HttpExample --template "HTTP trigger" --authlevel "anonymous"

az functionapp create --resource-group AzureFunctionsQuickstart-rg --consumption-plan-location <REGION> --runtime dotnet --functions-version 3 --name <APP_NAME> --storage-account <STORAGE_NAME>


func azure functionapp publish <APP_NAME>




## Create (details) example

ZBK15P>zhixian C:\src\github.com\ongzhixian\mini-tools\demo (main)
PS> `func init LocalFunctionProj --dotnet`

