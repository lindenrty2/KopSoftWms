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
    using System.Runtime.Serialization;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="OutsideStockInDto", Namespace="http://schemas.datacontract.org/2004/07/YL.Core.Dto")]
    public partial class OutsideStockInDto : object
    {
        
        private string BatchPlanIdField;
        
        private string ProductionPlanIdField;
        
        private WMSService.OutsideMaterialDto[] SuppliesInfoListField;
        
        private int SuppliesKindsField;
        
        private string WarehousingIdField;
        
        private string WarehousingTimeField;
        
        private string WarehousingTypeField;
        
        private string WorkAreaNameField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string BatchPlanId
        {
            get
            {
                return this.BatchPlanIdField;
            }
            set
            {
                this.BatchPlanIdField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ProductionPlanId
        {
            get
            {
                return this.ProductionPlanIdField;
            }
            set
            {
                this.ProductionPlanIdField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public WMSService.OutsideMaterialDto[] SuppliesInfoList
        {
            get
            {
                return this.SuppliesInfoListField;
            }
            set
            {
                this.SuppliesInfoListField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int SuppliesKinds
        {
            get
            {
                return this.SuppliesKindsField;
            }
            set
            {
                this.SuppliesKindsField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string WarehousingId
        {
            get
            {
                return this.WarehousingIdField;
            }
            set
            {
                this.WarehousingIdField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string WarehousingTime
        {
            get
            {
                return this.WarehousingTimeField;
            }
            set
            {
                this.WarehousingTimeField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string WarehousingType
        {
            get
            {
                return this.WarehousingTypeField;
            }
            set
            {
                this.WarehousingTypeField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string WorkAreaName
        {
            get
            {
                return this.WorkAreaNameField;
            }
            set
            {
                this.WorkAreaNameField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="OutsideMaterialDto", Namespace="http://schemas.datacontract.org/2004/07/YL.Core.Dto")]
    public partial class OutsideMaterialDto : object
    {
        
        private string SuppliesIdField;
        
        private string SuppliesNameField;
        
        private int SuppliesNumberField;
        
        private string SuppliesOnlyIdField;
        
        private string SuppliesTypeField;
        
        private string UnitField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string SuppliesId
        {
            get
            {
                return this.SuppliesIdField;
            }
            set
            {
                this.SuppliesIdField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string SuppliesName
        {
            get
            {
                return this.SuppliesNameField;
            }
            set
            {
                this.SuppliesNameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int SuppliesNumber
        {
            get
            {
                return this.SuppliesNumberField;
            }
            set
            {
                this.SuppliesNumberField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string SuppliesOnlyId
        {
            get
            {
                return this.SuppliesOnlyIdField;
            }
            set
            {
                this.SuppliesOnlyIdField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string SuppliesType
        {
            get
            {
                return this.SuppliesTypeField;
            }
            set
            {
                this.SuppliesTypeField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Unit
        {
            get
            {
                return this.UnitField;
            }
            set
            {
                this.UnitField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="OutSideStockInResult", Namespace="http://schemas.datacontract.org/2004/07/YL.Core.Dto")]
    public partial class OutSideStockInResult : object
    {
        
        private string ErrorIdField;
        
        private string ErrorInfoField;
        
        private bool SuccessField;
        
        private string WarehousingIdField;
        
        private string WarehousingTimeField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ErrorId
        {
            get
            {
                return this.ErrorIdField;
            }
            set
            {
                this.ErrorIdField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ErrorInfo
        {
            get
            {
                return this.ErrorInfoField;
            }
            set
            {
                this.ErrorInfoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool Success
        {
            get
            {
                return this.SuccessField;
            }
            set
            {
                this.SuccessField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string WarehousingId
        {
            get
            {
                return this.WarehousingIdField;
            }
            set
            {
                this.WarehousingIdField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string WarehousingTime
        {
            get
            {
                return this.WarehousingTimeField;
            }
            set
            {
                this.WarehousingTimeField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="OutSideStockOutResult", Namespace="http://schemas.datacontract.org/2004/07/YL.Core.Dto")]
    public partial class OutSideStockOutResult : object
    {
        
        private string ErrorIdField;
        
        private string ErrorInfoField;
        
        private bool SuccessField;
        
        private bool WarehousingIdField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ErrorId
        {
            get
            {
                return this.ErrorIdField;
            }
            set
            {
                this.ErrorIdField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ErrorInfo
        {
            get
            {
                return this.ErrorInfoField;
            }
            set
            {
                this.ErrorInfoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool Success
        {
            get
            {
                return this.SuccessField;
            }
            set
            {
                this.SuccessField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool WarehousingId
        {
            get
            {
                return this.WarehousingIdField;
            }
            set
            {
                this.WarehousingIdField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="WMSService.IMESHookController")]
    public interface IMESHookController
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMESHookController/Ping", ReplyAction="http://tempuri.org/IMESHookController/PingResponse")]
        System.Threading.Tasks.Task<string> PingAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMESHookController/Warehousing", ReplyAction="http://tempuri.org/IMESHookController/WarehousingResponse")]
        System.Threading.Tasks.Task<WMSService.OutSideStockInResult> WarehousingAsync(WMSService.OutsideStockInDto data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMESHookController/WarehouseEntry", ReplyAction="http://tempuri.org/IMESHookController/WarehouseEntryResponse")]
        System.Threading.Tasks.Task<WMSService.OutSideStockOutResult> WarehouseEntryAsync(WMSService.OutsideStockInDto data);
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
        
        public System.Threading.Tasks.Task<WMSService.OutSideStockInResult> WarehousingAsync(WMSService.OutsideStockInDto data)
        {
            return base.Channel.WarehousingAsync(data);
        }
        
        public System.Threading.Tasks.Task<WMSService.OutSideStockOutResult> WarehouseEntryAsync(WMSService.OutsideStockInDto data)
        {
            return base.Channel.WarehouseEntryAsync(data);
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
