using k8s.Models;
using Microsoft.AspNetCore.Components;

namespace ui.Pages.Node
{
    public partial class NodeSumView : ComponentBase
    {
        [Parameter]
        public V1Node Node { get; set; }
    }
}