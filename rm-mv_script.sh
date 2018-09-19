#!/bin/bash

# to print working directory = pwd
# to echo command output echo $([command name here])
# to pause at end of script put in read

# navigate to publish directory
cd src/SSHConnectCore/bin/Release/netcoreapp2.0/publish

# delete app setting files
rm appsettings.json

# rename Release file to app settings
mv appsettings.Release.json appsettings.json