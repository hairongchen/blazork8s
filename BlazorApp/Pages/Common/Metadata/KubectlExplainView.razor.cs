using System.Threading.Tasks;
using BlazorApp.Service.AI;
using BlazorApp.Service.k8s;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace BlazorApp.Pages.Common.Metadata;

public partial class KubectlExplainView : DrawerPageBase<string>

{
    private string _result;
    private string _resultCn;

    [Parameter] public string Field { get; set; }

    [Inject] private IKubectlService Kubectl { get; set; }

    [Inject] private IDocService DocService { get; set; }

    [Inject] private IAiService AiService { get; set; }

    [Inject] private ILogger<KubectlExplainView> Logger { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Field = base.Options;

        //先查询缓存中有没有解释
        //没有解释执行kubectl explain
        //缓存中没有中文，执行AI翻译
        if (!Field.IsNullOrEmpty())
        {
            var explainEntity = await DocService.GetExplainByField(Field);
            _result = explainEntity?.Explain;
            _resultCn = explainEntity?.ExplainCN;
        }

        if (string.IsNullOrWhiteSpace(_result))
        {
            Logger.LogWarning("no explain for {Field}", Field);
            //没有解释，执行kubectl explain
            _result = await Kubectl.Explain(Field);

            if (string.IsNullOrWhiteSpace(_resultCn))
            {
                Logger.LogWarning("translate {Field},AI enable:{Enabled}", Field, AiService.Enabled());
                //没有中文解释，查询翻译
                if (AiService.Enabled())
                {
                    AiService.SetChatEventHandler(EventHandler);
                    //todo 修改为可翻译当前选择的语言
                    _resultCn = await AiService.AIChat("请逐字翻译以下内容，注意请不要遗漏细节并保持原来的格式：" + _result);
                }
            }
        }

        await base.OnInitializedAsync();
    }


    private async void EventHandler(object sender, string resp)
    {
        _resultCn += resp;
        await InvokeAsync(StateHasChanged);
    }

    private async Task ReTranslate()
    {
        _resultCn = "";
        await InvokeAsync(StateHasChanged);
        _resultCn = await AiService.AIChat("请详细翻译下这段文字，不要遗漏细节：" + _result);
    }
}
