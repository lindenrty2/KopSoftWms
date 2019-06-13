using YL.Core.Entity;
using IRepository;
using SqlSugar;

namespace Repository
{
    public class Wms_mastaskRepository : BaseRepository<Wms_mestask>, IWms_mestaskRepository
    {
        public Wms_mastaskRepository(SqlSugarClient dbContext) : base(dbContext)
        {
        }
    }
}
