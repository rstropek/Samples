#!/bin/sh
curl DdoWebPrivate.azurewebsites.net
echo 
nslookup DdoWebPrivate.azurewebsites.net
echo 
curl http://10.0.0.4/foo
echo 
curl http://10.0.0.4/foo -H "Host: ddowebprivate.azurewebsites.net"
echo 
