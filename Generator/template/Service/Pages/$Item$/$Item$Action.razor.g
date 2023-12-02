using System;
using System.Threading.Tasks;
using AntDesign;
using BlazorApp.Pages.Common.Metadata;
using BlazorApp.Pages.Workload;
using BlazorApp.Service;
using BlazorApp.Service.k8s;
using k8s.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace BlazorApp.Pages.Pod;

public partial class $Item$Action : ComponentBase
{
    [Parameter]
    public $ItemType$ Item { get; set; }

    [Parameter]
    public MenuMode MenuMode { get; set; }=MenuMode.Vertical;



    [Inject]
    private IPageDrawerService PageDrawerService { get; set; }

    [Inject]
    private ILogger<$Item$Action> Logger { get; set; }


    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }

    private async Task On$Item$DeleteClick($ItemType$ item)
    {
        await $Item$Service.Delete(item.Namespace(), item.Name());
        StateHasChanged();
    }

 

    private async Task OnYamlClick($ItemType$ item)
    {
        var options = PageDrawerService.DefaultOptions($"Yaml:{item.Name()}", width: 1000);
        await PageDrawerService.ShowDrawerAsync<YamlView<$ItemType$>, $ItemType$, bool>(options, item);
    }
}