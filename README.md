# GIF Maker

A simple API used to make GIF images with specific rules:
- The user needs to select a unique name for the new GIF file.
- The user needs to select at least two images to make the new GIF.
- Once the new GIF is made, the user will recieve the name of the new GIF in the response code.
- With the new name, the user can make a GET request with this name to see the newly created GIF.
- The user can also make a GET request with a partial name and get a list of all GIF names containing this partial input.

## How to run:

Pull the repository from GitHub and run the docker compose file located in the root folder with `docker-compose up`.
Once the docker container is up and running, navigate to http://localhost:8080/swagger/index.html to get to the Swagger page of the API.

## Project Structure

- Since the project is simple, it is made with one controller which has only 3 methods.
- The controller uses a repository for all the working logic, and a gif generator where the working logic for making gifs is written.
- These both can be replaced with other instances with dependency injection (for instance, instead of using the file storage, we can make a repository which stores data in a SQL or NonSQL Database, or if we have found a better library/solution for gif generating).
- The project also contains of integration tests, where we have a couple of scenarios for the API functionality.
