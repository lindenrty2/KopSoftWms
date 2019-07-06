using System;
using System.Collections.Generic;
using System.Text;
using YL.Core.Entity;

namespace YL.Core.Dto
{
    public class Wms_InventoryBoxDto : Wms_inventorybox
    {
        public InventoryDetailDto[] Details { get; set; }

    }
}
