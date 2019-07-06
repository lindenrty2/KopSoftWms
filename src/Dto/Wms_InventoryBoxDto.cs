using System;
using System.Collections.Generic;
using System.Text;
using YL.Core.Entity;

namespace YL.Core.Dto
{
    public class Wms_InventoryBoxDto : Wms_inventorybox
    {
        public string Id { get; set; }
        public InventoryDetailDto[] Details { get; set; }

    }
}
