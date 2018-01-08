﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace PlaylisterUWP
{
	using Windows.UI.Core;
	using Windows.UI.ViewManagement;
	using Windows.UI.Xaml.Media.Animation;
	using MetroLog;
	using MetroLog.Targets;

	/// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
		/// <summary>
	    /// Initializes the singleton application object.  This is the first line of authored code
	    /// executed, and as such is the logical equivalent of main() or WinMain().
	    /// </summary>
	    public App()
	    {
		    this.InitializeComponent();
		    this.Suspending += OnSuspending;
			#if DEBUG
				LogManagerFactory.DefaultConfiguration.AddTarget(LogLevel.Trace, LogLevel.Fatal, new FileStreamingTarget());
			#else
				LogManagerFactory.DefaultConfiguration.AddTarget(LogLevel.Error, LogLevel.Fatal, new FileStreamingTarget());
			#endif
			GlobalCrashHandler.Configure();
		}

		/// <summary>
		/// Invoked when the application is launched normally by the end user.  Other entry points
		/// will be used such as when the application is launched to open a specific file.
		/// </summary>
		/// <param name="e">Details about the launch request and process.</param>
		protected override void OnLaunched(LaunchActivatedEventArgs e)
	    {
			Frame rootFrame = Window.Current.Content as Frame;

		    // Do not repeat app initialization when the Window already has content,
		    // just ensure that the window is active
		    if (rootFrame == null)
		    {
			    // Create a Frame to act as the navigation context and navigate to the first page
			    rootFrame = new Frame();

			    rootFrame.NavigationFailed += OnNavigationFailed;

			    if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
			    {
				    //TODO: Load state from previously suspended application
			    }

			    // Place the frame in the current Window
			    Window.Current.Content = rootFrame;

			}
		    rootFrame.ContentTransitions = new TransitionCollection
		    {
			    new NavigationThemeTransition()
		    };
		    if (e.PrelaunchActivated == false)
		    {
			    if (rootFrame.Content == null)
			    {
				    // When the navigation stack isn't restored navigate to the first page,
				    // configuring the new page by passing required information as a navigation
				    // parameter
				    rootFrame.Navigate(typeof(MainPage), e.Arguments);
			    }
			    // Ensure the current window is active
			    Window.Current.Activate();
				
			    var titleBar = ApplicationView.GetForCurrentView().TitleBar;
			    titleBar.BackgroundColor = Windows.UI.Colors.CadetBlue;
			    titleBar.ForegroundColor = Windows.UI.Colors.White;
			    titleBar.ButtonForegroundColor = Windows.UI.Colors.White;
			    titleBar.ButtonBackgroundColor = Windows.UI.Colors.CadetBlue;
			    titleBar.ButtonHoverForegroundColor = Windows.UI.Colors.CadetBlue;
			    titleBar.ButtonHoverBackgroundColor = Windows.UI.Colors.AliceBlue;
			    titleBar.ButtonPressedForegroundColor = Windows.UI.Colors.Gray;
			    titleBar.ButtonPressedBackgroundColor = Windows.UI.Colors.LightSlateGray;

			    // Set inactive window colors
			    titleBar.InactiveForegroundColor = Windows.UI.Colors.CadetBlue;
			    titleBar.InactiveBackgroundColor = Windows.UI.Colors.AliceBlue;
			    titleBar.ButtonInactiveForegroundColor = Windows.UI.Colors.CadetBlue;
			    titleBar.ButtonInactiveBackgroundColor = Windows.UI.Colors.AliceBlue;
			}
		    SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;
		}

	    /// <summary>
	    /// Invoked when the application is launched through a custom URI scheme, such as
	    /// is the case in an OAuth 2.0 authorization flow.
	    /// </summary>
	    /// <param name="args">Details about the URI that activated the app.</param>
	    protected override void OnActivated(IActivatedEventArgs args)
	    {
		    // When the app was activated by a Protocol (custom URI scheme), forwards
		    // the URI to the MainPage through a Navigate event.
		    if (args.Kind == ActivationKind.Protocol)
		    {
			    // Extracts the authorization response URI from the arguments.
			    ProtocolActivatedEventArgs protocolArgs = (ProtocolActivatedEventArgs)args;
			    Uri uri = protocolArgs.Uri;

			    // Gets the current frame, making one if needed.
			    var frame = Window.Current.Content as Frame;
			    if (frame == null)
				    frame = new Frame();

			    // Opens the URI for "navigation" (handling) on the MainPage.
			    frame.Navigate(typeof(MainPage), uri);
			    Window.Current.Content = frame;
			    Window.Current.Activate();
		    }
	    }

	    private void App_BackRequested(object sender, BackRequestedEventArgs e)
	    {
		    Frame rootFrame = Window.Current.Content as Frame;
		    if (rootFrame == null)
			    return;

		    // If we can go back and the event has not already been handled, do so.
		    if (rootFrame.CanGoBack && e.Handled == false)
		    {
			    e.Handled = true;
			    rootFrame.GoBack();
		    }
	    }

		/// <summary>
		/// Invoked when Navigation to a certain page fails
		/// </summary>
		/// <param name="sender">The Frame which failed navigation</param>
		/// <param name="e">Details about the navigation failure</param>
		void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
	    {
		    throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
	    }

	    /// <summary>
	    /// Invoked when application execution is being suspended.  Application state is saved
	    /// without knowing whether the application will be terminated or resumed with the contents
	    /// of memory still intact.
	    /// </summary>
	    /// <param name="sender">The source of the suspend request.</param>
	    /// <param name="e">Details about the suspend request.</param>
	    private void OnSuspending(object sender, SuspendingEventArgs e)
	    {
		    var deferral = e.SuspendingOperation.GetDeferral();
		    //TODO: Save application state and stop any background activity
		    deferral.Complete();
	    }
	}
}
