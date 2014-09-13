﻿
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MediaPortal.PackageManager.Core
{
  class RequestExecutionHelper
  {
    private readonly Uri _packageServerUri;
    private readonly string _credentials;

    public RequestExecutionHelper( string userName = null, string password = null, string packageServerHostAddress = null )
    {
      _packageServerUri = new Uri( packageServerHostAddress ?? "http://localhost:57235" );
      var hasCredentials = userName != null && password != null;
      _credentials = hasCredentials ? Convert.ToBase64String( Encoding.ASCII.GetBytes( userName + ":" + password ) ) : null;
    }

    // TODO use multi-part for file uploads
    // see http://stackoverflow.com/questions/16906711/httpclient-how-to-upload-multiple-files-at-once
    // and http://stackoverflow.com/questions/15638622/how-to-upload-files-to-asp-net-mvc-4-0-action-running-in-iis-express-with-httpcl/15638623#15638623

    public async Task<HttpResponseMessage> ExecuteRequestAsync( HttpMethod method, string path, object model = null )
    {
      using( var client = new HttpClient() )
      {
        var request = new HttpRequestMessage( method, new Uri( _packageServerUri, path ) );
        if( _credentials != null )
        {
          request.Headers.Authorization = new AuthenticationHeaderValue( "Basic", _credentials );
        }
        if( model != null )
        {
          string json = JsonConvert.SerializeObject( model, Formatting.None );
          request.Content = new ByteArrayContent( Encoding.UTF8.GetBytes( json ) );
          request.Content.Headers.Add( "Content-Type", "application/json" );
        }
        return await client.SendAsync( request, HttpCompletionOption.ResponseContentRead );
      }
    }

    public HttpResponseMessage ExecuteRequest( HttpMethod method, string path, object model = null )
    {
      return ExecuteRequestAsync( method, path, model ).Result;
    }

    public async Task<T> GetResponseContentAsync<T>( HttpResponseMessage response )
    {
      return JsonConvert.DeserializeObject<T>( await response.Content.ReadAsStringAsync() );
    }

    public T GetResponseContent<T>( HttpResponseMessage response )
    {
      return GetResponseContentAsync<T>( response ).Result;
    }
  }
}
