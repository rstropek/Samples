#!/bin/bash

DISPLAY_NAME="AzureADForDevsClient"
APP_IDENTIFIER=""
SILENT=""
SPA_REDIRECT=""
WEB_REDIRECT=""
CREATE_SP=""
APP_SECRET=""
ADD_SCOPES=""

usage() {
  echo "Usage: $0 [ -d DISPLAY_NAME ] [ -i ] [ -r SPA_REDIRECT ] [ -m ] [ -s ] [ -h ] [ -p ] [ -c ] [ -o ]
      -p ... Create service principal, too
      -s ... Silent
      -i ... Set API identifier
      -m ... Add web redirect for Postman
      -c ... Add client secret to app
      -o ... Add OAuth scopes
      -h ... Help usage" 1>&2 
}

exit_abnormal() {
  usage
  exit 1
}

while getopts "shd:ir:pmco" options; do  
    case "${options}" in
        d)
            DISPLAY_NAME=${OPTARG}
            ;;
        i)
            APP_IDENTIFIER="y"
            ;;
        h)
            usage
            exit 0
            ;;
        s)
            SILENT="s"
            ;;
        r)
            SPA_REDIRECT=${OPTARG}
            ;;
        p)
            CREATE_SP="y"
            ;;
        m)
            WEB_REDIRECT="https://oauth.pstmn.io/v1/callback"
            ;;
        c)
            APP_SECRET="y"
            ;;
        o)
            ADD_SCOPES="y"
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
    if [ -z "$SILENT" ]; then echo No app with this display name exists, creating one; fi
    AADAPPREG=$(az ad app create --display-name $DISPLAY_NAME --sign-in-audience AzureADMyOrg)
else
    # App registration exists -> we do not need to create a new one
    if [ -z "$SILENT" ]; then echo App registration already exists, getting registration data; fi
    AADAPPREG=$(echo $AADAPPREG | jq '.[0]')
fi
# Get app id and object id from app registration JSON
APPID=$(jq '.appId' -r <<< $AADAPPREG)
OBJID=$(jq '.id' -r <<< $AADAPPREG)

if [[ ! -z "$APP_IDENTIFIER" ]]
then
    if [ -z "$SILENT" ]; then echo Updating app identifier; fi
    az ad app update --id $APPID --identifier-uris api://$APPID
fi

if [[ ! -z "$SPA_REDIRECT" ]]
then
    if [ -z "$SILENT" ]; then echo Setting SPA redirect URI; fi

    # Set redirect URI for SPA app (with retry logic because AAD does not accept
    # this update immediately after AAD app has been registered; takes a few secs)
    until az rest --method PATCH --uri https://graph.microsoft.com/v1.0/applications/$OBJID \
        --headers 'Content-Type=application/json' --body "{spa:{redirectUris:['$SPA_REDIRECT']}}" \
        > /dev/null 2>&1; do
        if [ -z "$SILENT" ]; then echo Failed, retrying in a few seconds; fi
        sleep 5s
    done
fi

if [[ ! -z "$WEB_REDIRECT" ]]
then
    if [ -z "$SILENT" ]; then echo Setting WEB redirect URI; fi

    # Set redirect URI for WEB apps (with retry logic because AAD does not accept
    # this update immediately after AAD app has been registered; takes a few secs)
    until az rest --method PATCH --uri https://graph.microsoft.com/v1.0/applications/$OBJID \
        --headers 'Content-Type=application/json' --body "{web:{redirectUris:['$WEB_REDIRECT']}}" \
        > /dev/null 2>&1; do
        if [ -z "$SILENT" ]; then echo Failed, retrying in a few seconds; fi
        sleep 5s
    done
fi

if [[ ! -z "$CREATE_SP" ]]
then
    # Check if service principal exists
    AADSP=$(az ad sp list --display-name $DISPLAY_NAME)
    if [ $(echo $AADSP | jq length) == '0' ]
    then
        # Service principal does not exist -> create it
        if [ -z "$SILENT" ]; then echo Service principal does not exist, creating it; fi
        AADSP=$(az ad sp create --id $APPID)
    else
        # Service principal exists -> we do not need to create a new one
        if [ -z "$SILENT" ]; then echo Service principal already exists; fi
        AADSP=$(echo $AADSP | jq '.[0]')
    fi
fi

if [[ ! -z "$APP_SECRET" ]]
then
    # Reset app secret to get a new one
    if [ -z "$SILENT" ]; then echo Reset app secret to get a new one; fi
    APPCRED=$(az ad app credential reset --id $APPID --display-name "Autogen" --only-show-errors)
    APPSECRET=$(jq '.password' -r <<< $APPCRED)
fi

if [[ ! -z "$ADD_SCOPES" ]]
then
    if [ -z "$SILENT" ]; then echo Setting OAuth2 permission scopes; fi

    # Set scopes based on JSON file
    az rest --method PATCH --uri https://graph.microsoft.com/v1.0/applications/$OBJID \
        --headers 'Content-Type=application/json' --body "{'api': {'oauth2PermissionScopes':$(<read-write-scope.json)}}"
fi

# Get current AAD tenant
TENANT=$(az account show | jq '.tenantId' -r)

jq --arg key0 'appId' --arg value0 $APPID \
   --arg key1 'objectId' --arg value1 $OBJID \
   --arg key2 'tenant' --arg value2 $TENANT \
   --arg key3 'appSecret' --arg value3 ${APPSECRET:-""} \
    '. | .[$key0]=$value0 | .[$key1]=$value1 | .[$key2]=$value2 | .[$key3]=$value3' \
   <<<'{}'