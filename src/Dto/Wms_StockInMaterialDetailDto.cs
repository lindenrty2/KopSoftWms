﻿using System;
using System.Collections.Generic;
using System.Text;

namespace YL.Core.Dto
{
    public class Wms_StockInMaterialDetailDto
    {

        public string StockInId { get; set; }

        public string StockInDetailId { get; set; }

        public string MaterialId { get; set; }

        public string MaterialNo { get; set; }

        public string MaterialName { get; set; }

        public int PlanInQty { get; set; }

        public int ActInQty { get; set; }

        public int Qty { get; set; }

    }
}
