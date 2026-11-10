const string ConnectionVariableName = "HOLIDAY_MAKER_DB";

string? connectionString = Environment.GetEnvironmentVariable(ConnectionVariableName);

ConsoleUI ui = new();

if (string.IsNullOrWhiteSpace(connectionString))
{
    ui.ShowMissingConnectionVariable(ConnectionVariableName);
    return;
}

HolidayMakerRepository repository = new(connectionString);

List<Customer> customers = await repository.GetCustomersAsync();
ui.ShowCustomerList(customers);
