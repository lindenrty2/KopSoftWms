using IRepository;
using SqlSugar;
using YL.Core.Entity;

namespace Repository
{
    public class Wms_stockindetailboxRepository : BaseRepository<Wms_stockindetail_box>, IWms_stockindetailboxRepository
    {
        public Wms_stockindetailboxRepository(SqlSugarClient dbContext) : base(dbContext)
        {
        }
    }
}