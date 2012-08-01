using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVM.ViewModels;
using TableGameSidekick_Metro.DataEntity;
using MVVM.Reactive;
using System.Reactive;
using System.Reactive.Linq;
using System.Collections.ObjectModel;
using System.IO;
using Windows.ApplicationModel.Contacts.Provider;
using Windows.Storage;
using Windows.Storage.Streams;
using TableGameSidekick_Metro.Storages;

namespace TableGameSidekick_Metro.ViewModels
{
    public class SelectPlayers_Model : ViewModelBase<SelectPlayers_Model>
    {

        public SelectPlayers_Model(ContactPickerUI contactPickerUI, IStorage<IEnumerable<PlayerInfomation>> players, IEnumerable<StorageFile> imageFiles)
        {
            m_ContactPickerUI = contactPickerUI;
        }
        ContactPickerUI m_ContactPickerUI;



    }
}
