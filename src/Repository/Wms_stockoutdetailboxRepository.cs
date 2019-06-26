using IRepository;
using SqlSugar;
using YL.Core.Entity;

namespace Repository
{
    public class Wms_stockoutdetailboxRepository : BaseRepository<Wms_stockoutdetail_box>, IWms_stockoutdetailboxRepository
    {
        public Wms_stockoutdetailboxRepository(SqlSugarClient dbContext) : base(dbContext)
        {
        }
    }
}