using IRepository;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using YL.Core.Entity;

namespace Repository
{
    public class Wms_stockintaskRepository : BaseRepository<Wms_StockinTask>, IWms_stockintaskRepository
    {
        public Wms_stockintaskRepository(SqlSugarClient dbContext) : base(dbContext)
        {
        }
    }
}
