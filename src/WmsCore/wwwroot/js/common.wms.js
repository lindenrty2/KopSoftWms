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
    if (no == null || no.length < 7) return false;
    if (no[0] == "L" && no[1] == "K" && no[2] == "_"
        && no[3] == "L" && no[4] == "X" && no[5] == "_" ) {
        return true;
    }
    return false;
}

function GetInventoryBoxNo(no) {
    var words = no.split("_");
    if (words.length < 4) return no;
    return words[0] + "_" + words[1] + "_" + words[2];
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
 
function StorageRackStatusFormatter(value, row, index) {
    if (value == 'Normal' || value == '0') {
        return '<span class="label label-default radius">正常</span>';
    }
    else if (value == 'MissingBox' || value == '1') {
        return '<span class="label label-danger radius">空出</span>';
    }
    else if (value == 'UnknowBox' || value == '2') {
        return '<span class="label label-danger radius">满入</span>';
    }
    else {
        return '<span class="label label-danger radius">未知状态</span>';
    }
}

function StockStatusFormatter(value, row, index) { 
    switch (value) {
        case -1:
            return '<span class="label label-danger radius">已取消</span>';
        case 1: //任务创建中
            return '<span class="label label-default radius">任务创建中</span>';
        case 2://待操作
            return '<span class="label label-default radius">待操作</span>';
        case 3://进行中
            return '<span class="label label-primary radius">进行中</span>';
        case 4: //已完成
            return '<span class="label label-success radius">已完成</span>';
        default:
            return '<span class="label label-success radius">-</span>';
    }
}

function StockCountStatusFormatter(value, row, index) {
    switch (value) {
        case -1:
        case 'task_canceled':
            return '<span class="label label-danger radius">盘库任务取消</span>';
        case 1:
        case 'initial':
            return '<span class="label label-default radius">盘库任务创建中</span>';
        //return "初始";
        case 2:
        case 'task_confirm':
            return '<span class="label label-info radius">盘库任务确认</span>';
        //return "审核通过";
        case 3:
        case 'task_working':
            return '<span class="label label-primary radius">盘库中</span>';
        case 4:
        case 'task_finish':
            return '<span class="label label-success radius">盘库完成</span>';
        // "审核未通过";
        default:
            return '<span class="label label-success radius">-</span>';
    }
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
    if (row.IsPaused) { 
        return '<span class="label label-danger radius">暂停</span>';
    }
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
    if (value == null) {
        return '';
    }
    else if (value == 'None' || value == '0') {
        return '<span class="label label-default radius">未部署</span>';
    }
    else if (value == 'InPosition' || value == '1') {
        return '<span class="label label-success radius">在库</span>';
    }
    else if (value == 'Outing' || value == '2') {
        return '<span class="label label-info radius">出库中</span>';
    }
    else if (value == 'Outed' || value == '3') {
        return '<span class="label label-primary radius">出库完成</span>';
    }
    else if (value == 'Backing' || value == '4') {
        return '<span class="label label-info radius">归库中</span>';
    }
    else {
        return '<span class="label label-danger radius">异常状态</span>';
    }
}

//--------------------------------------------------------------------------
//MES
//--------------------------------------------------------------------------

function MESTaskTypeFormatter(value, row, index) {
    switch (value) {
        case 0:
        case 'Unknow':
            return '<span class="label label-default radius">不明</span>';
        case 1:
        case 'StockIn':
            return '<span class="label label-info radius">入库任务</span>';
        case 2:
        case 'StockOut':
            return '<span class="label label-danger radius">出库任务</span>';
        case 3:
        case 'StockCount':
            return '<span class="label label-success radius">盘库任务</span>';
        default:
            return '<span class="label label-default radius">-</span>';
    }
}

function MESWorkStatusFormatter(value, row, index) {
    switch (value) {
        case -1:
        case 'Failed':
            return '<span class="label label-danger radius">处理失败</span>';
        case 0:
        case 'Unknow':
            return '<span class="label label-default radius">不明</span>';
        case 1:
        case 'WaitPlan':
            return '<span class="label label-info radius">待计划</span>';
        case 2:
        case 'Planed':
            return '<span class="label label-info radius">已计划任务</span>';
        case 3:
        case 'Working':
            return '<span class="label label-primary radius">已开始处理</span>';
        case 4:
        case 'WorkComplated':
            return '<span class="label label-primary radius">处理已完成</span>';
        default:
            return '<span class="label label-success radius">-</span>';
    }
}

function MESNotifyStatusFormatter(value, row, index) {
    switch (value) {
        case -1:
        case 'Failed':
            return '<span class="label label-danger radius">通知失败</span>';
        case 0:
        case 'Unknow':
            return '<span class="label label-default radius">不明</span>';
        case 1:
        case 'Requested':
            return '<span class="label label-info radius">已接收任务</span>';
        case 2:
        case 'WaitResponse':
            return '<span class="label label-info radius">等待回馈MES</span>';
        case 3:
        case 'Responsed':
            return '<span class="label label-primary radius">已反馈MES</span>'; 
        default:
            return '<span class="label label-success radius">-</span>';
    }
}
//--------------------------------------------------------------------------
//WCS
//--------------------------------------------------------------------------

function WCSTaskTypeFormatter(value, row, index) {
    switch (value) {
        case 0:
        case 'Unknow':
            return '<span class="label label-default radius">不明</span>'; 
        case 1:
        case 'StockOut':
            return '<span class="label label-danger radius">出库指令</span>'; 
        case 2:
        case 'StockBack':
            return '<span class="label label-info radius">归库指令</span>';
        default:
            return '<span class="label label-default radius">-</span>';
    }
}

function WCSWorkStatusFormatter(value, row, index) {
    switch (value) {
        case -2:
        case 'Paused':
            return '<span class="label label-danger radius">暂停</span>';
        case -1:
        case 'Failed':
            return '<span class="label label-danger radius">处理失败</span>';
        case 0:
        case 'Unknow':
            return '<span class="label label-default radius">不明</span>';
        case 1:
        case 'WaitPlan':
            return '<span class="label label-default radius">待计划</span>';
        case 2:
        case 'Planed':
            return '<span class="label label-info radius">已计划任务</span>';
        case 3:
        case 'Working':
            return '<span class="label label-primary radius">已开始处理</span>';
        case 4:
        case 'WorkComplated':
            return '<span class="label label-primary radius">处理已完成</span>';
        default:
            return '<span class="label label-success radius">-</span>';
    } 
}

function WCSNotifyStatusFormatter(value, row, index) {
    switch (value) {
        case -1:
        case 'Failed':
            return '<span class="label label-danger radius">通知失败</span>';
        case 0:
        case 'Unknow':
            return '<span class="label label-default radius">不明</span>';
        case 1:
        case 'WaitRequest':
            return '<span class="label label-default radius">待通知</span>';
        case 2:
        case 'WaitResponse':
            return '<span class="label label-info radius">等待回馈</span>';
        case 3:
        case 'Responsed':
            return '<span class="label label-primary radius">已接收回馈</span>';
        case 4:
        case 'ManualResponsed':
            return '<span class="label label-primary radius">手工回馈</span>';
        default:
            return '<span class="label label-success radius">-</span>';
    } 
}


function DateTimeFormatter(value, row, index) { 
    return _self.jsonDateFormat(value);
}

function DateTimeFormatter2(value, row, index) {
    if (value == null) return "";
    value = value.replace("T", " ");
    return value;
}

function YesNoFormatter(value, row, index) {
    if (value == null) {
        return null;
    }
    else if (value == false) { 
        return "否";
    }
    else if (value == true) {
        return "是";
    }
}