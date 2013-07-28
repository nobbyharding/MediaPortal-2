#region Copyright (C) 2007-2013 Team MediaPortal

/*
    Copyright (C) 2007-2013 Team MediaPortal
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

using MediaPortal.Common.General;

namespace MediaPortal.UI.Presentation.DataObjects
{
  /// <summary>
  /// Represents a dynamic data collection that provides notifications when items get added, removed, or when the whole list is refreshed. This class extends the
  /// existing <see cref="System.Collections.ObjectModel.ObservableCollection&lt;T&gt;"/> to interoperate with MediaPortal's <see cref="IObservable"/> and <see cref="ISynchronizable"/> 
  /// interfaces. This class can be used as alternative to <see cref="ItemsList"/>.
  /// </summary>
  /// <typeparam name="T">The type of elements in the collection.</typeparam>
  public class ObservableCollection<T> : System.Collections.ObjectModel.ObservableCollection<T>, IObservable, ISynchronizable
  {
    protected WeakEventMulticastDelegate _objectChanged = new WeakEventMulticastDelegate();
    protected readonly object _syncObj = new object();

    protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
    {
      base.OnPropertyChanged(e);
      FireChange();
    }

    protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      base.OnCollectionChanged(e);
      FireChange();
    }

    /// <summary>
    /// Event which gets fired when the collection changes.
    /// </summary>
    public event ObjectChangedDlgt ObjectChanged
    {
      add { _objectChanged.Attach(value); }
      remove { _objectChanged.Detach(value); }
    }

    public void FireChange()
    {
      _objectChanged.Fire(new object[] { this });
    }

    public object SyncRoot
    {
      get { return _syncObj; }
    }
  }
}
