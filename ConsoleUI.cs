class ConsoleUI
{
    private string? _lastMessage;
    private const int FirstPosition = 1;
    private const int GoBackToMenu = 0;
    private const int NameMaxLength = 64;
    private const int PhoneNumberMaxLength = 32;
    private const int EmailMaxLength = 128;
    private const string DateFormat = "yyyy-MM-dd";
    private const int MinimumGuestCount = 1;

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
        2. Search available rooms with filter
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
    
    public void ShowEmptyCustomerListMessage()
    {
        _lastMessage = "No customers found.";
    }

    private void ShowCustomerMenu()
    {
        Console.WriteLine(
        """
        === Customer Menu ===

        1. New customer
        2. Existing customer
        0. Go back to main menu
        """);
    }

    public CustomerMenuChoice AskForCustomerMenuChoice()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine(_lastMessage);
            _lastMessage = null;
            ShowCustomerMenu();
            int? choice = ReadNumber("Choose an option: ");
            if (choice != null
                && Enum.IsDefined(typeof(CustomerMenuChoice), choice.Value))
            {
                return (CustomerMenuChoice)choice.Value;
            }
            _lastMessage = "Invalid option choice.";
        }
    }

    public string AskForCustomerName()
    {
        while (true)
        {
            string name = ReadText("What is the customer's name: ");
            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("Name can't be empty.");
                continue;
            }
            if (name.Length > NameMaxLength)
            {
                Console.WriteLine($"Name can't be longer than {NameMaxLength} characters.");
                continue;
            }
            return name;
        }
    }

    public string AskForCustomerPhoneNumber()
    {
        while (true)
        {
            string phoneNumber = ReadText("What is the customer's phone number: ");
            if (string.IsNullOrEmpty(phoneNumber))
            {
                Console.WriteLine("Phone number can't be empty.");
                continue;
            }
            if (phoneNumber.Length > PhoneNumberMaxLength)
            {
                Console.WriteLine($"Phone number can't be longer than {PhoneNumberMaxLength} characters.");
                continue;
            }
            return phoneNumber;
        }
    }

    public string? AskForCustomerEmail()
    {
        while (true)
        {
            string email = ReadText("What is the customer's email? Leave empty if none: ");
            if (string.IsNullOrEmpty(email))
            {
                return null;
            }
            if (email.Length > EmailMaxLength)
            {
                Console.WriteLine($"Email can't be longer than {EmailMaxLength} characters.");
                continue;
            }
            return email;
        }
    }

    public void ShowDuplicateCustomerMessage()
    {
        _lastMessage = "A customer with that phone number or email already exists.";
    }

    public void ShowCustomerAddedMessage(Customer customer)
    {
        _lastMessage = $"{customer.Name} has been registered.";
    }

    public City? AskForCityChoice(List<City> cities)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine(_lastMessage);
            _lastMessage = null;
            ShowCityList(cities);
            Console.WriteLine($"Enter {GoBackToMenu} to go back to the menu.");
            int? choice = ReadNumber("Choose a city: ");
            if (choice == GoBackToMenu)
            {
                return null;
            }
            if (choice <= cities.Count
                && choice >= FirstPosition)
            {
                return cities[choice.Value - FirstPosition];
            }
            _lastMessage = "Invalid option choice.";
        }
    }

    private void ShowCityList(List<City> cities)
    {
        Console.WriteLine("=== City List ===");
        for (int i = 0; i < cities.Count; i++)
        {
            Console.WriteLine($"{i + FirstPosition}. {cities[i].Name}");
        }
    }

    private DateOnly? ReadDate(string question)
    {
        return DateOnly.TryParse(ReadText(question), out DateOnly date)
        ? date
        : null;
    }

    private DateOnly AskForCheckInDate()
    {
        while (true)
        {
            DateOnly dateToday = DateOnly.FromDateTime(DateTime.Today);
            DateOnly? checkInDate = ReadDate($"Check-in date ({DateFormat}): ");
            if (checkInDate is null)
            {
                Console.WriteLine($"Invalid date, use {DateFormat}");
                continue;
            }
            if (checkInDate.Value < dateToday)
            {
                Console.WriteLine($"Check-in date cannot be before {dateToday.ToString(DateFormat)}");
                continue;
            }
            return checkInDate.Value;
        }
    }

    private DateOnly AskForCheckOutDate(DateOnly checkInDate)
    {
        while (true)
        {
            DateOnly? checkOutDate = ReadDate($"Check-out date ({DateFormat}): ");
            if (checkOutDate is null)
            {
                Console.WriteLine($"Invalid date, use {DateFormat}");
                continue;
            }
            if (checkOutDate.Value <= checkInDate)
            {
                Console.WriteLine($"Check-out date must be after {checkInDate.ToString(DateFormat)}");
                continue;
            }
            return checkOutDate.Value;
        }
    }

    private uint AskForGuestCount()
    {
        while (true)
        {
            int? guestCount = ReadNumber("Guest count: ");
            if (guestCount is null)
            {
                Console.WriteLine("Invalid number.");
                continue;
            }
            if (guestCount.Value < MinimumGuestCount)
            {
                Console.WriteLine($"The minimum guest count is {MinimumGuestCount}.");
                continue;
            }
            return (uint)guestCount.Value;
        }
    }

    private bool AskYesOrNo(string question)
    {
        while (true)
        {
            string answer = ReadText($"{question} (y/n): ").ToLower();
            if (answer == "y" || answer == "yes")
            {
                return true;
            }
            if (answer == "n" || answer == "no")
            {
                return false;
            }
            Console.WriteLine("Please answer y or n.");
        }
    }

    private bool AskForPoolRequired()
    {
        return AskYesOrNo("Pool required");
    }

    private bool AskForRestaurantRequired()
    {
        return AskYesOrNo("Restaurant required");
    }

    private uint? AskDistance(string question)
    {
        while (true)
        {
            string distance = ReadText($"{question} in meters (leave empty for no limit): ");
            if (string.IsNullOrEmpty(distance))
            {
                return null;
            }
            if (uint.TryParse(distance, out uint result))
            {
                return result;
            }
            Console.WriteLine("Enter a whole number of meters, or leave empty.");
        }
    }

    private uint? AskForMaxDistanceToBeach()
    {
        return AskDistance("Max distance to beach");
    }

    private uint? AskForMaxDistanceToCenter()
    {
        return AskDistance("Max distance to center");
    }

    private RoomSortOrder AskForSortOrder()
    {
        while (true)
        {
            ShowSortOrders();
            int? choice = ReadNumber("Choose an option: ");
            if (choice != null
                && Enum.IsDefined(typeof(RoomSortOrder), choice.Value))
            {
                return (RoomSortOrder)choice.Value;
            }
            Console.WriteLine("Invalid option choice.");
        }
    }

    private void ShowSortOrders()
    {
        Console.WriteLine(
            """
            === Sort Orders ===
            1. Lowest Price
            2. Highest Rating
            """);
    }

    public RoomSearch AskForRoomSearch(City city)
    {
        Console.Clear();
        Console.WriteLine($"Searching in {city.Name}");
        DateOnly checkInDate = AskForCheckInDate();
        return new RoomSearch(
            city.Id,
            checkInDate,
            AskForCheckOutDate(checkInDate),
            AskForGuestCount(),
            AskForPoolRequired(),
            AskForRestaurantRequired(),
            AskForMaxDistanceToBeach(),
            AskForMaxDistanceToCenter(),
            AskForSortOrder()
        );
    }

    public void ShowNoAvailableRoomsMessage()
    {
        _lastMessage = "There is no available room. Try other dates, fewer guests or fewer filters.";
    }

    public AvailableRoom? AskForAvailableRoomChoice(List<AvailableRoom> availableRooms)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine(_lastMessage);
            _lastMessage = null;
            ShowAvailableRoomList(availableRooms);
            Console.WriteLine($"Enter {GoBackToMenu} to go back to the menu.");
            int? choice = ReadNumber("Choose a room: ");
            if (choice == GoBackToMenu)
            {
                return null;
            }
            if (choice <= availableRooms.Count
                && choice >= FirstPosition)
            {
                return availableRooms[choice.Value - FirstPosition];
            }
            _lastMessage = "Invalid option choice.";
        }
    }

    private void ShowAvailableRoomList(List<AvailableRoom> availableRooms)
    {
        Console.WriteLine($"=== Available rooms in {availableRooms[0].CityName} ===");
        for (int i = 0; i < availableRooms.Count; i++)
        {
            Console.WriteLine($"{i + FirstPosition}. {availableRooms[i].AccommodationName} - Room {availableRooms[i].RoomName}");
            Console.WriteLine($"   Up to {availableRooms[i].GuestCapacity} guests | {availableRooms[i].PricePerNight} kr/night | Rating {availableRooms[i].Rating}/5 | Beach {availableRooms[i].DistanceToBeach} m | Center {availableRooms[i].DistanceToCenter} m");
        }
    }

    public void ShowMissingCustomerMessage()
    {
        _lastMessage = "Choose a customer for this action.";
    }

    public void ShowAlreadyBookedRoomMessage()
    {
        _lastMessage = "The room was just booked by someone else. Please search again.";
    }

    public void ShowBookingConfirmation(BookingConfirmation confirmation, AvailableRoom chosenRoom, RoomSearch roomSearch)
    {
        _lastMessage = $"The room was successfully booked!\n{chosenRoom.AccommodationName} - Room {chosenRoom.RoomName} during {roomSearch.CheckIn.ToString(DateFormat)} - {roomSearch.CheckOut.ToString(DateFormat)}\nBooking number: {confirmation.BookingId} | Total nights: {confirmation.Nights} | Total price: {confirmation.TotalPrice} kr";
    }

    public void ShowNoBookingsMessage()
    {
        _lastMessage = "There is no bookings for this customer.";
    }

    private void ShowBookingList(List<Booking> bookings, Customer customer)
    {
        Console.WriteLine($"=== Current bookings for {customer.Name} ===");
        for (int i = 0; i < bookings.Count; i++)
        {
            Console.Write($"{i + FirstPosition}. ");
            ShowBookingDetails(bookings[i]);
        }
    }

    public void ShowCustomerBookings(List<Booking> bookings, Customer customer)
    {
        Console.Clear();
        ShowBookingList(bookings, customer);
        PauseBeforeMenu();
    }

    private void PauseBeforeMenu()
    {
        Console.WriteLine("Press any key to return to the menu.");
        Console.ReadKey(true);
    }

    public Booking? AskForBookingChoice(List<Booking> bookings, Customer customer)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine(_lastMessage);
            _lastMessage = null;
            ShowBookingList(bookings, customer);
            Console.WriteLine($"Enter {GoBackToMenu} to go back to the menu.");
            int? choice = ReadNumber("Choose a booking: ");
            if (choice == GoBackToMenu)
            {
                return null;
            }
            if (choice <= bookings.Count
                && choice >= FirstPosition)
            {
                return bookings[choice.Value - FirstPosition];
            }
            _lastMessage = "Invalid option choice.";
        }
    }

    public uint AskForNewGuestCount(Booking booking)
    {
        Console.WriteLine($"Current guests: {booking.GuestCount} (max {booking.GuestCapacity})");
        while (true)
        {
            uint count = AskForGuestCount();
            if (count <= booking.GuestCapacity)
            {
                return count;
            }
            Console.WriteLine($"Room {booking.RoomName} fits at most {booking.GuestCapacity} guests.");
        }
    }

    public void ShowChangeGuestConfirmationMessage(Booking booking, uint newGuestCount)
    {
        string guestWord = newGuestCount == 1
        ? "guest"
        : "guests";
        _lastMessage = $"Booking number {booking.BookingId} now has {newGuestCount} {guestWord}.";
    }

    public void ShowChangeGuestErrorMessage()
    {
        _lastMessage = "The guest count could not be changed. The booking may have been cancelled. Please check the customer's bookings and try again.";
    }

    public bool ConfirmCancelBooking(Booking booking)
    {
        ShowBookingDetails(booking);
        return AskYesOrNo("Are you sure you want to cancel this booking");
    }

    private void ShowBookingDetails(Booking booking)
    {
        string nightWord = booking.Nights == 1
            ? "night"
            : "nights";
        Console.WriteLine($"Booking number: {booking.BookingId} | {booking.AccommodationName} - Room {booking.RoomName}");
        Console.WriteLine($"   Dates: {booking.CheckIn.ToString(DateFormat)} - {booking.CheckOut.ToString(DateFormat)} ({booking.Nights} {nightWord}) | Guests: {booking.GuestCount} (max {booking.GuestCapacity}) | Total price: {booking.TotalPrice} kr");
    }

    public void ShowCancelBookingConfirmationMessage(Booking booking)
    {
        _lastMessage = $"Booking {booking.BookingId} has been cancelled.";
    }

    public void ShowCancelBookingErrorMessage()
    {
        _lastMessage = "The booking could not be cancelled. The booking may have been cancelled already. Please check the customer's bookings and try again.";
    }
}