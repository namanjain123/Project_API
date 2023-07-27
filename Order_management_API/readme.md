## TechStack Used
<p>
<li> A .Net Core 6 Rest API</br></li>
<li> A Docker Based MSSQL server with persistent Storage for database</br></li>
<li> A Docker Based Redis For cache</br></li>
</p>


## Setup

<li>Start the docker using the 'docker-compose up' command</li>
<li>Then can start the API to run it</li>

## Database Structure used is 
<li>A customer table to store the customer data</li>
<li>A Item Table to store the items info and other details</li>
<li>A orderitem table where we store the ordered items info like the quantity and the itemid that is linked with item table</li>
<li>A order table with ordered item id , customer id and other details so we can map everything according to the order</li>

## Code Structure Used
<li>A API layer where all the calling is done mainly 2 controller namely customer and order</li>
<li>A Business Layer that is using IRepository Pattern and have the buisness logic</li>
<li>A Database Layer where database calling and setup is done in code first approach</li>
<li>API_Testing house the test of the Buisness logic unit test</li>
<li>Databbase layer also store the storage for SQL server and Cache folder house the Redis memory</li>

## Database Model
<li>SP used to get the page is also in the way with sql script in the script.sql</li>
You can use the swagger UI to get the input and output models