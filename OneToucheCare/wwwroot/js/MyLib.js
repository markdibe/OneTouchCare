var Helper = {
    GetElementValue: function (id) {
        return $("#" + id).val();
    },
    SetElementValue: function (id, avalue) {
        $("#" + id).val(avalue);
    },
    HideElement: function (id) {
        $("#" + id).hide();
    },
    ShowElement: function (id) {
        $("#" + id).show();
    },
    GenerateColumnsForDataTable: function (data) {
        let ObjectKeys = Object.keys(data[0]);
        let builder = "";
        $(ObjectKeys).each(function (i, d) {
            builder += "{'data':'" + d + "',name:'" + d + "',title:'" + d + "',visible:true,defaultContent:''},";
        });
        console.log(builder);
    },
    AjaxPostHeader: function (url, data) {
        return $.ajax({
            type: 'post',
            contentType: 'application/json;charset=utf-8',
            dataType: 'json',
            responseType: 'json',
            url: url + data
        });
    },
    AjaxPost: function (url, data) {
        return $.ajax({
            type: 'post',
            contentType: 'application/json;charset=utf-8',
            dataType: 'json',
            responseType: 'json',
            url: url,
            data: JSON.stringify(data)
        });
    }

};
