using System;
using System.Threading.Tasks;
using BlazorApp.Utils;
using FluentScheduler;
using Microsoft.Extensions.Logging;

namespace BlazorApp.Service.k8s;

/// <summary>
/// 集群巡检，每10秒执行一次
/// </summary>
/// <param name="kubeService"></param>
/// <param name="podService"></param>
/// <param name="deploymentService"></param>
/// <param name="logger"></param>
public class ClusterInspectionService(
    IKubeService                      kubeService,
    IPodService                       podService,
    IDeploymentService                deploymentService,
    ILogger<ClusterInspectionService> logger
)

{
    private async void Inspection()
    {
        logger.LogInformation("cluster inspection start at {Time}", DateTime.Now);
        var results = ClusterInspectionResultContainer.Instance.GetResults();
        results.Clear();
        results.AddRange(await podService.Analyze());
        results.AddRange(await deploymentService.Analyze());
        ClusterInspectionResultContainer.Instance.LastInspection = DateTime.Now.ToUniversalTime();
        logger.LogInformation("cluster inspection ended at {Time}", DateTime.Now);

    }

    public Task StartAsync()
    {
        //每10秒执行一次指标更新
        JobManager.Initialize();
        JobManager.AddJob(Inspection, (s) => s.ToRunEvery(10).Seconds());
        return Task.CompletedTask;
    }

}