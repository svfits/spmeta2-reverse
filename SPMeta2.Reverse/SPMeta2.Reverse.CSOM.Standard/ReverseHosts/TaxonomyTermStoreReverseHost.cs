﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.SharePoint.Client.Taxonomy;
using SPMeta2.Reverse.CSOM.ReverseHosts;

namespace SPMeta2.Reverse.CSOM.Standard.ReverseHosts
{
    public class TaxonomyTermStoreReverseHost : SiteReverseHost
    {
        public TermStore HostTermStore { get; set; }
    }
}
