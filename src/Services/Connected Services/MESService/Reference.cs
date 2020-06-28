//------------------------------------------------------------------------------
// <自动生成>
//     此代码由工具生成。
//     //
//     对此文件的更改可能导致不正确的行为，并在以下条件下丢失:
//     代码重新生成。
// </自动生成>
//------------------------------------------------------------------------------

namespace MESService
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://webservice.mbfw.com/", ConfigurationName="MESService.MyMethodImpl")]
    public interface MyMethodImpl
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://webservice.mbfw.com/MyMethodImpl/LogisticsFinishRequest", ReplyAction="http://webservice.mbfw.com/MyMethodImpl/LogisticsFinishResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<MESService.LogisticsFinishResponse> LogisticsFinishAsync(MESService.LogisticsFinishRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://webservice.mbfw.com/MyMethodImpl/testoneRequest", ReplyAction="http://webservice.mbfw.com/MyMethodImpl/testoneResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<MESService.testoneResponse> testoneAsync(MESService.testoneRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://webservice.mbfw.com/MyMethodImpl/WarehousingFinishRequest", ReplyAction="http://webservice.mbfw.com/MyMethodImpl/WarehousingFinishResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<MESService.WarehousingFinishResponse1> WarehousingFinishAsync(MESService.WarehousingFinishRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://webservice.mbfw.com/MyMethodImpl/setMaterialTrackingServiceRequest", ReplyAction="http://webservice.mbfw.com/MyMethodImpl/setMaterialTrackingServiceResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<MESService.setMaterialTrackingServiceResponse> setMaterialTrackingServiceAsync(MESService.setMaterialTrackingServiceRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://webservice.mbfw.com/MyMethodImpl/getMaterialTrackingServiceRequest", ReplyAction="http://webservice.mbfw.com/MyMethodImpl/getMaterialTrackingServiceResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<MESService.getMaterialTrackingServiceResponse> getMaterialTrackingServiceAsync(MESService.getMaterialTrackingServiceRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://webservice.mbfw.com/MyMethodImpl/setInLibriaryServiceRequest", ReplyAction="http://webservice.mbfw.com/MyMethodImpl/setInLibriaryServiceResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<MESService.setInLibriaryServiceResponse> setInLibriaryServiceAsync(MESService.setInLibriaryServiceRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://webservice.mbfw.com/MyMethodImpl/getInLibriaryServiceRequest", ReplyAction="http://webservice.mbfw.com/MyMethodImpl/getInLibriaryServiceResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<MESService.getInLibriaryServiceResponse> getInLibriaryServiceAsync(MESService.getInLibriaryServiceRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://webservice.mbfw.com/MyMethodImpl/WarehousingEntryFinishRequest", ReplyAction="http://webservice.mbfw.com/MyMethodImpl/WarehousingEntryFinishResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<MESService.WarehousingEntryFinishResponse> WarehousingEntryFinishAsync(MESService.WarehousingEntryFinishRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://webservice.mbfw.com/MyMethodImpl/WarehousingFinishtestRequest", ReplyAction="http://webservice.mbfw.com/MyMethodImpl/WarehousingFinishtestResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<MESService.WarehousingFinishtestResponse> WarehousingFinishtestAsync(MESService.WarehousingFinishtestRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="LogisticsFinish", WrapperNamespace="http://webservice.mbfw.com/", IsWrapped=true)]
    public partial class LogisticsFinishRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://webservice.mbfw.com/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string arg0;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://webservice.mbfw.com/", Order=1)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string arg1;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://webservice.mbfw.com/", Order=2)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string arg2;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://webservice.mbfw.com/", Order=3)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string arg3;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://webservice.mbfw.com/", Order=4)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string arg4;
        
        public LogisticsFinishRequest()
        {
        }
        
        public LogisticsFinishRequest(string arg0, string arg1, string arg2, string arg3, string arg4)
        {
            this.arg0 = arg0;
            this.arg1 = arg1;
            this.arg2 = arg2;
            this.arg3 = arg3;
            this.arg4 = arg4;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="LogisticsFinishResponse", WrapperNamespace="http://webservice.mbfw.com/", IsWrapped=true)]
    public partial class LogisticsFinishResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://webservice.mbfw.com/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string @return;
        
        public LogisticsFinishResponse()
        {
        }
        
        public LogisticsFinishResponse(string @return)
        {
            this.@return = @return;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webservice.mbfw.com/")]
    public partial class test
    {
        
        private string aField;
        
        private int bField;
        
        private int cField;
        
        private double dField;
        
        private double eField;
        
        private string[] fField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string a
        {
            get
            {
                return this.aField;
            }
            set
            {
                this.aField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public int b
        {
            get
            {
                return this.bField;
            }
            set
            {
                this.bField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public int c
        {
            get
            {
                return this.cField;
            }
            set
            {
                this.cField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=3)]
        public double d
        {
            get
            {
                return this.dField;
            }
            set
            {
                this.dField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=4)]
        public double e
        {
            get
            {
                return this.eField;
            }
            set
            {
                this.eField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("f", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true, Order=5)]
        public string[] f
        {
            get
            {
                return this.fField;
            }
            set
            {
                this.fField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webservice.mbfw.com/")]
    public partial class warehousingFinishResponse
    {
        
        private string warehousingIdField;
        
        private string isNormalExcutionField;
        
        private string errorIdField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string WarehousingId
        {
            get
            {
                return this.warehousingIdField;
            }
            set
            {
                this.warehousingIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string IsNormalExcution
        {
            get
            {
                return this.isNormalExcutionField;
            }
            set
            {
                this.isNormalExcutionField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public string ErrorId
        {
            get
            {
                return this.errorIdField;
            }
            set
            {
                this.errorIdField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webservice.mbfw.com/")]
    public partial class warehousingFinish
    {
        
        private string warehousingIdField;
        
        private string warehousingFinishTimeField;
        
        private string warehouseIdField;
        
        private string warehouseNameField;
        
        private string warehousePositionField;
        
        private string errorIdField;
        
        private string errorInfoField;
        
        private string suppliesKindsField;
        
        private string suppliesInfoListField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string WarehousingId
        {
            get
            {
                return this.warehousingIdField;
            }
            set
            {
                this.warehousingIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string WarehousingFinishTime
        {
            get
            {
                return this.warehousingFinishTimeField;
            }
            set
            {
                this.warehousingFinishTimeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public string WarehouseId
        {
            get
            {
                return this.warehouseIdField;
            }
            set
            {
                this.warehouseIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=3)]
        public string WarehouseName
        {
            get
            {
                return this.warehouseNameField;
            }
            set
            {
                this.warehouseNameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=4)]
        public string WarehousePosition
        {
            get
            {
                return this.warehousePositionField;
            }
            set
            {
                this.warehousePositionField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=5)]
        public string ErrorId
        {
            get
            {
                return this.errorIdField;
            }
            set
            {
                this.errorIdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=6)]
        public string ErrorInfo
        {
            get
            {
                return this.errorInfoField;
            }
            set
            {
                this.errorInfoField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=7)]
        public string SuppliesKinds
        {
            get
            {
                return this.suppliesKindsField;
            }
            set
            {
                this.suppliesKindsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=8)]
        public string SuppliesInfoList
        {
            get
            {
                return this.suppliesInfoListField;
            }
            set
            {
                this.suppliesInfoListField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webservice.mbfw.com/")]
    public partial class inLibriaryService
    {
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webservice.mbfw.com/")]
    public partial class materialTrackingService
    {
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://webservice.mbfw.com/")]
    public partial class testresponse
    {
        
        private string aaField;
        
        private int bbField;
        
        private double ccField;
        
        private string[] ddField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string aa
        {
            get
            {
                return this.aaField;
            }
            set
            {
                this.aaField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public int bb
        {
            get
            {
                return this.bbField;
            }
            set
            {
                this.bbField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public double cc
        {
            get
            {
                return this.ccField;
            }
            set
            {
                this.ccField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("dd", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true, Order=3)]
        public string[] dd
        {
            get
            {
                return this.ddField;
            }
            set
            {
                this.ddField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="testone", WrapperNamespace="http://webservice.mbfw.com/", IsWrapped=true)]
    public partial class testoneRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://webservice.mbfw.com/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public MESService.test arg0;
        
        public testoneRequest()
        {
        }
        
        public testoneRequest(MESService.test arg0)
        {
            this.arg0 = arg0;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="testoneResponse", WrapperNamespace="http://webservice.mbfw.com/", IsWrapped=true)]
    public partial class testoneResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://webservice.mbfw.com/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public MESService.testresponse @return;
        
        public testoneResponse()
        {
        }
        
        public testoneResponse(MESService.testresponse @return)
        {
            this.@return = @return;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="WarehousingFinish", WrapperNamespace="http://webservice.mbfw.com/", IsWrapped=true)]
    public partial class WarehousingFinishRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://webservice.mbfw.com/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string arg0;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://webservice.mbfw.com/", Order=1)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string arg1;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://webservice.mbfw.com/", Order=2)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string arg2;
        
        public WarehousingFinishRequest()
        {
        }
        
        public WarehousingFinishRequest(string arg0, string arg1, string arg2)
        {
            this.arg0 = arg0;
            this.arg1 = arg1;
            this.arg2 = arg2;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="WarehousingFinishResponse", WrapperNamespace="http://webservice.mbfw.com/", IsWrapped=true)]
    public partial class WarehousingFinishResponse1
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://webservice.mbfw.com/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string @return;
        
        public WarehousingFinishResponse1()
        {
        }
        
        public WarehousingFinishResponse1(string @return)
        {
            this.@return = @return;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="setMaterialTrackingService", WrapperNamespace="http://webservice.mbfw.com/", IsWrapped=true)]
    public partial class setMaterialTrackingServiceRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://webservice.mbfw.com/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public MESService.materialTrackingService arg0;
        
        public setMaterialTrackingServiceRequest()
        {
        }
        
        public setMaterialTrackingServiceRequest(MESService.materialTrackingService arg0)
        {
            this.arg0 = arg0;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="setMaterialTrackingServiceResponse", WrapperNamespace="http://webservice.mbfw.com/", IsWrapped=true)]
    public partial class setMaterialTrackingServiceResponse
    {
        
        public setMaterialTrackingServiceResponse()
        {
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="getMaterialTrackingService", WrapperNamespace="http://webservice.mbfw.com/", IsWrapped=true)]
    public partial class getMaterialTrackingServiceRequest
    {
        
        public getMaterialTrackingServiceRequest()
        {
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="getMaterialTrackingServiceResponse", WrapperNamespace="http://webservice.mbfw.com/", IsWrapped=true)]
    public partial class getMaterialTrackingServiceResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://webservice.mbfw.com/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public MESService.materialTrackingService @return;
        
        public getMaterialTrackingServiceResponse()
        {
        }
        
        public getMaterialTrackingServiceResponse(MESService.materialTrackingService @return)
        {
            this.@return = @return;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="setInLibriaryService", WrapperNamespace="http://webservice.mbfw.com/", IsWrapped=true)]
    public partial class setInLibriaryServiceRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://webservice.mbfw.com/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public MESService.inLibriaryService arg0;
        
        public setInLibriaryServiceRequest()
        {
        }
        
        public setInLibriaryServiceRequest(MESService.inLibriaryService arg0)
        {
            this.arg0 = arg0;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="setInLibriaryServiceResponse", WrapperNamespace="http://webservice.mbfw.com/", IsWrapped=true)]
    public partial class setInLibriaryServiceResponse
    {
        
        public setInLibriaryServiceResponse()
        {
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="getInLibriaryService", WrapperNamespace="http://webservice.mbfw.com/", IsWrapped=true)]
    public partial class getInLibriaryServiceRequest
    {
        
        public getInLibriaryServiceRequest()
        {
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="getInLibriaryServiceResponse", WrapperNamespace="http://webservice.mbfw.com/", IsWrapped=true)]
    public partial class getInLibriaryServiceResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://webservice.mbfw.com/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public MESService.inLibriaryService @return;
        
        public getInLibriaryServiceResponse()
        {
        }
        
        public getInLibriaryServiceResponse(MESService.inLibriaryService @return)
        {
            this.@return = @return;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="WarehousingEntryFinish", WrapperNamespace="http://webservice.mbfw.com/", IsWrapped=true)]
    public partial class WarehousingEntryFinishRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://webservice.mbfw.com/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string arg0;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://webservice.mbfw.com/", Order=1)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string arg1;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://webservice.mbfw.com/", Order=2)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string arg2;
        
        public WarehousingEntryFinishRequest()
        {
        }
        
        public WarehousingEntryFinishRequest(string arg0, string arg1, string arg2)
        {
            this.arg0 = arg0;
            this.arg1 = arg1;
            this.arg2 = arg2;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="WarehousingEntryFinishResponse", WrapperNamespace="http://webservice.mbfw.com/", IsWrapped=true)]
    public partial class WarehousingEntryFinishResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://webservice.mbfw.com/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string @return;
        
        public WarehousingEntryFinishResponse()
        {
        }
        
        public WarehousingEntryFinishResponse(string @return)
        {
            this.@return = @return;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="WarehousingFinishtest", WrapperNamespace="http://webservice.mbfw.com/", IsWrapped=true)]
    public partial class WarehousingFinishtestRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://webservice.mbfw.com/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public MESService.warehousingFinish arg0;
        
        public WarehousingFinishtestRequest()
        {
        }
        
        public WarehousingFinishtestRequest(MESService.warehousingFinish arg0)
        {
            this.arg0 = arg0;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="WarehousingFinishtestResponse", WrapperNamespace="http://webservice.mbfw.com/", IsWrapped=true)]
    public partial class WarehousingFinishtestResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://webservice.mbfw.com/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public MESService.warehousingFinishResponse @return;
        
        public WarehousingFinishtestResponse()
        {
        }
        
        public WarehousingFinishtestResponse(MESService.warehousingFinishResponse @return)
        {
            this.@return = @return;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    public interface MyMethodImplChannel : MESService.MyMethodImpl, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    public partial class MyMethodImplClient : System.ServiceModel.ClientBase<MESService.MyMethodImpl>, MESService.MyMethodImpl
    {
        
    /// <summary>
    /// 实现此分部方法，配置服务终结点。
    /// </summary>
    /// <param name="serviceEndpoint">要配置的终结点</param>
    /// <param name="clientCredentials">客户端凭据</param>
    static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);
        
        public MyMethodImplClient() : 
                base(MyMethodImplClient.GetDefaultBinding(), MyMethodImplClient.GetDefaultEndpointAddress())
        {
            this.Endpoint.Name = EndpointConfiguration.MyMethodImplPort.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public MyMethodImplClient(EndpointConfiguration endpointConfiguration) : 
                base(MyMethodImplClient.GetBindingForEndpoint(endpointConfiguration), MyMethodImplClient.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public MyMethodImplClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
                base(MyMethodImplClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public MyMethodImplClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(MyMethodImplClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public MyMethodImplClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<MESService.LogisticsFinishResponse> MESService.MyMethodImpl.LogisticsFinishAsync(MESService.LogisticsFinishRequest request)
        {
            return base.Channel.LogisticsFinishAsync(request);
        }
        
        public System.Threading.Tasks.Task<MESService.LogisticsFinishResponse> LogisticsFinishAsync(string arg0, string arg1, string arg2, string arg3, string arg4)
        {
            MESService.LogisticsFinishRequest inValue = new MESService.LogisticsFinishRequest();
            inValue.arg0 = arg0;
            inValue.arg1 = arg1;
            inValue.arg2 = arg2;
            inValue.arg3 = arg3;
            inValue.arg4 = arg4;
            return ((MESService.MyMethodImpl)(this)).LogisticsFinishAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<MESService.testoneResponse> MESService.MyMethodImpl.testoneAsync(MESService.testoneRequest request)
        {
            return base.Channel.testoneAsync(request);
        }
        
        public System.Threading.Tasks.Task<MESService.testoneResponse> testoneAsync(MESService.test arg0)
        {
            MESService.testoneRequest inValue = new MESService.testoneRequest();
            inValue.arg0 = arg0;
            return ((MESService.MyMethodImpl)(this)).testoneAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<MESService.WarehousingFinishResponse1> MESService.MyMethodImpl.WarehousingFinishAsync(MESService.WarehousingFinishRequest request)
        {
            return base.Channel.WarehousingFinishAsync(request);
        }
        
        public System.Threading.Tasks.Task<MESService.WarehousingFinishResponse1> WarehousingFinishAsync(string arg0, string arg1, string arg2)
        {
            MESService.WarehousingFinishRequest inValue = new MESService.WarehousingFinishRequest();
            inValue.arg0 = arg0;
            inValue.arg1 = arg1;
            inValue.arg2 = arg2;
            return ((MESService.MyMethodImpl)(this)).WarehousingFinishAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<MESService.setMaterialTrackingServiceResponse> MESService.MyMethodImpl.setMaterialTrackingServiceAsync(MESService.setMaterialTrackingServiceRequest request)
        {
            return base.Channel.setMaterialTrackingServiceAsync(request);
        }
        
        public System.Threading.Tasks.Task<MESService.setMaterialTrackingServiceResponse> setMaterialTrackingServiceAsync(MESService.materialTrackingService arg0)
        {
            MESService.setMaterialTrackingServiceRequest inValue = new MESService.setMaterialTrackingServiceRequest();
            inValue.arg0 = arg0;
            return ((MESService.MyMethodImpl)(this)).setMaterialTrackingServiceAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<MESService.getMaterialTrackingServiceResponse> MESService.MyMethodImpl.getMaterialTrackingServiceAsync(MESService.getMaterialTrackingServiceRequest request)
        {
            return base.Channel.getMaterialTrackingServiceAsync(request);
        }
        
        public System.Threading.Tasks.Task<MESService.getMaterialTrackingServiceResponse> getMaterialTrackingServiceAsync()
        {
            MESService.getMaterialTrackingServiceRequest inValue = new MESService.getMaterialTrackingServiceRequest();
            return ((MESService.MyMethodImpl)(this)).getMaterialTrackingServiceAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<MESService.setInLibriaryServiceResponse> MESService.MyMethodImpl.setInLibriaryServiceAsync(MESService.setInLibriaryServiceRequest request)
        {
            return base.Channel.setInLibriaryServiceAsync(request);
        }
        
        public System.Threading.Tasks.Task<MESService.setInLibriaryServiceResponse> setInLibriaryServiceAsync(MESService.inLibriaryService arg0)
        {
            MESService.setInLibriaryServiceRequest inValue = new MESService.setInLibriaryServiceRequest();
            inValue.arg0 = arg0;
            return ((MESService.MyMethodImpl)(this)).setInLibriaryServiceAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<MESService.getInLibriaryServiceResponse> MESService.MyMethodImpl.getInLibriaryServiceAsync(MESService.getInLibriaryServiceRequest request)
        {
            return base.Channel.getInLibriaryServiceAsync(request);
        }
        
        public System.Threading.Tasks.Task<MESService.getInLibriaryServiceResponse> getInLibriaryServiceAsync()
        {
            MESService.getInLibriaryServiceRequest inValue = new MESService.getInLibriaryServiceRequest();
            return ((MESService.MyMethodImpl)(this)).getInLibriaryServiceAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<MESService.WarehousingEntryFinishResponse> MESService.MyMethodImpl.WarehousingEntryFinishAsync(MESService.WarehousingEntryFinishRequest request)
        {
            return base.Channel.WarehousingEntryFinishAsync(request);
        }
        
        public System.Threading.Tasks.Task<MESService.WarehousingEntryFinishResponse> WarehousingEntryFinishAsync(string arg0, string arg1, string arg2)
        {
            MESService.WarehousingEntryFinishRequest inValue = new MESService.WarehousingEntryFinishRequest();
            inValue.arg0 = arg0;
            inValue.arg1 = arg1;
            inValue.arg2 = arg2;
            return ((MESService.MyMethodImpl)(this)).WarehousingEntryFinishAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<MESService.WarehousingFinishtestResponse> MESService.MyMethodImpl.WarehousingFinishtestAsync(MESService.WarehousingFinishtestRequest request)
        {
            return base.Channel.WarehousingFinishtestAsync(request);
        }
        
        public System.Threading.Tasks.Task<MESService.WarehousingFinishtestResponse> WarehousingFinishtestAsync(MESService.warehousingFinish arg0)
        {
            MESService.WarehousingFinishtestRequest inValue = new MESService.WarehousingFinishtestRequest();
            inValue.arg0 = arg0;
            return ((MESService.MyMethodImpl)(this)).WarehousingFinishtestAsync(inValue);
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
            if ((endpointConfiguration == EndpointConfiguration.MyMethodImplPort))
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
            if ((endpointConfiguration == EndpointConfiguration.MyMethodImplPort))
            {
                return new System.ServiceModel.EndpointAddress("http://192.163.136.200:8200/WS_Server/MyMethod");
            }
            throw new System.InvalidOperationException(string.Format("找不到名称为“{0}”的终结点。", endpointConfiguration));
        }
        
        private static System.ServiceModel.Channels.Binding GetDefaultBinding()
        {
            return MyMethodImplClient.GetBindingForEndpoint(EndpointConfiguration.MyMethodImplPort);
        }
        
        private static System.ServiceModel.EndpointAddress GetDefaultEndpointAddress()
        {
            return MyMethodImplClient.GetEndpointAddress(EndpointConfiguration.MyMethodImplPort);
        }
        
        public enum EndpointConfiguration
        {
            
            MyMethodImplPort,
        }
    }
}
