record Booking(
    uint BookingId,
    string AccommodationName,
    string RoomName,
    DateOnly CheckIn,
    DateOnly CheckOut,
    int Nights,
    uint GuestCount,
    uint GuestCapacity,
    uint TotalPrice
);