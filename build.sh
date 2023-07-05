#!/bin/bash

# Build the Docker image
docker build -t carbooking-image .

# Run a container based on the built image
docker run -d --name carbooking-container -p 5000:80 carbooking-image