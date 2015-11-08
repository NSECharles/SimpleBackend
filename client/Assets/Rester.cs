using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;
using UnityEngine;

public class Rester : MonoBehaviour
{
	public bool ShouldLog;
	
	
	public void PostJSON( string inURL, JsonObject inJSON, Action< string, JsonObject > inCallback )
	{
		var headers = new Dictionary< string, string >();
		headers[ "Content-Type" ] = "application/json";

		var jsonString = SimpleJson.SimpleJson.SerializeObject( inJSON );
		Post( inURL, System.Text.Encoding.UTF8.GetBytes( jsonString ), headers, ( string inError, WWW inWWW ) =>
		{
			if( inCallback != null )
			{
				JsonObject obj = null;
				if ( string.IsNullOrEmpty( inError ) && ! string.IsNullOrEmpty( inWWW.text ) )
				{
					if ( ShouldLog )
					{
						Debug.Log("Response: " + inWWW.text );
					}
										
					object parsedObj = null;
					if ( SimpleJson.SimpleJson.TryDeserializeObject( inWWW.text, out parsedObj ) )
					{
						obj = ( JsonObject ) parsedObj; 
					}
				}
				
				inCallback( inError, obj ); 
			}
		} );
	}


	public void Post( string inURL, byte[] inData, Dictionary< string, string > inHeaders,  Action< string, WWW > inCallback )
	{
		StartCoroutine( DoRequest( new WWWFactory( inURL, inData, inHeaders ), inCallback ) );
	}
	
	public void GetJSON( string inURL, Action< string, JsonObject > inCallback )
	{
		Get( inURL, ( string inError, WWW inWWW ) =>
		{
			if( inCallback != null )
			{
				JsonObject obj = null;
				if ( string.IsNullOrEmpty( inError ) && ! string.IsNullOrEmpty( inWWW.text ) )
				{
					if ( ShouldLog )
					{
						Debug.Log("Response: " + inWWW.text );
					}
										
					object parsedObj = null;
					if ( SimpleJson.SimpleJson.TryDeserializeObject( inWWW.text, out parsedObj ) )
					{
						obj = ( JsonObject ) parsedObj; 
					}
				}
				
				inCallback( inError, obj ); 
			}
		} );
	}
	

	public void Get( string inURL, Dictionary< string, string > inHeaders,  Action< string, WWW > inCallback )
	{
		StartCoroutine( DoRequest( new WWWFactory( inURL, null, inHeaders ), inCallback ) );
	}
	
	public void Get( string inURL, Action< string, WWW > inCallback )
	{
		var headers = new Dictionary< string, string >();
		StartCoroutine( DoRequest( new WWWFactory( inURL, null, headers ),( string inError,  WWW inWWW ) =>
		{
			if( inCallback != null ) { inCallback( inError, inWWW ); }
		} ) );
	}

	private IEnumerator DoRequest( WWWFactory inWWWFactory, Action< string, WWW > inCallback )
	{
		string err = string.Empty;

		WWW www = null;
	
		//Debug.Log ("Do PostRequest");
		if( Application.internetReachability != NetworkReachability.NotReachable || inWWWFactory.IsLocal )
		{
			www = inWWWFactory.GetWWW( ShouldLog );
			yield return www;
			
			if( !www.isDone )
			{
				err = "Can not connect";
			}
			else
			{
				err = www.error;

				bool hasErr = !string.IsNullOrEmpty( err ) ;

				if( hasErr && err.StartsWith( "304" ) )
				{
					err = string.Empty;
				}
			}
		}
		else
		{
			err = "No Internet Connection!";
		}

		inCallback( err, www );
	}
	
	
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
}
