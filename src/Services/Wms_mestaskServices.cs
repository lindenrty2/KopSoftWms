using IRepository;
using IServices;
using SqlSugar;
using System;
using YL.Core.Entity;
using YL.Utils.Excel;
using YL.Utils.Extensions;
using YL.Utils.Json;
using YL.Utils.Pub;
using YL.Utils.Table;

namespace Services
{
    public class Wms_mestaskServices : BaseServices<Wms_mestask>, IWms_mastaskServices
    {
        private readonly IWms_mestaskRepository _repository;
        private readonly SqlSugarClient _client;

        public Wms_mestaskServices(SqlSugarClient client, IWms_mestaskRepository repository) : base(repository)
        {
            _client = client;
            _repository = repository;
        }

        public string PageList(Bootstrap.BootstrapParams bootstrap)
        {
            return "";
        }
    }
}
