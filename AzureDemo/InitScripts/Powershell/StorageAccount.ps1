# https://learn.microsoft.com/en-us/powershell/module/az.storage/new-azstorageaccount?view=azps-10.0.0

$resourceGroupName = "RG-Demo"
$storageAccountLocation = ""
$storageAccountName = "sa01mi80"

# New-AzStorageAccount -ResourceGroupName $storageAccountName -Location "North Europe" -Name "sa01mi80" `
#     -Kind "StorageV2" <# StorageV2 supports Blobs, Tables, Queues and Files. This value is default #> `
#     -SkuName "Standard_LRS" <# Performabce=Standard and Redundancy=LRS (local) #>

# this line throws an exception so cannot get the storage key in order to create blob container 
# $primaryStorageAccountKey = Get-AzureStorageKey -StorageAccountName "sa01mi80" 
# so the key is retrieved using azure cli. The cli returns text, so the text must be converted to PS object
$storageAccountKey1 = az storage account keys list --account-name $storageAccountName --resource-group $resourceGroupName |`
            ConvertFrom-Json | Where-Object { $_.keyName -eq 'key1' } | Select-Object -ExpandProperty "Value"

$storageAccountContext = New-AzStorageContext -StorageAccountName $storageAccountName -StorageAccountKey $storageAccountKey1
New-AzStorageContainer -Name "container01" -Context $storageAccountContext 