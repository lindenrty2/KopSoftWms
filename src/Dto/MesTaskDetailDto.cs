using System;
using System.Collections.Generic;
using System.Text;

namespace YL.Core.Dto
{
    public class MesTaskDetailDto
    {

        public long StockInId { get; set; }

        public string StockInNo { get; set; }

        public Wms_MaterialInventoryDto[] Materials { get; set; }

    }
}
