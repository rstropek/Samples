docker pull nginx

mkdir static-html-directory
echo DevOpsCon15 > static-html-directory/test.html

echo FROM nginx > Dockerfile
echo COPY static-html-directory /usr/share/nginx/html >> Dockerfile

docker build -t some-content-nginx .
docker run --name some-nginx -d -p 80:80 some-content-nginx