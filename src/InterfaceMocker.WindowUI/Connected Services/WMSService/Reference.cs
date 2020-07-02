//------------------------------------------------------------------------------
// <自动生成>
//     此代码由工具生成。
//     //
//     对此文件的更改可能导致不正确的行为，并在以下条件下丢失:
//     代码重新生成。
// </自动生成>
//------------------------------------------------------------------------------

namespace WMSService
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="WMSService.IMESHookController")]
    public interface IMESHookController
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMESHookController/Ping", ReplyAction="http://tempuri.org/IMESHookController/PingResponse")]
        System.Threading.Tasks.Task<string> PingAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMESHookController/Warehousing", ReplyAction="http://tempuri.org/IMESHookController/WarehousingResponse")]
        System.Threading.Tasks.Task<string> WarehousingAsync(string WarehousingId, string WarehousingType, string WarehousingTime, string ProductionPlanId, string BatchPlanId, string WorkAreaName, string SuppliesKinds, string SuppliesInfoList);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMESHookController/WarehouseEntry", ReplyAction="http://tempuri.org/IMESHookController/WarehouseEntryResponse")]
        System.Threading.Tasks.Task<string> WarehouseEntryAsync(string WarehouseEntryId, string WarehouseEntryType, string WarehouseEntryTime, string ProductionPlanId, string TotalWorkOrder, string BatchNumber, string BatchPlanId, string WorkAreaName, string WorkStationId, string SuppliesKinds, string SuppliesInfoList);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMESHookController/MaterialStockEnquiry", ReplyAction="http://tempuri.org/IMESHookController/MaterialStockEnquiryResponse")]
        System.Threading.Tasks.Task<string> MaterialStockEnquiryAsync(string SuppliesId, string SuppliesName, string SuppliesType, string SuppliesUnit);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMESHookController/LogisticsControl", ReplyAction="http://tempuri.org/IMESHookController/LogisticsControlResponse")]
        System.Threading.Tasks.Task<string> LogisticsControlAsync(string LogisticsId, string StartPoint, string Destination);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMESHookController/LogisticsEnquiry", ReplyAction="http://tempuri.org/IMESHookController/LogisticsEnquiryResponse")]
        System.Threading.Tasks.Task<string> LogisticsEnquiryAsync(string LogisticsId, string EquipmentId, string EquipmentName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMESHookController/WarehousingStatusEnquiry", ReplyAction="http://tempuri.org/IMESHookController/WarehousingStatusEnquiryResponse")]
        System.Threading.Tasks.Task<string> WarehousingStatusEnquiryAsync(string WarehousingId, string WarehousingType);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMESHookController/WarehouseEntryStatusEnquiry", ReplyAction="http://tempuri.org/IMESHookController/WarehouseEntryStatusEnquiryResponse")]
        System.Threading.Tasks.Task<string> WarehouseEntryStatusEnquiryAsync(string WarehouseEntryId, string WarehouseEntryType);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMESHookController/StockInventory", ReplyAction="http://tempuri.org/IMESHookController/StockInventoryResponse")]
        System.Threading.Tasks.Task<string> StockInventoryAsync(string StockInventoryId, string Year, string Month, string WarehouseID, string SuppliesInfoList);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    public interface IMESHookControllerChannel : WMSService.IMESHookController, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    public partial class MESHookControllerClient : System.ServiceModel.ClientBase<WMSService.IMESHookController>, WMSService.IMESHookController
    {
        
    /// <summary>
    /// 实现此分部方法，配置服务终结点。
    /// </summary>
    /// <param name="serviceEndpoint">要配置的终结点</param>
    /// <param name="clientCredentials">客户端凭据</param>
    static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);
        
        public MESHookControllerClient() : 
                base(MESHookControllerClient.GetDefaultBinding(), MESHookControllerClient.GetDefaultEndpointAddress())
        {
            this.Endpoint.Name = EndpointConfiguration.BasicHttpBinding_IMESHookController.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public MESHookControllerClient(EndpointConfiguration endpointConfiguration) : 
                base(MESHookControllerClient.GetBindingForEndpoint(endpointConfiguration), MESHookControllerClient.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public MESHookControllerClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
                base(MESHookControllerClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public MESHookControllerClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(MESHookControllerClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public MESHookControllerClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        public System.Threading.Tasks.Task<string> PingAsync()
        {
            return base.Channel.PingAsync();
        }
        
        public System.Threading.Tasks.Task<string> WarehousingAsync(string WarehousingId, string WarehousingType, string WarehousingTime, string ProductionPlanId, string BatchPlanId, string WorkAreaName, string SuppliesKinds, string SuppliesInfoList)
        {
            return base.Channel.WarehousingAsync(WarehousingId, WarehousingType, WarehousingTime, ProductionPlanId, BatchPlanId, WorkAreaName, SuppliesKinds, SuppliesInfoList);
        }
        
        public System.Threading.Tasks.Task<string> WarehouseEntryAsync(string WarehouseEntryId, string WarehouseEntryType, string WarehouseEntryTime, string ProductionPlanId, string TotalWorkOrder, string BatchNumber, string BatchPlanId, string WorkAreaName, string WorkStationId, string SuppliesKinds, string SuppliesInfoList)
        {
            return base.Channel.WarehouseEntryAsync(WarehouseEntryId, WarehouseEntryType, WarehouseEntryTime, ProductionPlanId, TotalWorkOrder, BatchNumber, BatchPlanId, WorkAreaName, WorkStationId, SuppliesKinds, SuppliesInfoList);
        }
        
        public System.Threading.Tasks.Task<string> MaterialStockEnquiryAsync(string SuppliesId, string SuppliesName, string SuppliesType, string SuppliesUnit)
        {
            return base.Channel.MaterialStockEnquiryAsync(SuppliesId, SuppliesName, SuppliesType, SuppliesUnit);
        }
        
        public System.Threading.Tasks.Task<string> LogisticsControlAsync(string LogisticsId, string StartPoint, string Destination)
        {
            return base.Channel.LogisticsControlAsync(LogisticsId, StartPoint, Destination);
        }
        
        public System.Threading.Tasks.Task<string> LogisticsEnquiryAsync(string LogisticsId, string EquipmentId, string EquipmentName)
        {
            return base.Channel.LogisticsEnquiryAsync(LogisticsId, EquipmentId, EquipmentName);
        }
        
        public System.Threading.Tasks.Task<string> WarehousingStatusEnquiryAsync(string WarehousingId, string WarehousingType)
        {
            return base.Channel.WarehousingStatusEnquiryAsync(WarehousingId, WarehousingType);
        }
        
        public System.Threading.Tasks.Task<string> WarehouseEntryStatusEnquiryAsync(string WarehouseEntryId, string WarehouseEntryType)
        {
            return base.Channel.WarehouseEntryStatusEnquiryAsync(WarehouseEntryId, WarehouseEntryType);
        }
        
        public System.Threading.Tasks.Task<string> StockInventoryAsync(string StockInventoryId, string Year, string Month, string WarehouseID, string SuppliesInfoList)
        {
            return base.Channel.StockInventoryAsync(StockInventoryId, Year, Month, WarehouseID, SuppliesInfoList);
        }
        
        public virtual System.Threading.Tasks.Task OpenAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
        }
        
        public virtual System.Threading.Tasks.Task CloseAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginClose(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndClose));
        }
        
        private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.BasicHttpBinding_IMESHookController))
            {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                return result;
            }
            throw new System.InvalidOperationException(string.Format("找不到名称为“{0}”的终结点。", endpointConfiguration));
        }
        
        private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.BasicHttpBinding_IMESHookController))
            {
                return new System.ServiceModel.EndpointAddress("http://localhost:23456/Outside/MesHook.asmx");
            }
            throw new System.InvalidOperationException(string.Format("找不到名称为“{0}”的终结点。", endpointConfiguration));
        }
        
        private static System.ServiceModel.Channels.Binding GetDefaultBinding()
        {
            return MESHookControllerClient.GetBindingForEndpoint(EndpointConfiguration.BasicHttpBinding_IMESHookController);
        }
        
        private static System.ServiceModel.EndpointAddress GetDefaultEndpointAddress()
        {
            return MESHookControllerClient.GetEndpointAddress(EndpointConfiguration.BasicHttpBinding_IMESHookController);
        }
        
        public enum EndpointConfiguration
        {
            
            BasicHttpBinding_IMESHookController,
        }
    }
}
