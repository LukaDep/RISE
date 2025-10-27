window.scrollTop = (function () {
    const instances = {};

    function init(buttonId) {
        const btn = document.getElementById(buttonId);
        if (!btn || instances[buttonId]) return;

        // Debug: report initialization so you can see it's running in the console.

        // Ensure the button has the necessary inline transition styles so
        // showing/hiding via inline styles animates even if Tailwind
        // utilities are purged by the build.
        btn.style.transition = btn.style.transition || 'opacity 200ms ease, transform 200ms ease';
        btn.style.transform = btn.style.transform || 'translateY(2rem)';
        btn.style.opacity = btn.style.opacity || '0';
        btn.style.pointerEvents = btn.style.pointerEvents || 'none';

        let lastY = window.scrollY;
        let rafPending = false;

        function show() {
            btn.style.opacity = '1';
            btn.style.transform = 'translateY(0)';
            btn.style.pointerEvents = 'auto';
        }

        function hide() {
            btn.style.opacity = '0';
            btn.style.transform = 'translateY(2rem)';
            btn.style.pointerEvents = 'none';
        }

        function update() {
            rafPending = false;
            const y = window.scrollY;
            const atTop = y <= 0;
            const atBottom = (window.innerHeight + y) >= (document.documentElement.scrollHeight - 2);

            if (atTop || atBottom) {
                hide();
            } else if (y < lastY) {
                // scrolling up -> show
                show();
            } else {
                // scrolling down -> hide
                hide();
            }

            lastY = y;
        }

        // Briefly reveal the button on init for debugging so you can see where it is.
        // After a short timeout, run the normal update check so behavior continues.
        try {
            show();
            setTimeout(() => {
                // run an initial update after the brief reveal so the button will hide again
                window.requestAnimationFrame(update);
            }, 1400);
        } catch (e) {
            // ignore
        }

        function onScroll() {
            if (!rafPending) {
                rafPending = true;
                window.requestAnimationFrame(update);
            }
        }

        function onClick(e) {
            e.preventDefault();
            window.scrollTo({ top: 0, behavior: 'smooth' });
        }

        window.addEventListener('scroll', onScroll, { passive: true });
        btn.addEventListener('click', onClick);

        instances[buttonId] = { onScroll, onClick };

        // initial update in case user is not at top on first render
        window.requestAnimationFrame(update);
    }

    function dispose(buttonId) {
        const inst = instances[buttonId];
        const btn = document.getElementById(buttonId);
        if (!inst || !btn) return;
        window.removeEventListener('scroll', inst.onScroll);
        btn.removeEventListener('click', inst.onClick);
        delete instances[buttonId];
    }

    return { init, dispose };
})();

// Small global wrapper for Blazor JS interop compatibility. Call `initScrollTop(id)`
// from Blazor; it will delegate to scrollTop.init if available.
window.initScrollTop = function (buttonId) {
    try {
        if (window.scrollTop && typeof window.scrollTop.init === 'function') {
            window.scrollTop.init(buttonId);
        } else {
            console.warn('scrollTop.init is not available');
        }
    } catch (e) {
        console.error('initScrollTop error', e);
    }
};

window.disposeScrollTop = function (buttonId) {
    try {
        if (window.scrollTop && typeof window.scrollTop.dispose === 'function') {
            window.scrollTop.dispose(buttonId);
        }
    } catch (e) {
        console.error('disposeScrollTop error', e);
    }
};

// Auto-initialize on DOMContentLoaded as a fallback in case Blazor interop
// doesn't call `initScrollTop` early enough or there's a race.
try {
    document.addEventListener('DOMContentLoaded', function () {
        try {
            if (typeof window.initScrollTop === 'function') {
                window.initScrollTop('scrollToTopBtn');
            }
        } catch (e) {
            console.warn('scrollTop: auto-init failed', e);
        }
    });
} catch (e) {
    // ignore in older browsers
}
