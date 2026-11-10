-- Run only after running schema.sql
-- Creates operator. NOTE: Replace CHANGE_ME with your own password before running,
-- run the file as root, and don't commit the real password.
CREATE USER 'operator'@'%' IDENTIFIED BY 'CHANGE_ME';
-- Grants privileges to operator
GRANT SELECT ON holiday_maker.accommodations TO 'operator'@'%';
GRANT SELECT ON holiday_maker.cities TO 'operator'@'%';
GRANT SELECT ON holiday_maker.rooms TO 'operator'@'%';
GRANT SELECT, INSERT, UPDATE, DELETE ON holiday_maker.bookings TO 'operator'@'%';
GRANT SELECT, INSERT ON holiday_maker.customers TO 'operator'@'%';