# ASP.NET-WebAPI-Introduction
The project was created to get first insights into the ASP.NET framework. 
The basis for this was the data model with customers, articles and orders to provide an endpoint that simulates ordering processes.

Technologies used for this purpose: 
- EF Core 
-> migrated the database with the scaffold command.

- Caching 
-> Familiarized with the first concepts of caching. In-Memory-Caching and Distributed Caching (redis).  

- Authorization 
-> Dealing with the authentication options and used Json Webtokens.

- Validation 
-> validated input data with FluentValidator.

- Error Handling 
-> used the error handling practices of F#. Didn't throw the error, but outputted them as a result. (F# built-in Result type)

- Logging 
-> used the Serilog framework to log errors and specific datas into the Log directory.
