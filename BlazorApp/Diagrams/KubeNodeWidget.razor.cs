using System.Threading.Tasks;
using BlazorApp.Pages.Common;
using Extension.k8s;
using k8s;
using k8s.Models;
using Microsoft.AspNetCore.Components;

namespace BlazorApp.Diagrams;

public partial class KubeNodeWidget<T> : PageBase where T : IKubernetesObject<V1ObjectMeta>
{
    private string resName;

    private string typeName;
    [Parameter] public KubeNode<T> Node { get; set; }

    protected override async Task OnInitializedAsync()
    {
        typeName = Node.Item.GetType().Name;
        resName = Node.Item.Name();
        await base.OnInitializedAsync();
    }

    private bool? GetReadyStatus(T item)
    {
        return typeName switch
        {
            "V1Deployment" => (Node.Item as V1Deployment)?.IsReady(),
            "V1Pod" => (Node.Item as V1Pod)?.IsReady(),
            "V1ReplicaSet" => (Node.Item as V1ReplicaSet)?.IsReady(),
            // "V1Job" => (Node.Item as V1Job)?.IsReady(),
            // "V1CronJob" =>( Node.Item as V1CronJob)?.IsReady(),
            // "V1DaemonSet" => (Node.Item as V1DaemonSet)?.IsReady(),
            // "V1StatefulSet" => (Node.Item as V1StatefulSet)?.IsReady(),
            // "V1Node" => (Node.Item as V1Node)?.IsReady(),
            // "V1ReplicationController" => (Node.Item as V1ReplicationController)?.IsReady(),
            _ => true
        };
    }

    private bool? GetProcessingStatus(T item)
    {
        return typeName switch
        {
            "V1Deployment" => (Node.Item as V1Deployment)?.IsProcessing(),
            "V1Pod" => (Node.Item as V1Pod)?.IsProcessing(),
            "V1ReplicaSet" => (Node.Item as V1ReplicaSet)?.IsProcessing(),
            // "V1Job" => (Node.Item as V1Job)?.IsReady(),
            // "V1CronJob" =>( Node.Item as V1CronJob)?.IsReady(),
            // "V1DaemonSet" => (Node.Item as V1DaemonSet)?.IsReady(),
            // "V1StatefulSet" => (Node.Item as V1StatefulSet)?.IsReady(),
            // "V1Node" => (Node.Item as V1Node)?.IsReady(),
            // "V1ReplicationController" => (Node.Item as V1ReplicationController)?.IsReady(),
            _ => false
        };
    }

    private string GetIcon(string type)
    {
        return type switch
        {
            "V1Deployment" => "hdd",
            "V1Pod" => "appstore",
            "V1ReplicaSet" => "build",
            "V1Job" => "container",
            "V1CronJob" => "history",
            "V1DaemonSet" => "reconciliation",
            "V1StatefulSet" => "project",
            "V1ReplicationController" => "split-cells",
            "V1Node" => "database",
            "V1Ingress" => "gateway",
            "V1Service" => "node-expand",
            _ => "hdd"
        };
    }
}
