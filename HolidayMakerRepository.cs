using MySqlConnector;

class HolidayMakerRepository
{
    private readonly string _connectionString;

    public HolidayMakerRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    private async Task<MySqlConnection> OpenConnectionAsync()
    {
        MySqlConnection connection = new(_connectionString);
        await connection.OpenAsync();
        return connection;
    }

    public async Task<List<Customer>> GetCustomersAsync()
    {
        List<Customer> customers = new();

        await using MySqlConnection connection = await OpenConnectionAsync();

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

    public async Task<Customer> AddCustomerAsync(string name, string phoneNumber, string? email)
    {
        await using MySqlConnection connection = await OpenConnectionAsync();

        await using MySqlCommand addCustomerCommand = new("""
        INSERT INTO customers (name, phone_number, email)
        VALUES (@name, @phone_number, @email);
        """, connection);
        addCustomerCommand.Parameters.AddWithValue("@name", name);
        addCustomerCommand.Parameters.AddWithValue("@phone_number", phoneNumber);
        addCustomerCommand.Parameters.AddWithValue("@email", email);

        await addCustomerCommand.ExecuteNonQueryAsync();

        uint customerId = (uint)addCustomerCommand.LastInsertedId;

        return new Customer(customerId, name, phoneNumber);
    }
}