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
            _PlayersStorage = new Storage<List<PlayerInfomation>>();
            _PlayersStorage.Value = new List<PlayerInfomation>();
            {
            };
            this.PresavedImagePaths = new ObservableCollection<string>(Constants.PresavedPics);
        }


        public SelectPlayers_Model(ContactPickerUI contactPickerUI, IStorage<List<PlayerInfomation>> playersStorage, IEnumerable<string> imageFiles)
        {
            _ContactPickerUI = contactPickerUI;
            _PlayersStorage = playersStorage;
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
                             vm._PlayersStorage.Value = vm.SavedPlayers.ToList();
                             await vm._PlayersStorage.Save();
                         }
                     }
                     )
                )
                .DisposeWith(this);


            _SelectedItemsLocator(this)
                .GetValueChangeObservable()
                .Select(
                    x => x.EventArgs.Count() > 0
                )
                .Subscribe(this.DeleteSelectedSavedPlayerCommand
                                .CommandCore.CanExecuteObserver)
                .DisposeWith(this);
            DeleteSelectedSavedPlayerCommand.CommandCore
                .Subscribe
                (
                   async x =>
                   {
                       foreach (PlayerInfomation item in SelectedItems.ToArray())
                       {
                           SavedPlayers.Remove(item);
                       }
                       _PlayersStorage.Value = SavedPlayers.ToList();
                       await _PlayersStorage.Save();
                   }
                )
                .DisposeWith(this);

            _SelectedItemsLocator(this)
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
                               if (_ContactPickerUI.ContainsContact(item.Name))
                               {
                                   _ContactPickerUI.RemoveContact(item.Name);
                               }
                           }
                       }


                       var newv = e.EventArgs.NewValue;
                       if (newv != null)
                       {
                           foreach (PlayerInfomation item in newv)
                           {
                               if (!_ContactPickerUI.ContainsContact(item.Name))
                               {
                                   this._ContactPickerUI.AddContact(
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

        ContactPickerUI _ContactPickerUI;
        IStorage<List<PlayerInfomation>> _PlayersStorage;



        async void ConfigModel()
        {

            _NewPlayerPicResourcePath.LocateValueContainer(this).GetValueChangeObservable()
                //.Throttle(TimeSpan.FromSeconds(1))
             .Subscribe
             (
                 async v =>
                 {
                     await RefreshNewUserPicFromResource(v.EventArgs, this);

                 }

             )
             .DisposeWith(this);


            await _PlayersStorage.Refresh();
            this.SavedPlayers.Clear();

            if (_PlayersStorage.Value != null)
            {
                foreach (var item in _PlayersStorage.Value)
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
            get { return _SavedPlayersLocator(this).Value; }
            set { _SavedPlayersLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property ObservableCollection<PlayerInfomation> SavedPlayers Setup
        protected Property<ObservableCollection<PlayerInfomation>> _SavedPlayers = new Property<ObservableCollection<PlayerInfomation>> { LocatorFunc = _SavedPlayersLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<ObservableCollection<PlayerInfomation>>> _SavedPlayersLocator =
            RegisterContainerLocator<ObservableCollection<PlayerInfomation>>(
            "SavedPlayers",
            model =>
            {
                model._SavedPlayers = model._SavedPlayers ?? new Property<ObservableCollection<PlayerInfomation>> { LocatorFunc = _SavedPlayersLocator };
                return model._SavedPlayers.Container =
                    model._SavedPlayers.Container
                    ??
                    new ValueContainer<ObservableCollection<PlayerInfomation>>("SavedPlayers", model, new ObservableCollection<PlayerInfomation>());
            }
            );
        #endregion






        public IEnumerable<object> SelectedItems
        {
            get { return _SelectedItemsLocator(this).Value; }
            set
            {
                _SelectedItemsLocator(this).SetValueAndTryNotify(value);
            }
        }

        #region Property IEnumerable<object> SelectedItems Setup
        protected Property<IEnumerable<object>> _SelectedItems =
          new Property<IEnumerable<object>> { LocatorFunc = _SelectedItemsLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<IEnumerable<object>>> _SelectedItemsLocator =
            RegisterContainerLocator<IEnumerable<object>>(
            "SelectedItems",
            model =>
            {
                model._SelectedItems =
                    model._SelectedItems
                    ??
                    new Property<IEnumerable<object>> { LocatorFunc = _SelectedItemsLocator };
                return model._SelectedItems.Container =
                    model._SelectedItems.Container
                    ??
                    new ValueContainer<IEnumerable<object>>("SelectedItems", model);
            });
        #endregion






        public ObservableCollection<string> PresavedImagePaths
        {
            get { return _PresavedImagePathsLocator(this).Value; }
            set { _PresavedImagePathsLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property ObservableCollection<string> PresavedImagePaths Setup
        protected Property<ObservableCollection<string>> _PresavedImagePaths = new Property<ObservableCollection<string>> { LocatorFunc = _PresavedImagePathsLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<ObservableCollection<string>>> _PresavedImagePathsLocator =
            RegisterContainerLocator<ObservableCollection<string>>(
            "PresavedImagePaths",
            model =>
            {
                model._PresavedImagePaths = model._PresavedImagePaths ?? new Property<ObservableCollection<string>> { LocatorFunc = _PresavedImagePathsLocator };
                return model._PresavedImagePaths.Container =
                    model._PresavedImagePaths.Container
                    ??
                    new ValueContainer<ObservableCollection<string>>("PresavedImagePaths", model);
            }
            );
        #endregion







        public PlayerInfomation NewPlayer
        {
            get { return _NewPlayerLocator(this).Value; }
            set { _NewPlayerLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property PlayerInfomation NewPlayer Setup
        protected Property<PlayerInfomation> _NewPlayer = new Property<PlayerInfomation> { LocatorFunc = _NewPlayerLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<PlayerInfomation>> _NewPlayerLocator =
            RegisterContainerLocator<PlayerInfomation>(
            "NewPlayer",
            model =>
            {
                model._NewPlayer = model._NewPlayer ?? new Property<PlayerInfomation> { LocatorFunc = _NewPlayerLocator };
                return model._NewPlayer.Container =
                    model._NewPlayer.Container
                    ??
                    new ValueContainer<PlayerInfomation>("NewPlayer", model, new PlayerInfomation());
            }
            );
        #endregion






        public string NewPlayerPicResourcePath
        {
            get { return _NewPlayerPicResourcePathLocator(this).Value; }
            set { _NewPlayerPicResourcePathLocator(this).SetValueAndTryNotify(value); }
        }
        #region Property string NewPlayerPicResourcePath Setup
        protected Property<string> _NewPlayerPicResourcePath = new Property<string> { LocatorFunc = _NewPlayerPicResourcePathLocator };
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static Func<BindableBase, ValueContainer<string>> _NewPlayerPicResourcePathLocator =
            RegisterContainerLocator<string>(
            "NewPlayerPicResourcePath",
            model =>
            {
                model._NewPlayerPicResourcePath = model._NewPlayerPicResourcePath ?? new Property<string> { LocatorFunc = _NewPlayerPicResourcePathLocator };
                return model._NewPlayerPicResourcePath.Container =
                    model._NewPlayerPicResourcePath.Container
                    ??
                    new ValueContainer<string>("NewPlayerPicResourcePath", model);
            }
            );
        #endregion












        public CommandModel<ReactiveCommand, String> CreateNewUserCommand
        {
            get { return _CreateNewUserCommand.WithViewModel(this); }
            protected set { _CreateNewUserCommand = value; }
        }
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        CommandModel<ReactiveCommand, String> _CreateNewUserCommand
       = new ReactiveCommand(true).CreateCommandModel("AddNew");






        public CommandModel<ReactiveCommand, String> DeleteItemCommand
        {
            get { return _DeleteItemCommand.WithViewModel(this); }
            protected set { _DeleteItemCommand = value; }
        }

        #region DeleteItemCommand Configuration
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        CommandModel<ReactiveCommand, String> _DeleteItemCommand
            = new ReactiveCommand(canExecute: true).CreateCommandModel(default(String));
        #endregion






        public CommandModel<ReactiveCommand, String> DeleteSelectedSavedPlayerCommand
        {
            get { return _DeleteSelectedSavedPlayerCommand.WithViewModel(this); }
            protected set { _DeleteSelectedSavedPlayerCommand = value; }
        }

        #region DeleteSelectedSavedPlayerCommand Configuration
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        CommandModel<ReactiveCommand, String> _DeleteSelectedSavedPlayerCommand
            = new ReactiveCommand(canExecute: false).CreateCommandModel("DeleteSelectedPlayer");

        #endregion





    }
}
