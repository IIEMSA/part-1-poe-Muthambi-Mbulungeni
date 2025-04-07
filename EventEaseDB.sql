-- Create the database
CREATE DATABASE EventEaseDB;
GO

-- Use the newly created database
USE EventEaseDB;
GO

-- Create Venue table
CREATE TABLE Venue (
    VenueId INT PRIMARY KEY IDENTITY(1,1),
    VenueName NVARCHAR(100) NOT NULL,
    Location NVARCHAR(100),
    Capacity INT,
    ImageUrl NVARCHAR(255)
);

-- Create Event table
CREATE TABLE Event (
    EventId INT PRIMARY KEY IDENTITY(1,1),
    EventName NVARCHAR(100) NOT NULL,
    EventDate DATE,
    Description NVARCHAR(255),
    VenueId INT,
    FOREIGN KEY (VenueId) REFERENCES Venue(VenueId)
);

-- Create Booking table
CREATE TABLE Booking (
    BookingId INT PRIMARY KEY IDENTITY(1,1),
    EventId INT,
    VenueId INT,
    BookingDate DATE,
    FOREIGN KEY (EventId) REFERENCES Event(EventId),
    FOREIGN KEY (VenueId) REFERENCES Venue(VenueId)
);

-- Insert sample venues
INSERT INTO Venue (VenueName, Location, Capacity, ImageUrl)
VALUES 
('Skyline Hall', 'Downtown', 200, 'https://via.placeholder.com/150'),
('Oceanview Terrace', 'Seaside Blvd', 150, 'https://via.placeholder.com/150'),
('Greenfield Arena', 'Greenwood Ave', 500, 'https://via.placeholder.com/150'),
('Mountain Retreat', 'Highlands', 80, 'https://via.placeholder.com/150'),
('City Loft', 'Main Street', 120, 'https://via.placeholder.com/150'),
('Sunset Pavilion', 'Palm Road', 300, 'https://via.placeholder.com/150');

-- Insert sample events
INSERT INTO Event (EventName, EventDate, Description, VenueId)
VALUES 
('Tech Conference 2025', '2025-05-15', 'Annual technology conference.', 1),
('Wedding Expo', '2025-06-10', 'Showcasing wedding vendors.', 2),
('Startup Pitch Night', '2025-04-20', 'New startups pitch to investors.', 3),
('Music Gala', '2025-07-01', 'Live music performances.', 4),
('Food Festival', '2025-08-15', 'Food trucks and chef demos.', 5),
('Art Exhibit', '2025-09-10', 'Gallery of local artists.', 6);

-- Insert sample bookings
INSERT INTO Booking (EventId, VenueId, BookingDate)
VALUES 
(1, 1, '2025-03-01'),
(2, 2, '2025-03-05'),
(3, 3, '2025-03-10'),
(4, 4, '2025-03-15'),
(5, 5, '2025-03-20'),
(6, 6, '2025-03-25');
