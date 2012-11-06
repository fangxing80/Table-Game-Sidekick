using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVMSidekick.ViewModels;
using TableGameSidekick_Metro.DataEntity;
using MVVMSidekick.Reactive;
using System.Reactive;
using System.Reactive.Linq;
using System.Collections.ObjectModel;
using System.IO;
using Windows.ApplicationModel.Contacts.Provider;
using Windows.Storage;
using Windows.Storage.Streams;

using Windows.ApplicationModel.Contacts;
using MVVMSidekick.Storages;

namespace TableGameSidekick_Metro.ViewModels
{
    public class SelectPlayers_Model : ViewModelBase<SelectPlayers_Model>
    {

        public SelectPlayers_Model()
        {
            m_PlayersStorage = new Storage<List<PlayerInfomation>>();
            m_PlayersStorage.Value = new List<PlayerInfomation>();
            {
                //new  PlayerInfomation { GetFieldNames() }

            };
            this.PresavedImagePaths = new ObservableCollection<string>(Constants.PresavedPics);
        }


        public SelectPlayers_Model(ContactPickerUI contactPickerUI, IStorage<List<PlayerInfomation>> playersStorage, IEnumerable<string> imageFiles)
        {
            m_ContactPickerUI = contactPickerUI;
            m_PlayersStorage = playersStorage;
            this.PresavedImagePaths = new ObservableCollection<string>(imageFiles);
            ConfigModel();


            ConfigCommands();

        }

        private void ConfigCommands()
        {

            //this.NewPlayer.GetValueContainer(x => x.Name)
            //    .GetValueChangeObservable()
            //    .Select(x => x.EventArgs != "")
            //    .Subscribe(CreateNewUserCommand.CommandCore.CanExecuteObserver)
            //    .RegisterDisposeToViewModel(this);
            this.CreateNewUserCommand
                .CommandCore
                .Subscribe
                (
                     (async (e) =>
                     {
                         var vm = this;
                         var newu = vm.NewPlayer;
                         if (!string.IsNullOrEmpty(newu.Name.Trim()))
                         {
                             await RefreshNewUserPicFromResource(
                                    vm.NewPlayerPicResourcePath,
                                    vm);


                             vm.NewPlayer = new PlayerInfomation();
                             vm.SavedPlayers.Add(newu);
                             vm.m_PlayersStorage.Value = vm.SavedPlayers.ToList();
                             await vm.m_PlayersStorage.Save();
                         }
                     }
                     )
                )
                .RegisterDisposeToViewModel(this);


            m_SelectedItemsLocator(this)
                .GetValueChangeObservable()
                .Select(
                    x => x.EventArgs.Count() > 0
                )
                .Subscribe(this.DeleteSelectedSavedPlayerCommand
                                .CommandCore.CanExecuteObserver)
                .RegisterDisposeToViewModel(this);
            DeleteSelectedSavedPlayerCommand.CommandCore
                .Subscribe
                (
                   async x =>
                   {
                       foreach (PlayerInfomation item in SelectedItems.ToArray())
                       {
                           SavedPlayers.Remove(item);
                       }
                       m_PlayersStorage.Value = SavedPlayers.ToList();
                       await m_PlayersStorage.Save();
                   }
                )
                .RegisterDisposeToViewModel(this);

            m_SelectedItemsLocator(this)
                .GetValueChangeEventArgObservable()
                .Subscribe
                (
                   async e =>
                   {
                       var oldv = e.EventArgs.OldValue;
                       if (oldv != null)
                       {
                           foreach (PlayerInfomation item in oldv)
                           {
                               if (m_ContactPickerUI.ContainsContact(item.Name))
                               {
                                   m_ContactPickerUI.RemoveContact(item.Name);
                               }
                           }
                       }


                       var newv = e.EventArgs.NewValue;
                       if (newv != null)
                       {
                           foreach (PlayerInfomation item in newv)
                           {
                               if (!m_ContactPickerUI.ContainsContact(item.Name))
                               {
                                   this.m_ContactPickerUI.AddContact(
                                       item.Name,
                                       new Contact()
                                       {
                                           Name = item.Name,
                                           Thumbnail = item.Image ==
                                                null ?
                                                    null : RandomAccessStreamReference.CreateFromStream(await item.Image.GetStreamAsync())
                                       }
                                   );
                               }

                           }
                       }

                   }
                );


            DeleteItemCommand.CommandCore
                .Subscribe
                (
                    p =>
                    {
                        var pi = p.EventArgs.Parameter as PlayerInfomation;
                        if (pi != null)
                        {
                            SavedPlayers.Remove(pi);
                        }
                    }

                );
        }

