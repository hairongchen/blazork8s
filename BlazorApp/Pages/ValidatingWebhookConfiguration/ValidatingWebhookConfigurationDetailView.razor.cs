using System.Collections.Generic;
using System.Threading.Tasks;
using AntDesign;
using BlazorApp.Service.k8s;
using k8s.Models;
using Microsoft.AspNetCore.Components;

namespace BlazorApp.Pages.ValidatingWebhookConfiguration
{
    public partial class ValidatingWebhookConfigurationDetailView : FeedbackComponent<V1ValidatingWebhookConfiguration, bool>
    {
        private V1ValidatingWebhookConfiguration Item { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Item = base.Options;
            await base.OnInitializedAsync();
        }
    }
}
