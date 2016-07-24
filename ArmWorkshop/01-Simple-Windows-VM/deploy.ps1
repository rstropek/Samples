# Login-AzureRmAccount
Get-AzureRmSubscription | where { $_.SubscriptionName -like "*Sponsorship*" } | Select-AzureRmSubscription

$rg = "RG-Simple-VM"
$dep = "Simple-Windows-VM-Deployment"
$path = "C:\Code\Github\Samples\ArmWorkshop\01-Simple-Windows-VM"

$group = Get-AzureRmResourceGroup -Name $rg -ErrorAction SilentlyContinue
if (!$group) {
    New-AzureRmResourceGroup -Name $rg -Location "northeurope"
}

New-AzureRmResourceGroupDeployment -ResourceGroupName $rg -TemplateFile "$path\azuredeploy.json" `
    -TemplateParameterFile "$path\azuredeploy.parameters.json" -Name $dep

# Get-AzureRmResourceGroup -Name $rg | Remove-AzureRmResourceGroup -Force