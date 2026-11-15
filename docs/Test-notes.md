# Test notes

## A free room is shown and a booked room is filtered out.
- **Input:** Malmö, the dates 2026-12-12 - 2026-12-14 and 2026-12-14 - 2026-12-16 and 5 guests.
- **Expected:** Room 401 filtered out for 12–14 and shown for 14–16, because it's the only room in Malmö that fits 5 guests and it's booked 10–13 December (booking 1).
- **Actual:** For 2026-12-12 - 2026-12-14 Old Town Grand - Room 401 was filtered out and gave the message "There is no available room. Try other dates, fewer guests or fewer filters." while 2026-12-14 - 2026-12-16 the room was shown.
## A room can be booked on the same day a previous guest checks out.
- **Input:** Malmö, the dates 2026-12-13 - 2026-12-15 and 5 guests.
- **Expected:** Room 401 is shown and can be booked, because booking 1 checks out on 13 December and the checkout day doesn't occupy the room.
- **Actual:** The room was booked by Erik Eriksson with the success message:
    ```
    The room was successfully booked!
    Old Town Grand - Room 401 during 2026-12-13 - 2026-12-15
    Booking number: 26 | Total nights: 2 | Total price: 6998 kr
    ```
## Overlapping bookings are refused, even when one period completely encloses the other.
- **Input:** Malmö, the dates 2026-12-09 - 2026-12-16, 2026-12-11 - 2026-12-12, 2026-12-12 - 2026-12-14 and 5 guests.
- **Expected:** Room 401 is filtered out because booking 1 (10-13) and 26 (13-15) lie entirely inside 09-16. 11-12 lies inside booking 1 (10-13) and 12-14 overlaps partly.
- **Actual:** 
    - *In the search:* Room 401 was filtered out in all three cases, with the message "There is no available room...".
    - *When saving:* I tested the dates 2026-12-20 - 2026-12-22 where room 401 was shown as free, then booked the same room and dates in DataGrip while the program waited at the room list, and the program refused with "The room was just booked by someone else. Please search again." The program was also waiting while DataGrip held an uncommitted booking, and refused after the commit.
## A party that is too large cannot book the room.
- **Input:** Malmö, the dates 2026-12-20 - 2026-12-22 with both 5 guests and 6 guests.
- **Expected:** Room 401 shown with the 5 guests search because that is the room's max capacity and filtered out with 6 guests as it is above the limit.
- **Actual:** Room 401 was filtered out with a 6 guests search and gave the message "There is no available room. Try other dates, fewer guests or fewer filters." However with 5 guests the room is shown and up for booking.
## Combined search filters and both sorting options work.
- **Input:** Malmö, the dates 2026-12-18 - 2026-12-20, 2 guests, pool:yes, restaurant:no, max distance to beach: empty, max distance to center: 1000
- **Expected:** Only rooms from Old Town Grand (pool:yes, 200 m from the center) and Pinewood Lodge (pool:yes, 1000 m from the center). Seabreeze Hotel has no pool, so it should be filtered out.
    - **Lowest price:** Pinewood's room (699 kr) first, then Old Town Grand (from 1599 kr)
    - **Highest rating:** Old Town Grand (5/5) first, then Pinewood Lodge (2/5)
- **Actual:** With the lowest price sort order, Pinewood Lodge - Room 102 for 699 kr/night was first in the list and the rest of the prices following the price order. The highest rating order gave all rooms from Old Town Grand first and Pinewood Lodge last. No other hotels were listed.
## Changing the number of guests respects the room's capacity.
- **Input:** Existing customer: Anna Andersson, booking number 6 (Seabreeze Hotel - Room 201, 2 guests, max 3), Guest count: 3, 4 and 0.
- **Expected:** Rooms have a minimum and maximum capacity limit. An input of 3 guests is accepted (equal to max), 4 is refused (above max) and 0 is refused (below minimum).
- **Actual:** Changing the guest amount to 3 gives the message "Booking number 6 now has 3 guests." as that is still within the maximum capacity of the room and shows Guests: 3 (max 3) on the customer's booking list afterwards. Trying to change to 4 gives the message "Room 201 fits at most 3 guests." as 3 is the maximum capacity and asks again. Likewise with 0 the message is "The minimum guest count is 1." and asks again. 
## Cancelling a booking frees the room.
- **Input:** Existing customer: Erik Eriksson (used for the booking in point 2), option 5, cancel booking number 26 (room 401, 13-15 December), and answer first n, then y.
- **Expected:** "Booking 26 has been cancelled.", and the booking is gone from the customer's list. Searching Malmö, 13-15 December, 5 guests Old Town Grand - Room 401 is shown again.
- **Actual:** Choosing the answer n keeps the booking and still shows it in the customer's bookings. Answer y gives the message "Booking 26 has been cancelled." and checking Erik's bookings gives the message "There are no bookings for this customer." The search again shows Room 401 as free.
## The data is still there after the application has been restarted.
- **Input:** Existing customer: Erik Eriksson, Booking room 401 (13-15 December, 5 guests), quit with 0 in the main menu and start again `dotnet run`, identify Erik and then option 3.
- **Expected:** The booking is there with the same dates, guests and total price. So are the other changes (booking 6 with 3 guests and booking 26 cancelled).
- **Actual:** Booking 26 is cancelled. Erik's bookings show booking number 27: Old Town Grand – Room 401, 13–15 December (2 nights), 5 guests, 6998 kr. Anna Andersson's booking number 6 still has 3 guests.