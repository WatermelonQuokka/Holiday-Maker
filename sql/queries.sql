-- Run one statement at a time. The INSERT, UPDATE and DELETE statements change data.
-- Rebuild with schema.sql and testdata.sql afterwards.
USE holiday_maker;

-- List of rooms
SELECT rooms.id, accommodations.name AS accommodation_name, cities.name AS city_name, rooms.name AS room_name
FROM rooms
JOIN accommodations ON rooms.accommodation_id = accommodations.id
JOIN cities ON accommodations.city_id = cities.id
ORDER BY city_name, accommodation_name, room_name;

-- Total bookings
SELECT COUNT(*) FROM bookings;

-- Bookings over room capacity (expect 0)
SELECT COUNT(*) FROM bookings
JOIN rooms ON bookings.room_id = rooms.id
WHERE bookings.guest_count > rooms.max_capacity;

-- Cities
SELECT cities.id, cities.name
FROM cities
ORDER BY cities.name;

-- Test values for room search
SET @check_in = '2026-12-12';
SET @check_out = '2026-12-14';
SET @city_id = 1;
SET @guests = 2;
SET @pool_required = FALSE;
SET @restaurant_required = FALSE;
SET @max_distance_to_beach = NULL;
SET @max_distance_to_center = NULL;

-- Search free rooms with filters
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
ORDER BY rooms.price_per_night ASC, accommodations.rating DESC;
-- ORDER BY accommodations.rating DESC, rooms.price_per_night ASC;

-- Test values for new customer
SET @name = 'Arnold Arnoldsson';
SET @phone_number = '+4676583954';
SET @email = 'arnold@example.com';

-- Adds new customer
INSERT INTO customers (name, phone_number, email)
VALUES (@name, @phone_number, @email);

-- List of all customers
SELECT id, name, phone_number
FROM customers
ORDER BY name;

-- Checking availability of the room before booking
SET @room_id = 19;

START TRANSACTION;

SELECT COUNT(*)
FROM bookings
WHERE bookings.room_id = @room_id
    AND @check_in < booking_end
    AND @check_out > booking_start
    FOR UPDATE;

-- Add customer booking
SET @customer_id = 2;

-- The program only runs this if the count above is 0
INSERT INTO bookings (customer_id, room_id, booking_start, booking_end, guest_count)
VALUES (@customer_id, @room_id, @check_in, @check_out, @guests);

COMMIT;

-- Shows last id added
SELECT LAST_INSERT_ID();

-- Total price calculator for booking
SET @booking_id = 1;

SELECT bookings.id, DATEDIFF(bookings.booking_end, bookings.booking_start) AS total_nights,
       (DATEDIFF(bookings.booking_end, bookings.booking_start)* rooms.price_per_night) AS total_price
FROM bookings
JOIN rooms ON bookings.room_id = rooms.id
WHERE bookings.id = @booking_id;

-- All bookings for one customer
SELECT bookings.id,
    accommodations.name AS accommodation_name,
    rooms.name AS room_name,
    bookings.booking_start,
    bookings.booking_end,
    bookings.guest_count,
    rooms.max_capacity AS guest_capacity,
    DATEDIFF(bookings.booking_end, bookings.booking_start) AS total_nights,
    (DATEDIFF(bookings.booking_end, bookings.booking_start) * rooms.price_per_night) AS total_price
FROM bookings
JOIN rooms ON bookings.room_id = rooms.id
JOIN accommodations ON rooms.accommodation_id = accommodations.id
WHERE bookings.customer_id = @customer_id
ORDER BY bookings.booking_start;

-- Shows the booked room max guest capacity
SELECT rooms.max_capacity
FROM rooms
JOIN bookings ON rooms.id = bookings.room_id
WHERE bookings.id = @booking_id;

-- Change guest count for a booking if it fits the room
UPDATE bookings
JOIN rooms ON bookings.room_id = rooms.id
SET bookings.guest_count = @guests
WHERE bookings.id = @booking_id
AND rooms.max_capacity >= @guests;

-- Cancelling a booking
DELETE
FROM bookings
WHERE bookings.id = @booking_id;