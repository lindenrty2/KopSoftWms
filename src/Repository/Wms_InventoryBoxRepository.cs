using IRepository;
using SqlSugar;
using YL.Core.Entity;

namespace Repository
{
    public class Wms_inventoryBoxRepository : BaseRepository<wms_inventorybox>, IWms_inventoryBoxRepository
    {
        public Wms_inventoryBoxRepository(SqlSugarClient dbContext) : base(dbContext)
        {
        }
    }
}