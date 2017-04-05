# Login-AzureRmAccount
Get-AzureRmSubscription | where { $_.SubscriptionName -like "*MVP*" } | Select-AzureRmSubscription

$rg = "RG-Docker"
$dep = "Simple-Docker-Deployment"
$path = "C:\Code\Github\Samples\ArmWorkshop\02-Docker"
New-AzureRmResourceGroup -Name $rg -Location "northeurope" -Force
New-AzureRmResourceGroupDeployment -ResourceGroupName $rg -TemplateFile "$path\azuredeploy.json" `
    -TemplateParameterFile "$path\azuredeploy.parameters.json" -Name $dep

# Get-AzureRmResourceGroup -Name $rg | Remove-AzureRmResourceGroup -Force
