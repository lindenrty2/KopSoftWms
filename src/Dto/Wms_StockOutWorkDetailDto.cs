﻿using System;
using System.Collections.Generic;
using System.Text;
using YL.Utils.Pub;

namespace YL.Core.Dto
{
    public class Wms_StockOutWorkDetailDto 
    {
        public string DetailId { get; set; }
        public string InventoryBoxTaskId { get; set; }

        public string InventoryBoxId { get; set; }

        public string InventoryBoxNo { get; set; }

        public int? InventoryBoxStatus { get; set; }

        public string MaterialId { get; set; }

        public string MaterialNo { get; set; }

        public string MaterialOnlyId { get; set; }

        public string MaterialName { get; set; }

        public int PlanQty { get; set; }

        public int Qty { get; set; }

        public int StockOutStatus { get; set; }

        public string ModifiedUser { get; set; }

        public DateTime? ModifiedDate { get; set; }

    }
}
