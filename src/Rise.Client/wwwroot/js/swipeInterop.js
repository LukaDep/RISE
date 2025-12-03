window.initSwipe = function (elementId, dotNetHelper) {
    const el = document.getElementById(elementId)
    if (!el) return

    const hammer = new Hammer(el, {
        touchAction: 'pan-y',
        recognizers: [
            [
                Hammer.Swipe,
                {
                    direction: Hammer.DIRECTION_HORIZONTAL,
                    threshold: 50,
                    velocity: 0.3,
                },
            ],
        ],
    })

    hammer.on('swipeleft', () => dotNetHelper.invokeMethodAsync('SwipeNext'))
    hammer.on('swiperight', () =>
        dotNetHelper.invokeMethodAsync('SwipePrevious')
    )
}
