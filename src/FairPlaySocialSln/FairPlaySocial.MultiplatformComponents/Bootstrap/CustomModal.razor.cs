using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlaySocial.MultiplatformComponents.Bootstrap
{
    public partial class CustomModal
    {
        [Parameter]
        [EditorRequired]
        public RenderFragment? Title { get; set; }
        [Parameter]
        [EditorRequired]
        public RenderFragment? Content { get; set; }
        [Parameter]
        [EditorRequired]
        public EventCallback OnCloseButtonClicked { get; set; }
        [Parameter]
        public string? CloseButtonText { get; set; }
        [Parameter]
        public EventCallback OnOkButtonClicked { get; set; }
        [Parameter]
        public string? OkButtonText { get; set; }
        [Parameter]
        [EditorRequired]
        public bool ShowFooter { get; set; } = true;
    }
}
