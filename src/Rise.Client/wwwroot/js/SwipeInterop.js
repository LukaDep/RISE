window.initSwipe = function (elementId, dotNetHelper) {
    const el = document.getElementById(elementId)
    if (!el) return

    const hammer = new Hammer(el)

    hammer.on('swipeleft', function () {
        dotNetHelper.invokeMethodAsync('SwipeNext')
    })

    hammer.on('swiperight', function () {
        dotNetHelper.invokeMethodAsync('SwipePrevious')
    })
}
