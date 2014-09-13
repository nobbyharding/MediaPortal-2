#region Copyright (C) 2007-2014 Team MediaPortal

/*
    Copyright (C) 2007-2014 Team MediaPortal
    http://www.team-mediaportal.com

    This file is part of MediaPortal 2

    MediaPortal 2 is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    MediaPortal 2 is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with MediaPortal 2. If not, see <http://www.gnu.org/licenses/>.
*/

#endregion

#if !DEBUG
using MediaPortal.Common.PathManager;
using System.IO;
#endif
using System;
using System.Threading;
using MediaPortal.Common;
using MediaPortal.Common.Exceptions;
using MediaPortal.Common.Logging;
using MediaPortal.Common.PluginManager;
using MediaPortal.Common.ResourceAccess;
using MediaPortal.Common.Runtime;
using MediaPortal.Common.Services.Logging;
using MediaPortal.Common.Services.Runtime;

[assembly: CLSCompliant(true)]
namespace MediaPortal.Package.UpdateService
{
  public class ApplicationLauncher
  {
//	protected SystemStateService _systemStateService = null;
//	protected string _dataDirectory = null;

//	public ApplicationLauncher(string dataDirectory)
//	{
//	  _dataDirectory = dataDirectory;
//	}

//	public void Start()
//	{
//	  Thread.CurrentThread.Name = "Main";

//#if !DEBUG
//	  string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"Team MediaPortal\MP2-Server\Log");
//#endif

//	  _systemStateService = new SystemStateService();
//	  ServiceRegistration.Set<ISystemStateService>(_systemStateService);
//	  _systemStateService.SwitchSystemState(SystemState.Initializing, false);

//	  try
//	  {
//		ILogger logger = null;
//		try
//		{
//		  // Check if user wants to override the default Application Data location.
//		  ApplicationCore.RegisterVitalCoreServices(_dataDirectory);
//		  ApplicationCore.RegisterCoreServices();
//		  logger = ServiceRegistration.Get<ILogger>();

//#if !DEBUG
//		  IPathManager pathManager = ServiceRegistration.Get<IPathManager>();
//		  logPath = pathManager.GetPath("<LOG>");
//#endif

//		  BackendExtension.RegisterBackendServices();
//		}
//		catch (Exception e)
//		{
//		  if (logger != null)
//			logger.Critical("Error starting application", e);
//		  _systemStateService.SwitchSystemState(SystemState.ShuttingDown, true);
//		  ServiceRegistration.IsShuttingDown = true;

//		  BackendExtension.DisposeBackendServices();
//		  ApplicationCore.DisposeCoreServices();

//		  throw;
//		}

//		// Start the core
//		logger.Debug("ApplicationLauncher: Starting core");

//		try
//		{
//		  var mediaAccessor = ServiceRegistration.Get<IMediaAccessor>();
//		  var pluginManager = ServiceRegistration.Get<IPluginManager>();
//		  pluginManager.Initialize();
//		  pluginManager.Startup(false);
//		  ApplicationCore.StartCoreServices();

//		  BackendExtension.StartupBackendServices();
//		  ApplicationCore.RegisterDefaultMediaItemAspectTypes(); // To be done after backend services are running

//		  mediaAccessor.Initialize();

//		  _systemStateService.SwitchSystemState(SystemState.Running, true);
//		  BackendExtension.ActivateImporterWorker();
//			// To be done after default media item aspect types are present and when the system is running (other plugins might also install media item aspect types)
//		}
//		catch (Exception e)
//		{
//		  logger.Critical("Error starting application", e);
//		  _systemStateService.SwitchSystemState(SystemState.ShuttingDown, true);
//		  ServiceRegistration.IsShuttingDown = true;
//		  BackendExtension.DisposeBackendServices();
//		  ApplicationCore.DisposeCoreServices();
//		  _systemStateService.SwitchSystemState(SystemState.Ending, false);
//		  throw; // needed to cancel OnStart of the Service
//		}
//	  }
//	  catch (Exception ex)
//	  {
//#if DEBUG
//		ConsoleLogger log = new ConsoleLogger(LogLevel.All, false);
//		log.Error(ex);
//#else
//		ServerCrashLogger crash = new ServerCrashLogger(logPath);
//		crash.CreateLog(ex);
//#endif
//		_systemStateService.SwitchSystemState(SystemState.Ending, false);
//		throw; // needed to cancel OnStart of the Service
//	  }
//	}


//	public void Stop()
//	{
//	  try
//	  {
//		_systemStateService.SwitchSystemState(SystemState.ShuttingDown, true);
//		ServiceRegistration.IsShuttingDown = true; // Block ServiceRegistration from trying to load new services in shutdown phase

//		ServiceRegistration.Get<IMediaAccessor>().Shutdown();
//		ServiceRegistration.Get<IPluginManager>().Shutdown();
//		BackendExtension.ShutdownBackendServices();
//		ApplicationCore.StopCoreServices();
//	  }
//	  catch (Exception ex)
//	  {
//		//ServiceRegistration.Get<ILogger.Critical("Error stopping application", e);
//#if DEBUG
//		ConsoleLogger log = new ConsoleLogger(LogLevel.All, false);
//		log.Error(ex);
//#else
//		var pathManager = ServiceRegistration.Get<IPathManager>();
//		var logPath = pathManager.GetPath("<LOG>");
//		ServerCrashLogger crash = new ServerCrashLogger(logPath);
//		crash.CreateLog(ex);
//#endif
//		_systemStateService.SwitchSystemState(SystemState.ShuttingDown, true);
//		ServiceRegistration.IsShuttingDown = true;
//	  }
//	  finally
//	  {
//		BackendExtension.DisposeBackendServices();
//		ApplicationCore.DisposeCoreServices();
//		_systemStateService.SwitchSystemState(SystemState.Ending, false);
//		_systemStateService.Dispose();
//	  }
//	}

//	protected void RunAsConsole()
//	{
//	  Application.ThreadException += LauncherExceptionHandling.Application_ThreadException;
//	  AppDomain.CurrentDomain.UnhandledException += LauncherExceptionHandling.CurrentDomain_UnhandledException;
//	  Start();

//	  try
//	  {
//		Application.Run(new MainForm());
//	  }
//	  catch (Exception e)
//	  {
//		ServiceRegistration.Get<ILogger>().Critical("Error executing application", e);
//	  }

//	  Stop();
//	  Application.ThreadException -= LauncherExceptionHandling.Application_ThreadException;
//	  AppDomain.CurrentDomain.UnhandledException -= LauncherExceptionHandling.CurrentDomain_UnhandledException;
//	}
  }
}