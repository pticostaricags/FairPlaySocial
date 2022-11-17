export function selectTheme(themeName) {
    var themeLink = document.getElementById("selectedThemeLink");
    themeLink.setAttribute("href", "_content/FairPlaySocial.SharedUI/themes/" + themeName + "/theme.css");
}