class ConsoleUI
{
    private string? _lastMessage;
    private const int FirstPosition = 1;
    private const int GoBackToMenu = 0;
    public void ShowMissingConnectionVariable(string variableName)
    {
        Console.WriteLine($"The environment variable {variableName} is missing or empty.");
    }

    public void ShowAccessDeniedMessage()
    {
        _lastMessage = "The connection string contains the wrong username or password.";
    }

    public void ShowDatabaseErrorMessage()
    {
        _lastMessage = "There seems to be a problem with the database right now, try again in a minute.";
    }

    private void ShowCustomerList(List<Customer> customers)
    {
        Console.WriteLine("=== Customer List ===");
        for (int i = 0; i < customers.Count; i++)
        {
            Console.WriteLine($"{i + FirstPosition}. {customers[i].Name}, {customers[i].PhoneNumber}");
        }
    }

    public Customer? AskForCustomerChoice(List<Customer> customers)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine(_lastMessage);
            _lastMessage = null;
            ShowCustomerList(customers);
            Console.WriteLine($"Enter {GoBackToMenu} to go back to the menu.");
            int? choice = ReadNumber("Choose a customer: ");
            if (choice == GoBackToMenu)
            {
                return null;
            }
            if (choice <= customers.Count
                && choice >= FirstPosition)
            {
                return customers[choice.Value - FirstPosition];
            }
            _lastMessage = "Invalid option choice.";
        }
    }


    private void ShowCurrentCustomer(Customer? customer)
    {
        Console.WriteLine(customer == null
        ? "No customer chosen."
        : $"Current customer: {customer.Name}");
    }

    private void ShowMenu(Customer? currentCustomer)
    {
        Console.WriteLine("""
        === Holiday Maker ===
        
        """);
        ShowCurrentCustomer(currentCustomer);
        Console.WriteLine(
        """

        1. Identify customer
        2. Search free rooms with filter
        3. Show current customer's bookings
        4. Change guest count on customer's booking
        5. Cancel current customer's booking
        0. Quit
        """);
    }

    public MenuChoice AskForMenuChoice(Customer? currentCustomer)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine(_lastMessage);
            _lastMessage = null;
            ShowMenu(currentCustomer);
            int? choice = ReadNumber("Choose an option: ");
            if (choice != null
                && Enum.IsDefined(typeof(MenuChoice), choice.Value))
            {
                return (MenuChoice)choice.Value;
            }
            _lastMessage = "Invalid option choice.";
        }
    }

    private string ReadText(string question)
    {
        Console.Write(question);
        string? input = Console.ReadLine();
        if (input == null)
        {
            throw new EndOfStreamException("The input could not be read.");
        }
        return input.Trim();
    }

    private int? ReadNumber(string question)
    {
        return int.TryParse(ReadText(question), out int number)
        ? number
        : null;
    }

    public void ShowNotImplementedMessage()
    {
        _lastMessage = "Not implemented yet";
    }

    public void ShowEmptyCustomerListMessage()
    {
        _lastMessage = "No customers found.";
    }
}