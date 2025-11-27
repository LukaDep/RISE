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

    g.mapItemsToDto = function (items) {
        if (!items) return []

        return items.map((i) => {
            const el = i.el || i._el || null

            const id =
                (el && (el.getAttribute('data-widget-id') || el.id)) ||
                i.id ||
                null

            return {
                id: id,
                x: i.x,
                y: i.y,
                width: i.w,
                height: i.h,
                minWidth: i.minW ?? 0,
                minHeight: i.minH ?? 0,
            }
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
                        column: options?.column || 12,
                        float: options?.float || false,
                        disableOneColumnMode:
                            options?.disableOneColumnMode || false,
                        staticGrid: options?.staticGrid || true,
                        ...(options || {}),
                    },
                    el
                )
            }
        } catch (err) {
            console.warn('gridstackInterop.initGrid error', err)
            return false
        }

        if (!grid) {
            console.warn(
                'gridstackInterop.initGrid: init did not return a grid instance for',
                gridId
            )
            return false
        }

        // normalizeer naar één instance
        if (Array.isArray(grid)) {
            const found = grid.find((gItem) => gItem && gItem.el === el)
            grid = found || grid[0]
        }

        instances[gridId] = { grid, dotNetRef }

        return true
    }

    g.setEditMode = function (gridId, enabled) {
        // Guard: if enabled isn't explicitly a boolean, ignore the call (prevents undefined toggles)
        if (typeof enabled !== 'boolean') {
            try {
                console.warn(
                    'gridstackInterop.setEditMode: ignoring call with non-boolean enabled',
                    enabled
                )
            } catch (e) {}
            return
        }
        try {
            const inst = instances[gridId]
            if (!inst?.grid) return
            let grid = inst.grid
            if (Array.isArray(grid)) grid = grid[0]
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
            let grid = inst.grid
            if (Array.isArray(grid)) grid = grid[0]

            const nodes = grid?.engine?.nodes
                ? grid.engine.nodes.map((n) => ({
                      x: n.x,
                      y: n.y,
                      w: n.w,
                      h: n.h,
                  }))
                : null
            return { nodes, opts: grid?.opts || null }
        } catch (err) {
            console.warn('gridstackInterop.getInfo error', err)
            return null
        }
    }

    g.fitToContent = function (gridId, widgetId) {
        try {
            const inst = instances[gridId]
            if (!inst || !inst.grid) {
                console.warn(
                    'fitToContent: no grid instance for',
                    gridId,
                    instances
                )
                return
            }

            let grid = inst.grid
            if (Array.isArray(grid)) grid = grid[0]

            const root = grid.el || document.getElementById(gridId)
            if (!root) {
                console.warn('fitToContent: no root element for grid', gridId)
                return
            }

            const item = document.querySelector(
                `.grid-stack-item[id="${widgetId}"]`
            )

            if (!item) {
                console.warn('fitToContent: widget not found for id', widgetId)
                return
            }

            if (typeof grid.resizeToContent === 'function') {
                grid.resizeToContent(item)
            } else {
                console.error(
                    'fitToContent: resizeToContent is not a function op grid',
                    grid
                )
            }
        } catch (err) {
            console.error('resizeWidgetToContent error', err)
        }
    }

    // globale helpers
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
    window.fitToContent = function () {
        return (
            window.gridstackInterop &&
            window.gridstackInterop.fitToContent &&
            window.gridstackInterop.fitToContent.apply(
                window.gridstackInterop,
                arguments
            )
        )
    }

    // Helper function to get current widget layout
    g.getWidgets = function () {
        const elements = document.querySelectorAll('.grid-stack-item')
        const normalizedWidgets = Array.from(elements).map((element) => {
            const id = element.id
            const x = parseInt(element.getAttribute('gs-x') || '0', 10)
            const y = parseInt(element.getAttribute('gs-y') || '0', 10)
            const width = parseInt(element.getAttribute('gs-w'), 10)
            const height = parseInt(element.getAttribute('gs-h'), 10)
            return {
                id: id,
                x: x,
                y: y,
                width: width,
                height: height,
            }
        })
        return normalizedWidgets
    }

    // Expose both naming conventions for compatibility
    window.GridStackInterop = window.GridStackInterop || {}
    window.GridStackInterop.getWidgets = function () {
        return g.getWidgets()
    }

    // Also expose via lowercase for consistency
    window.gridstackInterop.getWidgets = g.getWidgets
})()
