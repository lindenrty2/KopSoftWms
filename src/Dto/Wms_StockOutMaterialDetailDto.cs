using System;
using System.Collections.Generic;
using System.Text;

namespace YL.Core.Dto
{
    public class Wms_StockOutMaterialDetailDto
    {
        public string StockOutId { get; set; }

        public string StockOutDetailId { get; set; }

        public string MaterialId { get; set; }

        public string MaterialNo { get; set; }

        public string MaterialName { get; set; }

        public string OrderNo { get; set; }

        public int PlanOutQty { get; set; }

        public int ActOutQty { get; set; }

        public int Qty { get; set; }

    }
}
