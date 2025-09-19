const toggle = document.getElementById('themeControlToggle');
const html = document.documentElement;
const label = toggle.nextElementSibling;

function updateThemeUI(theme) {
    if (theme === 'dark') {
        html.setAttribute('data-bs-theme', 'dark');
        toggle.checked = true;
        label.innerHTML = '<span class="bi bi-moon-fill"></span>';
        label.style.color = '#1758bc';
    } else {
        html.setAttribute('data-bs-theme', 'light');
        toggle.checked = false;
        label.innerHTML = '<span class="bi bi-sun-fill"></span>';
        label.style.color = '#ffc107';
    }
}

toggle.addEventListener('change', () => {
    const newTheme = toggle.checked ? 'dark' : 'light';
    localStorage.setItem('theme', newTheme);
    updateThemeUI(newTheme);
});

document.addEventListener('DOMContentLoaded', () => {
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(el => new bootstrap.Tooltip(el));

    const savedTheme = localStorage.getItem('theme') || 'light';
    updateThemeUI(savedTheme);
});