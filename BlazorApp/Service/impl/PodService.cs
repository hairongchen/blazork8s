using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using k8s;
using k8s.Models;

namespace BlazorApp.Service.impl
{
    public class PodService : IPodService
    {
        private readonly IBaseService _baseService;
        private          bool         _podListChangedByWatch;
        private readonly List<V1Pod>  _sharedPods = new();

        public PodService(IBaseService baseService)
        {
            _baseService = baseService;
            // Console.WriteLine("PodService 初始化");
        }


        public async Task<IList<V1Pod>> ListByOwnerUid(string controllerByUid)
        {
            var list = await ListPods();
            return list.Where(x => x.GetController() != null && x.GetController().Uid == controllerByUid)
                .ToList();
        }


        public void UpdateSharePods(WatchEventType type, V1Pod item)
        {
            var index = _sharedPods.FindIndex(0, r => r.Uid() == item.Uid());
            // Console.WriteLine($"{index}:{item.Name()}");
            switch (type)
            {
                case WatchEventType.Added:
                    if (index == -1)
                    {
                        //不存在
                        _sharedPods.Insert(0, item);
                        _podListChangedByWatch = true;
                    }

                    break;
                case WatchEventType.Modified:
                    if (index > -1)
                    {
                        //已存在
                        _sharedPods[index]     = item;
                        _podListChangedByWatch = true;
                    }

                    break;
                case WatchEventType.Deleted:
                    if (index > -1)
                    {
                        //已存在
                        _sharedPods.RemoveAt(index);
                        _podListChangedByWatch = true;
                    }

                    break;
                case WatchEventType.Error:
                    break;
                case WatchEventType.Bookmark:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }


        public async Task<IList<V1Pod>> ListPods()
        {
            if (_sharedPods.Count == 0)
            {
                var pods = await _baseService.Client().ListPodForAllNamespacesAsync();
                foreach (var item in pods.Items)
                {
                    _sharedPods.Add(item);
                }
            }

            _podListChangedByWatch = false;
            return _sharedPods;
        }

        private async Task<IList<V1Pod>> ListPodByNamespace(string ns)
        {
            if (string.IsNullOrEmpty(ns))
            {
                return await ListPods();
            }

            return _sharedPods.Where(x => x.Namespace() == ns).ToList();
        }


        //
        public async Task<bool> DeletePod(string ns, string name)
        {
            // Console.WriteLine($"DeletePod,{ns},{name}");
            return await _baseService.Client().DeleteNamespacedPodAsync(name, ns) != null;
        }

        public async Task Logs(V1Pod pod, bool follow = false, bool previous = false)
        {
            var response = await _baseService.Client().CoreV1.ReadNamespacedPodLogWithHttpMessagesAsync(
                pod.Metadata.Name,
                pod.Metadata.NamespaceProperty,
                container: pod.Spec.Containers[0].Name,
                tailLines: 1,
                previous: previous,
                follow: follow).ConfigureAwait(false);
            var stream = response.Body;
            await stream.CopyToAsync(Console.OpenStandardOutput());
        }

        public async Task<IList<V1Pod>> ListItemsByNamespaceAsync(string ns)
        {
            return await ListPodByNamespace(ns);
        }

        public async Task<int> NodePodsNum()
        {
            var pods = await ListPods();
            var tuples = pods.GroupBy(s => s.Spec.NodeName)
                .OrderBy(g => g.Key)
                .Select(g => Tuple.Create(g.Key, g.Count()));
            foreach (var tuple in tuples)
            {
                Console.WriteLine($"{tuple.Item1}={tuple.Item2}");
            }

            return await Task.FromResult(0);
        }

        public async Task WatchAllPod()
        {
            var podlistResp = _baseService.Client().CoreV1
                .ListPodForAllNamespacesWithHttpMessagesAsync(watch: true);
            await foreach (var (type, item) in podlistResp.WatchAsync<V1Pod, V1PodList>())
            {
                // Console.WriteLine("==on watch event start ==");
                Console.WriteLine($"{type}:{item.Metadata.Name}");
                UpdateSharePods(type, item);
                // Console.WriteLine("==on watch event end ==");
            }
        }

        public bool PodListChangedByWatch()
        {
            return _podListChangedByWatch;
        }
    }
}
