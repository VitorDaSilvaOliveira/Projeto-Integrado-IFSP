document.addEventListener('DOMContentLoaded', () => {
    const toggle = document.getElementById('themeControlToggle');
    const html = document.documentElement;
    const icon = document.getElementById('themeIcon');

    const applyTheme = (theme) => {
        html.setAttribute('data-bs-theme', theme);
        localStorage.setItem('theme', theme);
        icon.className = `fa-solid ${theme === 'dark' ? 'fa-moon' : 'fa-sun'}`;
    };

    // Tema salvo ou default
    let savedTheme = localStorage.getItem('theme') || 'light';
    toggle.checked = savedTheme === 'dark';
    applyTheme(savedTheme);

    toggle.addEventListener('change', () => {
        const newTheme = toggle.checked ? 'dark' : 'light';
        applyTheme(newTheme);
    });
});

const toggle = document.getElementById('themeControlToggle');
const html = document.documentElement;
const label = toggle.nextElementSibling;

function updateThemeUI(theme) {
    if (theme === 'dark') {
        html.setAttribute('data-bs-theme', 'dark');
        toggle.checked = true;
        label.innerHTML = '<span class="fa-solid fa-moon"></span>';
        label.style.color = '#1758bc';
    } else {
        html.setAttribute('data-bs-theme', 'light');
        toggle.checked = false;
        label.innerHTML = '<span class="fa-solid fa-sun"></span>';
        label.style.color = '#ffc107';
    }
}

const savedTheme = localStorage.getItem('theme') || 'light';
updateThemeUI(savedTheme);

toggle.addEventListener('change', () => {
    const newTheme = toggle.checked ? 'dark' : 'light';
    localStorage.setItem('theme', newTheme);
    updateThemeUI(newTheme);
});

document.addEventListener('DOMContentLoaded', () => {
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(el => new bootstrap.Tooltip(el));
});