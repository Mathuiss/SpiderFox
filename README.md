# SpiderFox
Cumulative link following web scraper

## How to use

### Modes
There are multiple modes in which the scraper can run
1. The scraper can run as many times as the user inputs
2. The scraper can run until the user stops the application

### Prompt
The user is first prompted to enter an entry point URL.
This sould be formatted like http://myentrypoint.com.
The user can of course choose any site he likes.
Secondly the user is prompted to enter the amount of scrapes the application has to perform.
If the user enters 0 the application will run untill the users stops.
Else the application runs untill the amount of scrapes is reached.

## How it works
The webscraper itself first does the initialisation of al static variables.
It then proceeds to ask and set the startup parameters.
Next it tries to download the web page that has been entered as entrypoint.
The application proceeds to scrape the page after the found links have been identified and given a certain id.
This id corresponds with the position in the queue this 'Discovery' has to be scraped itself.
The application saves the found links and asks for the next link in the queue.

## Planned
Fix the bug that causes discoveries to go up but not enter the database, and eventually causes the scraper to crash.

I'm planning to make the application multi threaded or at least do the downloading of the page and the actual scraping either asynchonously or concurrently.

Create logger static class with log levels.

Choose wether or not to continue with existing data or clean database and restart.
