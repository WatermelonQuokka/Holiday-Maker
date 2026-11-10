using MySqlConnector;

const string ConnectionVariableName = "HOLIDAY_MAKER_DB";

string? connectionString = Environment.GetEnvironmentVariable(ConnectionVariableName);

ConsoleUI ui = new();

if (string.IsNullOrWhiteSpace(connectionString))
{
    ui.ShowMissingConnectionVariable(ConnectionVariableName);
    return;
}

HolidayMakerRepository repository = new(connectionString);

try
{
    List<Customer> customers = await repository.GetCustomersAsync();
    ui.ShowCustomerList(customers);
}
catch (MySqlException ex) when (ex.ErrorCode == MySqlErrorCode.AccessDenied)
{
    ui.ShowAccessDeniedMessage();
}
catch (MySqlException)
{
    ui.ShowDatabaseErrorMessage();
}