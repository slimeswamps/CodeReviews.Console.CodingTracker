# Coding Tracker
Console based C# using Dapper ORM to access a database

Project from: [C# Academy](https://thecsharpacademy.com/)

## Requirements:
- [x] This application has the same requirements as the previous project, except that now you'll be logging your daily coding time.
- [x] To show the data on the console, you should use the Spectre.Console library.
- [x] You're required to have separate classes in different files (i.e. UserInput.cs, Validation.cs, CodingController.cs)
- [x] You should tell the user the specific format you want the date and time to be logged and not allow any other format.
- [x] You'll need to create a configuration file called appsettings.json, which will contain your database path and connection strings (and any other configs you might need).
- [x] You'll need to create a CodingSession class in a separate file. It will contain the properties of your coding session: Id, StartTime, EndTime, Duration. When reading from the database, you can't use an anonymous object, you have to read your table into a List of CodingSession.
- [x] The user shouldn't input the duration of the session. It should be calculated based on the Start and End times
- [x] The user should be able to input the start and end times manually.
- [x] You need to use Dapper ORM for the data access instead of ADO.NET. (This requirement was included in Feb/2024)
- [x] Follow the DRY Principle, and avoid code repetition.
- [x] Don't forget the ReadMe explaining your thought process.

## Features:
* Database
  - Using a SQLite database
  - If the database doesn't exist it is created at the start
  - Using [Dapper ORM](https://www.learndapper.com/) to interact with the database

* UI
  - Using [Spectre.Console](https://spectreconsole.net/) for a nicer user experince
  -Main menu:

    <img width="363" height="216" alt="image" src="https://github.com/user-attachments/assets/8732e719-4499-41d8-96c5-f87fbbd784af" />

* Viewing records
  - Options to filter results

      <img width="577" height="173" alt="image" src="https://github.com/user-attachments/assets/300345ef-65ef-4b8c-9c26-471bc68b6350" />

  - Presents the results in a table

      <img width="772" height="320" alt="image" src="https://github.com/user-attachments/assets/2f92872b-c245-4285-ab24-e7b8c9666be3" />
      
* Adding records
    - Entering data such as start time, end time, and date
    - Offering current time/date as automatic options
    - Input validation to ensure everything is in the right format
   
        <img width="926" height="230" alt="image" src="https://github.com/user-attachments/assets/8a653e7c-e4eb-4616-8870-3266ad04a500" />

* Updating records
  - Asking user what they would like to update

      <img width="368" height="115" alt="image" src="https://github.com/user-attachments/assets/cd343428-ddf5-4bc1-8630-88a5db9d085e" />

  - Asking the user which record to update
 
      <img width="978" height="206" alt="image" src="https://github.com/user-attachments/assets/aa3e790c-64c5-476b-8c8e-d4efdbec9300" />

  - Asking the user what the new value is
  - Input validation to ensure everything is in the right format
  - Automatically updating the duration if the start/endtime has been updated
 
      <img width="667" height="121" alt="image" src="https://github.com/user-attachments/assets/39f5d0e1-3e6b-4f03-9690-ed04614e3078" />

* Deleting records
  - Askinking the user which record to delete
 
      <img width="988" height="217" alt="image" src="https://github.com/user-attachments/assets/4654e782-3bc4-401a-a6d1-010beb35a7d9" />

  - Making sure that the user wants to delete it
 
      <img width="698" height="27" alt="image" src="https://github.com/user-attachments/assets/91140334-95e7-4f55-8252-4e4bf882d53f" />

## My thoughts
* Challenges
  - At the beginning I found working with [Dapper ORM](https://www.learndapper.com/) and [Spectre.Console](https://spectreconsole.net/) rather difficult however using the documentation I was quickly able to get a hand of it
* Improvements
  - I think that much more of the code could be in methords to decrease repetition

## Resources used
 - [Project information](https://thecsharpacademy.com/project/13/coding-tracker)
 - [Dapper ORM](https://www.learndapper.com/)
 - [Spectre.Console](https://spectreconsole.net/)
