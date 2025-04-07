docker run -d --restart=always -p 8081:8081 \
    -v $(pwd)/sessions.json:/usr/src/app/sessions.json:ro \
    -v /home/rainer/live/:/home/rainer/live/:ro \
    --name publisherapi publisherapi
    