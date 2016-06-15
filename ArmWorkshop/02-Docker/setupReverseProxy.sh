apt-get -qqy install nginx
curl https://raw.githubusercontent.com/rstropek/Samples/master/ArmWorkshop/02-Docker/default -o /etc/nginx/sites-enabled/default
nginx -s reload
