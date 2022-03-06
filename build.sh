#!/bin/bash

(cd libpts; make all;)
mkdir build
cp libpts/libpts.so build
dotnet build -c Release

cp SerialOverWebsocketClient/bin/Release/net6.0/{*.dll,*.runtimeconfig.json} build
#tar 
