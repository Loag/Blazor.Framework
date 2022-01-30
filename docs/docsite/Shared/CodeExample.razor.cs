using Microsoft.AspNetCore.Components;

namespace Tavenem.Blazor.Framework.Docs.Shared;

public partial class CodeExample
{
    [Parameter] public string? Code { get; set; }

    private RenderFragment? CodeFragment { get; set; }

    private RenderFragment AddContent()
        => builder => builder.AddMarkupContent(1, Code);

    protected override void OnInitialized()
        => CodeFragment = AddContent();
}