        ContactPickerUI m_ContactPickerUI;
        IStorage<List<PlayerInfomation>> m_PlayersStorage;



        async void ConfigModel()
        {

            m_NewPlayerPicResourcePath.LocateValueContainer(this).GetValueChangeObservable()
                //.Throttle(TimeSpan.FromSeconds(1))
             .Subscribe
             (
                 async v =>
                 {
                     await RefreshNewUserPicFromResource(v.EventArgs, this);

                 }

             )
             .RegisterDisposeToViewModel(this);


            await m_PlayersStorage.Refresh();
            this.SavedPlayers.Clear();

            if (m_PlayersStorage.Value !=null)
            {
                foreach (var item in m_PlayersStorage.Value)
                {
                    SavedPlayers.Add(item);
                    await Task.Delay(100);
                }

            }



        }

        private static async Task RefreshNewUserPicFromResource(string source, SelectPlayers_Model model)
        {
            if (source == null)
            {
                return;
            }
            var fl = await StorageFile.GetFileFromApplicationUriAsync(new Uri(source));
            var bi = await fl.GetBasicPropertiesAsync();
            using (var stream = await fl.OpenSequentialReadAsync())
            {
                byte[] bts = new byte[bi.Size];
                await stream.AsStreamForRead().ReadAsync(bts, 0, bts.Length);
                model.NewPlayer = model.NewPlayer ?? new PlayerInfomation();
                model.NewPlayer.Image = new ImageData() { ByteArray = bts };
            }
            //return v;
        }




