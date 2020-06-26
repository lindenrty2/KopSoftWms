using System;
using System.Collections.Generic;
using System.Text;
using YL.Core.Entity;

namespace YL.Core.Dto
{
    public class OutsideStockCountDto : Wms_stockcount
    {
        public OutsideStockCountMaterial[] MaterialList { get; set; }

        public OutsideStockCountStep[] StepList { get; set; }
    }

    public class OutsideStockCountStep : Wms_stockcount_step
    {

    }


}
