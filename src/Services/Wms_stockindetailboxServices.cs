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
    public class Wms_stockindetailboxServices : BaseServices<Wms_stockindetail_box>, IWms_stockindetailboxServices
    {
        private readonly IWms_stockindetailboxRepository _repository;
        private readonly SqlSugarClient _client;

        public Wms_stockindetailboxServices(SqlSugarClient client, IWms_stockindetailboxRepository repository) : base(repository)
        {
            _client = client;
            _repository = repository;
        }
         
    }
}