using MySqlConnector;
class HolidayMakerApp
{
    private readonly ConsoleUI _ui;
    private readonly HolidayMakerRepository _repository;
    private Customer? _currentCustomer = null;

    public HolidayMakerApp(ConsoleUI ui, HolidayMakerRepository repository)
    {
        _ui = ui;
        _repository = repository;
    }

    public async Task RunAsync()
    {
        bool isRunning = true;

        while (isRunning)
        {
            MenuChoice choice = _ui.AskForMenuChoice(_currentCustomer);
            try
            {
                switch (choice)
                {
                    case MenuChoice.IdentifyCustomer:
                        {
                            await IdentifyCustomerAsync();
                            break;
                        }
                    case MenuChoice.SearchAvailableRooms:
                        {
                            await SearchAvailableRoomsAsync();
                            break;
                        }
                    case MenuChoice.ShowCustomerBookings:
                        {
                            await ShowCustomerBookingsAsync();
                            break;
                        }
                    case MenuChoice.ChangeGuestCount:
                        {
                            _ui.ShowNotImplementedMessage();
                            break;
                        }
                    case MenuChoice.CancelBooking:
                        {
                            _ui.ShowNotImplementedMessage();
                            break;
                        }
                    case MenuChoice.Quit:
                        {
                            isRunning = false;
                            break;
                        }
                    default:
                        throw new ArgumentOutOfRangeException(
                            nameof(choice), choice, "Unknown choice.");
                }
            }
            catch (MySqlException ex) when (ex.ErrorCode == MySqlErrorCode.AccessDenied)
            {
                _ui.ShowAccessDeniedMessage();
            }
            catch (MySqlException)
            {
                _ui.ShowDatabaseErrorMessage();
            }
        }
    }

    private async Task IdentifyCustomerAsync()
    {
        CustomerMenuChoice choice = _ui.AskForCustomerMenuChoice();
        switch (choice)
        {
            case CustomerMenuChoice.New:
                {
                    await RegisterCustomerAsync();
                    break;
                }
            case CustomerMenuChoice.Existing:
                {
                    await SelectCustomerAsync();
                    break;
                }
            case CustomerMenuChoice.Back:
                {
                    break;
                }
            default:
                throw new ArgumentOutOfRangeException(
                    nameof(choice), choice, "Unknown choice.");
        }
    }

    private async Task SelectCustomerAsync()
    {
        List<Customer> customers = await _repository.GetCustomersAsync();

        if (customers.Count == 0)
        {
            _ui.ShowEmptyCustomerListMessage();
            return;
        }

        Customer? chosenCustomer = _ui.AskForCustomerChoice(customers);
        if (chosenCustomer != null)
        {
            _currentCustomer = chosenCustomer;
        }
    }

    private async Task RegisterCustomerAsync()
    {
        string name = _ui.AskForCustomerName();
        string phoneNumber = _ui.AskForCustomerPhoneNumber();
        string? email = _ui.AskForCustomerEmail();
        try
        {
            Customer newCustomer = await _repository.AddCustomerAsync(name, phoneNumber, email);
            _currentCustomer = newCustomer;
            _ui.ShowCustomerAddedMessage(newCustomer);
        }
        catch (MySqlException ex) when (ex.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
        {
            _ui.ShowDuplicateCustomerMessage();
        }
    }

    private async Task SearchAvailableRoomsAsync()
    {
        List<City> cities = await _repository.GetCitiesAsync();
        City? chosenCity = _ui.AskForCityChoice(cities);
        if (chosenCity is null)
        {
            return;
        }
        RoomSearch roomSearch = _ui.AskForRoomSearch(chosenCity);
        List<AvailableRoom> availableRooms = await _repository.SearchAvailableRoomsAsync(roomSearch);
        if (availableRooms.Count == 0)
        {
            _ui.ShowNoAvailableRoomsMessage();
            return;
        }
        AvailableRoom? chosenRoom = _ui.AskForAvailableRoomChoice(availableRooms);
        if (chosenRoom is null)
        {
            return;
        }
        Customer? customer = await RequireCustomerAsync();
        if (customer is null)
        {
            return;
        }
        BookingConfirmation? confirmation = await _repository.BookRoomAsync(customer.Id, chosenRoom.RoomId, roomSearch.CheckIn, roomSearch.CheckOut, roomSearch.Guests);
        if (confirmation is null)
        {
            _ui.ShowAlreadyBookedRoomMessage();
            return;
        }
        _ui.ShowBookingConfirmation(confirmation, chosenRoom, roomSearch);
    }

    private async Task ShowCustomerBookingsAsync()
    {
        Customer? customer = await RequireCustomerAsync();
        if (customer is null)
        {
            return;
        }
        List<Booking> bookings = await _repository.GetCustomerBookingsAsync(customer.Id);
        if (bookings.Count == 0)
        {
            _ui.ShowNoBookingsMessage();
            return;
        }
        _ui.ShowCustomerBookings(bookings, customer);
    }

    private async Task<Customer?> RequireCustomerAsync()
    {
        if (_currentCustomer is null)
        {
            _ui.ShowMissingCustomerMessage();
            await IdentifyCustomerAsync();
        }
        return _currentCustomer;
    }
}