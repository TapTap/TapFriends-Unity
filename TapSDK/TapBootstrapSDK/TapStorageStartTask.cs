using LeanCloud;
using LeanCloud.Storage;

namespace TapTap.Bootstrap
{
    public class TapStorageStartTask : IStartTask
    {
        public void Invoke(TapConfig config)
        {
            LCApplication.Initialize(config.ClientID, config.ClientToken, config.ServerURL);
            LCObject.RegisterSubclass("_User", () => new TDSUser());
        }
    }
}