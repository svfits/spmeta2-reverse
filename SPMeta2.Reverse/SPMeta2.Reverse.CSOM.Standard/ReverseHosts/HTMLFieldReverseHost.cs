using Microsoft.SharePoint.Client;
using SPMeta2.Reverse.CSOM.ReverseHosts;

namespace SPMeta2.Reverse.CSOM.Standard.ReverseHosts
{
    public class HTMLFieldReverseHost : WebReverseHost
    {
        #region properties

        public Field HostField { get; set; }

        #endregion
    }
}
