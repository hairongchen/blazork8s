using System.Threading.Tasks;

namespace BlazorApp.Service.impl;

public interface IKubectlService
{
    //调用kubectl执行命令
    public Task<string> Apply(string  yaml);
    public Task<string> Delete(string yaml);
}
