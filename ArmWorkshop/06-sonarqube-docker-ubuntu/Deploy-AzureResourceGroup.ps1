$ResourceGroupName = 'MobileDevOps'
$ResourceGroupLocation = 'westeurope'
$TemplateFile = [System.IO.Path]::GetFullPath([System.IO.Path]::Combine($PSScriptRoot, "azuredeploy.json"))
$TemplateParametersFile = [System.IO.Path]::GetFullPath([System.IO.Path]::Combine($PSScriptRoot, "azuredeploy.parameters.1.json"))


# Check if resource group already exists
$ResourceGroupCount = Get-AzureRmResourceGroup | Where { $_.ResourceGroupName -match $ResourceGroupName } | Measure | Select Count
if ($ResourceGroupCount.Count -eq 0) {
    # Resource group does not yet exist, create it
    New-AzureRmResourceGroup -Name $ResourceGroupName -Location $ResourceGroupLocation -Verbose -Force -ErrorAction Stop 
}


# Trigger deployment
New-AzureRmResourceGroupDeployment -Name ((Get-ChildItem $TemplateFile).BaseName + '-' + ((Get-Date).ToUniversalTime()).ToString('MMdd-HHmm')) `
                                   -ResourceGroupName $ResourceGroupName `
                                   -TemplateFile $TemplateFile `
								   -TemplateParameterFile $TemplateParametersFile `
                                   -Force

# Just some useful other commands:
# Login-AzureRMAccount
# Get-AzureRmVMImageSku -Location 'westeurope' -Offer 'UbuntuServer' -PublisherName 'Canonical'
# Remove-AzureRmResourceGroup -Name $ResourceGroupName -Force -Verbose
