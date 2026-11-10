using MySqlConnector;

class HolidayMakerRepository
{
    private readonly string _connectionString;

    public HolidayMakerRepository(string connectionString)
    {
        _connectionString = connectionString;
    }


    public async Task<List<Customer>> GetCustomersAsync()
    {
        List<Customer> customers = new();

        await using MySqlConnection connection = new(_connectionString);
        await connection.OpenAsync();

        await using MySqlCommand customerListCommand = new
        ("""
        SELECT id, name, phone_number
        FROM customers
        ORDER BY name;
        """, connection);

        await using MySqlDataReader customerListReader = await customerListCommand.ExecuteReaderAsync();

        while (await customerListReader.ReadAsync())
        {
            Customer customer = new(
                customerListReader.GetUInt32("id"),
                customerListReader.GetString("name"),
                customerListReader.GetString("phone_number"));
            customers.Add(customer);
        }
        return customers;
    }
}