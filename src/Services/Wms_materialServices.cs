using IRepository;
using IServices;
using Microsoft.AspNetCore.Http;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YL.Core.Dto;
using YL.Core.Entity;
using YL.Utils.Excel;
using YL.Utils.Extensions;
using YL.Utils.Json;
using YL.Utils.Pub;
using YL.Utils.Table;

namespace Services
{
    public class Wms_materialServices : BaseServices<Wms_material>, IWms_materialServices
    {
        private readonly IWms_materialRepository _repository;
        private readonly SqlSugarClient _client;

        public Wms_materialServices(SqlSugarClient client, IWms_materialRepository repository) : base(repository)
        {
            _client = client;
            _repository = repository;
        }

        public string PageList(Bootstrap.BootstrapParams bootstrap)
        {
            int totalNumber = 0;
            if (bootstrap.offset != 0)
            {
                bootstrap.offset = bootstrap.offset / bootstrap.limit + 1;
            }
            var query = _client.Queryable<Wms_material, Sys_dict, Sys_dict, Wms_warehouse, Sys_user, Sys_user>
                ((s, t, ut, w, c, u) => new object[] {
                   JoinType.Left,s.MaterialType==t.DictId,
                   JoinType.Left,s.Unit==ut.DictId,
                   JoinType.Left,s.WarehouseId==w.WarehouseId,
                   JoinType.Left,s.CreateBy==c.UserId,
                   JoinType.Left,s.ModifiedBy==u.UserId,
                 })
                 .Where((s, t, ut, w, c, u) => s.WarehouseId == bootstrap.storeId && s.IsDel == 1 && t.IsDel == 1 && ut.IsDel == 1 && w.IsDel == 1)
                 .Select((s, t, ut, w, c, u) => new
                 {
                     MaterialId = s.MaterialId.ToString(),
                     s.MaterialOnlyId,
                     s.MaterialNo,
                     s.MaterialName,
                     w.WarehouseNo,
                     w.WarehouseName,
                     MaterialType = t.DictName,
                     Unit = ut.DictName, 
                     s.IsDel,
                     s.Remark,
                     CName = c.UserNickname,
                     s.CreateDate,
                     UName = u.UserNickname,
                     s.ModifiedDate
                 }).MergeTable();
            if (!bootstrap.search.IsEmpty())
            {
                query.Where((s) => s.MaterialNo.Contains(bootstrap.search) || s.MaterialName.Contains(bootstrap.search) || s.MaterialOnlyId.Contains(bootstrap.search) );
            }
            if (!bootstrap.datemin.IsEmpty() && !bootstrap.datemax.IsEmpty())
            {
                query.Where(s => s.CreateDate > bootstrap.datemin.ToDateTimeB() && s.CreateDate <= bootstrap.datemax.ToDateTimeE());
            }
            if (bootstrap.order.Equals("desc", StringComparison.OrdinalIgnoreCase))
            {
                query.OrderBy($"MergeTable.{bootstrap.sort} desc");
            }
            else
            {
                query.OrderBy($"MergeTable.{bootstrap.sort} asc");
            }
            var list = query.ToPageList(bootstrap.offset, bootstrap.limit, ref totalNumber);
            return Bootstrap.GridData(list, totalNumber).JilToJson();
        }

        public byte[] ExportList(Bootstrap.BootstrapParams bootstrap)
        {
            bootstrap.sort = "创建时间";
            bootstrap.order = "desc";
            var query = _client.Queryable<Wms_material, Sys_dict, Sys_dict, Wms_warehouse, Sys_user, Sys_user>
                ((s, t, ut, w, c, u) => new object[] {
                   JoinType.Left,s.MaterialType==t.DictId,
                   JoinType.Left,s.Unit==ut.DictId, 
                   JoinType.Left,s.WarehouseId==w.WarehouseId,
                   JoinType.Left,s.CreateBy==c.UserId,
                   JoinType.Left,s.ModifiedBy==u.UserId,
                 })
                 .Where((s, t, ut, w, c, u) => s.IsDel == 1 && t.IsDel == 1 && ut.IsDel == 1 && w.IsDel == 1)
                 .Select((s, t, ut, w, c, u) => new
                 {
                     物料编号 = s.MaterialNo,
                     物料名称 = s.MaterialName,
                     单位类别 = ut.DictName,
                     物料分类 = t.DictName, 
                     仓库编号 = w.WarehouseNo,
                     仓库名称 = w.WarehouseName,
                     备注 = s.Remark,
                     创建人 = c.UserNickname,
                     创建时间 = s.CreateDate,
                     修改人 = u.UserNickname,
                     修改时间 = s.ModifiedDate
                 }).MergeTable();
            if (!bootstrap.datemin.IsEmpty() && !bootstrap.datemax.IsEmpty())
            {
                query.Where(s => s.创建时间 > bootstrap.datemin.ToDateTimeB() && s.创建时间 <= bootstrap.datemax.ToDateTimeE());
            }
            if (bootstrap.order.Equals("desc", StringComparison.OrdinalIgnoreCase))
            {
                query.OrderBy($"MergeTable.{bootstrap.sort} desc");
            }
            else
            {
                query.OrderBy($"MergeTable.{bootstrap.sort} asc");
            }
            var list = query.ToList();
            var buffer = NpoiUtil.Export(list, ExcelVersion.V2007);
            return buffer;
        }

