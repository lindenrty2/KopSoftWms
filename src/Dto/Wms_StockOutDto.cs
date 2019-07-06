using System;
using System.Collections.Generic;
using System.Text;
using YL.Core.Entity;

namespace YL.Core.Dto
{
    public class Wms_StockOutDto : Wms_stockout
    {

        public Wms_StockMaterialDetailDto[] Details { get; set; }
    }
}
