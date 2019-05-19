using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace YL.Core.Entity.Fluent.Validation
{
    public class InventoryBoxFluent : AbstractValidator<wms_inventorybox>
    {
        public InventoryBoxFluent()
        {
            RuleFor(x => x.Remark).MaximumLength(200).WithMessage("备注长度不能超过200");
        }
    }
}
