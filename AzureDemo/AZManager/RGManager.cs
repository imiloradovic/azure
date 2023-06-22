using Azure;
using Azure.Core;
using Azure.ResourceManager.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZManager
{
    public class RGManager 
    {
        // IMPLEMENT SINGLETON !!!
        AzManager azManager;
        ResourceGroupCollection rgCollection; // this collection is auto synchronized !? Don't know why!
        public RGManager(AzManager azManager) 
        { 
            this.azManager = azManager;
            this.rgCollection = this.azManager.subscription.GetResourceGroups();
        }
        
        public bool Exists(string rgName)
        {
            return rgCollection.Exists(rgName); 
        }
        public ResourceGroupResource Get(string rgName)
        {
            ResourceGroupResource resourceGroup = null;
            try 
            { 
                resourceGroup = rgCollection.FirstOrDefault(x => x.Data.Name == rgName);
            }
            catch(Azure.RequestFailedException ex) 
            {
                if(ex.ErrorCode.Equals("ResourceGroupNotFound", StringComparison.OrdinalIgnoreCase)) { throw new Exception($"There is no resource group {rgName}"); }  
            }
            catch { throw; }

            return resourceGroup;
        }

        public ResourceGroupResource Create(string rgName, AzureLocation azureLocation)
        {
            ResourceGroupResource resourceGroup = null;
            ResourceGroupData resourceGroupData = new ResourceGroupData(azureLocation);
            try
            {
                resourceGroup = rgCollection.CreateOrUpdate(WaitUntil.Completed, rgName, resourceGroupData).Value;
            }
            catch { throw; }

            return resourceGroup;
        }

        public void Delete(string rgName) 
        {
            ResourceGroupResource rg = Get(rgName);
            try
            {
                rg.Delete(WaitUntil.Completed);
            }
            catch { throw; }
        }
    }
}
