# Helper functions for generating unique strings
# Source: https://blogs.technet.microsoft.com/389thoughts/2017/12/23/get-uniquestring-generate-unique-id-for-azure-deployments/
function Get-UniqueString ([string]$id, $length=13)
{
    $hashArray = (new-object System.Security.Cryptography.SHA512Managed).ComputeHash($id.ToCharArray())
    -join ($hashArray[1..$length] | ForEach-Object { [char]($_ % 26 + [byte][char]'a') })
}

function New-KeyVault([string]$vaultName, [string]$rg, [string]$location, [string]$adminUserPrincipalName) 
{
    # Check if key vault already exists
    $vault = Get-AzureRmKeyVault -VaultName $vaultName -ResourceGroupName $rg -ErrorAction SilentlyContinue
    if (!$vault) {
        # Key vault does not yet exist -> create it and give current user admin permissions
        $vault = New-AzureRmKeyVault -VaultName $vaultName -ResourceGroupName $rg -Location $location -EnabledForTemplateDeployment
        $userId = $(Get-AzureRmADUser -UserPrincipalName $adminUserPrincipalName).Id
        Set-AzureRmKeyVaultAccessPolicy -VaultName $vaultName -ObjectId $userId -PermissionsToKeys All -PermissionsToSecrets All
    }

    $vault
}

function Set-DbPasswordInKeyVault([string]$vaultName, [securestring]$secureDbPassword)
{
    # Check if DB password already in key vault
    $dbPwdSecret = Get-AzureKeyVaultSecret -VaultName $vaultName -Name "DbPassword"
    if (!$dbPwdSecret) {
        # Key vault does not yet have DB password stored in it -> add it
        $dbPwdSecret = Set-AzureKeyVaultSecret -VaultName $vaultname -Name "DbPassword" -SecretValue $secureDbPassword
    }

    $dbPwdSecret
}

function Set-ServiceBusInKeyVault([string]$vaultName, [securestring]$sbConnectionString)
{
    # Check if DB password already in key vault
    $serviceBusSecret = Get-AzureKeyVaultSecret -VaultName $vaultName -Name "ServiceBus"
    if (!$serviceBusSecret) {
        # Key vault does not yet have DB password stored in it -> add it
        $serviceBusSecret = Set-AzureKeyVaultSecret -VaultName $vaultname -Name "ServiceBus" -SecretValue $sbConnectionString
    }

    $serviceBusSecret
}

function New-SecureRandomPassword()
{
    $dbPassword = -join (33..126 | ForEach-Object {[char]$_} | Get-Random -Count 20)
    ConvertTo-SecureString $dbPassword -AsPlainText -Force
}

function New-Application([string]$displayName, [string]$idUri)
{
    $app = Get-AzureRmADApplication -IdentifierUri $idUri
    if (!$app) {
        $app = New-AzureRmADApplication -DisplayName $displayName -IdentifierUris $idUri
    }

    $app
}

function New-ApplicationServicePrincipal([guid]$applicationId, [securestring]$securePassword, [string]$vaultName)
{
    $principal = Get-AzureRmADServicePrincipal -ServicePrincipalName $applicationId
    if (!$principal) {
        $principal = New-AzureRmADServicePrincipal -ApplicationId $applicationId -Password $securePassword
    }
    else {
        Remove-AzureRmADAppCredential -ApplicationId $applicationId -All -Force
        New-AzureRmADAppCredential -ApplicationId $applicationId -Password $securePassword
    }

    Set-AzureRmKeyVaultAccessPolicy -VaultName $vaultName -ServicePrincipalName $applicationId -PermissionsToSecrets get

    $principal
}


# Check if user is already signed in
$context = Get-AzureRmContext
if (!$context.Subscription.Name) {
    # User is not signed in yet -> trigger login
    Login-AzureRmAccount
    $context = Get-AzureRmContext
}

# Select subscription where name contains `MSDN Subscription`
# Note that you DO HAVE TO CHANGE THAT LINE if your subscription is named differently
Get-AzureRmSubscription | Where-Object { $_.Name -like "*MSDN Subscription*" } | Select-AzureRmSubscription

# Set some string constants
$rg = "microservices-workshop"
$location = "westeurope"
$dep = "Deployment-$((Get-Date -Format s).Replace(":", "-"))"
$dbAdminUser = "demo"
$vaultNameProd = "secretsprod"
$vaultNameProd += Get-UniqueString($vaultNameProd)
$vaultNameTest = "secretstest"
$vaultNameTest += Get-UniqueString($vaultNameTest)
$secureDbPassword = New-SecureRandomPassword
$secureVaultProdPassword = New-SecureRandomPassword
$secureVaultTestPassword = New-SecureRandomPassword

# Check if resource group already exists
$group = Get-AzureRmResourceGroup -Name $rg -ErrorAction SilentlyContinue
if (!$group) {
    # User group could not be found -> create it
    New-AzureRmResourceGroup -Name $rg -Location $location
}

# Create Azure Key Vaults for prod and test
New-KeyVault -vaultName $vaultNameProd -rg $rg -location $location -adminUserPrincipalName $context.Account
New-KeyVault -vaultName $vaultNameTest -rg $rg -location $location -adminUserPrincipalName $context.Account

# Store DB passwords in Azure Key Vault
# Note that using DB admin user is a SIMPLIFICATION and SHOULD NOT BE DONE in real-world applications.
# Create a user with less privileges instead!
Set-DbPasswordInKeyVault -vaultName $vaultNameProd -secureDbPassword $secureDbPassword
Set-DbPasswordInKeyVault -vaultName $vaultNameTest -secureDbPassword $secureDbPassword

$sbConnString = "Endpoint=sb://bike-rental.servicebus.windows.net/;SharedAccessKeyName=send-bike-rental-end;SharedAccessKey=AmdV8rrZUu5WTHxxNwgLcUcA6WuaJ8VEQoMjszBt5Lg=;";
$sbSecureConnString = ConvertTo-SecureString $sbConnString -AsPlainText -Force
Set-ServiceBusInKeyVault -vaultName $vaultNameProd -sbConnectionString $sbSecureConnString
Set-ServiceBusInKeyVault -vaultName $vaultNameTest -sbConnectionString $sbSecureConnString

# Register prod and test app in Azure AD
$appProd = New-Application -displayName "BikeRentals" -idUri "http://bike-rentals"
$appTest = New-Application -displayName "BikeRentalsTest" -idUri "http://test.bike-rentals"

# Create service principals for prod and test
New-ApplicationServicePrincipal -applicationId $appProd.ApplicationId -securePassword $secureVaultProdPassword -vaultName $vaultNameProd
New-ApplicationServicePrincipal -applicationId $appTest.ApplicationId -securePassword $secureVaultTestPassword -vaultName $vaultNameTest

# Deploy ARM template
New-AzureRmResourceGroupDeployment -ResourceGroupName $rg -TemplateFile "$PSScriptRoot\main.json" `
    -Name $dep -dbAdminUser $dbAdminUser -dbAdminPwd $secureDbPassword -clientIdProd $appProd.ApplicationId `
    -clientIdTest $appTest.ApplicationId -clientSecretProd $secureVaultProdPassword -clientSecretTest $secureVaultTestPassword `
    -vaultNameProd $vaultNameProd -vaultNameTest $vaultNameTest
