using System;
using System.Collections.Generic;
using System.Text;

namespace YL.Core.Dto
{
    public class Wms_StockMaterialDetailDto
    {

        public string StockId { get; set; }

        public string StockDetailId { get; set; }

        public int Position { get; set; }

        public string MaterialId { get; set; }

        public string MaterialNo { get; set; }

        public string MaterialName { get; set; }

        public int PlanQty { get; set; }

        public int ActQty { get; set; }

        public int Qty { get; set; }

    }
}
