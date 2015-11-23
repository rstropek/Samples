$rg = "RG-Simple-VM"
$dep = [guid]::NewGuid()
$path = "C:\Code\Github\Samples\ArmWorkshop\01-Simple-Windows-VM"
New-AzureRmResourceGroup -Name $rg -Location "northeurope"
New-AzureRmResourceGroupDeployment -ResourceGroupName $rg -TemplateFile "$path\azuredeploy.json" -TemplateParameterFile "$path\azuredeploy.parameters.json" -Name $dep
