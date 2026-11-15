# Reflection

## Technical choices
<!-- Why did you organise the database and its relationships the way you did?
     How did you solve searching and booking with SQL and C#, and why? -->
- The database is organised and structured this way with its relationships because of not duplicating data since one customer can have many bookings and one room can be booked for many periods.
- I chose to do the total calculation in SQL because that way it can't disagree with the dates however the trade-off is that a price change would affect old bookings. 
- The searching and booking was solved in SQL with a collision rule `check_in < booking_end AND check_out > booking_start` which was worked out from test cases. A new booking can start on the checkout day because its check-in isn't earlier than the existing booking's end. In the query I also chose to use `NOT EXISTS` instead of a `JOIN` for the search because a `JOIN` shows a room as free if any one booking doesn't collide, and drops rooms with no bookings.
- In C#, I put the booking at the end of the search because then no room, dates or guests are stored between menu choices, only the current customer is. Another alternative would have been having a separate book option but that would have required storing data that could be wrong if not reset properly.
## Security and data integrity
<!-- How have your decisions affected the handling of input, booking rules
     and correct information? Are there limitations? -->
- The input is handled by parameterised SQL because protection against SQL injection is very important else user text becomes part of the SQL statement which can manipulate data in a not intended way. There was one exception I chose which is the sort order. `ORDER BY` can't be a parameter, so my choice was an enum that picks a fixed string. It is a safe way because the SQL text comes from two strings written in the code so nothing the operator types reaches the SQL.
- To prevent double booking I chose transaction with `FOR UPDATE` because between the check and the `INSERT` there is a small window where a double booking can happen. My testing with DataGrip and the program simultaneously showed that the room gets locked so the double booking gets prevented however there is a limitation that can occur which would be a deadlock. A deadlock means that MySQL rolls back one of the two bookings, so a double booking is still prevented, but that operator gets the general error message. The brief requires availability to be checked again when saving, and I went one step further with a lock.
- Privileges for the operator were also kept at the least what the operator should be possible to do because if something would go wrong with a bug or misuse, the program **can't** drop tables or change towns, accommodations, existing customers and rooms. 
- I chose environment variable because the repo is public and that way the connection string is not inside the code.
- The capacity check exists in two layers, there is a check in C# that gives a clear message and in MySQL the conditional `UPDATE` is a guarantee. A CHECK constraint would not be possible because the values are in different tables.
- **Limitations:** The phone format and name format for customer is not validated.
## Result and improvements
<!-- How well does the solution meet the requirements? What would you do
     differently, or prioritise, with more time, and why? -->
- All six tasks are met and tested which can be checked in `docs/Test-notes.md`
- With more time I would want to use one generic method to my list choices in C# because of their similarities they feel like duplication. I didn't do it though because generics are new for me and my prioritisation was a working and explainable solution. Catching the deadlock in case that occurs so that the operator gets a clear message ("the room was just booked by someone else") instead of the general database error. I would like as well to store the current agreed price on the booking because price can fluctuate which should not affect old bookings.
## AI and other help
<!-- Briefly describe whether and how you used AI or other help during the work,
     and how you checked the result. -->
- **AI:** AI was used as a tutor for hints and questions however the schema, queries and C# code were written by me. The test data names, the 24 extra test bookings and README text were produced by AI at my request. The README was checked line by line against the code. The test data was checked with a `COUNT` query (25 bookings) and the capacity query (0 over capacity). The transaction with `FOR UPDATE` was suggested and I **tested** the lock with DataGrip instead of trusting it.
- **Other help:** The group made the conceptual E/R diagram and my database diagram is generated from my own final schema with for example `guest_count` added. 
- **Literature:** Berglund, B. (2026). *Studera SQL – prata med data: Datafilosofi och
praktisk implementation* (NH2026-3). Node Hill Research Laboratory.
https://weehorse.github.io/Studera-SQL-prata-med-data/sv/
It was my main source for learning SQL and MySQL in this course and this project is the result of applying what I've learned to my own design.
