$rg = "RG-PaaS-Web-DB"
$dep = [guid]::NewGuid()
$path = "C:\Code\Github\Samples\ArmWorkshop\04-PaaS-Web-DB"
New-AzureRmResourceGroup -Name $rg -Location "northeurope"
New-AzureRmResourceGroupDeployment -ResourceGroupName $rg -TemplateFile "$path\azuredeploy.json" -TemplateParameterFile "$path\azuredeploy-parameters.json" -Name $dep
