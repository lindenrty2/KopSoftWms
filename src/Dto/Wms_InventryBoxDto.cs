using System;
using System.Collections.Generic;
using System.Text;
using YL.Core.Entity;

namespace YL.Core.Dto
{
    public class Wms_InventryBoxDto : Wms_inventorybox
    {



        public List<Wms_inventory> Detail { get; set; }

    }
}
