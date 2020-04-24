using System;
using System.Collections.Generic;
using System.Text;

namespace YL.Core.Dto
{
    public class Wms_StockCountInventoryBoxDto
    {
        public string StorageRackNo { get; set; }
        public string InventoryBoxId { get; set; }
        public string InventoryBoxNo { get; set; }

        public int Row { get; set; }
        public int Column { get; set; }
        public int Floor { get; set; }
        public int Position { get; set; }
        public int Qty { get; set; }

        public string MaterialNo { get; set; }
        public string MaterialName { get; set; }
        public string MaterialType { get; set; }
         
        public long? ModifiedBy { get; set; } 
        public string ModifiedUser { get; set; } 
        public DateTime? ModifiedDate { get; set; }

    }
}
