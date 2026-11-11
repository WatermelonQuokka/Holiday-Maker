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

    public async Task<List<City>> GetCitiesAsync()
    {
        List<City> cities = new();

        await using MySqlConnection connection = await OpenConnectionAsync();

        await using MySqlCommand cityListCommand = new
        ("""
        SELECT cities.id, cities.name
        FROM cities
        ORDER BY cities.name;
        """, connection);

        await using MySqlDataReader cityListReader = await cityListCommand.ExecuteReaderAsync();

        while (await cityListReader.ReadAsync())
        {
            City city = new(
                cityListReader.GetUInt32("id"),
                cityListReader.GetString("name"));
            cities.Add(city);
        }
        return cities;
    }

    public async Task<List<AvailableRoom>> SearchAvailableRoomsAsync(RoomSearch search)
    {
        List<AvailableRoom> availableRooms = new();

        await using MySqlConnection connection = await OpenConnectionAsync();

        string orderBy = search.SortOrder switch
        {
            RoomSortOrder.LowestPrice => "rooms.price_per_night ASC, accommodations.rating DESC",
            RoomSortOrder.HighestRating => "accommodations.rating DESC, rooms.price_per_night ASC",
            _ => throw new ArgumentOutOfRangeException(nameof(search.SortOrder), search.SortOrder, "Does not exist in Sort Order enum.")
        };

        await using MySqlCommand availableRoomCommand = new(
        $"""
        SELECT rooms.id,
            accommodations.name AS accommodation_name,
            cities.name AS city_name,
            rooms.name AS room_name,
            rooms.max_capacity AS guest_capacity,
            rooms.price_per_night,
            accommodations.distance_to_beach,
            accommodations.distance_to_center,
            accommodations.rating
        FROM rooms
        JOIN accommodations ON rooms.accommodation_id = accommodations.id
        JOIN cities ON accommodations.city_id = cities.id
        WHERE cities.id = @city_id
            AND rooms.max_capacity >= @guests
            AND (@pool_required = FALSE OR accommodations.has_pool = TRUE)
            AND (@restaurant_required = FALSE OR accommodations.has_restaurant = TRUE)
            AND (@max_distance_to_beach IS NULL OR accommodations.distance_to_beach <= @max_distance_to_beach)
            AND (@max_distance_to_center IS NULL OR accommodations.distance_to_center <= @max_distance_to_center)
            AND NOT EXISTS (
                SELECT 1
                FROM bookings
                WHERE bookings.room_id = rooms.id
                    AND @check_in < booking_end
                    AND @check_out > booking_start
                )
        ORDER BY {orderBy};
        """, connection);

        availableRoomCommand.Parameters.AddWithValue("@city_id", search.CityId);
        availableRoomCommand.Parameters.AddWithValue("@check_in", search.CheckIn);
        availableRoomCommand.Parameters.AddWithValue("@check_out", search.CheckOut);
        availableRoomCommand.Parameters.AddWithValue("@guests", search.Guests);
        availableRoomCommand.Parameters.AddWithValue("@pool_required", search.PoolRequired);
        availableRoomCommand.Parameters.AddWithValue("@restaurant_required", search.RestaurantRequired);
        availableRoomCommand.Parameters.AddWithValue("@max_distance_to_beach", search.MaxDistanceToBeach);
        availableRoomCommand.Parameters.AddWithValue("@max_distance_to_center", search.MaxDistanceToCenter);

        await using MySqlDataReader availableRoomReader = await availableRoomCommand.ExecuteReaderAsync();

        while (await availableRoomReader.ReadAsync())
        {
            AvailableRoom availableRoom = new(
                availableRoomReader.GetUInt32("id"),
                availableRoomReader.GetString("accommodation_name"),
                availableRoomReader.GetString("city_name"),
                availableRoomReader.GetString("room_name"),
                availableRoomReader.GetUInt32("guest_capacity"),
                availableRoomReader.GetUInt32("price_per_night"),
                availableRoomReader.GetUInt32("distance_to_beach"),
                availableRoomReader.GetUInt32("distance_to_center"),
                availableRoomReader.GetInt32("rating"));
            availableRooms.Add(availableRoom);
        }
        return availableRooms;
    }
}