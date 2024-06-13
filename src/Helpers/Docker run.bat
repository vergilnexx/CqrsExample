call docker network create --driver=bridge tombstone
cd ..

call docker-compose -f docker-compose.yml up -d
pause