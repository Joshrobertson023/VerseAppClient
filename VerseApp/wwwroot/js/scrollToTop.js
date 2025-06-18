window.scrollToTopInit = () => {
    const btn = document.getElementById('scrollTopBtn');
    const container = document.getElementById('scrollContainer');
    if (!btn || !container) return;

    btn.classList.add('hidden');

    container.addEventListener('scroll', () => {
        if (container.scrollTop > 0 && btn.classList.contains('hidden')) {
            btn.classList.remove('hidden');
            btn.classList.add('visible', 'animate__zoomIn');
            btn.classList.remove('animate__zoomOut');
        }
        else if (container.scrollTop === 0 && btn.classList.contains('visible')) {
            btn.classList.remove('animate__zoomIn');
            btn.classList.add('animate__zoomOut');
            btn.addEventListener('animationend', () => {
                btn.classList.remove('animate__zoomOut', 'visible');
                btn.classList.add('hidden');
            }, { once: true });
        }
    });

    btn.addEventListener('click', () =>
        container.scrollTo({ top: 0, behavior: 'smooth' })
    );
};
