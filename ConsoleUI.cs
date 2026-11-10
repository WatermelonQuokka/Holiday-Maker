class ConsoleUI
{
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
}