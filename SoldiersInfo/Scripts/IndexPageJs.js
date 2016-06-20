// print modal
$('#print_modal').on('show.bs.modal', function (event) {
    var button_triggered = $(event.relatedTarget) // Button that triggered the modal
    if (button_triggered.attr('id') == "printOne") {
        var soldier_ID = button_triggered.data('soldierid')+";" // Extract info from data-* attributes
    }
    if (button_triggered.attr('id') == "printCompany") {
        var soldier_ID = ''
        $("input:checkbox[name=printSoldier]:checked").each(function () {
            soldier_ID = soldier_ID + $(this).val() + ";"
        });
    }
    var company = button_triggered.data('company')
    var datenow = button_triggered.data('datenow')
    var modal = $(this)

    $('#divreceiver').remove()
    
    modal.find('#decisionString').val('')
    
    modal.find('#defaultreceiver').val('')
    modal.find('.newreceiverstatic').val('')
    modal.find('.newreceiver').parent().parent().remove()

    var newelement_receiver = '<div id="divreceiver"><div class="form-group col-md-8 col-sm-12 col-xs-12"><div class="input-group"><input type="text" class="form-control receiver" id="defaultreceiverone" name="defaultreceiverone"  /><span class="input-group-btn"><button class="btn btn-danger deleteinput" type="button">Xóa</button></span></div></div><div class="form-group col-md-8 col-sm-12 col-xs-12"><div class="input-group"><input type="text" class="form-control receiver" id="defaultreceivertwo" name="defaultreceivertwo" /><span class="input-group-btn"><button class="btn btn-danger deleteinput" type="button">Xóa</button></span></div></div><div class="form-group col-md-8 col-sm-12 col-xs-12"><div class="input-group"><input type="text" class="form-control receiver newreceiverstatic" autocomplete="off" /><span class="input-group-btn"><button class="btn btn-danger deleteinput" type="button">Xóa</button></span></div></div></div>'
    $('#receiverdiv').append(newelement_receiver)
    modal.find('#soldier_ID').val(soldier_ID)
    modal.find('#company').val(company)
    modal.find('#decisionDate').val(datenow)
    modal.find('#deciderName').val('Phạm Văn Vĩnh')
    modal.find('#deciderArmyRank').val('Thượng tá')
    modal.find('#deciderRight').val('PHÓ TRƯỞNG PHÒNG')
    modal.find('#defaultreceiverone').val('Như trên')
    modal.find('#defaultreceivertwo').val('Lưu: PX13(CSBH)')
    
});
$('#receiverdiv').on('keypress', '.receiver', function () {
    var do_add = true, parent = $('#receiverdiv')
    count = 0
    parent.find('input[type=text]').each(function () {
        
        if ($(this).val() == '')
            do_add = false
    })

    if (!do_add) return;
    
    var newelement = '<div class="form-group col-md-8 col-sm-12 col-xs-12"> <div class="input-group"> <input type="text" class="form-control receiver newreceiver" autocomplete="off" /> <span class="input-group-btn">  <button class="btn btn-danger deleteinput" type="button">Xóa</button> </span> </div> </div>'
    $('#receiverdiv').append(newelement)

})
$('#receiverdiv').on('click', '.deleteinput', function () {
    var inputgroup2delete = $(this).parent().parent();
    var inputs = $('#receiverdiv').find('input')
    //console.log('', inputgroup2delete)
    //console.log('', inputs)
    if (inputs.length > 1) {
        inputgroup2delete.parent().remove()
    } else {
        inputgroup2delete.find('input').val('')
        $('#defaultreceiver').val('')
    }
})
$('#print_button').click(function () {
    var result = ""
    $('.receiver').each(function (index) {
        if ($(this).val() != '')
            result = result + $(this).val() + ";"
    })
    $('#defaultreceiver').val(result)
    $('#divreceiver').remove()
    $('#print_modal').modal('hide')
})
//code from "http://getbootstrap.com/javascript/#modals-related-target"
// code from "http://stackoverflow.com/questions/34113158/jquery-appends-new-input-row-after-filling-current-input-row-n-times"

