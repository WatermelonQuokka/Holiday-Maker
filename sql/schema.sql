CREATE DATABASE IF NOT EXISTS holiday_maker;
USE holiday_maker;

DROP TABLE IF EXISTS bookings;
DROP TABLE IF EXISTS rooms;
DROP TABLE IF EXISTS customers;
DROP TABLE IF EXISTS accommodations;
DROP TABLE IF EXISTS cities;

CREATE TABLE cities
(
    id   int unsigned AUTO_INCREMENT
        PRIMARY KEY,
    name varchar(64) NOT NULL,
    CONSTRAINT cities_name_unique
        UNIQUE (name)
);

CREATE TABLE accommodations
(
    id                 int unsigned AUTO_INCREMENT
        PRIMARY KEY,
    name               varchar(64)  NOT NULL,
    city_id            int unsigned NOT NULL,
    has_pool           tinyint(1)   NOT NULL,
    has_restaurant     tinyint(1)   NOT NULL,
    distance_to_beach  int unsigned NOT NULL,
    distance_to_center int unsigned NOT NULL,
    rating             tinyint      NOT NULL,
    CONSTRAINT accommodations_cities_id_fk
        FOREIGN KEY (city_id) REFERENCES cities (id),
    CONSTRAINT accommodations_rating_in_range
        CHECK (`rating` BETWEEN 1 AND 5)
);

CREATE TABLE customers
(
    id           int unsigned AUTO_INCREMENT
        PRIMARY KEY,
    name         varchar(64)  NOT NULL,
    phone_number varchar(32)  NOT NULL,
    email        varchar(128) NULL,
    CONSTRAINT customers_phone_number_unique
        UNIQUE (phone_number),
    CONSTRAINT customers_email_unique
        UNIQUE (email)
);

CREATE TABLE rooms
(
    id               int unsigned AUTO_INCREMENT
        PRIMARY KEY,
    name             varchar(64)  NOT NULL,
    accommodation_id int unsigned NOT NULL,
    max_capacity     int unsigned NOT NULL,
    price_per_night  int unsigned NOT NULL,
    CONSTRAINT rooms_accommodation_name_unique
        UNIQUE (name, accommodation_id),
    CONSTRAINT rooms_accommodations_id_fk
        FOREIGN KEY (accommodation_id) REFERENCES accommodations (id),
    CONSTRAINT rooms_max_capacity_positive
        CHECK (`max_capacity` >= 1),
    CONSTRAINT rooms_price_per_night_positive
        CHECK (`price_per_night` >= 1)
);

CREATE TABLE bookings
(
    id            int unsigned AUTO_INCREMENT
        PRIMARY KEY,
    customer_id   int unsigned NOT NULL,
    room_id       int unsigned NOT NULL,
    booking_start date         NOT NULL,
    booking_end   date         NOT NULL,
    guest_count   int unsigned NOT NULL,
    CONSTRAINT bookings_customers_id_fk
        FOREIGN KEY (customer_id) REFERENCES customers (id),
    CONSTRAINT bookings_rooms_id_fk
        FOREIGN KEY (room_id) REFERENCES rooms (id),
    CONSTRAINT bookings_end_after_start
        CHECK (`booking_end` > `booking_start`),
    CONSTRAINT bookings_guest_count_positive
        CHECK (`guest_count` >= 1)
);