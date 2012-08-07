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
using TableGameSidekick_Metro.Storages;

namespace TableGameSidekick_Metro.ViewModels
{
    public class SelectPlayers_Model : ViewModelBase<SelectPlayers_Model>
    {

        public SelectPlayers_Model()
        {
            m_PlayersStorage = new Storage<IEnumerable<PlayerInfomation>>();
            m_PlayersStorage.Value = new PlayerInfomation[]
            {
                //new  PlayerInfomation { GetFieldNames() }
            
            };
            this.PresavedImagePaths = new ObservableCollection<string>(App.PresavedPics);
        }


        public SelectPlayers_Model(ContactPickerUI contactPickerUI, IStorage<IEnumerable<PlayerInfomation>> playersStorage, IEnumerable<string> imageFiles)
        {
            m_ContactPickerUI = contactPickerUI;
            m_PlayersStorage = playersStorage;
            this.PresavedImagePaths = new ObservableCollection<string>(imageFiles);
            ConfigModel();

        }

        ContactPickerUI m_ContactPickerUI;
        IStorage<IEnumerable<PlayerInfomation>> m_PlayersStorage;



        async void ConfigModel()
        {

            m_NewPlayerPicResourcePath.Locate(this).GetValueChangeObservable()
                //.Throttle(TimeSpan.FromSeconds(1))
             .Subscribe
             (
                 async v =>
                 {
                     await RefreshNewUserPicFromResource(v.EventArgs, this);

                 }

             )
             .RegisterDispose(this);


            await m_PlayersStorage.Refresh();
            this.SavedPlayers.Clear();

            foreach (var item in m_PlayersStorage.Value)
            {
                SavedPlayers.Add(item);
                await Task.Delay(100);
            }

            this.CreateNewUserCommand
                .CommandCore
                .Subscribe
                (
                     (async (e) =>
                     {
                         var vm = this;
                         await RefreshNewUserPicFromResource(
                               vm.NewPlayerPicResourcePath,
                               vm);

                         var newu = vm.NewPlayer;
                         vm.NewPlayer = new PlayerInfomation();
                         vm.SavedPlayers.Add(newu);
                         vm.m_PlayersStorage.Value = vm.SavedPlayers.ToArray();
                         await vm.m_PlayersStorage.Save();

                     }
                     )
                )
                .RegisterDispose(this);


            m_SelectedSavedPlayerIndexLocator(this)
                .GetValueChangeObservable()
                .Select(
                    x => x.EventArgs != -1
                )
                .Subscribe(this.DeleteSelectedSavedPlayerCommand
                                .CommandCore.CanExecuteObserver)
                .RegisterDispose(this);
            DeleteSelectedSavedPlayerCommand.CommandCore
                .Subscribe
                (
                   async x =>
                   {
                       SavedPlayers.RemoveAt(SelectedSavedPlayerIndex);
                       m_PlayersStorage.Value = SavedPlayers.ToArray();
                       await m_PlayersStorage.Save();
                   }
                )
                .RegisterDispose(this);


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
        static Func<ViewModelBase, ValueContainer<ObservableCollection<PlayerInfomation>>> m_SavedPlayersLocator =
            RegisterContainerLocator<ObservableCollection<PlayerInfomation>>(
                "SavedPlayers",
                model =>
                {
                    model.m_SavedPlayers = model.m_SavedPlayers ?? new Property<ObservableCollection<PlayerInfomation>> { LocatorFunc = m_SavedPlayersLocator };
                    return model.m_SavedPlayers.Container =
                        model.m_SavedPlayers.Container
                        ??
                        new ValueContainer<ObservableCollection<PlayerInfomation>>("SavedPlayers", new ObservableCollection<PlayerInfomation>(), model);
                }
            );
        #endregion



        public int SelectedSavedPlayerIndex
        {
            get {
                return m_SelectedSavedPlayerIndexLocator(this).Value; }
            set {
                m_SelectedSavedPlayerIndexLocator(this).SetValueAndTryNotify(value); }
        }

        #region Property int SelectedSavedPlayerIndex Setup
        protected Property<int> m_SelectedSavedPlayerIndex =
          new Property<int> { LocatorFunc = m_SelectedSavedPlayerIndexLocator };
        static Func<ViewModelBase, ValueContainer<int>> m_SelectedSavedPlayerIndexLocator =
            RegisterContainerLocator<int>(
                "SelectedSavedPlayerIndex",
                model =>
                {
                    model.m_SelectedSavedPlayerIndex =
                        model.m_SelectedSavedPlayerIndex
                        ??
                        new Property<int> { LocatorFunc = m_SelectedSavedPlayerIndexLocator };
                    return model.m_SelectedSavedPlayerIndex.Container =
                        model.m_SelectedSavedPlayerIndex.Container
                        ??
                        new ValueContainer<int>("SelectedSavedPlayerIndex",-1, model);
                });
        #endregion




        public ObservableCollection<string> PresavedImagePaths
        {
            get { return m_PresavedImagePathsLocator(this).Value; }
            set { m_PresavedImagePathsLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property ObservableCollection<string> PresavedImagePaths Setup
        protected Property<ObservableCollection<string>> m_PresavedImagePaths = new Property<ObservableCollection<string>> { LocatorFunc = m_PresavedImagePathsLocator };
        static Func<ViewModelBase, ValueContainer<ObservableCollection<string>>> m_PresavedImagePathsLocator =
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
        static Func<ViewModelBase, ValueContainer<PlayerInfomation>> m_NewPlayerLocator =
            RegisterContainerLocator<PlayerInfomation>(
                "NewPlayer",
                model =>
                {
                    model.m_NewPlayer = model.m_NewPlayer ?? new Property<PlayerInfomation> { LocatorFunc = m_NewPlayerLocator };
                    return model.m_NewPlayer.Container =
                        model.m_NewPlayer.Container
                        ??
                        new ValueContainer<PlayerInfomation>("NewPlayer", new PlayerInfomation(), model);
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
        static Func<ViewModelBase, ValueContainer<string>> m_NewPlayerPicResourcePathLocator =
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

        CommandModel<ReactiveCommand, String> m_CreateNewUserCommand
       = new ReactiveCommand(true).CreateCommandModel("AddNew");










        public CommandModel<ReactiveCommand, String> DeleteSelectedSavedPlayerCommand
        {
            get { return m_DeleteSelectedSavedPlayerCommand.WithViewModel(this); }
            protected set { m_DeleteSelectedSavedPlayerCommand = value; }
        }

        #region DeleteSelectedSavedPlayerCommand Configuration
        CommandModel<ReactiveCommand, String> m_DeleteSelectedSavedPlayerCommand
            = new ReactiveCommand(canExecute: false).CreateCommandModel("DeleteSelectedPlayer");

        #endregion





    }
}
