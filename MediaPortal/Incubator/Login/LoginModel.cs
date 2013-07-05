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

using System;
using System.Windows.Forms;
using MediaPortal.Common;
using MediaPortal.Common.General;
using MediaPortal.Common.Services.ServerCommunication;
using MediaPortal.Common.UserProfileDataManagement;
using MediaPortal.UI.Presentation.DataObjects;
using MediaPortal.UI.ServerCommunication;
using MediaPortal.UiComponents.SkinBase.General;

namespace MediaPortal.UiComponents.Login
{
  /// <summary>
  /// viewmodel for handling logins
  /// </summary>
  public class LoginModel
  {
    private readonly ItemsList _usersExposed = new ItemsList();
    private AbstractProperty _currentUser;
    const string KEY_PROFILE_ID = "ProfileId";

    /// <summary>
    /// constructor
    /// </summary>
    public LoginModel()
    {
      _currentUser = new WProperty(typeof(UserProfile), null);
      LoadUsers();
    }

    protected IUserProfileDataManagement UserProfileManagement
    {
      get
      {
        UPnPClientControlPoint controlPoint = ServiceRegistration.Get<IServerConnectionManager>().ControlPoint;
        return controlPoint != null ? controlPoint.UserProfileDataManagementService : null;
      }
    }

    /// <summary>
    /// will load the users from somewhere
    /// </summary>
    private void LoadUsers()
    {
      UserProfile u1;
      string profileName = SystemInformation.ComputerName.ToLower();
      if (!UserProfileManagement.GetProfileByName(profileName, out u1))
      {
        // add a few dummy users, later this should be more flexible and handled by a login manager / user account control
        Guid userId1 = UserProfileManagement.CreateProfile(profileName);
        if (!UserProfileManagement.GetProfile(userId1, out u1))
          return;
      }

      //u1.NeedsPassword = true;
      //u1.LastLogin = new DateTime(2007, 10, 25, 12, 20, 30);
      //u1.UserImage = SystemInformation.ComputerName.ToLower() + ".jpg";
      //IUser u2 = userService.AddUser(SystemInformation.UserName.ToLower());
      //u2.NeedsPassword = false;
      //u2.LastLogin = new DateTime(2007, 10, 26, 10, 30, 13);
      //u2.UserImage = SystemInformation.UserName.ToLower() + ".jpg";
      RefreshUserList();
    }

    /// <summary>
    /// this will turn the _users list into the _usersExposed list
    /// </summary>
    private void RefreshUserList()
    {
      // clear the exposed users list
      Users.Clear();
      // add users to expose them
      var users = UserProfileManagement.GetProfiles();
      foreach (UserProfile user in users)
      {
        if (user == null)
        {
          continue;
        }
        ListItem item = new ListItem();
        item.SetLabel(Consts.KEY_NAME, user.Name);

        item.AdditionalProperties[KEY_PROFILE_ID] = user.ProfileId;
        item.SetLabel("UserName", user.Name);
        //buff.SetLabel("UserImage", user.UserImage);
        //buff.SetLabel("NeedsPassword", user.NeedsPassword ? "true" : "false");
        //buff.SetLabel("LastLogin", user.LastLogin.ToString("G"));
        Users.Add(item);
      }
      // tell the skin that something might have changed
      Users.FireChange();
    }

    /// <summary>
    /// selects a user
    /// </summary>
    /// <param name="item"></param>
    public void SelectUser(ListItem item)
    {
      Guid profileId = (Guid) item.AdditionalProperties[KEY_PROFILE_ID];

      UserProfile userProfile;
      if (!UserProfileManagement.GetProfile(profileId, out userProfile))
        return;

      CurrentUser = userProfile;
    }

    /// <summary>
    /// exposes the current user to the skin
    /// </summary>
    public AbstractProperty CurrentUserProperty
    {
      get { return _currentUser; }
      set { _currentUser = value; }
    }

    /// <summary>
    /// exposes the current user to the skin
    /// </summary>
    public UserProfile CurrentUser
    {
      get { return (UserProfile) _currentUser.GetValue(); }
      set { _currentUser.SetValue(value); }
    }

    /// <summary>
    /// exposes the users to the skin
    /// </summary>
    public ItemsList Users
    {
      get { return _usersExposed; }
    }
  }
}
