// HDA.gg — JavaScript helpers

window.HDA = {
    // Scroll to element
    scrollTo: (elementId) => {
        const el = document.getElementById(elementId);
        if (el) el.scrollIntoView({ behavior: 'smooth' });
    },

    // Copy text to clipboard
    copyToClipboard: async (text) => {
        try {
            await navigator.clipboard.writeText(text);
            return true;
        } catch {
            return false;
        }
    },

    // Format countdown timer
    startCountdown: (targetTime, dotNetRef) => {
        const tick = () => {
            const diff = new Date(targetTime) - new Date();
            if (diff <= 0) {
                dotNetRef.invokeMethodAsync('OnCountdownComplete');
                return;
            }
            const h = Math.floor(diff / 3600000);
            const m = Math.floor((diff % 3600000) / 60000);
            const s = Math.floor((diff % 60000) / 1000);
            dotNetRef.invokeMethodAsync('UpdateCountdown', `${h}h ${m}m ${s}s`);
        };
        tick();
        return setInterval(tick, 1000);
    },

    clearTimer: (timerId) => clearInterval(timerId),
};