        public async Task<RouteData> ImportList(IFormFile file)
        {
            try
            {
                MaterialImportData[] importList = null;
                using (Stream stream = file.OpenReadStream())
                {
                    importList = NpoiUtil.Import<MaterialImportData>(
                        stream, file.FileName.EndsWith("xlsx") ? ExcelVersion.V2007 : ExcelVersion.V2003);
                    stream.Close();
                    stream.Dispose(); 
                }
                List<Sys_dict> typeDicts = await _client.Queryable<Sys_dict>().Where(x => x.DictType == PubDictType.material.ToByte().ToString()).ToListAsync();
                List<Sys_dict> unitDicts = await _client.Queryable<Sys_dict>().Where(x => x.DictType == PubDictType.unit.ToByte().ToString()).ToListAsync();
                foreach (MaterialImportData importData in importList)
                {

                    Wms_material material = null;
                    if (string.IsNullOrWhiteSpace(importData.MaterialOnlyId))
                    {
                        material = await _client.Queryable<Wms_material>().FirstAsync(x => x.MaterialNo == importData.MaterialNo);
                    }
                    else
                    {
                        material = await _client.Queryable<Wms_material>().FirstAsync(x => x.MaterialOnlyId == importData.MaterialOnlyId);
                    }

                    if (material == null)
                    {
                        Sys_dict typeDict = typeDicts.FirstOrDefault(x => x.DictName == importData.MaterialType);
                        if (typeDict == null)
                        {
                            return RouteData<Wms_material>.From(PubMessages.E1001_SUPPLIESTYPE_NOTFOUND);
                        }
                        else if (typeDict.WarehouseId == null)
                        {
                            return RouteData<Wms_material>.From(PubMessages.E1002_SUPPLIESTYPE_WAREHOUSEID_NOTSET);
                        }

                        Sys_dict unitDict = unitDicts.FirstOrDefault(x => x.DictName == importData.Unit);
                        if (unitDict == null)
                        {
                            return RouteData<Wms_material>.From(PubMessages.E1003_UNIT_NOTFOUND);
                        }
                        long warehouseId = typeDict.WarehouseId.Value;

                        material = new Wms_material()
                        {
                            MaterialId = PubId.SnowflakeId,
                            MaterialOnlyId = importData.MaterialOnlyId ?? "",
                            MaterialNo = importData.MaterialNo ?? "",
                            MaterialName = importData.MaterialName,
                            MaterialType = typeDict.DictId,
                            WarehouseId = warehouseId,
                            Unit = unitDict.DictId,
                        };
                        this.Insert(material);
                    }
                    else
                    {
                        string unit = unitDicts.FirstOrDefault(x => x.DictId == material.Unit).DictName;
                        string type = typeDicts.FirstOrDefault(x => x.DictId == material.MaterialType).DictName;
                        if (material.MaterialNo != importData.MaterialNo)
                        {
                            return RouteData.From(PubMessages.E4102_MATERIAL_IMPORT_EXIST_NOTMATCH,
                                $",物料编号不匹配,已保存的值为[{material.MaterialNo}],导入值为[{importData.MaterialNo}]");
                        }
                        else if (material.MaterialOnlyId != importData.MaterialOnlyId)
                        {
                            return RouteData.From(PubMessages.E4102_MATERIAL_IMPORT_EXIST_NOTMATCH,
                                $",物料唯一Id不匹配,已保存的值为[{material.MaterialOnlyId}],导入值为[{importData.MaterialOnlyId}]");
                        }
                        else if (material.MaterialName != importData.MaterialName)
                        {
                            return RouteData.From(PubMessages.E4102_MATERIAL_IMPORT_EXIST_NOTMATCH,
                                $",物料({importData.MaterialNo},{importData.MaterialOnlyId})名称不匹配,已保存的值为[{material.MaterialName}],导入值为[{importData.MaterialName}]");
                        }
                        else if (type != importData.MaterialType)
                        {
                            return RouteData.From(PubMessages.E4102_MATERIAL_IMPORT_EXIST_NOTMATCH,
                                $",物料({importData.MaterialNo},{importData.MaterialOnlyId})单位不匹配,已保存的值为[{type}],导入值为[{importData.MaterialType}]");
                        }

                    } 
                }
                return RouteData.From(PubMessages.I4100_MATERIAL_IMPORT_SCCUESS);
            }
            catch
            {
                return RouteData.From(PubMessages.E4100_MATERIAL_IMPORT_FAIL);
            } 
        }
    }

    public class MaterialImportData
    {
        [NpoiColumn("B","物料编号")]
        public string MaterialNo { get; set; }
         
        [NpoiColumn("C", "物料唯一Id")]
        public string MaterialOnlyId { get; set; } 

        [NpoiColumn("D", "物料名称")]
        public string MaterialName { get; set; }

        [NpoiColumn("E", "物料类型")]
        public string MaterialType { get; set; }

        [NpoiColumn("F", "物料单位")]
        public string Unit { get; set; }

        public string Result { get; set; }
    }
}