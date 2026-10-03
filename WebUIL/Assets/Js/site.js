(function () {
    var toggle = document.querySelector('[data-menu-toggle]');
    var menu = document.querySelector('[data-site-menu]');
    if (toggle && menu) {
        toggle.addEventListener('click', function () {
            var open = menu.classList.toggle('open');
            toggle.setAttribute('aria-expanded', open ? 'true' : 'false');
        });
    }
    var arrival = document.querySelector('[data-arrival]');
    var departure = document.querySelector('[data-departure]');
    if (arrival && departure && !arrival.value) {
        var today = new Date();
        var nextWeek = new Date();
        nextWeek.setDate(today.getDate() + 5);
        arrival.value = today.toISOString().slice(0, 10);
        departure.value = nextWeek.toISOString().slice(0, 10);
    }
})();
