$rg = "RG-Simple-VM-Imp"
$location = "northeurope"
$newStorageAccountName = "devopscon15imp"
$publicIPAddressName = "SimpleVMPublicIP"
$publicIPAddressType = "Dynamic"
$dnsNameForPublicIP = "devopscon-simple-vm-imp"
$virtualNetworkName = "SimpleVMVNet"
$addressPrefix = "10.0.0.0/16"
$subnetName = "Subnet"
$subnetPrefix = "10.0.0.0/24"
$nicName = "myVMNic"
$vmName = "MyWindowsVM"
$vmSize = "Standard_A1"
$imagePublisher = "MicrosoftWindowsServer"
$imageOffer = "WindowsServer"
$windowsOSVersion = "2012-R2-Datacenter"

# Create necessary resources for VMs
New-AzureRmResourceGroup -Name $rg -Location $location

$storageAccount = New-AzureRmStorageAccount -Name $newStorageAccountName -ResourceGroupName $rg -Type Standard_LRS -Location $location

$ipAddress = New-AzureRmPublicIpAddress -Name $publicIPAddressName -ResourceGroupName $rg -Location $location -AllocationMethod $publicIPAddressType -DomainNameLabel $dnsNameForPublicIP

$subnet = New-AzureRmVirtualNetworkSubnetConfig -Name $subnetName -AddressPrefix $subnetPrefix
$vnet = New-AzureRmVirtualNetwork -Name $virtualNetworkName -ResourceGroupName $rg -Location $location -AddressPrefix $addressPrefix -Subnet $subnet

$nic = New-AzureRmNetworkInterface -ResourceGroupName $rg -Name $nicName -Location $location -PublicIpAddressId $ipAddress.Id -SubnetId $vnet.Subnets[0].Id

$cred = Get-Credential -UserName "rainer" -Message "Password for new VM"
$vmConfig = New-AzureRmVMConfig -VMName $vmName -VMSize $vmSize |

    Set-AzureRmVMOperatingSystem -Windows -ComputerName $vmName -Credential $cred |
        
    Set-AzureRmVMSourceImage -PublisherName $imagePublisher -Offer $imageOffer -Skus $windowsOSVersion -Version "latest" | 

    Set-AzureRmVMOSDisk -Name "osDisk" -VhdUri "http://$newStorageAccountName.blob.core.windows.net/vhds/osDisk.vhd" -Caching ReadWrite -CreateOption fromImage  | 

    Add-AzureRmVMNetworkInterface -Id $nic.Id

New-AzureRmVM -ResourceGroupName $rg -Location $location -VM $vmConfig
