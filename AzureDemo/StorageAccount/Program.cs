using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Storage;
using Azure.ResourceManager.Storage.Models;
using Microsoft.Extensions.Configuration;

namespace StorageAccount
{
    internal class Program
    {

        static async Task Main(string[] args)
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            IConfiguration config = configurationBuilder.Build();

            string resourceGroupName = config["ResourceGroupName"];
            string storageAccountName = config["StorageAccountName"];

            // get azure subscription.
            // When VS uses a personal account to log in Azure (igor.miloradovic@outlook.com), an exception is thrown 
            // https://stackoverflow.com/questions/72698461/azure-armclient-not-finding-subscriptions-for-one-of-my-accounts
            // A workaround is to create a separate user in Azure AD, set that user to log in Azure (vs2022@igormiloradovicoutlook.onmicrosoft.com in this case)
            // Contributor role for the subscription must be set for the user mentioned above.
            ArmClient armClient = new ArmClient(new DefaultAzureCredential());
            SubscriptionResource subscription = await armClient.GetDefaultSubscriptionAsync();

            // create new resource group
            ResourceGroupCollection resourceGroupCollection = subscription.GetResourceGroups();
            ResourceGroupData resourceGroupData = new ResourceGroupData(AzureLocation.NorthEurope);
            ResourceGroupResource resourceGroup = resourceGroupCollection.CreateOrUpdate(WaitUntil.Completed, resourceGroupName, resourceGroupData).Value;

            // create storage account
            StorageAccountCollection storageAccountCollection = resourceGroup.GetStorageAccounts();
            StorageAccountCreateOrUpdateContent content = new StorageAccountCreateOrUpdateContent(new StorageSku("Standard_LRS"), StorageKind.StorageV2, AzureLocation.NorthEurope);           
            StorageAccountResource storageAccount = storageAccountCollection.CreateOrUpdate(WaitUntil.Completed, "sa01mi80", content).Value;

            // create blob container
            BlobContainerCollection blobContainerCollection = storageAccount.GetBlobService().GetBlobContainers();
            BlobContainerData blobContainerData = new BlobContainerData();
            blobContainerCollection.CreateOrUpdate(WaitUntil.Completed, "container01", blobContainerData);

            // create fileshare
            FileShareCollection fileShareCollection = storageAccount.GetFileService().GetFileShares();
            FileShareData fileShareData = new FileShareData();
            fileShareCollection.CreateOrUpdate(WaitUntil.Completed, "fileshare01", fileShareData);

            // create table 
            TableCollection tableCollection = storageAccount.GetTableService().GetTables();
            TableData tableData = new TableData();
            tableCollection.CreateOrUpdate(WaitUntil.Completed, "table01", tableData);

            // create queue
            StorageQueueCollection storageQueueCollection = storageAccount.GetQueueService().GetStorageQueues();
            StorageQueueData storageQueueData = new StorageQueueData();
            storageQueueCollection.CreateOrUpdate(WaitUntil.Completed, "queue01", storageQueueData);
        }
                
    }
}