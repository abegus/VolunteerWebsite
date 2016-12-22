$(function () {
    $('[mask]').each(function (e) {
        $(this).mask($(this).attr('mask'));
        //$('#phone_us').mask('(000) 000-0000');
    });
});