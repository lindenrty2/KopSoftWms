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
    public class Wms_inventoryBoxTaskServices : BaseServices<Wms_inventoryboxTask>, IWms_inventoryBoxTaskServices
    {
        private readonly SqlSugarClient _client;
        private readonly IWms_inventoryboxtaskRepository _repository;

        public Wms_inventoryBoxTaskServices(
              SqlSugarClient client,
              IWms_inventoryboxtaskRepository repository) : base(repository)
        {
            _client = client;
            _repository = repository; 
        }

    }
}
