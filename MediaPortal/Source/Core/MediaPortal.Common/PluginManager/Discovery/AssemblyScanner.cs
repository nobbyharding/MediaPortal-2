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
using System.Collections.Generic;
using System.Linq;
using MediaPortal.Attributes;
using MediaPortal.Common.PluginManager.Models;

namespace MediaPortal.Common.PluginManager.Discovery
{
  /// <summary>
  /// Class providing logic to enumerate all core components from currently loaded assemblies.
  /// </summary>
  internal class AssemblyScanner
  {
    public IDictionary<string, CoreComponent> PerformDiscovery()
    {
      var coreComponents = new Dictionary<string, CoreComponent>();
      foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        var coreApi = assembly.GetCustomAttributes( typeof(CoreAPIAttribute), false ).FirstOrDefault() as CoreAPIAttribute;
        if( coreApi != null )
        {
          var componentName = assembly.GetName().Name;
          coreComponents.Add( componentName, new CoreComponent( componentName, coreApi.CurrentAPI, coreApi.MinCompatibleAPI ) );
        }
      }
      return coreComponents;
    }
  }
}
