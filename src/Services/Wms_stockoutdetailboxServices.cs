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
    public class Wms_stockoutdetailboxServices : BaseServices<Wms_stockoutdetail_box>, IWms_stockoutdetailboxServices
    {
        private readonly IWms_stockoutdetailboxRepository _repository;
        private readonly SqlSugarClient _client;

        public Wms_stockoutdetailboxServices(SqlSugarClient client, IWms_stockoutdetailboxRepository repository) : base(repository)
        {
            _client = client;
            _repository = repository;
        }
         
    }
}