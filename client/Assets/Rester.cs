using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;
using UnityEngine;

public class Rester : MonoBehaviour
{
	private class WWWFactory
	{
		public WWWFactory( string inURL, WWWForm inForm )
		{
			_URL = inURL;
			
			if( inForm != null )
			{
				_Data = inForm.data;
				if( _Data.Length > 0 )
				{
					_Headers = new Dictionary< string, string >();

					foreach( System.Collections.DictionaryEntry  keyPair in inForm.headers )
					{
						_Headers.Add( ( string ) keyPair.Key, ( string ) keyPair.Value );
					}
				}
				else
				{
					_Data = null;
					_Headers = new Dictionary< string, string >();
				}
			}
			else
			{
				_Data = null;
				_Headers = new Dictionary< string, string >();
			}
		}

		public WWWFactory( string inURL, byte[] inData, Dictionary< string, string > inHeaders )
		{
			_URL = inURL;
			_Data = inData;
			_Headers = inHeaders;
		}

		public WWW GetWWW( bool inShouldLogRequests )
		{
			if( inShouldLogRequests )
			{
				Debug.Log( "Request: " + _URL );
			}
			return new WWW( _URL, _Data, _Headers );
		}

		public bool IsLocal
		{
			get
			{
				return _URL != null && _URL.StartsWith( "file://" );
			}
		}

		private string _URL;
		private byte[] _Data;
		private Dictionary< string, string > _Headers;
	}

	public void Post( string inURL, JsonObject inJSON, Action< string, string > inCallback )
	{
		var headers = new Dictionary< string, string >();
		headers[ "Content-Type" ] = "application/json";

		Post( inURL, System.Text.Encoding.UTF8.GetBytes( inJSON.ToString() ), headers, inCallback );
	}

	public void Post( string inURL, JsonObject inJSON, Action< string, string, WWW > inCallback )
	{
		var headers = new Dictionary< string, string >();
		headers[ "Content-Type" ] = "application/json";

		Post( inURL, System.Text.Encoding.UTF8.GetBytes( inJSON.ToString() ), headers, inCallback );
	}

	public void Post( string inURL, WWWForm inForm,  Action< string, string > inCallback )
	{
		StartCoroutine( DoRequest( new WWWFactory( inURL, inForm ), inRequestType, ( string inError, string inResult, WWW inWWW )=>
		{
			if( inCallback != null ) { inCallback( inError, inResult ); }
		} ) );
	}

	public void Post( string inURL, byte[] inData, Dictionary< string, string > inHeaders,  Action< string, string > inCallback )
	{
		StartCoroutine( DoRequest( new WWWFactory( inURL, inData, inHeaders ), inRequestType, ( string inError, string inResult, WWW inWWW ) =>
		{
			if( inCallback != null ) { inCallback( inError, inResult ); }
		} ) );
	}

	public void Post( string inURL, byte[] inData, Dictionary< string, string > inHeaders, Action< string, string, WWW > inCallback )
	{
		StartCoroutine( DoRequest( new WWWFactory( inURL, inData, inHeaders ), inRequestType, inCallback ) );
	}

	public void Get( string inURL, Dictionary< string, string > inHeaders,  Action< string, string, WWW > inCallback )
	{
		StartCoroutine( DoRequest( new WWWFactory( inURL, null, inHeaders ), inRequestType, inCallback ) );
	}

	private IEnumerator DoRequest( WWWFactory inWWWFactory, Action< string, string, WWW > inCallback )
	{
		string err = string.Empty;
		string text = string.Empty;

		WWW www = null;
	
		//Debug.Log ("Do PostRequest");
		if( Application.internetReachability != NetworkReachability.NotReachable || inWWWFactory.IsLocal )
		{
			www = inWWWFactory.GetWWW( ShouldLogRequests );
			yield return www;
			
			if( !www.isDone )
			{
				err = "Can not connect";
				text = string.Empty;
			}
			else
			{
				err = www.error;

				bool hasErr = !string.IsNullOrEmpty( err ) ;

				if( hasErr && err.StartsWith( "304" ) )
				{
					err = string.Empty;
					text = string.Empty;
				}
				else
				{
					text = !hasErr ? www.text : null;
				}
			}
		}
		else
		{
			err = "No Internet Connection!";
			text = string.Empty;
		}

		inCallback( err, "", www );
	}
}
