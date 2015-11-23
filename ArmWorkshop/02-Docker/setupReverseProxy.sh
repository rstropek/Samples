apt-get -qqy install nginx
curl https://raw.githubusercontent.com/rstropek/Battleship/master/AzureEnvironment/default -o /etc/nginx/sites-enabled/default
nginx -s reload
