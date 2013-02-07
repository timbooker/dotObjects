var nyroSettings = 
{ 
    bgColor: '#ffffff' , minHeight: 0, minWidth : 0,
    closeButton : "<input type='button' id='closeBut' class='button nyroModalClose' value='X' />"
};

/* -- start - ModelSingle | ModelList -- */
function OpenDialog(id)
{   
    $.nyroModalManual($.extend(nyroSettings, {content : $("#" + id).html()}));
}
/* -- end - ModelSingle | ModelList -- */

/* -- start - ModelSingle -- */
function LoadSingleSelection(type, id)
{
    AjaxHelper.Process(type + "/Search/All", function(data)
    {
        var tbody = $("#sel" + id + " table tbody");
        tbody.empty();
        if(data != null)
            for(var i=0;i<data.Result.length;i++)
                tbody.append(CreateSingleSelectionRow(id, data.Result[i]));
        OpenDialog("sel" + id);
    });
}

function CreateSingleSelectionRow(id, domain)
{
    return $('<tr class="with-border"><td class="field"><a href="javascript:;" class="nyroModalClose" '
            + ' onclick="SelectSingleValue(\'' + id + '\', \'' + domain.Value + '\', this.innerHTML);" >' + domain.Instance + '</a>'
            + '</td><td></td></tr>');
}

function SelectSingleValue(id, value, label)
{
    $("#val" + id).val(value);
    $("#" + id).val(label);
}

function ClearSingleValue(id)
{
    $("#val" + id).val("");
    $("#" + id).val("");
}
/* -- end - ModelSingle -- */


/* -- start - ModelList -- */
function LoadListNew(id)
{
    OpenDialog("new" + id);
}

function SaveListNew(type, element, id, stateField)
{
    var data = {};
    
    $(element).parents("table").find(":input[@type!=button]").each(function()
    {
        var name = ($(this).attr("name") == "state" + id) ? stateField : $(this).attr("name");
        data[name] = $(this).val();
    });
    
    AjaxHelper.Process(type + "/New/0", data, function(response)
    {
        alert(response.Message);
        if(response.MessageType == "Success")
            LoadListSelection(type, id);
    });
}

function LoadListSelection(type, id)
{
    AjaxHelper.Process(type + "/Search/All", function(data)
    {
        var table = $("#sel" + id + " table");
        
        var tbody = table.children("tbody");
        tbody.empty();
        for(var i=0;i<data.Result.length;i++)
            tbody.append(CreateListSelectionRow(data.Result[i]));
        
        var tfoot = table.children("tfoot"); 
        tfoot.empty();
        tfoot.append(CreateListFooterRow(id));
        
        OpenDialog("sel" + id);
    });
}

function CreateListSelectionRow(domain)
{
    return $('<tr class="with-border"><td class="field">' + domain.Instance + '</td>'
        + '<td><input type="checkbox" class="checkbox" value="' + domain.Value + '" rel="' + domain.Instance + '" /></td></tr>');
}

function CreateListFooterRow(id)
{
    return $('<tr class="with-border"><td colspan=2>' + 
        '<input type="button" class="button nyroModalClose" value="Add Selected"' +
        ' onclick="SelectListValues($(this),\'' + id + '\')" /> ' +
        '<input type="button" class="button nyroModalClose" value="Cancel" />' +
        '</td></tr>');
}

function SelectListValues(element, id)
{
    
    var tbody = $("#added" + id + " tbody");
    element.parents("table").find("tbody :checkbox").each(function()
    {
        if(this.checked && !ItemListAlreadyExists(tbody, this.value))
            tbody.append(CreateListAddedRow(id, $(this).attr("rel"), $(this).val()));
    });
}

function ItemListAlreadyExists(body, value)
{
    if(body.find(":hidden[@value='" + value + "']").length > 0)
        return true;
    return false;
}

function CreateListAddedRow(id, text, value)
{
    return $('<tr class="with-border field"><td>' + text + '</td>' + 
        '<td style="width:1%;"><input type="hidden" name="' + id + '" value="' + value + '" />' +
        '<input type="checkbox" class="checkbox" /></td></tr>');
}

function RemoveListSelection(id)
{
    $("#added" + id + " tbody :checkbox").each(function()
    {
        if(this.checked)
            $(this).parent().parent().remove();
    });
}

/* -- end - ModelList -- */