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
using System.Collections.Concurrent;
using System.Collections.Generic;
using MediaPortal.Common.PluginManager.Exceptions;
using MediaPortal.Common.PluginManager.Models;

namespace MediaPortal.Common.PluginManager.Validation
{
  /// <summary>
  /// Validation helper class responsible for version compatibility checking.
  /// </summary>
  public class CompatibilityValidator : IValidator
  {
    #region Fields

    private readonly ConcurrentDictionary<Guid, PluginMetadata> _availablePlugins;
    private readonly IDictionary<string, CoreComponent> _coreComponents;

    #endregion

    #region Ctor

    public CompatibilityValidator( ConcurrentDictionary<Guid, PluginMetadata> availablePlugins, IDictionary<string, CoreComponent> coreComponents )
    {
      _availablePlugins = availablePlugins;
      _coreComponents = coreComponents;
    }

    #endregion

    #region IValidate

    /// <summary>
    /// Validates the given <paramref name="plugin"/> by checking for version incompatibilities
    /// with core components and other installed plugins.
    /// Currently disabled plugins are considered during validation, and the fact that they are
    /// disabled does not make them count as being incompatible.
    /// </summary>
    /// <param name="plugin">The plugin to validate.</param>
    /// <returns>
    /// A set of plugin ids found to be incompatible, or an empty set if no
    /// incompatible plugins were found.
    /// </returns>
    public HashSet<Guid> Validate(PluginMetadata plugin)
    {
      return FindIncompatible(plugin, new HashSet<Guid>());
    }

    #endregion

    #region Validation Implementation (FindIncompatible)

    /// <summary>
    /// Conflicts are searched recursive, but plugins might be referenced multiple times in the hierarchy.
    /// So in order to speed up this process and prevent a StackOverflowException we pass a list of already checked plugin Ids.
    /// </summary>
    /// <param name="plugin"></param>
    /// <param name="alreadyCheckedPlugins"></param>
    /// <returns></returns>
    private HashSet<Guid> FindIncompatible(PluginMetadata plugin, HashSet<Guid> alreadyCheckedPlugins)
    {
      var result = new HashSet<Guid>();
      if (alreadyCheckedPlugins.Contains(plugin.PluginId))
        return result;
      alreadyCheckedPlugins.Add(plugin.PluginId);

      foreach (PluginDependency dependency in plugin.DependencyInfo.DependsOn)
      {
        if (dependency.IsCoreDependency)
        {
          CoreComponent api;
          if (!_coreComponents.TryGetValue(dependency.CoreDependencyName, out api))
            throw new PluginMissingDependencyException("Plugin dependency '{0}' is not available", dependency.CoreDependencyName);
          if( api.MinCompatibleApi > dependency.CompatibleApi || api.CurrentApi < dependency.CompatibleApi )
            throw new PluginIncompatibleException( "Dependency '{0}' requires API level ({1}) and available is [min compatible ({2}) -> ({3}) current]", dependency.CoreDependencyName, dependency.CompatibleApi, api.MinCompatibleApi, api.CurrentApi );
        }
        else
        {
          PluginMetadata dependencyMetadata;
          if (!_availablePlugins.TryGetValue(dependency.PluginId, out dependencyMetadata))
            throw new PluginMissingDependencyException("Plugin dependency '{0}' is not available", dependency.PluginId);
          if (dependencyMetadata.DependencyInfo.MinCompatibleApi > dependency.CompatibleApi ||
              dependencyMetadata.DependencyInfo.CurrentApi < dependency.CompatibleApi)
            throw new PluginIncompatibleException("Dependency '{0}' requires API level ({1}) and available is [min compatible ({2}) -> ({3}) current]",
              dependencyMetadata.Name, dependency.CompatibleApi, dependencyMetadata.DependencyInfo.MinCompatibleApi, dependencyMetadata.DependencyInfo.CurrentApi);
          result.UnionWith(FindIncompatible(dependencyMetadata, alreadyCheckedPlugins));
        }
      }
      return result;
    }

    #endregion
  }
}
