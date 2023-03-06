Feature: GifMaker

As a user of the GifMaker API
I want to be able to perform various actions related to Gifs
So that I can manage and view Gifs in the system


Scenario: Get Gifs by name containing a string
	Given the user has navigated to the API endpoint "/api/gifs?name=cat"
	And the user has some Gifs:
	| Name |
	| cat  |
	| rat  |
	| bat  |
	| cat2 |
	When the user performs a GET request
	Then the response should be a list of all Gif names whose name contains the string "cat"

Scenario: Get Gif by name
	Given the user has navigated to the API endpoint "/api/gifs/cat"
    And the user has some Gifs:
	| Name |
	| cat  |
    When the user performs a GET request
    Then the response should be the file for the Gif named "cat"

 Scenario: Create a new Gif
    Given the user has navigated to the API endpoint "/api/gifs"
    And the user has filled out the necessary Gif information in a form
    When the user performs a POST request with the form data
    Then the response should be a success status code
    And the new Gif should be added to the system

  Scenario: Create a Gif with an invalid file type
    Given the user has navigated to the API endpoint "/api/gifs"
    And the user has filled out the necessary Gif information in a form with an invalid file
    When the user performs a POST request with the form data
    Then the response should be a bad request status code

  Scenario: Create a Gif with a duplicate name
    Given the user has navigated to the API endpoint "/api/gifs"
    And the user has filled out the necessary Gif information in a form
    And a Gif with the same name already exists in the system
    When the user performs a POST request with the form data
    Then the response should be a conflict status code