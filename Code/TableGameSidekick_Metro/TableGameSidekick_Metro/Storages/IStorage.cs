using System;
namespace TableGameSidekick_Metro.Storages
{
    public interface IStorage<T>
    {
        global::Windows.Storage.StorageFolder Folder { get; set; }
        System.Threading.Tasks.Task Refresh();
        System.Threading.Tasks.Task Save();
        T Value { get; set; }
    }
}
