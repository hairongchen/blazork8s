﻿using BlazorApp.GptWorkflow.Actions;
using BlazorApp.GptWorkflow.Steps;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace BlazorApp.GptWorkflow.Workflow;

public class DoWhileYamlWorkflow : IGptWorkflow<GlobalContext>
{
    public static string Name => "DoWhileWorkYamlflow";
    public string Id => "DoWhileWorkYamlflow";
    public int Version => 1;

    public void Build(IWorkflowBuilder<GlobalContext> builder)
    {
        builder
            .UseDefaultErrorBehavior(WorkflowErrorHandling.Suspend)
            //开始
            .StartWith<Start>()
            .Input(step => step.HumanCommand, ctx => ctx.UserTask)
            .Input(step => step.GlobalContext, ctx => ctx)
            .Input(step => step.WorkflowName, ctx => DoWhileYamlWorkflow.Name)
            //启动k8s顾问，将用户输入语言，转换为k8s命令
            .Then<ExpertKubernetesYaml>()
            .Input(step => step.GlobalContext, ctx => ctx)
            //启动命令提取执行子流程
            .Then<SubWorkflowRunner>()
            .Input(step => step.GlobalContext, ctx => ctx)
            .Input(step => step.WorkflowName, ctx => FindCodeRunWorkflow.Name)
            .WaitFor(WorkflowConst.SubWorkflowEnd, ctx => WorkflowConst.SubWorkflowEnd)
            .Then<End>()
            .Input(step => step.GlobalContext, ctx => ctx);
    }
}
