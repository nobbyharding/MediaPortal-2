﻿#region Copyright (C) 2007-2014 Team MediaPortal
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
using System.Net;
using System.Net.Http;
using MediaPortal.Common.Logging;
using MediaPortal.Common.PluginManager.Packages.DataContracts.UserAdmin;
using MediaPortal.PackageManager.Options.Admin;
using MediaPortal.PackageManager.Options.Shared;

namespace MediaPortal.PackageManager.Core
{
	internal class PackageAdmin : Requestor
	{
	  private const string CREATE_USER_PATH = "/user/create";
	  private const string REVOKE_USER_PATH = "/user/revoke";

	  public PackageAdmin( ILogger log ) : base( log )
	  {
	  }

	  public static bool Dispatch( ILogger log, Operation operation, object options )
	  {
	    if( options == null )
	      return false;

	    var core = new PackageAdmin( log );
	    switch( operation )
	    {
	      case Operation.CreateUser:
          return core.CreateUser( options as CreateUserOptions );
	      case Operation.RevokeUser:
	        return core.RevokeUser( options as RevokeUserOptions );
        default:
          return false;
      }
	  }    

    public bool CreateUser( CreateUserOptions options )
		{
		  VerifyOptions( options );

      var proxy = new RequestExecutionHelper( options.UserName, options.Password );
      var model = new CreateUserModel( options.Login, options.Secret, options.Name, options.Email, null );
      var response = proxy.ExecuteRequest( HttpMethod.Post, CREATE_USER_PATH, model );

      var successMessage = string.Format( "The user '{0}' has been created.{1}"
        +"Hint: if this was a mistake, you can use the 'revoke-user' command to disable the user account.",
        options.Login, Environment.NewLine );
      return IsSuccess( response, successMessage, HttpStatusCode.OK, HttpStatusCode.Created );
		}

    public bool RevokeUser( RevokeUserOptions options )
		{
		  VerifyOptions( options );

      var proxy = new RequestExecutionHelper( options.UserName, options.Password );
      var model = new RevokeUserModel( options.Login, options.Reason );
      var response = proxy.ExecuteRequest( HttpMethod.Post, REVOKE_USER_PATH, model );

      var successMessage = string.Format( "The user '{0}' has been revoked.", options.Login );
      return IsSuccess( response, successMessage, HttpStatusCode.OK );
		}

    #region VerifyOptions (and QueryUserForPassword)
    private static void VerifyOptions( CreateUserOptions options )
	  {
      if( string.IsNullOrWhiteSpace(options.Login) )
        throw new ArgumentException("Unable to create user (the users login is invalid or unspecified).");

      if( string.IsNullOrWhiteSpace(options.Secret) || options.Secret.Length < 6 )
        throw new ArgumentException("Unable to create user (the users secret (password) must be at least 6 characters).");

      if( string.IsNullOrWhiteSpace(options.Name) )
        throw new ArgumentException("Unable to create user (the user name is invalid or unspecified).");
      
      VerifyAuthOptions( options );
	  }

	  private static void VerifyOptions( RevokeUserOptions options )
	  {
      if( string.IsNullOrWhiteSpace(options.Login) )
        throw new ArgumentException("Unable to revoke user (login is invalid or unspecified).");

      if( string.IsNullOrWhiteSpace(options.Reason) || options.Reason.Length < 5 )
        throw new ArgumentException("Unable to revoke user (a proper reason must be supplied).");

      VerifyAuthOptions( options );
	  }

	  private static void VerifyAuthOptions( AuthOptions options )
	  {
      // ask user for password if one was not specified
	    if( options.Password == null )
	      options.Password = QueryUserForPassword( options.UserName );

      // verify credentials
      if( string.IsNullOrWhiteSpace(options.UserName) || string.IsNullOrWhiteSpace(options.Password) )
        throw new ArgumentException("Unable to authenticate at the MediaPortal package server using the given credentials.");
	  }

	  private static string QueryUserForPassword( string userName )
	  {
      Console.Write("Please specify the password for user '{0}' at the MediaPortal package server: ", userName );
	    return Console.ReadLine();
	  }
    #endregion
	}
}
