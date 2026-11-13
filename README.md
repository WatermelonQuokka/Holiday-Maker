# Holiday Maker

A console application for a travel agency operator who books accommodation over the phone.
Built with C# (.NET 10) and MySQL, using MySqlConnector.

The operator can:

1. Identify a customer (choose an existing one or register a new one)
2. Search available rooms in a town for given dates and number of guests, with optional filters
   (pool, restaurant, max distance to beach/center) and sorting (lowest price or highest rating),
   then book a chosen room
3. Show the current customer's bookings
4. Change the guest count on a booking (must fit the room)
5. Cancel a booking

## Requirements

- .NET 10 SDK
- MySQL 8 or later (for example running in Docker)
- A MySQL client to run the scripts, for example DataGrip or the `mysql` command line

## 1. Create the database

Connect to MySQL as **root** and run the scripts in the `sql` folder **in this order**:

| Order | Script | What it does |
|---|---|---|
| 1 | `schema.sql` | Creates the `holiday_maker` database if it doesn't exist, then (re)creates the tables |
| 2 | `testdata.sql` | Inserts towns, accommodations, rooms, customers and bookings |
| 3 | `user.sql` | Creates the `operator` user with limited privileges |

**Before running `user.sql`:** replace `CHANGE_ME` with a password of your choice.
`user.sql` must run **after** `schema.sql`, because it grants privileges on existing tables.
It only needs to run once. Rebuilding the tables with `schema.sql` keeps the privileges.

The `operator` user can only read towns, accommodations and rooms, read and add customers,
and read, add, change and delete bookings. The program only needs these privileges,
so it should never connect as root.

> The user is created as `'operator'@'%'` because MySQL running in Docker sees connections
> from the host as coming from the Docker network, not from `localhost`.

`queries.sql` contains every query the program uses, with test values. It is meant to be run
**one statement at a time**. Its INSERT, UPDATE and DELETE statements change data, so rebuild
with `schema.sql` and `testdata.sql` afterwards.

## 2. Set the connection string

The program reads the connection string from the environment variable **`HOLIDAY_MAKER_DB`**,
so no password is stored in the code.

On Windows, run this in a terminal (with your own values):

```
setx HOLIDAY_MAKER_DB "Server=localhost;Port=3306;Database=holiday_maker;User ID=operator;Password=YOUR_PASSWORD;"
```

Or set it under *Edit environment variables for your account* in Windows settings.

**Restart your terminal or VS Code afterwards.** Programs that are already open do not see
new environment variables. If the variable is missing, the program shows a message and exits.

## 3. Run the program

From the project folder:

```
dotnet run
```

It is best run in an ordinary terminal (Windows Terminal, PowerShell or cmd).
VS Code's built-in terminal only clears the visible part of the screen, so earlier output
stays in the scroll history.

Dates are entered as `yyyy-MM-dd`, for example `2026-12-10`. In the main menu, `0` quits.
In the customer menu and in every list, `0` goes back to the main menu.

## Project structure

| File | Responsibility |
|---|---|
| `Program.cs` | Reads the connection string, creates the objects and starts the app |
| `HolidayMakerApp.cs` | The menu loop and the flow of each operator task |
| `ConsoleUI.cs` | All input and output (the only class that uses `Console`) |
| `HolidayMakerRepository.cs` | All SQL, with parameterized commands (the only class that talks to MySQL) |
| `Customer.cs`, `City.cs`, `AvailableRoom.cs`, `Booking.cs`, `BookingConfirmation.cs`, `RoomSearch.cs` | Records for the data passed between the classes |
| `MenuChoice.cs`, `CustomerMenuChoice.cs`, `RoomSortOrder.cs` | Enums for menu and sort choices |
| `sql/` | Database scripts |
| `diagram/` | The group's conceptual E/R diagram (drawSQL) and the database diagram generated from the final schema (DataGrip) |
