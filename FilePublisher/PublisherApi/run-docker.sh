docker run -d --restart=always -p 8081:80 \
    -v $(pwd)/sessions.json:/usr/src/app/sessions.json:ro \
    -v /home/rainer/live/basta2025/CSharpWorkshop/:/home/rainer/live/basta2025/CSharpWorkshop/:ro \
    --name publisherapi publisherapi
    