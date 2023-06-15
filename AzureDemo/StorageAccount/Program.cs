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
            // an exception is thrown when vs uses personal account to log in azure(igor.miloradovic@outlook.com)
            // a workaround is to create a separate user and use it to log vs in azure (vs2022@igormiloradovicoutlook.onmicrosoft.com in this case)
            // to the user, assign contributor role for the subscription
            // https://stackoverflow.com/questions/72698461/azure-armclient-not-finding-subscriptions-for-one-of-my-accounts
            ArmClient armClient = new ArmClient(new DefaultAzureCredential());
            SubscriptionResource subscription = await armClient.GetDefaultSubscriptionAsync();

            // create new resource group
            ResourceGroupCollection resourceGroupCollection = subscription.GetResourceGroups();
            ResourceGroupData resourceGroupData = new ResourceGroupData(AzureLocation.NorthEurope);
            ResourceGroupResource resourceGroup = resourceGroupCollection.CreateOrUpdate(WaitUntil.Completed, resourceGroupName, resourceGroupData).Value;

            // create storage account
            StorageAccountCollection storageAccountCollection = resourceGroup.GetStorageAccounts();
            StorageAccountCreateOrUpdateContent content = new StorageAccountCreateOrUpdateContent(new StorageSku("Standard_LRS"), StorageKind.StorageV2, AzureLocation.NorthEurope);           
            storageAccountCollection.CreateOrUpdate(WaitUntil.Completed, "sa01mi80", content);


        }

        private void CreateStorageAccount()
        {

        }
    }
}