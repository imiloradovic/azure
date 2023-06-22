using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;

namespace AZManager
{
    public class AzManager
    {
        ArmClient armClient;
        internal SubscriptionResource subscription;
        public AzManager()
        {
            armClient = new ArmClient(new DefaultAzureCredential());
            subscription = armClient.GetDefaultSubscription();
        }
    }
}