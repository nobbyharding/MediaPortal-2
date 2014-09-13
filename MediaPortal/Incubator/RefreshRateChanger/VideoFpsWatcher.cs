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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using MediaPortal.Common;
using MediaPortal.Common.Logging;
using MediaPortal.Common.MediaManagement;
using MediaPortal.Common.MediaManagement.DefaultItemAspects;
using MediaPortal.Common.PluginManager;
using MediaPortal.Common.PluginManager.Activation;
using MediaPortal.Common.Services.Settings;
using MediaPortal.Plugins.RefreshRateChanger.Settings;
using MediaPortal.UI.Presentation.Players;
using MediaPortal.UI.Presentation.Screens;
using MediaPortal.UI.SkinEngine.SkinManagement;

namespace MediaPortal.Plugins.RefreshRateChanger
{
  public class VideoFpsWatcher : IPluginStateTracker, IDisposable
  {
    protected RefreshRateChanger _refreshRateChanger;
    protected Timer _timer;
    protected object _syncObj = new object();
    protected bool _isEnabled;
    protected SettingsChangeWatcher<RefreshRateChangerSettings> _settings = new SettingsChangeWatcher<RefreshRateChangerSettings>();

    public VideoFpsWatcher()
    {
      _settings.SettingsChanged += SettingsChanged;
    }

    protected uint GetScreenNum()
    {
      return (uint)Array.IndexOf(System.Windows.Forms.Screen.AllScreens, System.Windows.Forms.Screen.FromControl(SkinContext.Form));
    }

    private void SyncToPlayer(IVideoPlayer player)
    {
      if (_refreshRateChanger != null)
      {
        _refreshRateChanger.Dispose();
        _refreshRateChanger = null;
      }
      MediaItem mediaItem = GetCurrentMediaItem(player);
      if (mediaItem == null)
        return;

      int intFps;
      if (MediaItemAspect.TryGetAttribute(mediaItem.Aspects, VideoAspect.ATTR_FPS, out intFps))
      {
        ICollection<int> excludeRates = TryParseIntList(_settings.Settings.NoChangeForRate);
        if (excludeRates.Contains(intFps))
        {
          ServiceRegistration.Get<ILogger>().Debug("RefreshRateChanger: Video fps: {0}; No change due to settings.", intFps);
          return;
        }
        _refreshRateChanger = new TemporaryRefreshRateChanger(GetScreenNum());
        double fps = intFps;
        if (intFps == 23)
          fps = 23.976;
        if (intFps == 29)
          fps = 29.970;
        if (intFps == 59)
          fps = 59.940;

        var currentRefreshRate = _refreshRateChanger.GetRefreshRate();
        if (!IsMultipleOf(currentRefreshRate, fps))
        {
          ServiceRegistration.Get<ILogger>().Debug("RefreshRateChanger: Video fps: {0}; Screen refresh rate {1}, trying to change it.", fps, currentRefreshRate);
          _refreshRateChanger.SetRefreshRate(fps);
        }
        else
        {
          ServiceRegistration.Get<ILogger>().Debug("RefreshRateChanger: Video fps: {0}; Screen refresh rate {1}, no change required.", fps, currentRefreshRate);
        }
      }
    }

    private ICollection<int> TryParseIntList(string noChangeForRate)
    {
      HashSet<int> rates = new HashSet<int>();
      if (!string.IsNullOrWhiteSpace(noChangeForRate))
        foreach (string rateString in noChangeForRate.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
        {
          int rate;
          if (int.TryParse(rateString.Trim(), out rate))
            rates.Add(rate);
        }
      return rates;
    }

    private bool IsMultipleOf(double screenRefreshRate, double videoFps)
    {
      return (int)(screenRefreshRate * 1000) % (int)(videoFps * 1000) == 0;
    }

    private MediaItem GetCurrentMediaItem(IVideoPlayer player)
    {
      IPlayerContextManager playerContextManager = ServiceRegistration.Get<IPlayerContextManager>();
      for (int index = 0; index < playerContextManager.NumActivePlayerContexts; index++)
      {
        IPlayerContext playerContext = playerContextManager.GetPlayerContext(index);
        if (playerContext == null || playerContext.CurrentPlayer != player)
          continue;
        return playerContext.CurrentMediaItem;
      }
      return null;
    }

    private void SettingsChanged(object sender, EventArgs eventArgs)
    {
      if (_settings.Settings.IsEnabled && !_isEnabled)
        Activate();
      if (!_settings.Settings.IsEnabled && _isEnabled)
        Stop();
    }

    public void Activated(PluginRuntime pluginRuntime)
    {
      Activate();
    }

    private void Activate()
    {
      if (_isEnabled)
        return;
      _timer = new Timer(1000);
      _timer.Elapsed += ActivateWhenReady;
      _timer.Start();
    }

    private void ActivateWhenReady(object sender, ElapsedEventArgs e)
    {
      IScreenControl screenControl = ServiceRegistration.Get<IScreenControl>(false);
      if (screenControl == null || screenControl.VideoPlayerSynchronizationStrategy == null)
        return;

      _timer.Close();
      _timer = null;

      screenControl.VideoPlayerSynchronizationStrategy.SynchronizeToVideoPlayerFramerate += SyncToPlayer;
      _isEnabled = true;
    }

    public bool RequestEnd()
    {
      return true;
    }

    public void Stop()
    {
      IScreenControl screenControl = ServiceRegistration.Get<IScreenControl>();
      screenControl.VideoPlayerSynchronizationStrategy.SynchronizeToVideoPlayerFramerate -= SyncToPlayer;
      _isEnabled = false;
    }

    public void Continue()
    {
    }

    public void Shutdown()
    {
      Dispose();
    }

    public void Dispose()
    {
      if (_refreshRateChanger != null)
        _refreshRateChanger.Dispose();
      if (_settings != null)
        _settings.Dispose();
    }
  }
}
