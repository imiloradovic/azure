using AZManager;
using Azure.Core;
using Azure.ResourceManager.Resources;

namespace Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AzManager azManager = new AzManager();

            RGTest(azManager);            
        }

        static void RGTest(AzManager azManager)
        {
            
            RGManager rgManager = new RGManager(azManager);

            ResourceGroupResource rg;
            string rgName = "RG-NEW";
            if (!rgManager.Exists(rgName)) { rg = rgManager.Create(rgName, AzureLocation.NorthEurope); }
            else { rg = rgManager.Get(rgName); }

            rgManager.Delete(rgName);
        }
    }
}