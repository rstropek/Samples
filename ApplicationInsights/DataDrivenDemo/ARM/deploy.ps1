# Check if user is already signed in
$context = Get-AzureRmContext
if (!$context.Subscription.Name) {
    Login-AzureRmAccount
}

# Select subscription where name contains `MSDN Subscription`
Get-AzureRmSubscription | where { $_.Name -like "*MSDN Subscription*" } | Select-AzureRmSubscription

# Set some string constants
$rg = "data-driven-app-insights"
$location = "westeurope"
$dep = "Deployment-" + [guid]::NewGuid()
$dbAdminUser = "demo"

# Check if resource group already exists
$group = Get-AzureRmResourceGroup -Name $rg -ErrorAction SilentlyContinue
if (!$group) {
    New-AzureRmResourceGroup -Name $rg -Location $location
}

if (!$cred) {
    $cred = Get-Credential
}

# Deploy ARM template
New-AzureRmResourceGroupDeployment -ResourceGroupName $rg -TemplateFile "$PSScriptRoot\main.json" `
    -Name $dep -dbAdminUser $dbAdminUser -dbAdminPassword $cred.Password
