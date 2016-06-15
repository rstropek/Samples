# Login-AzureRmAccount
Get-AzureRmSubscription | where { $_.SubscriptionName -like "*Sponsorship*" } | Select-AzureRmSubscription

$rg = "RG-PaaS-Web-DB"
$dep = "PaaS-Deployment-" + [guid]::NewGuid()
$path = "C:\Code\Github\Samples\ArmWorkshop\04-PaaS-Web-DB"

$group = Get-AzureRmResourceGroup -Name $rg -ErrorAction SilentlyContinue
if (!$group) {
    New-AzureRmResourceGroup -Name $rg -Location "northeurope"
}

New-AzureRmResourceGroupDeployment -ResourceGroupName $rg -TemplateFile "$path\azuredeploy.json" `    -TemplateParameterFile "$path\azuredeploy-parameters.json" -Name $dep

# Get-AzureRmResourceGroup -Name $rg | Remove-AzureRmResourceGroup -Force
