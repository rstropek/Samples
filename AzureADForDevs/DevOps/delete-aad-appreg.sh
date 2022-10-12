#!/bin/bash

DISPLAY_NAME="AzureADForDevsClient"

usage() {
  echo "Usage: $0 [ -d DISPLAY_NAME ] [ -s ] [ -h ]
      -s ... Silent
      -h ... Help usage" 1>&2 
}

exit_abnormal() {
  usage
  exit 1
}

while getopts "shd:i:r:p" options; do  
    case "${options}" in
        d)
            DISPLAY_NAME=${OPTARG}
            ;;
        h)
            usage
            exit 0
            ;;
        s)
            SILENT="s"
            ;;
        *)
            exit_abnormal
            ;;
    esac
done

# Check if app registration exists
AADAPPREG=$(az ad app list --display-name $DISPLAY_NAME)
if [ $(echo $AADAPPREG | jq length) == '0' ]
then
    # App registration does not exist -> create it
    if [ -z "$SILENT" ]; then echo No app with this display name exists; fi
else
    # App registration exists -> we do not need to create a new one
    if [ -z "$SILENT" ]; then echo App registration exists, deleting it; fi
    AADAPPREG=$(echo $AADAPPREG | jq '.[0]')
    APPID=$(jq '.appId' -r <<< $AADAPPREG)
    az ad app delete --id $APPID
fi
