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
        Console.WriteLine("The connection string contains the wrong username or password.");
    }

    public void ShowDatabaseErrorMessage()
    {
        Console.WriteLine("There seems to be a problem with the database right now, try again in a minute.");
    }

    public void ShowCustomerList(List<Customer> customers)
    {
        if (customers.Count == 0)
        {
            Console.WriteLine("No customers found.");
        }
        else
        {
            Console.WriteLine("=== Customer List ===");
            for (int i = 0; i < customers.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {customers[i].Name}, {customers[i].PhoneNumber}");
            }
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
            Console.Write("Choose a customer: ");
            string? input = Console.ReadLine();
            if (input == null)
            {
                throw new EndOfStreamException("The option could not be read.");
            }
            if (int.TryParse(input, out int number))
            {
                if (number == GoBackToMenu)
                {
                    return null;
                }
                if (number <= customers.Count && number >= FirstPosition)
                {
                    return customers[number - FirstPosition];
                }
            }
            _lastMessage = "Invalid option choice.";
        }
    }

    public void ShowCurrentCustomer(Customer customer)
    {
        Console.WriteLine($"Current customer: {customer.Name}");
    }
}