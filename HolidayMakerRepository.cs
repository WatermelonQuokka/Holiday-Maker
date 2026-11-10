using MySqlConnector;

class HolidayMakerRepository
{
    private readonly string _connectionString;

    public HolidayMakerRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
}