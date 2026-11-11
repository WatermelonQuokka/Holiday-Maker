using System.Collections;
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
                    case MenuChoice.SearchFreeRooms:
                        {
                            _ui.ShowNotImplementedMessage();
                            break;
                        }
                    case MenuChoice.ShowCustomerBookings:
                        {
                            _ui.ShowNotImplementedMessage();
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
                    _ui.ShowNotImplementedMessage();
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
}