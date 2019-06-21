using IRepository;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using YL.Core.Entity;

namespace Repository
{
    public class Wms_inventoryboxtaskRepository : BaseRepository<Wms_inventoryboxTask>, IWms_inventoryboxtaskRepository
    {
        public Wms_inventoryboxtaskRepository(SqlSugarClient dbContext) : base(dbContext)
        {
        }
    }
}
