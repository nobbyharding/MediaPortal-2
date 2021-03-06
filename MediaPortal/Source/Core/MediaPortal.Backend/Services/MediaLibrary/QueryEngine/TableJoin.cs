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

using System.Text;

namespace MediaPortal.Backend.Services.MediaLibrary.QueryEngine
{
  /// <summary>
  /// Encapsulates a requested table, identified by a <see cref="TableQueryData"/> instance, which is joined into a query.
  /// </summary>
  public class TableJoin
  {
    protected string _joinType;
    protected TableQueryData _table;
    protected object _joinAttr1;
    protected object _joinAttr2;

    public TableJoin(string joinType, TableQueryData table, RequestedAttribute joinAttr1, RequestedAttribute joinAttr2) :
        this(joinType, table, (object) joinAttr1, (object) joinAttr2) { }

    public TableJoin(string joinType, TableQueryData table, object joinAttr1, object joinAttr2)
    {
      _joinType = joinType;
      _table = table;
      _joinAttr1 = joinAttr1;
      _joinAttr2 = joinAttr2;
    }

    /// <summary>
    /// Join type, like "inner join" or "left outer join".
    /// </summary>
    public string JoinType
    {
      get { return _joinType; }
    }

    /// <summary>
    /// Table which is joined.
    /// </summary>
    public TableQueryData JoinedTable
    {
      get { return _table; }
    }

    public string GetJoinDeclaration(Namespace ns)
    {
      StringBuilder result = new StringBuilder(100);
      result.Append(_joinType);
      result.Append(" ");
      result.Append(_table.GetDeclarationWithAlias(ns));
      result.Append(" ON ");
      RequestedAttribute ra = _joinAttr1 as RequestedAttribute;
      if (ra != null)
        result.Append(ra.GetQualifiedName(ns));
      else
        result.Append(_joinAttr1);
      result.Append(" = ");
      ra = _joinAttr2 as RequestedAttribute;
      if (ra != null)
        result.Append(ra.GetQualifiedName(ns));
      else
        result.Append(_joinAttr2);
      return result.ToString();
    }

    public override string ToString()
    {
      return GetJoinDeclaration(new Namespace());
    }
  }
}