window.blazorCulture = {
    get: () => localStorage['BlazorCulture'],
    set: (value) => localStorage['BlazorCulture'] = value
};
function scrollToTop() {
    document.documentElement.scrollTop = 0;
}