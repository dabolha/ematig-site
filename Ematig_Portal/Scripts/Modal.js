$(function () {
    //Optional: turn the chache off
    $.ajaxSetup({ cache: false });

    $('.modal-link').click(function () {
        var href = $(this).attr('href');
        $('.modal-content').load(href, function () {
            $('.modal').modal({
                backdrop: 'static',
                keyboard: true
            }, 'show');
            $('.modal').removeClass('hide');
            centerModals();
        });
        return false;
    });
});

/* center modal */
function centerModals() {
    $('.modal').each(function (i) {
        var $clone = $(this).clone().css('display', 'block').appendTo('body');
        var top = Math.round(($clone.height() - $clone.find('.modal-content').height()) / 2);
        top = top > 0 ? top : 0;
        $clone.remove();
        $(this).find('.modal-content').attr("style", "margin:" + top + "px auto 0 auto");
    });
}
$('.modal').on('.modal', centerModals);
$(window).on('resize', centerModals);