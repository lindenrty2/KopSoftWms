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