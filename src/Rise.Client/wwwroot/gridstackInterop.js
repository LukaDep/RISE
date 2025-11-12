;(function () {
    const g = (window.gridstackInterop = window.gridstackInterop || {})
    const instances = (window.__gridstack_instances =
        window.__gridstack_instances || {})

    function waitForGridStack(maxAttempts = 50, delayMs = 100) {
        let attempts = 0
        return new Promise((resolve, reject) => {
            ;(function tryNow() {
                attempts++
                if (window.GridStack) return resolve(window.GridStack)
                if (attempts >= maxAttempts)
                    return reject(new Error('GridStack not found'))
                setTimeout(tryNow, delayMs)
            })()
        })
    }

    g.initGrid = async function (gridId, options, dotNetRef) {
        const el = document.getElementById(gridId)
        if (!el) return false

        try {
            await waitForGridStack()
        } catch {
            console.warn('gridstackInterop: GridStack not available')
            return false
        }

        let grid = null
        try {
            if (typeof window.GridStack.get === 'function') {
                grid = window.GridStack.get(el)
            }
            if (!grid && typeof window.GridStack.init === 'function') {
                grid = window.GridStack.init(
                    {
                        column: 12,
                        float: false,
                        disableOneColumnMode: false,
                        staticGrid: true,
                        ...(options || {}),
                    },
                    el
                )
            }
        } catch (err) {
            console.warn('gridstackInterop.initGrid error', err)
            return false
        }

        if (!grid) return false

        instances[gridId] = { grid, dotNetRef }

        try {
            if (
                dotNetRef &&
                typeof grid.on === 'function' &&
                !instances[gridId].hasChangeHandler
            ) {
                grid.on('change', (event, items) => {
                    dotNetRef
                        .invokeMethodAsync('OnJsLayoutChanged', items)
                        .catch((e) => console.warn(e))
                })
                instances[gridId].hasChangeHandler = true
            }
        } catch (e) {
            // ignore
        }

        return true
    }

    g.setEditMode = function (gridId, enabled) {
        try {
            const inst = instances[gridId]
            if (!inst?.grid) return
            const grid = inst.grid
            if (typeof grid.setStatic === 'function') grid.setStatic(!enabled)
            if (typeof grid.enableMove === 'function') grid.enableMove(enabled)
            if (typeof grid.enableResize === 'function')
                grid.enableResize(enabled)
        } catch (err) {
            console.warn('gridstackInterop.setEditMode error', err)
        }
    }

    g.destroy = function (gridId) {
        try {
            const inst = instances[gridId]
            if (!inst) return
            inst.grid.destroy(false)
            delete instances[gridId]
        } catch (err) {
            console.warn('gridstackInterop.destroy error', err)
        }
    }

    g.getInfo = function (gridId) {
        try {
            const inst = instances[gridId]
            if (!inst) return null
            const grid = inst.grid
            const nodes = grid.engine?.nodes
                ? grid.engine.nodes.map((n) => ({
                      x: n.x,
                      y: n.y,
                      w: n.w,
                      h: n.h,
                  }))
                : null
            return { nodes, opts: grid.opts || null }
        } catch (err) {
            console.warn('gridstackInterop.getInfo error', err)
            return null
        }
    }

    window.initGrid = function () {
        return (
            window.gridstackInterop &&
            window.gridstackInterop.initGrid &&
            window.gridstackInterop.initGrid.apply(
                window.gridstackInterop,
                arguments
            )
        )
    }
    window.setDraggable = function () {
        return (
            window.gridstackInterop &&
            window.gridstackInterop.setDraggable &&
            window.gridstackInterop.setDraggable.apply(
                window.gridstackInterop,
                arguments
            )
        )
    }
    window.setResizable = function () {
        return (
            window.gridstackInterop &&
            window.gridstackInterop.setResizable &&
            window.gridstackInterop.setResizable.apply(
                window.gridstackInterop,
                arguments
            )
        )
    }
    window.destroyGrid = function () {
        return (
            window.gridstackInterop &&
            window.gridstackInterop.destroy &&
            window.gridstackInterop.destroy.apply(
                window.gridstackInterop,
                arguments
            )
        )
    }
    window.setEditMode = function () {
        return (
            window.gridstackInterop &&
            window.gridstackInterop.setEditMode &&
            window.gridstackInterop.setEditMode.apply(
                window.gridstackInterop,
                arguments
            )
        )
    }
})()
