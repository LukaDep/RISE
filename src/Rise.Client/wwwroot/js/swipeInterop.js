window.initSwipe = function (elementId, dotNetHelper) {
    const el = document.getElementById(elementId)
    if (!el) return

    requestAnimationFrame(() => {
        const hammer = new Hammer(el)
        hammer.get('swipe').set({
            direction: Hammer.DIRECTION_HORIZONTAL,
            threshold: 20,
            velocity: 0.3,
        })

        hammer.on('swipeleft', () =>
            dotNetHelper.invokeMethodAsync('SwipeNext')
        )
        hammer.on('swiperight', () =>
            dotNetHelper.invokeMethodAsync('SwipePrevious')
        )
    })
}
