record RoomSearch(
    uint CityId,
    DateOnly CheckIn,
    DateOnly CheckOut,
    uint Guests,
    bool PoolRequired,
    bool RestaurantRequired,
    uint? MaxDistanceToBeach,
    uint? MaxDistanceToCenter,
    RoomSortOrder SortOrder
);