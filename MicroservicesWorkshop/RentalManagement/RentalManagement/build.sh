#!/bin/bash

docker rm -f basta-bike-rental
docker rmi rstropek/basta-bike-rental
docker build -t rstropek/basta-bike-rental .
docker run -d --name basta-bike-rental -p 5000:5000 rstropek/basta-bike-rental

sleep 2
echo Webserver returns `curl http://localhost:5000/api/bikeRental -s`

docker rm -f basta-bike-rental
