using System.Collections.Generic;
using System.Threading.Tasks;
using AntDesign;
using BlazorApp.Pages.Deployment;
using BlazorApp.Pages.Node;
using BlazorApp.Pages.ReplicaSet;
using BlazorApp.Service.k8s;
using BlazorApp.Utils;
using k8s.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace BlazorApp.Pages.Common.Metadata;

public partial class ControllerBy : ComponentBase
{
    [Parameter]
    public IList<V1OwnerReference> Owner { get; set; }

    [Parameter]
    public bool ShowOwnerName { get; set; }

    [Inject]
    private IReplicaSetService ReplicaSetService { get; set; }

    [Inject]
    private INodeService NodeService { get; set; }

    [Inject]
    private IPodService PodService { get; set; }

    [Inject]
    private IDeploymentService DeploymentService { get; set; }

    [Inject]
    private ILogger<ControllerBy> Logger { get; set; }

    [Inject]
    private IMessageService Message { get; set; }
    [Inject]
    private DrawerService DrawerService { get; set; }

    private async Task OnRsNameClick(string rsName)
    {
        var item = ReplicaSetService.GetByName(rsName);
        await PageDrawerHelper<V1ReplicaSet>.Instance
            .SetDrawerService(DrawerService)
            .ShowDrawerAsync<ReplicaSetDetailView, V1ReplicaSet, bool>(item);
    }

    private async Task OnNodeNameClick(string nodeName)
    {
        var item = NodeService.GetByName(nodeName);
        await PageDrawerHelper<V1Node>.Instance
            .SetDrawerService(DrawerService)
            .ShowDrawerAsync<NodeDetailView, V1Node, bool>(item);

    }

    private async Task OnDeploymentNameClick(string name)
    {
        var item  = DeploymentService.GetByName(name);
        await PageDrawerHelper<V1Deployment>.Instance
            .SetDrawerService(DrawerService)
            .ShowDrawerAsync<DeploymentDetailView, V1Deployment, bool>(item);
    }

    private Task OnXClick(string name)
    {
        Logger.LogError($"{name}点击未实现");
        Message.Error($"{name}点击未实现");
        return Task.CompletedTask;
    }


    private Task OnObjClick(V1OwnerReference owner)
    {
        var name = owner.Name;
        var kind = owner.Kind;

        var task = kind switch
        {
            "Deployment" => OnDeploymentNameClick(name),
            "ReplicaSet" => OnRsNameClick(name),
            "Node"       => OnNodeNameClick(name),
            _            => OnXClick(name)
        };

        return task;
    }
}
