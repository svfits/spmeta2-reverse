﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;

namespace SPMeta2.Reverse.CSOM.ReverseHosts
{
    public class FolderReverseHost : ListReverseHost
    {
        #region properties

        public Folder HostFolder { get; set; }

        #endregion
    }
}