using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IServices.Outside;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using WMSCore.Outside;
using YL.Core.Dto;
using YL.Core.Entity;
using YL.NetCore.NetCoreApp;
using YL.Utils.Table;

namespace WMSCore.Controllers
{
    public class BasicInfoController : BaseController
    {
        SqlSugarClient _client;
        public BasicInfoController( 
            SqlSugarClient client
            )
        { 
            _client = client;
        }

        [HttpGet] 
        public async Task<RouteData<Sys_dict[]>> GetMaterialTypes(string storeId="", string search = "")
        {
            IWMSManagementApiAccessor wmsAccessor = WMSApiManager.GetManagementApiAccessor(storeId, _client, this.UserDto);
            return await wmsAccessor.GetMaterialTypes(search); 
        }
        
        [HttpGet] 
        public async Task<Wms_MaterialDto[]> MaterialList(Bootstrap.BootstrapParams bootstrap)
        {
            IWMSBaseApiAccessor wmsAccessor = WMSApiManager.GetBaseApiAccessor(bootstrap.storeId.ToString(), _client);
            RouteData<Wms_MaterialDto[]> result = (await wmsAccessor.GetMateralList(
                bootstrap.pageIndex, bootstrap.limit, bootstrap.search,
                new string[] { bootstrap.sort + " " + bootstrap.order },
                bootstrap.datemin, bootstrap.datemax));
            if (!result.IsSccuess)
            {
                return new Wms_MaterialDto[0];
            }
            return result.Data;
        }
    }
}