using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Tavenem.Blazor.Framework;

/// <summary>
/// A collapsible panel.
/// </summary>
public partial class Collapse : IDisposable
{
    private readonly List<AnchorLink> _links = new();

    private bool _disposedValue;

    /// <summary>
    /// The child content of this component.
    /// </summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    private bool _isOpen;
    /// <summary>
    /// Whether the collapsed content is currently displayed.
    /// </summary>
    [Parameter]
    public bool IsOpen
    {
        get => _isOpen;
        set => _ = SetOpenAsync(value);
    }

    /// <summary>
    /// Invoked when <see cref="IsOpen"/> changes.
    /// </summary>
    [Parameter] public EventCallback<bool> IsOpenChanged { get; set; }

    /// <summary>
    /// Invoked before opening. Can be used to load content.
    /// </summary>
    [Parameter] public EventCallback<Collapse> OnOpening { get; set; }

    /// <summary>
    /// Raised when <see cref="IsOpen"/> changes.
    /// </summary>
    public event EventHandler<bool>? OnIsOpenChanged;

    /// <summary>
    /// <para>
    /// A simple header for the component.
    /// </para>
    /// <para>
    /// Ignored if <see cref="TitleContent"/> is non-<see langword="null"/>.
    /// </para>
    /// </summary>
    [Parameter] public string? Title { get; set; }

    /// <summary>
    /// Complex header content.
    /// </summary>
    [Parameter] public RenderFragment? TitleContent { get; set; }

    /// <summary>
    /// The group to which this component belongs, if any.
    /// </summary>
    [CascadingParameter] protected Accordion? Accordion { get; set; }

    /// <summary>
    /// The final value assigned to the class attribute, including component
    /// values and anything assigned by the user in <see
    /// cref="TavenemComponentBase.AdditionalAttributes"/>.
    /// </summary>
    protected string CssClass => new CssBuilder("collapse")
        .Add("closed", !IsOpen)
        .Add(Class)
        .AddClassFromDictionary(AdditionalAttributes)
        .ToString();

    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    /// <summary>
    /// Method invoked when the component is ready to start, having received its
    /// initial parameters from its parent in the render tree. Override this
    /// method if you will perform an asynchronous operation and want the
    /// component to refresh when that operation is completed.
    /// </summary>
    /// <returns>
    /// A <see cref="Task" /> representing any asynchronous operation.
    /// </returns>
    protected override async Task OnInitializedAsync()
    {
        NavigationManager.LocationChanged += OnLocationChanged;
        if (Accordion is not null)
        {
            await Accordion.AddAsync(this);
        }
    }

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
        if (firstRender && !_isOpen)
        {
            var open = _links.Any(x => x.IsActive);
            if (open)
            {
                await SetOpenAsync(true);
            }
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing,
    /// or resetting unmanaged resources.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                NavigationManager.LocationChanged -= OnLocationChanged;
                Accordion?.Remove(this);
            }

            _disposedValue = true;
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing,
    /// or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Set the open state of this collapse.
    /// </summary>
    /// <param name="value">The open state.</param>
    public async Task SetOpenAsync(bool value)
    {
        if (_isOpen == value)
        {
            return;
        }

        await OnOpening.InvokeAsync(this);
        _isOpen = value;
        OnIsOpenChanged?.Invoke(this, _isOpen);
        await IsOpenChanged.InvokeAsync(_isOpen);
        StateHasChanged();
    }

    /// <summary>
    /// Toggle the open state of this collapse.
    /// </summary>
    public Task ToggleAsync() => SetOpenAsync(!_isOpen);

    internal void Add(AnchorLink link) => _links.Add(link);

    internal void ForceRedraw() => StateHasChanged();

    internal void Remove(AnchorLink link) => _links.Remove(link);

    private async void OnLocationChanged(object? sender, LocationChangedEventArgs args)
    {
        if (_isOpen)
        {
            return;
        }
        _links.ForEach(x => x.UpdateState(args));
        var open = _links.Any(x => x.IsActive);
        if (open)
        {
            await SetOpenAsync(true);
        }
    }
}