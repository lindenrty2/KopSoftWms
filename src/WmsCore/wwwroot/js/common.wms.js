/**
 */

function IsStockInNo(no) {
    if (no == null || no.length <= 4) return false;
    if ((no[0] == "R" && no[1] == "K" && no[2] == "-")) {
        return true;
    }
    return false;
}

function IsStockOutNo(no) {
    if (no == null || no.length <= 4) return false;
    if ((no[0] == "W" && no[1] == "L" && no[2] == "-")) {
        return true;
    }
    return false;
}

function IsInventoryBoxNo(no) {
    if (no == null || no.length == 0) return false;
    if (no.split('-').length == 2) {
        return true;
    }
    return false;
}

function IsMaterialNo(no) {
    if (no == null || no.length == 0) return false;
    if ((no[0] == "B") || (no[0] == "Q" && no[1] == "N") ) {
        return true;
    }
    else if (no.split('-').length > 3) {
        return true;
    }
    return false;
}

function StockInStatusFormatter(value, row, index) {
    switch (value) {
        case -1:
        case 'task_canceled':
            return '<span class="label label-danger radius">已取消</span>';
        case 1:
        case 'initial':
            return '<span class="label label-default radius">待审核</span>';
        case 2:
        case 'task_confirm':
            return '<span class="label label-info radius">待操作</span>';
        case 3:
        case 'task_working':
            return '<span class="label label-primary radius">入库中</span>';
        case 4:
        case 'task_finish':
            return '<span class="label label-success radius">入库完成</span>';
        default:
            return '<span class="label label-success radius">-</span>';
    }
}

function StockOutStatusFormatter(value, row, index) {
    switch (value) {
        case -1:
        case 'task_canceled':
            return '<span class="label label-danger radius">出库任务取消</span>';
        case 1:
        case 'initial':
            return '<span class="label label-default radius">出库任务创建中</span>';
        //return "初始";
        case 2:
        case 'task_confirm':
            return '<span class="label label-info radius">出库任务确认</span>';
        //return "审核通过";
        case 3:
        case 'task_working':
            return '<span class="label label-primary radius">出库中</span>';
        case 4:
        case 'task_finish':
            return '<span class="label label-success radius">出库完成</span>';
        // "审核未通过";
        default:
            return '<span class="label label-success radius">-</span>';
    }
}

function InventoryBoxStatusFormatter(value, row, index) {
    if (value == 'None') {
        return "未部署";
    }
    else if (value == 'InPosition') {
        return "在库";
    }
    else if (value == 'Outing') {
        return "出库中";
    }
    else if (value == 'Outed') {
        return "出库完成";
    }
    else if (value == 'Backing') {
        return "归库中";
    }
    else {
        return "异常";
    }
}

function DateTimeFormatter (value, row, index) {
    return _self.jsonDateFormat(value);
}