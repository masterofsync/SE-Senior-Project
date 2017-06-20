
/* This is for ModelStateErorr Bootstrap Alerts*/

 
        //$(".alert").alert();
        //$(".alert").slideDown(500);
        //$(".alert")
        //    .fadeTo(8000, 500)
        //    .slideUp(500,
        //        function () {
        //            $(".alert").slideUp(500);
        //        });


$(function () {
    $('.clickableClose')
        .on('click',
            function () {
                //var effect = $(this).data('effect');
                $(this).closest('.panel').addClass('hidden');
            });

});