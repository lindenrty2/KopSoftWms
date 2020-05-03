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
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="WMSService.WMSSoap")]
    public interface WMSSoap
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/HelloWorld", ReplyAction="*")]
        System.Threading.Tasks.Task<WMSService.HelloWorldResponse> HelloWorldAsync(WMSService.HelloWorldRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/WarehousingStatusEnquiry", ReplyAction="*")]
        System.Threading.Tasks.Task<WMSService.WarehousingStatusEnquiryResponse> WarehousingStatusEnquiryAsync(WMSService.WarehousingStatusEnquiryRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/WarehouseEntryStatusEnquiry", ReplyAction="*")]
        System.Threading.Tasks.Task<WMSService.WarehouseEntryStatusEnquiryResponse> WarehouseEntryStatusEnquiryAsync(WMSService.WarehouseEntryStatusEnquiryRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Warehousing", ReplyAction="*")]
        System.Threading.Tasks.Task<WMSService.WarehousingResponse> WarehousingAsync(WMSService.WarehousingRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/WarehouseEntry", ReplyAction="*")]
        System.Threading.Tasks.Task<WMSService.WarehouseEntryResponse> WarehouseEntryAsync(WMSService.WarehouseEntryRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/MaterialStockEnquiry", ReplyAction="*")]
        System.Threading.Tasks.Task<WMSService.MaterialStockEnquiryResponse> MaterialStockEnquiryAsync(WMSService.MaterialStockEnquiryRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/LogisticsControl", ReplyAction="*")]
        System.Threading.Tasks.Task<WMSService.LogisticsControlResponse> LogisticsControlAsync(WMSService.LogisticsControlRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/LogisticsEnquiry", ReplyAction="*")]
        System.Threading.Tasks.Task<WMSService.LogisticsEnquiryResponse> LogisticsEnquiryAsync(WMSService.LogisticsEnquiryRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class HelloWorldRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="HelloWorld", Namespace="http://tempuri.org/", Order=0)]
        public WMSService.HelloWorldRequestBody Body;
        
        public HelloWorldRequest()
        {
        }
        
        public HelloWorldRequest(WMSService.HelloWorldRequestBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute()]
    public partial class HelloWorldRequestBody
    {
        
        public HelloWorldRequestBody()
        {
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class HelloWorldResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="HelloWorldResponse", Namespace="http://tempuri.org/", Order=0)]
        public WMSService.HelloWorldResponseBody Body;
        
        public HelloWorldResponse()
        {
        }
        
        public HelloWorldResponse(WMSService.HelloWorldResponseBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class HelloWorldResponseBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string HelloWorldResult;
        
        public HelloWorldResponseBody()
        {
        }
        
        public HelloWorldResponseBody(string HelloWorldResult)
        {
            this.HelloWorldResult = HelloWorldResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class WarehousingStatusEnquiryRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="WarehousingStatusEnquiry", Namespace="http://tempuri.org/", Order=0)]
        public WMSService.WarehousingStatusEnquiryRequestBody Body;
        
        public WarehousingStatusEnquiryRequest()
        {
        }
        
        public WarehousingStatusEnquiryRequest(WMSService.WarehousingStatusEnquiryRequestBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class WarehousingStatusEnquiryRequestBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string WarehousingId;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string WarehousingType;
        
        public WarehousingStatusEnquiryRequestBody()
        {
        }
        
        public WarehousingStatusEnquiryRequestBody(string WarehousingId, string WarehousingType)
        {
            this.WarehousingId = WarehousingId;
            this.WarehousingType = WarehousingType;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class WarehousingStatusEnquiryResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="WarehousingStatusEnquiryResponse", Namespace="http://tempuri.org/", Order=0)]
        public WMSService.WarehousingStatusEnquiryResponseBody Body;
        
        public WarehousingStatusEnquiryResponse()
        {
        }
        
        public WarehousingStatusEnquiryResponse(WMSService.WarehousingStatusEnquiryResponseBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class WarehousingStatusEnquiryResponseBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string WarehousingStatusEnquiryResult;
        
        public WarehousingStatusEnquiryResponseBody()
        {
        }
        
        public WarehousingStatusEnquiryResponseBody(string WarehousingStatusEnquiryResult)
        {
            this.WarehousingStatusEnquiryResult = WarehousingStatusEnquiryResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class WarehouseEntryStatusEnquiryRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="WarehouseEntryStatusEnquiry", Namespace="http://tempuri.org/", Order=0)]
        public WMSService.WarehouseEntryStatusEnquiryRequestBody Body;
        
        public WarehouseEntryStatusEnquiryRequest()
        {
        }
        
        public WarehouseEntryStatusEnquiryRequest(WMSService.WarehouseEntryStatusEnquiryRequestBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class WarehouseEntryStatusEnquiryRequestBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string WarehouseEntryId;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string WarehouseEntryType;
        
        public WarehouseEntryStatusEnquiryRequestBody()
        {
        }
        
        public WarehouseEntryStatusEnquiryRequestBody(string WarehouseEntryId, string WarehouseEntryType)
        {
            this.WarehouseEntryId = WarehouseEntryId;
            this.WarehouseEntryType = WarehouseEntryType;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class WarehouseEntryStatusEnquiryResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="WarehouseEntryStatusEnquiryResponse", Namespace="http://tempuri.org/", Order=0)]
        public WMSService.WarehouseEntryStatusEnquiryResponseBody Body;
        
        public WarehouseEntryStatusEnquiryResponse()
        {
        }
        
        public WarehouseEntryStatusEnquiryResponse(WMSService.WarehouseEntryStatusEnquiryResponseBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class WarehouseEntryStatusEnquiryResponseBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string WarehouseEntryStatusEnquiryResult;
        
        public WarehouseEntryStatusEnquiryResponseBody()
        {
        }
        
        public WarehouseEntryStatusEnquiryResponseBody(string WarehouseEntryStatusEnquiryResult)
        {
            this.WarehouseEntryStatusEnquiryResult = WarehouseEntryStatusEnquiryResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class WarehousingRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="Warehousing", Namespace="http://tempuri.org/", Order=0)]
        public WMSService.WarehousingRequestBody Body;
        
        public WarehousingRequest()
        {
        }
        
        public WarehousingRequest(WMSService.WarehousingRequestBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class WarehousingRequestBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string warehousingid;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string warehousingtype;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string warehousingtime;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=3)]
        public string productionplanid;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=4)]
        public string batchplanid;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=5)]
        public string workareaname;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=6)]
        public string supplieskinds;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=7)]
        public string suppliesinfolist;
        
        public WarehousingRequestBody()
        {
        }
        
        public WarehousingRequestBody(string warehousingid, string warehousingtype, string warehousingtime, string productionplanid, string batchplanid, string workareaname, string supplieskinds, string suppliesinfolist)
        {
            this.warehousingid = warehousingid;
            this.warehousingtype = warehousingtype;
            this.warehousingtime = warehousingtime;
            this.productionplanid = productionplanid;
            this.batchplanid = batchplanid;
            this.workareaname = workareaname;
            this.supplieskinds = supplieskinds;
            this.suppliesinfolist = suppliesinfolist;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class WarehousingResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="WarehousingResponse", Namespace="http://tempuri.org/", Order=0)]
        public WMSService.WarehousingResponseBody Body;
        
        public WarehousingResponse()
        {
        }
        
        public WarehousingResponse(WMSService.WarehousingResponseBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class WarehousingResponseBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string WarehousingResult;
        
        public WarehousingResponseBody()
        {
        }
        
        public WarehousingResponseBody(string WarehousingResult)
        {
            this.WarehousingResult = WarehousingResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class WarehouseEntryRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="WarehouseEntry", Namespace="http://tempuri.org/", Order=0)]
        public WMSService.WarehouseEntryRequestBody Body;
        
        public WarehouseEntryRequest()
        {
        }
        
        public WarehouseEntryRequest(WMSService.WarehouseEntryRequestBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class WarehouseEntryRequestBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string WarehouseEntryid;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string WarehouseEntryType;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string WarehouseEntryTime;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=3)]
        public string ProductionPlanId;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=4)]
        public string BatchPlanId;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=5)]
        public string WorkStationId;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=6)]
        public string WorkAreaName;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=7)]
        public string SuppliesKinds;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=8)]
        public string SuppliesInfoList;
        
        public WarehouseEntryRequestBody()
        {
        }
        
        public WarehouseEntryRequestBody(string WarehouseEntryid, string WarehouseEntryType, string WarehouseEntryTime, string ProductionPlanId, string BatchPlanId, string WorkStationId, string WorkAreaName, string SuppliesKinds, string SuppliesInfoList)
        {
            this.WarehouseEntryid = WarehouseEntryid;
            this.WarehouseEntryType = WarehouseEntryType;
            this.WarehouseEntryTime = WarehouseEntryTime;
            this.ProductionPlanId = ProductionPlanId;
            this.BatchPlanId = BatchPlanId;
            this.WorkStationId = WorkStationId;
            this.WorkAreaName = WorkAreaName;
            this.SuppliesKinds = SuppliesKinds;
            this.SuppliesInfoList = SuppliesInfoList;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class WarehouseEntryResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="WarehouseEntryResponse", Namespace="http://tempuri.org/", Order=0)]
        public WMSService.WarehouseEntryResponseBody Body;
        
        public WarehouseEntryResponse()
        {
        }
        
        public WarehouseEntryResponse(WMSService.WarehouseEntryResponseBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class WarehouseEntryResponseBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string WarehouseEntryResult;
        
        public WarehouseEntryResponseBody()
        {
        }
        
        public WarehouseEntryResponseBody(string WarehouseEntryResult)
        {
            this.WarehouseEntryResult = WarehouseEntryResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class MaterialStockEnquiryRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="MaterialStockEnquiry", Namespace="http://tempuri.org/", Order=0)]
        public WMSService.MaterialStockEnquiryRequestBody Body;
        
        public MaterialStockEnquiryRequest()
        {
        }
        
        public MaterialStockEnquiryRequest(WMSService.MaterialStockEnquiryRequestBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class MaterialStockEnquiryRequestBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string suppliesid;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string suppliesname;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string suppliestype;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=3)]
        public string suppliesunit;
        
        public MaterialStockEnquiryRequestBody()
        {
        }
        
        public MaterialStockEnquiryRequestBody(string suppliesid, string suppliesname, string suppliestype, string suppliesunit)
        {
            this.suppliesid = suppliesid;
            this.suppliesname = suppliesname;
            this.suppliestype = suppliestype;
            this.suppliesunit = suppliesunit;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class MaterialStockEnquiryResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="MaterialStockEnquiryResponse", Namespace="http://tempuri.org/", Order=0)]
        public WMSService.MaterialStockEnquiryResponseBody Body;
        
        public MaterialStockEnquiryResponse()
        {
        }
        
        public MaterialStockEnquiryResponse(WMSService.MaterialStockEnquiryResponseBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class MaterialStockEnquiryResponseBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string MaterialStockEnquiryResult;
        
        public MaterialStockEnquiryResponseBody()
        {
        }
        
        public MaterialStockEnquiryResponseBody(string MaterialStockEnquiryResult)
        {
            this.MaterialStockEnquiryResult = MaterialStockEnquiryResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class LogisticsControlRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="LogisticsControl", Namespace="http://tempuri.org/", Order=0)]
        public WMSService.LogisticsControlRequestBody Body;
        
        public LogisticsControlRequest()
        {
        }
        
        public LogisticsControlRequest(WMSService.LogisticsControlRequestBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class LogisticsControlRequestBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string startpoint;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string destination;
        
        public LogisticsControlRequestBody()
        {
        }
        
        public LogisticsControlRequestBody(string startpoint, string destination)
        {
            this.startpoint = startpoint;
            this.destination = destination;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class LogisticsControlResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="LogisticsControlResponse", Namespace="http://tempuri.org/", Order=0)]
        public WMSService.LogisticsControlResponseBody Body;
        
        public LogisticsControlResponse()
        {
        }
        
        public LogisticsControlResponse(WMSService.LogisticsControlResponseBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class LogisticsControlResponseBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string LogisticsControlResult;
        
        public LogisticsControlResponseBody()
        {
        }
        
        public LogisticsControlResponseBody(string LogisticsControlResult)
        {
            this.LogisticsControlResult = LogisticsControlResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class LogisticsEnquiryRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="LogisticsEnquiry", Namespace="http://tempuri.org/", Order=0)]
        public WMSService.LogisticsEnquiryRequestBody Body;
        
        public LogisticsEnquiryRequest()
        {
        }
        
        public LogisticsEnquiryRequest(WMSService.LogisticsEnquiryRequestBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class LogisticsEnquiryRequestBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string equipmentid;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string equipmentname;
        
        public LogisticsEnquiryRequestBody()
        {
        }
        
        public LogisticsEnquiryRequestBody(string equipmentid, string equipmentname)
        {
            this.equipmentid = equipmentid;
            this.equipmentname = equipmentname;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class LogisticsEnquiryResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="LogisticsEnquiryResponse", Namespace="http://tempuri.org/", Order=0)]
        public WMSService.LogisticsEnquiryResponseBody Body;
        
        public LogisticsEnquiryResponse()
        {
        }
        
        public LogisticsEnquiryResponse(WMSService.LogisticsEnquiryResponseBody Body)
        {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class LogisticsEnquiryResponseBody
    {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string LogisticsEnquiryResult;
        
        public LogisticsEnquiryResponseBody()
        {
        }
        
        public LogisticsEnquiryResponseBody(string LogisticsEnquiryResult)
        {
            this.LogisticsEnquiryResult = LogisticsEnquiryResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    public interface WMSSoapChannel : WMSService.WMSSoap, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    public partial class WMSSoapClient : System.ServiceModel.ClientBase<WMSService.WMSSoap>, WMSService.WMSSoap
    {
        
    /// <summary>
    /// 实现此分部方法，配置服务终结点。
    /// </summary>
    /// <param name="serviceEndpoint">要配置的终结点</param>
    /// <param name="clientCredentials">客户端凭据</param>
    static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);
        
        public WMSSoapClient(EndpointConfiguration endpointConfiguration) : 
                base(WMSSoapClient.GetBindingForEndpoint(endpointConfiguration), WMSSoapClient.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public WMSSoapClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
                base(WMSSoapClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public WMSSoapClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(WMSSoapClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public WMSSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<WMSService.HelloWorldResponse> WMSService.WMSSoap.HelloWorldAsync(WMSService.HelloWorldRequest request)
        {
            return base.Channel.HelloWorldAsync(request);
        }
        
        public System.Threading.Tasks.Task<WMSService.HelloWorldResponse> HelloWorldAsync()
        {
            WMSService.HelloWorldRequest inValue = new WMSService.HelloWorldRequest();
            inValue.Body = new WMSService.HelloWorldRequestBody();
            return ((WMSService.WMSSoap)(this)).HelloWorldAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<WMSService.WarehousingStatusEnquiryResponse> WMSService.WMSSoap.WarehousingStatusEnquiryAsync(WMSService.WarehousingStatusEnquiryRequest request)
        {
            return base.Channel.WarehousingStatusEnquiryAsync(request);
        }
        
        public System.Threading.Tasks.Task<WMSService.WarehousingStatusEnquiryResponse> WarehousingStatusEnquiryAsync(string WarehousingId, string WarehousingType)
        {
            WMSService.WarehousingStatusEnquiryRequest inValue = new WMSService.WarehousingStatusEnquiryRequest();
            inValue.Body = new WMSService.WarehousingStatusEnquiryRequestBody();
            inValue.Body.WarehousingId = WarehousingId;
            inValue.Body.WarehousingType = WarehousingType;
            return ((WMSService.WMSSoap)(this)).WarehousingStatusEnquiryAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<WMSService.WarehouseEntryStatusEnquiryResponse> WMSService.WMSSoap.WarehouseEntryStatusEnquiryAsync(WMSService.WarehouseEntryStatusEnquiryRequest request)
        {
            return base.Channel.WarehouseEntryStatusEnquiryAsync(request);
        }
        
        public System.Threading.Tasks.Task<WMSService.WarehouseEntryStatusEnquiryResponse> WarehouseEntryStatusEnquiryAsync(string WarehouseEntryId, string WarehouseEntryType)
        {
            WMSService.WarehouseEntryStatusEnquiryRequest inValue = new WMSService.WarehouseEntryStatusEnquiryRequest();
            inValue.Body = new WMSService.WarehouseEntryStatusEnquiryRequestBody();
            inValue.Body.WarehouseEntryId = WarehouseEntryId;
            inValue.Body.WarehouseEntryType = WarehouseEntryType;
            return ((WMSService.WMSSoap)(this)).WarehouseEntryStatusEnquiryAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<WMSService.WarehousingResponse> WMSService.WMSSoap.WarehousingAsync(WMSService.WarehousingRequest request)
        {
            return base.Channel.WarehousingAsync(request);
        }
        
        public System.Threading.Tasks.Task<WMSService.WarehousingResponse> WarehousingAsync(string warehousingid, string warehousingtype, string warehousingtime, string productionplanid, string batchplanid, string workareaname, string supplieskinds, string suppliesinfolist)
        {
            WMSService.WarehousingRequest inValue = new WMSService.WarehousingRequest();
            inValue.Body = new WMSService.WarehousingRequestBody();
            inValue.Body.warehousingid = warehousingid;
            inValue.Body.warehousingtype = warehousingtype;
            inValue.Body.warehousingtime = warehousingtime;
            inValue.Body.productionplanid = productionplanid;
            inValue.Body.batchplanid = batchplanid;
            inValue.Body.workareaname = workareaname;
            inValue.Body.supplieskinds = supplieskinds;
            inValue.Body.suppliesinfolist = suppliesinfolist;
            return ((WMSService.WMSSoap)(this)).WarehousingAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<WMSService.WarehouseEntryResponse> WMSService.WMSSoap.WarehouseEntryAsync(WMSService.WarehouseEntryRequest request)
        {
            return base.Channel.WarehouseEntryAsync(request);
        }
        
        public System.Threading.Tasks.Task<WMSService.WarehouseEntryResponse> WarehouseEntryAsync(string WarehouseEntryid, string WarehouseEntryType, string WarehouseEntryTime, string ProductionPlanId, string BatchPlanId, string WorkStationId, string WorkAreaName, string SuppliesKinds, string SuppliesInfoList)
        {
            WMSService.WarehouseEntryRequest inValue = new WMSService.WarehouseEntryRequest();
            inValue.Body = new WMSService.WarehouseEntryRequestBody();
            inValue.Body.WarehouseEntryid = WarehouseEntryid;
            inValue.Body.WarehouseEntryType = WarehouseEntryType;
            inValue.Body.WarehouseEntryTime = WarehouseEntryTime;
            inValue.Body.ProductionPlanId = ProductionPlanId;
            inValue.Body.BatchPlanId = BatchPlanId;
            inValue.Body.WorkStationId = WorkStationId;
            inValue.Body.WorkAreaName = WorkAreaName;
            inValue.Body.SuppliesKinds = SuppliesKinds;
            inValue.Body.SuppliesInfoList = SuppliesInfoList;
            return ((WMSService.WMSSoap)(this)).WarehouseEntryAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<WMSService.MaterialStockEnquiryResponse> WMSService.WMSSoap.MaterialStockEnquiryAsync(WMSService.MaterialStockEnquiryRequest request)
        {
            return base.Channel.MaterialStockEnquiryAsync(request);
        }
        
        public System.Threading.Tasks.Task<WMSService.MaterialStockEnquiryResponse> MaterialStockEnquiryAsync(string suppliesid, string suppliesname, string suppliestype, string suppliesunit)
        {
            WMSService.MaterialStockEnquiryRequest inValue = new WMSService.MaterialStockEnquiryRequest();
            inValue.Body = new WMSService.MaterialStockEnquiryRequestBody();
            inValue.Body.suppliesid = suppliesid;
            inValue.Body.suppliesname = suppliesname;
            inValue.Body.suppliestype = suppliestype;
            inValue.Body.suppliesunit = suppliesunit;
            return ((WMSService.WMSSoap)(this)).MaterialStockEnquiryAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<WMSService.LogisticsControlResponse> WMSService.WMSSoap.LogisticsControlAsync(WMSService.LogisticsControlRequest request)
        {
            return base.Channel.LogisticsControlAsync(request);
        }
        
        public System.Threading.Tasks.Task<WMSService.LogisticsControlResponse> LogisticsControlAsync(string startpoint, string destination)
        {
            WMSService.LogisticsControlRequest inValue = new WMSService.LogisticsControlRequest();
            inValue.Body = new WMSService.LogisticsControlRequestBody();
            inValue.Body.startpoint = startpoint;
            inValue.Body.destination = destination;
            return ((WMSService.WMSSoap)(this)).LogisticsControlAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<WMSService.LogisticsEnquiryResponse> WMSService.WMSSoap.LogisticsEnquiryAsync(WMSService.LogisticsEnquiryRequest request)
        {
            return base.Channel.LogisticsEnquiryAsync(request);
        }
        
        public System.Threading.Tasks.Task<WMSService.LogisticsEnquiryResponse> LogisticsEnquiryAsync(string equipmentid, string equipmentname)
        {
            WMSService.LogisticsEnquiryRequest inValue = new WMSService.LogisticsEnquiryRequest();
            inValue.Body = new WMSService.LogisticsEnquiryRequestBody();
            inValue.Body.equipmentid = equipmentid;
            inValue.Body.equipmentname = equipmentname;
            return ((WMSService.WMSSoap)(this)).LogisticsEnquiryAsync(inValue);
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
            if ((endpointConfiguration == EndpointConfiguration.WMSSoap))
            {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                return result;
            }
            if ((endpointConfiguration == EndpointConfiguration.WMSSoap12))
            {
                System.ServiceModel.Channels.CustomBinding result = new System.ServiceModel.Channels.CustomBinding();
                System.ServiceModel.Channels.TextMessageEncodingBindingElement textBindingElement = new System.ServiceModel.Channels.TextMessageEncodingBindingElement();
                textBindingElement.MessageVersion = System.ServiceModel.Channels.MessageVersion.CreateVersion(System.ServiceModel.EnvelopeVersion.Soap12, System.ServiceModel.Channels.AddressingVersion.None);
                result.Elements.Add(textBindingElement);
                System.ServiceModel.Channels.HttpTransportBindingElement httpBindingElement = new System.ServiceModel.Channels.HttpTransportBindingElement();
                httpBindingElement.AllowCookies = true;
                httpBindingElement.MaxBufferSize = int.MaxValue;
                httpBindingElement.MaxReceivedMessageSize = int.MaxValue;
                result.Elements.Add(httpBindingElement);
                return result;
            }
            throw new System.InvalidOperationException(string.Format("找不到名称为“{0}”的终结点。", endpointConfiguration));
        }
        
        private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.WMSSoap))
            {
                return new System.ServiceModel.EndpointAddress("http://localhost:5713/WMS.asmx");
            }
            if ((endpointConfiguration == EndpointConfiguration.WMSSoap12))
            {
                return new System.ServiceModel.EndpointAddress("http://localhost:5713/WMS.asmx");
            }
            throw new System.InvalidOperationException(string.Format("找不到名称为“{0}”的终结点。", endpointConfiguration));
        }
        
        public enum EndpointConfiguration
        {
            
            WMSSoap,
            
            WMSSoap12,
        }
    }
}
