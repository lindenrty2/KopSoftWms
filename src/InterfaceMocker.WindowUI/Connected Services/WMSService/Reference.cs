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
        
        private string SuppliesInfoListField;
        
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
        public string SuppliesInfoList
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
    [System.Runtime.Serialization.DataContractAttribute(Name="OutsideStockInResult", Namespace="http://schemas.datacontract.org/2004/07/YL.Core.Dto")]
    public partial class OutsideStockInResult : object
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
    [System.Runtime.Serialization.DataContractAttribute(Name="OutsideStockOutDto", Namespace="http://schemas.datacontract.org/2004/07/YL.Core.Dto")]
    public partial class OutsideStockOutDto : object
    {
        
        private string BatchPlanIdField;
        
        private string ProductionPlanIdField;
        
        private string SuppliesInfoListField;
        
        private int SuppliesKindsField;
        
        private string WarehouseEntryIdField;
        
        private string WarehouseEntryTimeField;
        
        private string WarehouseEntryTypeField;
        
        private string WorkAreaNameField;
        
        private string WorkStationIdField;
        
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
        public string SuppliesInfoList
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
        public string WarehouseEntryId
        {
            get
            {
                return this.WarehouseEntryIdField;
            }
            set
            {
                this.WarehouseEntryIdField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string WarehouseEntryTime
        {
            get
            {
                return this.WarehouseEntryTimeField;
            }
            set
            {
                this.WarehouseEntryTimeField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string WarehouseEntryType
        {
            get
            {
                return this.WarehouseEntryTypeField;
            }
            set
            {
                this.WarehouseEntryTypeField = value;
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
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string WorkStationId
        {
            get
            {
                return this.WorkStationIdField;
            }
            set
            {
                this.WorkStationIdField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="OutsideStockOutResult", Namespace="http://schemas.datacontract.org/2004/07/YL.Core.Dto")]
    public partial class OutsideStockOutResult : object
    {
        
        private string ErrorIdField;
        
        private string ErrorInfoField;
        
        private bool SuccessField;
        
        private string WarehouseEntryIdField;
        
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
        public string WarehouseEntryId
        {
            get
            {
                return this.WarehouseEntryIdField;
            }
            set
            {
                this.WarehouseEntryIdField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="OutsideMaterialStockEnquiryArg", Namespace="http://schemas.datacontract.org/2004/07/YL.Core.Dto")]
    public partial class OutsideMaterialStockEnquiryArg : object
    {
        
        private string SuppliesIdField;
        
        private string SuppliesNameField;
        
        private string SuppliesTypeField;
        
        private string SuppliesUnitField;
        
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
        public string SuppliesUnit
        {
            get
            {
                return this.SuppliesUnitField;
            }
            set
            {
                this.SuppliesUnitField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="OutsideMaterialStockEnquiryResult", Namespace="http://schemas.datacontract.org/2004/07/YL.Core.Dto")]
    public partial class OutsideMaterialStockEnquiryResult : object
    {
        
        private string ErrorIDField;
        
        private WMSService.OutsideMaterialStockEnquiryItem[] MaterialStockInfoField;
        
        private bool SuccessField;
        
        private string SuppliesIdField;
        
        private string SuppliesNameField;
        
        private string SuppliesTypeField;
        
        private string SuppliesUnitField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ErrorID
        {
            get
            {
                return this.ErrorIDField;
            }
            set
            {
                this.ErrorIDField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public WMSService.OutsideMaterialStockEnquiryItem[] MaterialStockInfo
        {
            get
            {
                return this.MaterialStockInfoField;
            }
            set
            {
                this.MaterialStockInfoField = value;
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
        public string SuppliesUnit
        {
            get
            {
                return this.SuppliesUnitField;
            }
            set
            {
                this.SuppliesUnitField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="OutsideMaterialStockEnquiryItem", Namespace="http://schemas.datacontract.org/2004/07/YL.Core.Dto")]
    public partial class OutsideMaterialStockEnquiryItem : object
    {
        
        private string BalanceStockField;
        
        private string InventoryBoxNoField;
        
        private string PaperStockField;
        
        private string PositionField;
        
        private string StorageRackPositionField;
        
        private string WarehouseIdField;
        
        private string WarehouseNameField;
        
        private string WarehousePositionField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string BalanceStock
        {
            get
            {
                return this.BalanceStockField;
            }
            set
            {
                this.BalanceStockField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string InventoryBoxNo
        {
            get
            {
                return this.InventoryBoxNoField;
            }
            set
            {
                this.InventoryBoxNoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string PaperStock
        {
            get
            {
                return this.PaperStockField;
            }
            set
            {
                this.PaperStockField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Position
        {
            get
            {
                return this.PositionField;
            }
            set
            {
                this.PositionField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string StorageRackPosition
        {
            get
            {
                return this.StorageRackPositionField;
            }
            set
            {
                this.StorageRackPositionField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string WarehouseId
        {
            get
            {
                return this.WarehouseIdField;
            }
            set
            {
                this.WarehouseIdField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string WarehouseName
        {
            get
            {
                return this.WarehouseNameField;
            }
            set
            {
                this.WarehouseNameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string WarehousePosition
        {
            get
            {
                return this.WarehousePositionField;
            }
            set
            {
                this.WarehousePositionField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="OutsideLogisticsControlArg", Namespace="http://schemas.datacontract.org/2004/07/YL.Core.Dto")]
    public partial class OutsideLogisticsControlArg : object
    {
        
        private string DestinationField;
        
        private string LogisticsIdField;
        
        private string StartPointField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Destination
        {
            get
            {
                return this.DestinationField;
            }
            set
            {
                this.DestinationField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string LogisticsId
        {
            get
            {
                return this.LogisticsIdField;
            }
            set
            {
                this.LogisticsIdField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string StartPoint
        {
            get
            {
                return this.StartPointField;
            }
            set
            {
                this.StartPointField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="OutsideLogisticsControlResult", Namespace="http://schemas.datacontract.org/2004/07/YL.Core.Dto")]
    public partial class OutsideLogisticsControlResult : object
    {
        
        private string EquipmentIdField;
        
        private string EquipmentNameField;
        
        private string ErrorIdField;
        
        private string ErrorInfoField;
        
        private bool IsNormalExecutionField;
        
        private bool SuccessField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string EquipmentId
        {
            get
            {
                return this.EquipmentIdField;
            }
            set
            {
                this.EquipmentIdField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string EquipmentName
        {
            get
            {
                return this.EquipmentNameField;
            }
            set
            {
                this.EquipmentNameField = value;
            }
        }
        
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
        public bool IsNormalExecution
        {
            get
            {
                return this.IsNormalExecutionField;
            }
            set
            {
                this.IsNormalExecutionField = value;
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
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="OutsideLogisticsEnquiryArg", Namespace="http://schemas.datacontract.org/2004/07/YL.Core.Dto")]
    public partial class OutsideLogisticsEnquiryArg : object
    {
        
        private string EquipmentIdField;
        
        private string EquipmentNameField;
        
        private string LogisticsIdField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string EquipmentId
        {
            get
            {
                return this.EquipmentIdField;
            }
            set
            {
                this.EquipmentIdField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string EquipmentName
        {
            get
            {
                return this.EquipmentNameField;
            }
            set
            {
                this.EquipmentNameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string LogisticsId
        {
            get
            {
                return this.LogisticsIdField;
            }
            set
            {
                this.LogisticsIdField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="OutsideLogisticsEnquiryResult", Namespace="http://schemas.datacontract.org/2004/07/YL.Core.Dto")]
    public partial class OutsideLogisticsEnquiryResult : object
    {
        
        private string EquipmentIdField;
        
        private string EquipmentNameField;
        
        private string PositionField;
        
        private string StatusField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string EquipmentId
        {
            get
            {
                return this.EquipmentIdField;
            }
            set
            {
                this.EquipmentIdField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string EquipmentName
        {
            get
            {
                return this.EquipmentNameField;
            }
            set
            {
                this.EquipmentNameField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Position
        {
            get
            {
                return this.PositionField;
            }
            set
            {
                this.PositionField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Status
        {
            get
            {
                return this.StatusField;
            }
            set
            {
                this.StatusField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="OutsideWarehousingStatusEnquiryArg", Namespace="http://schemas.datacontract.org/2004/07/YL.Core.Dto")]
    public partial class OutsideWarehousingStatusEnquiryArg : object
    {
        
        private string WarehousingIdField;
        
        private string WarehousingTypeField;
        
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
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="OutsideWarehousingStatusEnquiryResult", Namespace="http://schemas.datacontract.org/2004/07/YL.Core.Dto")]
    public partial class OutsideWarehousingStatusEnquiryResult : object
    {
        
        private string ErrorIdField;
        
        private string ErrorInfoField;
        
        private bool IsNormalWarehousingField;
        
        private string SuccessField;
        
        private string WarehousingIdField;
        
        private string WarehousingStatusInfoListField;
        
        private string WarehousingTypeField;
        
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
        public bool IsNormalWarehousing
        {
            get
            {
                return this.IsNormalWarehousingField;
            }
            set
            {
                this.IsNormalWarehousingField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Success
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
        public string WarehousingStatusInfoList
        {
            get
            {
                return this.WarehousingStatusInfoListField;
            }
            set
            {
                this.WarehousingStatusInfoListField = value;
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
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="OutsideWarehouseEntryStatusEnquiryArg", Namespace="http://schemas.datacontract.org/2004/07/YL.Core.Dto")]
    public partial class OutsideWarehouseEntryStatusEnquiryArg : object
    {
        
        private string WarehouseEntryIdField;
        
        private string WarehouseEntryTypeField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string WarehouseEntryId
        {
            get
            {
                return this.WarehouseEntryIdField;
            }
            set
            {
                this.WarehouseEntryIdField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string WarehouseEntryType
        {
            get
            {
                return this.WarehouseEntryTypeField;
            }
            set
            {
                this.WarehouseEntryTypeField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="OutsideWarehouseEntryStatusEnquiryResult", Namespace="http://schemas.datacontract.org/2004/07/YL.Core.Dto")]
    public partial class OutsideWarehouseEntryStatusEnquiryResult : object
    {
        
        private string ErrorIdField;
        
        private string ErrorInfoField;
        
        private bool IsNormalWarehouseEntryField;
        
        private string SuccessField;
        
        private string WarehouseEntryIdField;
        
        private string WarehouseEntryStatusInfoListField;
        
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
        public bool IsNormalWarehouseEntry
        {
            get
            {
                return this.IsNormalWarehouseEntryField;
            }
            set
            {
                this.IsNormalWarehouseEntryField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Success
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
        public string WarehouseEntryId
        {
            get
            {
                return this.WarehouseEntryIdField;
            }
            set
            {
                this.WarehouseEntryIdField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string WarehouseEntryStatusInfoList
        {
            get
            {
                return this.WarehouseEntryStatusInfoListField;
            }
            set
            {
                this.WarehouseEntryStatusInfoListField = value;
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
        System.Threading.Tasks.Task<WMSService.OutsideStockInResult> WarehousingAsync(WMSService.OutsideStockInDto data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMESHookController/WarehouseEntry", ReplyAction="http://tempuri.org/IMESHookController/WarehouseEntryResponse")]
        System.Threading.Tasks.Task<WMSService.OutsideStockOutResult> WarehouseEntryAsync(WMSService.OutsideStockOutDto data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMESHookController/MaterialStockEnquiry", ReplyAction="http://tempuri.org/IMESHookController/MaterialStockEnquiryResponse")]
        System.Threading.Tasks.Task<WMSService.OutsideMaterialStockEnquiryResult> MaterialStockEnquiryAsync(WMSService.OutsideMaterialStockEnquiryArg arg);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMESHookController/LogisticsControl", ReplyAction="http://tempuri.org/IMESHookController/LogisticsControlResponse")]
        System.Threading.Tasks.Task<WMSService.OutsideLogisticsControlResult> LogisticsControlAsync(WMSService.OutsideLogisticsControlArg arg);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMESHookController/LogisticsEnquiry", ReplyAction="http://tempuri.org/IMESHookController/LogisticsEnquiryResponse")]
        System.Threading.Tasks.Task<WMSService.OutsideLogisticsEnquiryResult> LogisticsEnquiryAsync(WMSService.OutsideLogisticsEnquiryArg arg);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMESHookController/WarehousingStatusEnquiry", ReplyAction="http://tempuri.org/IMESHookController/WarehousingStatusEnquiryResponse")]
        System.Threading.Tasks.Task<WMSService.OutsideWarehousingStatusEnquiryResult> WarehousingStatusEnquiryAsync(WMSService.OutsideWarehousingStatusEnquiryArg arg);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMESHookController/WarehouseEntryStatusEnquiry", ReplyAction="http://tempuri.org/IMESHookController/WarehouseEntryStatusEnquiryResponse")]
        System.Threading.Tasks.Task<WMSService.OutsideWarehouseEntryStatusEnquiryResult> WarehouseEntryStatusEnquiryAsync(WMSService.OutsideWarehouseEntryStatusEnquiryArg arg);
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
        
        public System.Threading.Tasks.Task<WMSService.OutsideStockInResult> WarehousingAsync(WMSService.OutsideStockInDto data)
        {
            return base.Channel.WarehousingAsync(data);
        }
        
        public System.Threading.Tasks.Task<WMSService.OutsideStockOutResult> WarehouseEntryAsync(WMSService.OutsideStockOutDto data)
        {
            return base.Channel.WarehouseEntryAsync(data);
        }
        
        public System.Threading.Tasks.Task<WMSService.OutsideMaterialStockEnquiryResult> MaterialStockEnquiryAsync(WMSService.OutsideMaterialStockEnquiryArg arg)
        {
            return base.Channel.MaterialStockEnquiryAsync(arg);
        }
        
        public System.Threading.Tasks.Task<WMSService.OutsideLogisticsControlResult> LogisticsControlAsync(WMSService.OutsideLogisticsControlArg arg)
        {
            return base.Channel.LogisticsControlAsync(arg);
        }
        
        public System.Threading.Tasks.Task<WMSService.OutsideLogisticsEnquiryResult> LogisticsEnquiryAsync(WMSService.OutsideLogisticsEnquiryArg arg)
        {
            return base.Channel.LogisticsEnquiryAsync(arg);
        }
        
        public System.Threading.Tasks.Task<WMSService.OutsideWarehousingStatusEnquiryResult> WarehousingStatusEnquiryAsync(WMSService.OutsideWarehousingStatusEnquiryArg arg)
        {
            return base.Channel.WarehousingStatusEnquiryAsync(arg);
        }
        
        public System.Threading.Tasks.Task<WMSService.OutsideWarehouseEntryStatusEnquiryResult> WarehouseEntryStatusEnquiryAsync(WMSService.OutsideWarehouseEntryStatusEnquiryArg arg)
        {
            return base.Channel.WarehouseEntryStatusEnquiryAsync(arg);
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
