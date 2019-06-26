using IRepository;
using IServices;
using SqlSugar;
using System.Collections.Generic;
using System.Linq;
using YL.Core.Entity;
using YL.Utils.Json;
using YL.Utils.Table;

namespace Services
{
    public class Wms_stockindetailServices : BaseServices<Wms_stockindetail>, IWms_stockindetailServices
    {
        private readonly IWms_stockindetailRepository _repository;
        private readonly SqlSugarClient _client;

        public Wms_stockindetailServices(SqlSugarClient client, IWms_stockindetailRepository repository) : base(repository)
        {
            _client = client;
            _repository = repository;
        }

        public string PageList(string pid)
        {
            var query = _client.Queryable<Wms_stockindetail, Wms_material, Wms_stockin, Sys_user, Sys_user>
                ((s, m, p, c, u) => new object[] {
                   JoinType.Left,s.MaterialId==m.MaterialId && m.IsDel == 1,
                   JoinType.Left,s.StockInId==p.StockInId && p.IsDel == 1,
                   JoinType.Left,s.CreateBy==c.UserId && c.IsDel == 1,
                   JoinType.Left,s.ModifiedBy==u.UserId && u.IsDel == 1
                 })
                 //.Where((s, m, p, c, u) => )
                 .Select((s, m, p, c, u) => new
                 {
                     StockInId = s.StockInId.ToString(),
                     StockInDetailId = s.StockInDetailId.ToString(),
                     m.MaterialNo,
                     m.MaterialName,
                     s.Status,
                     s.PlanInQty,
                     s.ActInQty,
                     s.IsDel,
                     s.Remark,
                     CName = c.UserNickname,
                     s.CreateDate,
                     UName = u.UserNickname,
                     s.ModifiedDate
                 })
                 .MergeTable();
            query.Where(c => c.StockInId == pid).OrderBy(c => c.CreateDate, OrderByType.Desc) 
                ;

            var list = query.ToList().Select(item => new
            {
                Detail = item,
                Tasks = _client.Queryable<Wms_stockindetail_box, Wms_inventoryboxTask, Wms_inventorybox, Sys_user>(
                    (stb, st, ib, ou) => new object[] {
                             JoinType.Left,stb.InventoryBoxTaskId == st.InventoryBoxTaskId,
                             JoinType.Left,st.InventoryBoxId == ib.InventoryBoxId,
                             JoinType.Left,st.OperaterId == ou.UserId

                    })
                    .Where((stb, st, ib, ou) => stb.StockinDetailId.ToString() == item.StockInDetailId)
                    .Select((stb, st, ib, ou) => new 
                    {
                        InventoryBoxTaskId = st.InventoryBoxTaskId.ToString(), 
                        InventoryBoxId = st.InventoryBoxId.ToString(), 
                        TaskStatus = (int)st.Status,
                        ib.InventoryBoxNo,
                        ib.InventoryBoxName,
                        Status = (int)ib.Status,
                        ou.UserNickname
                    })
                    .MergeTable().ToList()
            });
            //var list = query.ToList();
            return Bootstrap.GridData(list, list.Count()).JilToJson();
        }
    }
}