        public ObservableCollection<PlayerInfomation> SavedPlayers
        {
            get { return m_SavedPlayersLocator(this).Value; }
            set { m_SavedPlayersLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property ObservableCollection<PlayerInfomation> SavedPlayers Setup
        protected Property<ObservableCollection<PlayerInfomation>> m_SavedPlayers = new Property<ObservableCollection<PlayerInfomation>> { LocatorFunc = m_SavedPlayersLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<ObservableCollection<PlayerInfomation>>> m_SavedPlayersLocator =
            RegisterContainerLocator<ObservableCollection<PlayerInfomation>>(
            "SavedPlayers",
            model =>
            {
                model.m_SavedPlayers = model.m_SavedPlayers ?? new Property<ObservableCollection<PlayerInfomation>> { LocatorFunc = m_SavedPlayersLocator };
                return model.m_SavedPlayers.Container =
                    model.m_SavedPlayers.Container
                    ??
                    new ValueContainer<ObservableCollection<PlayerInfomation>>("SavedPlayers", model, new ObservableCollection<PlayerInfomation>());
            }
            );
        #endregion






        public IEnumerable<object> SelectedItems
        {
            get { return m_SelectedItemsLocator(this).Value; }
            set
            {
                m_SelectedItemsLocator(this).SetValueAndTryNotify(value);
            }
        }

        #region Property IEnumerable<object> SelectedItems Setup
        protected Property<IEnumerable<object>> m_SelectedItems =
          new Property<IEnumerable<object>> { LocatorFunc = m_SelectedItemsLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<IEnumerable<object>>> m_SelectedItemsLocator =
            RegisterContainerLocator<IEnumerable<object>>(
            "SelectedItems",
            model =>
            {
                model.m_SelectedItems =
                    model.m_SelectedItems
                    ??
                    new Property<IEnumerable<object>> { LocatorFunc = m_SelectedItemsLocator };
                return model.m_SelectedItems.Container =
                    model.m_SelectedItems.Container
                    ??
                    new ValueContainer<IEnumerable<object>>("SelectedItems", model);
            });
        #endregion






        public ObservableCollection<string> PresavedImagePaths
        {
            get { return m_PresavedImagePathsLocator(this).Value; }
            set { m_PresavedImagePathsLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property ObservableCollection<string> PresavedImagePaths Setup
        protected Property<ObservableCollection<string>> m_PresavedImagePaths = new Property<ObservableCollection<string>> { LocatorFunc = m_PresavedImagePathsLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<ObservableCollection<string>>> m_PresavedImagePathsLocator =
            RegisterContainerLocator<ObservableCollection<string>>(
            "PresavedImagePaths",
            model =>
            {
                model.m_PresavedImagePaths = model.m_PresavedImagePaths ?? new Property<ObservableCollection<string>> { LocatorFunc = m_PresavedImagePathsLocator };
                return model.m_PresavedImagePaths.Container =
                    model.m_PresavedImagePaths.Container
                    ??
                    new ValueContainer<ObservableCollection<string>>("PresavedImagePaths", model);
            }
            );
        #endregion







        public PlayerInfomation NewPlayer
        {
            get { return m_NewPlayerLocator(this).Value; }
            set { m_NewPlayerLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property PlayerInfomation NewPlayer Setup
        protected Property<PlayerInfomation> m_NewPlayer = new Property<PlayerInfomation> { LocatorFunc = m_NewPlayerLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<PlayerInfomation>> m_NewPlayerLocator =
            RegisterContainerLocator<PlayerInfomation>(
            "NewPlayer",
            model =>
            {
                model.m_NewPlayer = model.m_NewPlayer ?? new Property<PlayerInfomation> { LocatorFunc = m_NewPlayerLocator };
                return model.m_NewPlayer.Container =
                    model.m_NewPlayer.Container
                    ??
                    new ValueContainer<PlayerInfomation>("NewPlayer", model, new PlayerInfomation());
            }
            );
        #endregion






        public string NewPlayerPicResourcePath
        {
            get { return m_NewPlayerPicResourcePathLocator(this).Value; }
            set { m_NewPlayerPicResourcePathLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property string NewPlayerPicResourcePath Setup
        protected Property<string> m_NewPlayerPicResourcePath = new Property<string> { LocatorFunc = m_NewPlayerPicResourcePathLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<string>> m_NewPlayerPicResourcePathLocator =
            RegisterContainerLocator<string>(
            "NewPlayerPicResourcePath",
            model =>
            {
                model.m_NewPlayerPicResourcePath = model.m_NewPlayerPicResourcePath ?? new Property<string> { LocatorFunc = m_NewPlayerPicResourcePathLocator };
                return model.m_NewPlayerPicResourcePath.Container =
                    model.m_NewPlayerPicResourcePath.Container
                    ??
                    new ValueContainer<string>("NewPlayerPicResourcePath", model);
            }
            );
        #endregion












        public CommandModel<ReactiveCommand, String> CreateNewUserCommand
        {
            get { return m_CreateNewUserCommand.WithViewModel(this); }
            protected set { m_CreateNewUserCommand = value; }
        }
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        CommandModel<ReactiveCommand, String> m_CreateNewUserCommand
       = new ReactiveCommand(true).CreateCommandModel("AddNew");






        public CommandModel<ReactiveCommand, String> DeleteItemCommand
        {
            get { return m_DeleteItemCommand.WithViewModel(this); }
            protected set { m_DeleteItemCommand = value; }
        }

        #region DeleteItemCommand Configuration
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        CommandModel<ReactiveCommand, String> m_DeleteItemCommand
            = new ReactiveCommand(canExecute: true).CreateCommandModel(default(String));
        #endregion






        public CommandModel<ReactiveCommand, String> DeleteSelectedSavedPlayerCommand
        {
            get { return m_DeleteSelectedSavedPlayerCommand.WithViewModel(this); }
            protected set { m_DeleteSelectedSavedPlayerCommand = value; }
        }

        #region DeleteSelectedSavedPlayerCommand Configuration
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        CommandModel<ReactiveCommand, String> m_DeleteSelectedSavedPlayerCommand
            = new ReactiveCommand(canExecute: false).CreateCommandModel("DeleteSelectedPlayer");

        #endregion





    }
}
