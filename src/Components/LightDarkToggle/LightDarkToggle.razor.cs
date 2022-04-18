using Microsoft.AspNetCore.Components;

namespace Tavenem.Blazor.Framework;

/// <summary>
/// Toggles between light and dark mode.
/// </summary>
public partial class LightDarkToggle
{
    /// <summary>
    /// Custom icon when set to dark mode.
    /// </summary>
    [Parameter] public string DarkModeIcon { get; set; } = DefaultIcons.DarkMode;

    /// <summary>
    /// Custom icon when set to light mode.
    /// </summary>
    [Parameter] public string LightModeIcon { get; set; } = DefaultIcons.LightMode;

    /// <summary>
    /// Whether the app is current set to a dark color scheme.
    /// </summary>
    public bool IsDarkMode { get; private set; }

    /// <summary>
    /// Text to display on the button.
    /// </summary>
    [Parameter] public string? Text { get; set; }

    /// <summary>
    /// One of the built-in color themes.
    /// </summary>
    [Parameter] public ThemeColor ThemeColor { get; set; }

    /// <summary>
    /// The final value assigned to the class attribute, including component
    /// values and anything assigned by the user in <see
    /// cref="TavenemComponentBase.AdditionalAttributes"/>.
    /// </summary>
    protected override string? CssClass => new CssBuilder("btn")
        .Add("btn-icon", string.IsNullOrWhiteSpace(Text))
        .Add(ThemeColor.ToCSS())
        .Add(Class)
        .AddClassFromDictionary(AdditionalAttributes)
        .ToString();

    private string Icon => IsDarkMode
        ? (DarkModeIcon ?? LightModeIcon)
        : (LightModeIcon ?? DarkModeIcon);

    [Inject] private ThemeService ThemeService { get; set; } = default!;

    /// <summary>
    /// Method invoked after each time the component has been rendered. Note
    /// that the component does not automatically re-render after the completion
    /// of any returned <see cref="Task" />, because that would cause an
    /// infinite render loop.
    /// </summary>
    /// <param name="firstRender">
    /// Set to <c>true</c> if this is the first time <see
    /// cref="ComponentBase.OnAfterRender(bool)" /> has been invoked on this
    /// component instance; otherwise <c>false</c>.
    /// </param>
    /// <returns>A <see cref="Task" /> representing any asynchronous
    /// operation.</returns>
    /// <remarks>
    /// The <see cref="ComponentBase.OnAfterRender(bool)" /> and <see
    /// cref="ComponentBase.OnAfterRenderAsync(bool)" /> lifecycle methods are
    /// useful for performing interop, or interacting with values received from
    /// <c>@ref</c>. Use the <paramref name="firstRender" /> parameter to ensure
    /// that initialization work is only performed once.
    /// </remarks>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var mode = await ThemeService.GetPreferredColorScheme();
            IsDarkMode = mode == ThemePreference.Dark;
        }
    }

    private async Task OnClickAsync()
    {
        await ThemeService.SetColorScheme(IsDarkMode ? ThemePreference.Light : ThemePreference.Dark);
        IsDarkMode = !IsDarkMode;
    }
}