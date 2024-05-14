﻿using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace BlazorApp.GptWorkflow.Actions;

public class LoopEnd : StepBody
{
    private const string StepName = "LoopEnd";

    public GlobalContext GlobalContext { get; set; }

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        var msg = Message.NewMessage(GlobalContext, StepName);
        msg.StepResponse = msg.StepInput;
        GlobalContext.AllowLoop = false;
        return ExecutionResult.Next();
    }
}
