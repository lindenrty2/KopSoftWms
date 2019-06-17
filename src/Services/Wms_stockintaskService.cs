using IRepository;
using IServices;
using Repository;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using YL.Core.Entity;

namespace Services
{
    public class Wms_stockintaskService : BaseServices<Wms_StockinTask>, IWms_stockintaskServices
    {
        private readonly SqlSugarClient _client;
        private readonly IWms_stockintaskRepository _repository;

        public Wms_stockintaskService(
              SqlSugarClient client, 
              IWms_stockintaskRepository repository) : base(repository)
        {
            _client = client;
            _repository = repository; 
        }

    }
}
