$rg = "RG-Windows-DSC"
$dep = [guid]::NewGuid()
$path = "C:\Code\Github\Samples\ArmWorkshop\03-Windows-DSC"
New-AzureRmResourceGroup -Name $rg -Location "northeurope"
New-AzureRmResourceGroupDeployment -ResourceGroupName $rg -TemplateFile "$path\azuredeploy.json" -TemplateParameterFile "$path\azuredeploy.parameters.json" -Name $dep
