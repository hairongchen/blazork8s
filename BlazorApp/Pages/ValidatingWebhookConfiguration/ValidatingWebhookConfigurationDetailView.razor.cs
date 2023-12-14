using System.Threading.Tasks;
using k8s.Models;
using BlazorApp.Pages.Common;
namespace BlazorApp.Pages.ValidatingWebhookConfiguration
{
    public partial class ValidatingWebhookConfigurationDetailView :  DrawerPageBase<V1ValidatingWebhookConfiguration>
    {
        private V1ValidatingWebhookConfiguration Item { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Item = base.Options;
            await base.OnInitializedAsync();
        }
    }
}
