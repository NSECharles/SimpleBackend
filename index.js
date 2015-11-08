"use strict";

// EXPRESS INIT / HELLO WORLD
var express = require( 'express');

var app = express();

app.get( '/hello', function( req,res ) 
{
	res.send("Hello World!");
});

app.listen( process.env.PORT || 3000 );	

/// MONGO INIT
var mongodb = require( 'mongodb');
var mongo = mongodb.MongoClient;
var db;

var test = { foo : "bar", };

mongo.connect( 'mongodb://localhost:27017/test', ( err, inDB ) =>
{
	db = inDB;
	if ( !err )
	{
		console.log( 'connected!');
	}
});


// SAVESTUFF
app.post( '/stuff', ( req, res ) =>
{
	var stuff = db.collection( 'stuff');
	stuff.insert( req.body, ( err, data ) =>
	{
		// todo, res.send _id
	} );
});
