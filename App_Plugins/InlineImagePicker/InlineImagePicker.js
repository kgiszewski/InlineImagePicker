var InlineImagepicker={
    sortSelectedToTop: function(element) {
        var $selected=$(element).closest(".InlineImagePickerWrapperDiv").find('.InlineImageSelected');
        
        var $wrapper=$selected.parent().prepend($selected);
    },
    
    deselectAll: function (element){
        $(element).siblings('.InlineImageWrapper').each(function(){
            var $this=$(this);
            $this.removeClass('InlineImageSelected');
        });
    },
    
    buildXML: function(element){
    
        //console.log("building...");
        
        var $wrapperDiv=$(element).closest(".InlineImagePickerWrapperDiv"),
            $saveBox=$wrapperDiv.siblings(".InlineImagePickerSaveBox"),
            xml="";
        
        //console.log($saveBox);
        
        xml+="<imagePicker>";
        
        $wrapperDiv.find('.InlineImageSelected').each(function(){
        
            var $thisWrapperDiv=$(this);
        
            xml+="<image>"+$thisWrapperDiv.attr('data-mediaID')+"</image>";
        });
        
        xml+="</imagePicker>";
        
        $saveBox.val(xml);
    },
    
    buildAll: function(){

        var thisInstance=this;
    
        $(".InlineImagePickerWrapperDiv").each(function(){
            InlineImagepicker.buildXML(this);
            
            thisInstance.sortSelectedToTop(this);
        });
    }
}

$(function(){    
    //ini
    InlineImagepicker.buildAll();
    
    $('.InlineImageWrapper').click(function(){
        InlineImagepicker.deselectAll(this);
        var $thisWrapper=$(this);
        
        $thisWrapper.addClass('InlineImageSelected');
        
        InlineImagepicker.buildXML(this);
    });
});