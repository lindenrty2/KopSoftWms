using IRepository;
using IServices;
using Repository;
using YL.Core.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using YL.Utils.Table;
using SqlSugar;
using YL.Utils.Json;
using System.Linq;
using System.Threading.Tasks;
using YL.Core.Dto;

namespace Services
{
    public class Wms_stockoutdetailServices : BaseServices<Wms_stockoutdetail>, IWms_stockoutdetailServices
    {
        private readonly IWms_stockoutdetailRepository _repository;
        private readonly SqlSugarClient _client;

        public Wms_stockoutdetailServices(IWms_stockoutdetailRepository repository,
            SqlSugarClient client
            ) : base(repository)
        {
            _repository = repository;
            _client = client;
        }

        public string PageList(string pid)
        {
            var query = _client.Queryable<Wms_stockoutdetail, Wms_material, Wms_stockout, Sys_user, Sys_user>
               ((s, m, p, c, u) => new object[] {
                   JoinType.Left,s.MaterialId==m.MaterialId,
                   JoinType.Left,s.StockOutId==p.StockOutId, 
                   JoinType.Left,s.CreateBy==c.UserId,
                   JoinType.Left,s.ModifiedBy==u.UserId,
                })
                .Where((s, m, p, c, u) => s.IsDel == 1 && m.IsDel == 1 && p.IsDel == 1 )
                .Select((s, m, p, c, u) => new
                {
                    StockOutId = s.StockOutId.ToString(),
                    StockOutDetailId = s.StockOutDetailId.ToString(),
                    UniqueIndex = s.UniqueIndex,
                    m.MaterialNo,
                    m.MaterialName, 
                    s.Status,
                    s.PlanOutQty,
                    s.ActOutQty,
                    s.IsDel,
                    s.Remark,
                    CName = c.UserNickname,
                    s.CreateDate,
                    UName = u.UserNickname,
                    s.ModifiedDate
                }).MergeTable();
            query.Where(c => c.StockOutId == pid).OrderBy(c => c.CreateDate, OrderByType.Desc);
            //var list = query.ToList();
             
            var list = query.ToList().Select(item => new
            {
                Detail = item,
                Tasks = _client.Queryable<Wms_stockoutdetail_box, Wms_inventoryboxTask, Wms_inventorybox, Sys_user>(
                    (stb, st, ib, ou) => new object[] {
                             JoinType.Left,stb.InventoryBoxTaskId == st.InventoryBoxTaskId,
                             JoinType.Left,st.InventoryBoxId == ib.InventoryBoxId,
                             JoinType.Left,st.OperaterId == ou.UserId
                    })
                    .Where((stb, st, ib, ou) => stb.StockOutDetailId.ToString() == item.StockOutDetailId)
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

            return Bootstrap.GridData(list, list.Count()).JilToJson();
        }
    }
}