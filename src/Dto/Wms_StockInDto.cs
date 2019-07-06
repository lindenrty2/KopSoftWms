using System;
using System.Collections.Generic;
using System.Text;
using YL.Core.Entity;

namespace YL.Core.Dto
{
    public class Wms_StockInDto : Wms_stockin
    {

        public Wms_StockMaterialDetailDto[] Details { get; set; }
    }
}
