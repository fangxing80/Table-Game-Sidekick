using MVVMSidekick.ViewModels;
using MVVMSidekick.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MVVMSidekick
{
    namespace ViewModels
    {
        /// <summary>
        /// 用于控制Frame 浏览的控制器
        /// </summary>
        public interface IFrameNavigator
        {
            /// <summary>
            /// 用async 工作流的方式浏览一个View
            /// </summary>
            /// <param name="targetViewName">View名</param>
            /// <param name="parameters">参数</param>
            /// <returns>返回Task</returns>
            Task FrameNavigate(string targetViewName, System.Collections.Generic.Dictionary<string, object> parameters = null);

            /// <summary>
            /// 用async 工作流的方式浏览一个View 并且返回结果
            /// </summary>
            /// <param name="targetViewName">View名</param>
            /// <param name="parameters">参数</param>
            /// <returns>返回结果</returns>
            Task<TResult> FrameNavigate<TResult>(string targetViewName, System.Collections.Generic.Dictionary<string, object> parameters = null);

            //Task FrameNavigate(string targetViewName, out object page, System.Collections.Generic.Dictionary<string, object> parameters = null);
            //Task<TResult> FrameNavigate<TResult>(string targetViewName, out object page, System.Collections.Generic.Dictionary<string, object> parameters = null);
        }

        public partial interface IViewModelBase
        {
            IFrameNavigator Navigator { get; set; }
        
        }
        public abstract partial class ViewModelBase<TViewModel>
        {
            public IFrameNavigator Navigator { get; set; }
        }
    }

    namespace Views
    {
        public static class NavigateParameterKeys
        {
            public static readonly string ViewInitActionName = "InitAction";
            public static readonly string GameInfomation_ChosenGame = "GameInfomation_ChosenGame";
            public static readonly string NavigateToCallback = "NavigateToCallback";
            public static readonly string FinishedCallback = "FinishedCallback";

        }

        /// <summary>
        /// Typical implementation of Page that provides several important conveniences:
        /// <list type="bullet">
        /// <item>
        /// <description>Application view state to visual state mapping</description>
        /// </item>
        /// <item>
        /// <description>GoBack, GoForward, and GoHome event handlers</description>
        /// </item>
        /// <item>
        /// <description>Mouse and keyboard shortcuts for navigation</description>
        /// </item>
        /// <item>
        /// <description>State management for navigation and process lifetime management</description>
        /// </item>
        /// <item>
        /// <description>A default view model</description>
        /// </item>
        /// </list>
        /// </summary>
        [Windows.Foundation.Metadata.WebHostHidden]
        public class LayoutAwarePage : Page
        {
            /// <summary>
            /// Identifies the <see cref="DefaultViewModel"/> dependency property.
            /// </summary>
            public static readonly DependencyProperty DefaultViewModelProperty =
                DependencyProperty.Register("DefaultViewModel", typeof(IViewModelBase),
                typeof(LayoutAwarePage), new PropertyMetadata(new DefaultViewModel()));

            private List<Control> _layoutAwareControls;

            public static Dictionary<string, Action<LayoutAwarePage, IDictionary<string, object>>>
                PageInitActions = new Dictionary<string, Action<LayoutAwarePage, IDictionary<string, object>>>();
            /// <summary>
            /// Initializes a new instance of the <see cref="LayoutAwarePage"/> class.
            /// </summary>
            public LayoutAwarePage()
            {
                if (Windows.ApplicationModel.DesignMode.DesignModeEnabled) return;

                // Create an empty default view model
                //this.DefaultViewModel = new ObservableDictionary<String, Object>();

                // When this page is part of the visual tree make two changes:
                // 1) Map application view state to visual state for the page
                // 2) Handle keyboard and mouse navigation requests
                this.Loaded += (sender, e) =>
                {
                    this.StartLayoutUpdates(sender, e);

                    // Keyboard and mouse navigation only apply when occupying the entire window
                    if (this.ActualHeight == Window.Current.Bounds.Height &&
                        this.ActualWidth == Window.Current.Bounds.Width)
                    {
                        // Listen to the window directly so focus isn't required
                        Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated +=
                            CoreDispatcher_AcceleratorKeyActivated;
                        Window.Current.CoreWindow.PointerPressed +=
                            this.CoreWindow_PointerPressed;
                    }
                };

                // Undo the same changes when the page is no longer visible
                this.Unloaded += (sender, e) =>
                {
                    this.StopLayoutUpdates(sender, e);
                    Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated -=
                        CoreDispatcher_AcceleratorKeyActivated;
                    Window.Current.CoreWindow.PointerPressed -=
                        this.CoreWindow_PointerPressed;
                };

                //var bEnable = new Binding()
                //{
                //    Source = this,
                //    Path = new PropertyPath("DefaultViewModel.IsUIBusy"),
                //    Mode = BindingMode.OneWay,
                //    Converter = BooleanNotConverter.Instance,                
                //};

                //this.SetBinding(IsEnabledProperty, bEnable);

            }



            public IViewModelBase DefaultViewModel
            {
                get
                {
                    return this.GetValue(DefaultViewModelProperty) as IViewModelBase;
                }

                set
                {
                    DisposeViewModel();
                    this.SetValue(DefaultViewModelProperty, value);
                }
            }

            protected void DisposeViewModel()
            {
                var oldv = this.GetValue(DefaultViewModelProperty) as BindableBase;
                if (oldv != null)
                {
                    try
                    {
                        oldv.Dispose();
                    }
                    catch (Exception)
                    {


                    }
                }
            }

            #region Navigation support

            /// <summary>
            /// Invoked as an event handler to navigate backward in the page's associated
            /// <see cref="Frame"/> until it reaches the top of the navigation stack.
            /// </summary>
            /// <param name="sender">Instance that triggered the event.</param>
            /// <param name="e">Event data describing the conditions that led to the event.</param>
            protected virtual void GoHome(object sender, RoutedEventArgs e)
            {
                // Use the navigation frame to return to the topmost page
                if (this.Frame != null)
                {
                    while (this.Frame.CanGoBack) this.Frame.GoBack();
                }
            }

            /// <summary>
            /// Invoked as an event handler to navigate backward in the navigation stack
            /// associated with this page's <see cref="Frame"/>.
            /// </summary>
            /// <param name="sender">Instance that triggered the event.</param>
            /// <param name="e">Event data describing the conditions that led to the
            /// event.</param>
            protected virtual void GoBack(object sender, RoutedEventArgs e)
            {
                DefaultViewModel.Close();



            }

            /// <summary>
            /// Invoked as an event handler to navigate forward in the navigation stack
            /// associated with this page's <see cref="Frame"/>.
            /// </summary>
            /// <param name="sender">Instance that triggered the event.</param>
            /// <param name="e">Event data describing the conditions that led to the
            /// event.</param>
            protected virtual void GoForward(object sender, RoutedEventArgs e)
            {
                //// Use the navigation frame to move to the next page
                //if (this.Frame != null && this.Frame.CanGoForward) this.Frame.GoForward();
            }

            /// <summary>
            /// Invoked on every keystroke, including system keys such as Alt key combinations, when
            /// this page is active and occupies the entire window.  Used to detect keyboard navigation
            /// between pages even when the page itself doesn't have focus.
            /// </summary>
            /// <param name="sender">Instance that triggered the event.</param>
            /// <param name="args">Event data describing the conditions that led to the event.</param>
            private void CoreDispatcher_AcceleratorKeyActivated(CoreDispatcher sender,
                AcceleratorKeyEventArgs args)
            {
                var virtualKey = args.VirtualKey;

                // Only investigate further when Left, Right, or the dedicated Previous or Next keys
                // are pressed
                if ((args.EventType == CoreAcceleratorKeyEventType.SystemKeyDown ||
                    args.EventType == CoreAcceleratorKeyEventType.KeyDown) &&
                    (virtualKey == VirtualKey.Left || virtualKey == VirtualKey.Right ||
                    (int)virtualKey == 166 || (int)virtualKey == 167))
                {
                    var coreWindow = Window.Current.CoreWindow;
                    var downState = CoreVirtualKeyStates.Down;
                    bool menuKey = (coreWindow.GetKeyState(VirtualKey.Menu) & downState) == downState;
                    bool controlKey = (coreWindow.GetKeyState(VirtualKey.Control) & downState) == downState;
                    bool shiftKey = (coreWindow.GetKeyState(VirtualKey.Shift) & downState) == downState;
                    bool noModifiers = !menuKey && !controlKey && !shiftKey;
                    bool onlyAlt = menuKey && !controlKey && !shiftKey;

                    if (((int)virtualKey == 166 && noModifiers) ||
                        (virtualKey == VirtualKey.Left && onlyAlt))
                    {
                        // When the previous key or Alt+Left are pressed navigate back
                        args.Handled = true;
                        this.GoBack(this, new RoutedEventArgs());
                    }
                    else if (((int)virtualKey == 167 && noModifiers) ||
                        (virtualKey == VirtualKey.Right && onlyAlt))
                    {
                        // When the next key or Alt+Right are pressed navigate forward
                        args.Handled = true;
                        this.GoForward(this, new RoutedEventArgs());
                    }
                }
            }

            /// <summary>
            /// Invoked on every mouse click, touch screen tap, or equivalent interaction when this
            /// page is active and occupies the entire window.  Used to detect browser-style next and
            /// previous mouse button clicks to navigate between pages.
            /// </summary>
            /// <param name="sender">Instance that triggered the event.</param>
            /// <param name="args">Event data describing the conditions that led to the event.</param>
            private void CoreWindow_PointerPressed(CoreWindow sender,
                PointerEventArgs args)
            {
                var properties = args.CurrentPoint.Properties;

                // Ignore button chords with the left, right, and middle buttons
                if (properties.IsLeftButtonPressed || properties.IsRightButtonPressed ||
                    properties.IsMiddleButtonPressed) return;

                // If back or foward are pressed (but not both) navigate appropriately
                bool backPressed = properties.IsXButton1Pressed;
                bool forwardPressed = properties.IsXButton2Pressed;
                if (backPressed ^ forwardPressed)
                {
                    args.Handled = true;
                    if (backPressed) this.GoBack(this, new RoutedEventArgs());
                    if (forwardPressed) this.GoForward(this, new RoutedEventArgs());
                }
            }

            #endregion

            #region Visual state switching

            /// <summary>
            /// Invoked as an event handler, typically on the <see cref="FrameworkElement.Loaded"/>
            /// event of a <see cref="Control"/> within the page, to indicate that the sender should
            /// start receiving visual state management changes that correspond to application view
            /// state changes.
            /// </summary>
            /// <param name="sender">Instance of <see cref="Control"/> that supports visual state
            /// management corresponding to view states.</param>
            /// <param name="e">Event data that describes how the request was made.</param>
            /// <remarks>The current view state will immediately be used to set the corresponding
            /// visual state when layout updates are requested.  A corresponding
            /// <see cref="FrameworkElement.Unloaded"/> event handler connected to
            /// <see cref="StopLayoutUpdates"/> is strongly encouraged.  Instances of
            /// <see cref="LayoutAwarePage"/> automatically invoke these handlers in their Loaded and
            /// Unloaded events.</remarks>
            /// <seealso cref="DetermineVisualState"/>
            /// <seealso cref="InvalidateVisualState"/>
            public void StartLayoutUpdates(object sender, RoutedEventArgs e)
            {
                var control = sender as Control;
                if (control == null) return;
                if (this._layoutAwareControls == null)
                {
                    // Start listening to view state changes when there are controls interested in updates
                    Window.Current.SizeChanged += this.WindowSizeChanged;
                    this._layoutAwareControls = new List<Control>();
                }
                this._layoutAwareControls.Add(control);

                // Set the initial visual state of the control
                VisualStateManager.GoToState(control, DetermineVisualState(ApplicationView.Value), false);
            }

            private void WindowSizeChanged(object sender, WindowSizeChangedEventArgs e)
            {
                this.InvalidateVisualState();
            }

            /// <summary>
            /// Invoked as an event handler, typically on the <see cref="FrameworkElement.Unloaded"/>
            /// event of a <see cref="Control"/>, to indicate that the sender should start receiving
            /// visual state management changes that correspond to application view state changes.
            /// </summary>
            /// <param name="sender">Instance of <see cref="Control"/> that supports visual state
            /// management corresponding to view states.</param>
            /// <param name="e">Event data that describes how the request was made.</param>
            /// <remarks>The current view state will immediately be used to set the corresponding
            /// visual state when layout updates are requested.</remarks>
            /// <seealso cref="StartLayoutUpdates"/>
            public void StopLayoutUpdates(object sender, RoutedEventArgs e)
            {
                var control = sender as Control;
                if (control == null || this._layoutAwareControls == null) return;
                this._layoutAwareControls.Remove(control);
                if (this._layoutAwareControls.Count == 0)
                {
                    // Stop listening to view state changes when no controls are interested in updates
                    this._layoutAwareControls = null;
                    Window.Current.SizeChanged -= this.WindowSizeChanged;
                }
            }

            /// <summary>
            /// Translates <see cref="ApplicationViewState"/> values into strings for visual state
            /// management within the page.  The default implementation uses the names of enum values.
            /// Subclasses may override this method to control the mapping scheme used.
            /// </summary>
            /// <param name="viewState">View state for which a visual state is desired.</param>
            /// <returns>Visual state name used to drive the
            /// <see cref="VisualStateManager"/></returns>
            /// <seealso cref="InvalidateVisualState"/>
            protected virtual string DetermineVisualState(ApplicationViewState viewState)
            {
                return viewState.ToString();
            }

            /// <summary>
            /// Updates all controls that are listening for visual state changes with the correct
            /// visual state.
            /// </summary>
            /// <remarks>
            /// Typically used in conjunction with overriding <see cref="DetermineVisualState"/> to
            /// signal that a different value may be returned even though the view state has not
            /// changed.
            /// </remarks>
            public void InvalidateVisualState()
            {
                if (this._layoutAwareControls != null)
                {
                    string visualState = DetermineVisualState(ApplicationView.Value);
                    foreach (var layoutAwareControl in this._layoutAwareControls)
                    {
                        VisualStateManager.GoToState(layoutAwareControl, visualState, false);
                    }
                }
            }

            #endregion

            #region Process lifetime management

            private String _pageKey;

            /// <summary>
            /// Invoked when this page is about to be displayed in a Frame.
            /// </summary>
            /// <param name="e">Event data that describes how this page was reached.  The Parameter
            /// property provides the group to be displayed.</param>
            protected override void OnNavigatedTo(NavigationEventArgs e)
            {
                base.OnNavigatedTo(e);
                // Returning to a cached page through navigation shouldn't trigger state loading



                if (this._pageKey != null) return;

                var frameState = SuspensionManager.SessionStateForFrame(this.Frame);
                this._pageKey = "Page-" + this.Frame.BackStackDepth;
                var dic = e.Parameter as Dictionary<string, object>;
                if (e.NavigationMode == NavigationMode.New)
                {
                    // Clear existing state for forward navigation when adding a new page to the
                    // navigation stack
                    var nextPageKey = this._pageKey;
                    int nextPageIndex = this.Frame.BackStackDepth;
                    while (frameState.Remove(nextPageKey))
                    {
                        nextPageIndex++;
                        nextPageKey = "Page-" + nextPageIndex;
                    }






                    // Pass the navigation parameter to the new page
                    this.LoadState(e.Parameter, dic);
                }
                else
                {
                    // Pass the navigation parameter and preserved page state to the page, using
                    // the same strategy for loading suspended state and recreating pages discarded
                    // from cache
                    this.LoadState(e.Parameter, (Dictionary<String, Object>)frameState[this._pageKey]);
                }

                Action<LayoutAwarePage, IDictionary<string, object>> init = null;
                if (PageInitActions.TryGetValue(this.GetType().FullName, out init))
                {
                    init(this, dic);
                }
                this.DefaultViewModel.Navigator = this.Frame.GetFrameNavigator();
                object fin = null;
                if (dic != null)
                {



                    if (dic.TryGetValue(NavigateParameterKeys.FinishedCallback, out fin))
                    {
                        Action<LayoutAwarePage> finishNavCallback = fin as Action<LayoutAwarePage>;
                        DefaultViewModel.AddDisposeAction(() =>
                        {
                            finishNavCallback(this);
                            if (this.Frame != null && this.Frame.CanGoBack) this.Frame.GoBack();
                            //GoBack(this, null);
                        }
                            );

                    }
                    fin = null;
                    if (dic.TryGetValue(NavigateParameterKeys.NavigateToCallback, out fin))
                    {
                        Action<LayoutAwarePage> navigateToCallback = fin as Action<LayoutAwarePage>;
                        navigateToCallback(this);
                    }
                }



            }

            /// <summary>
            /// Invoked when this page will no longer be displayed in a Frame.
            /// </summary>
            /// <param name="e">Event data that describes how this page was reached.  The Parameter
            /// property provides the group to be displayed.</param>
            protected override void OnNavigatedFrom(NavigationEventArgs e)
            {
                var frameState = SuspensionManager.SessionStateForFrame(this.Frame);
                var pageState = new Dictionary<string, object> { { "", DefaultViewModel } };
                this.SaveState(pageState);
                frameState[_pageKey] = pageState;
            }

            /// <summary>
            /// Populates the page with content passed during navigation.  Any saved state is also
            /// provided when recreating a page from a prior session.
            /// </summary>
            /// <param name="navigationParameter">The parameter value passed to
            /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
            /// </param>
            /// <param name="pageState">A dictionary of state preserved by this page during an earlier
            /// session.  This will be null the first time a page is visited.</param>
            protected virtual void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
            {


            }

            /// <summary>
            /// Preserves state associated with this page in case the application is suspended or the
            /// page is discarded from the navigation cache.  Values must conform to the serialization
            /// requirements of <see cref="SuspensionManager.SessionState"/>.
            /// </summary>
            /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
            protected virtual void SaveState(Dictionary<String, Object> pageState)
            {

            }

            #endregion



            protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
            {
                //  DisposeViewModel();
                base.OnNavigatingFrom(e);

            }




            public TResult GetResult<TResult>()
            {
                if (DefaultViewModel["Result"] is TResult)
                    return (TResult)DefaultViewModel["Result"];
                else
                    return default(TResult);
            }


        }

        internal sealed class SuspensionManager
        {
            private static Dictionary<string, object> _sessionState = new Dictionary<string, object>();
            private static List<Type> _knownTypes = new List<Type>();
            private const string sessionStateFilename = "_sessionState.xml";

            /// <summary>
            /// Provides access to global session state for the current session.  This state is
            /// serialized by <see cref="SaveAsync"/> and restored by
            /// <see cref="RestoreAsync"/>, so values must be serializable by
            /// <see cref="DataContractSerializer"/> and should be as compact as possible.  Strings
            /// and other self-contained data types are strongly recommended.
            /// </summary>
            public static Dictionary<string, object> SessionState
            {
                get { return _sessionState; }
            }

            /// <summary>
            /// List of custom types provided to the <see cref="DataContractSerializer"/> when
            /// reading and writing session state.  Initially empty, additional types may be
            /// added to customize the serialization process.
            /// </summary>
            public static List<Type> KnownTypes
            {
                get { return _knownTypes; }
            }

            /// <summary>
            /// Save the current <see cref="SessionState"/>.  Any <see cref="Frame"/> instances
            /// registered with <see cref="RegisterFrame"/> will also preserve their current
            /// navigation stack, which in turn gives their active <see cref="Page"/> an opportunity
            /// to save its state.
            /// </summary>
            /// <returns>An asynchronous task that reflects when session state has been saved.</returns>
            public static async Task SaveAsync()
            {
                // Save the navigation state for all registered frames
                foreach (var weakFrameReference in _registeredFrames)
                {
                    Frame frame;
                    if (weakFrameReference.TryGetTarget(out frame))
                    {
                        SaveFrameNavigationState(frame);
                    }
                }

                // Serialize the session state synchronously to avoid asynchronous access to shared
                // state
                MemoryStream sessionData = new MemoryStream();
                DataContractSerializer serializer = new DataContractSerializer(typeof(Dictionary<string, object>), _knownTypes);
                serializer.WriteObject(sessionData, _sessionState);

                // Get an output stream for the SessionState file and write the state asynchronously
                StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(sessionStateFilename, CreationCollisionOption.ReplaceExisting);
                using (Stream fileStream = await file.OpenStreamForWriteAsync())
                {
                    sessionData.Seek(0, SeekOrigin.Begin);
                    await sessionData.CopyToAsync(fileStream);
                    await fileStream.FlushAsync();
                }
            }

            /// <summary>
            /// Restores previously saved <see cref="SessionState"/>.  Any <see cref="Frame"/> instances
            /// registered with <see cref="RegisterFrame"/> will also restore their prior navigation
            /// state, which in turn gives their active <see cref="Page"/> an opportunity restore its
            /// state.
            /// </summary>
            /// <returns>An asynchronous task that reflects when session state has been read.  The
            /// content of <see cref="SessionState"/> should not be relied upon until this task
            /// completes.</returns>
            public static async Task RestoreAsync()
            {
                _sessionState = new Dictionary<String, Object>();

                // Get the input stream for the SessionState file
                StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(sessionStateFilename);
                using (IInputStream inStream = await file.OpenSequentialReadAsync())
                {
                    // Deserialize the Session State
                    DataContractSerializer serializer = new DataContractSerializer(typeof(Dictionary<string, object>), _knownTypes);
                    _sessionState = (Dictionary<string, object>)serializer.ReadObject(inStream.AsStreamForRead());
                }

                // Restore any registered frames to their saved state
                foreach (var weakFrameReference in _registeredFrames)
                {
                    Frame frame;
                    if (weakFrameReference.TryGetTarget(out frame))
                    {
                        frame.ClearValue(FrameSessionStateProperty);
                        RestoreFrameNavigationState(frame);
                    }
                }
            }

            private static DependencyProperty FrameSessionStateKeyProperty =
                DependencyProperty.RegisterAttached("_FrameSessionStateKey", typeof(String), typeof(SuspensionManager), null);
            private static DependencyProperty FrameSessionStateProperty =
                DependencyProperty.RegisterAttached("_FrameSessionState", typeof(Dictionary<String, Object>), typeof(SuspensionManager), null);
            private static List<WeakReference<Frame>> _registeredFrames = new List<WeakReference<Frame>>();

            /// <summary>
            /// Registers a <see cref="Frame"/> instance to allow its navigation history to be saved to
            /// and restored from <see cref="SessionState"/>.  Frames should be registered once
            /// immediately after creation if they will participate in session state management.  Upon
            /// registration if state has already been restored for the specified key
            /// the navigation history will immediately be restored.  Subsequent invocations of
            /// <see cref="RestoreAsync"/> will also restore navigation history.
            /// </summary>
            /// <param name="frame">An instance whose navigation history should be managed by
            /// <see cref="SuspensionManager"/></param>
            /// <param name="sessionStateKey">A unique key into <see cref="SessionState"/> used to
            /// store navigation-related information.</param>
            public static void RegisterFrame(Frame frame, String sessionStateKey)
            {
                if (frame.GetValue(FrameSessionStateKeyProperty) != null)
                {
                    throw new InvalidOperationException("Frames can only be registered to one session state key");
                }

                if (frame.GetValue(FrameSessionStateProperty) != null)
                {
                    throw new InvalidOperationException("Frames must be either be registered before accessing frame session state, or not registered at all");
                }

                // Use a dependency property to associate the session key with a frame, and keep a list of frames whose
                // navigation state should be managed
                frame.SetValue(FrameSessionStateKeyProperty, sessionStateKey);
                _registeredFrames.Add(new WeakReference<Frame>(frame));

                // Check to see if navigation state can be restored
                RestoreFrameNavigationState(frame);
            }

            /// <summary>
            /// Disassociates a <see cref="Frame"/> previously registered by <see cref="RegisterFrame"/>
            /// from <see cref="SessionState"/>.  Any navigation state previously captured will be
            /// removed.
            /// </summary>
            /// <param name="frame">An instance whose navigation history should no longer be
            /// managed.</param>
            public static void UnregisterFrame(Frame frame)
            {
                // Remove session state and remove the frame from the list of frames whose navigation
                // state will be saved (along with any weak references that are no longer reachable)
                SessionState.Remove((String)frame.GetValue(FrameSessionStateKeyProperty));
                _registeredFrames.RemoveAll((weakFrameReference) =>
                {
                    Frame testFrame;
                    return !weakFrameReference.TryGetTarget(out testFrame) || testFrame == frame;
                });
            }

            /// <summary>
            /// Provides storage for session state associated with the specified <see cref="Frame"/>.
            /// Frames that have been previously registered with <see cref="RegisterFrame"/> have
            /// their session state saved and restored automatically as a part of the global
            /// <see cref="SessionState"/>.  Frames that are not registered have transient state
            /// that can still be useful when restoring pages that have been discarded from the
            /// navigation cache.
            /// </summary>
            /// <remarks>Apps may choose to rely on <see cref="LayoutAwarePage"/> to manage
            /// page-specific state instead of working with frame session state directly.</remarks>
            /// <param name="frame">The instance for which session state is desired.</param>
            /// <returns>A collection of state subject to the same serialization mechanism as
            /// <see cref="SessionState"/>.</returns>
            public static Dictionary<String, Object> SessionStateForFrame(Frame frame)
            {
                var frameState = (Dictionary<String, Object>)frame.GetValue(FrameSessionStateProperty);

                if (frameState == null)
                {
                    var frameSessionKey = (String)frame.GetValue(FrameSessionStateKeyProperty);
                    if (frameSessionKey != null)
                    {
                        // Registered frames reflect the corresponding session state
                        if (!_sessionState.ContainsKey(frameSessionKey))
                        {
                            _sessionState[frameSessionKey] = new Dictionary<String, Object>();
                        }
                        frameState = (Dictionary<String, Object>)_sessionState[frameSessionKey];
                    }
                    else
                    {
                        // Frames that aren't registered have transient state
                        frameState = new Dictionary<String, Object>();
                    }
                    frame.SetValue(FrameSessionStateProperty, frameState);
                }
                return frameState;
            }

            private static void RestoreFrameNavigationState(Frame frame)
            {
                var frameState = SessionStateForFrame(frame);
                if (frameState.ContainsKey("Navigation"))
                {
                    frame.SetNavigationState((String)frameState["Navigation"]);
                }
            }

            private static void SaveFrameNavigationState(Frame frame)
            {
                var frameState = SessionStateForFrame(frame);
                frameState["Navigation"] = frame.GetNavigationState();
            }
        }



        public static class NavigationHelper
        {


            public static IFrameNavigator GetFrameNavigator(this Frame obj)
            {
                return (IFrameNavigator)obj.GetValue(FrameNavigatorProperty);
            }

            public static void SetFrameNavigator(this Frame obj, IFrameNavigator value)
            {
                obj.SetValue(FrameNavigatorProperty, value);
            }

            // Using a DependencyProperty as the backing store for FrameNavigator.  This enables animation, styling, binding, etc...
            public static readonly DependencyProperty FrameNavigatorProperty =
                DependencyProperty.RegisterAttached("FrameNavigator", typeof(IFrameNavigator), typeof(Frame), new PropertyMetadata(null));


        }
    }


}
