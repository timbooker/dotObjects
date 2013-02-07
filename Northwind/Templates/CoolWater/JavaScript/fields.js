function addSelected(id, sid, tid) {
    var selected = $("#" + sid + " :selected");
    var body = $("#" + tid + " tbody");
    var item = "<tr class='with-border'><td><input type='hidden' name='" + id + "' id='" + id + "' value='" + selected.val() + "' text='" + selected.text() + "' />" + selected.text() + "</td>"
        + "<td style='text-align:right;'><input type='checkbox' /></td></tr>";

    body.append(item);
    selected.remove();
}

function removeSelected(id, sid, tid) {
    var select = $("#" + sid);
    var checkbox = $("#" + tid + " tbody :checked");
    var items = checkbox.parent().parent();
    for (i = 0; i < items.length; i++) {
        var item = $(items[i]);
        var value = item.find(":hidden").val();
        var text = item.text();
        var option = "<option value='" + value + "'>" + text + "</option>";
        select.append(option);
        item.remove();
    }
}