create database EventVenueManagment;

use EventVenueManagment;

create table VENUE(
VenueID int primary key,
Loacation varchar(100) NOT NULL,
Capacity int NOT NULL CHECK (Capacity > 0)
);

create table EVENT(
EventID int primary key,
Name varchar(100) NOT NULL,
Date DATE NOT NULL,
VenueID int NOT NULL,
foreign key (VenueID)
references Venue(VenueID)
);

create table PATRON(
PatronID int primary key,
Name varchar(100) NOT NULL,
Email varchar(50) UNIQUE NOT NULL
);

create table TICKET_TYPE(
TicketTypeID int primary key,
TypeName varchar(50) NOT NULL,
Price float NOT NULL CHECK (Price > 0),
Seats int NOT NULL CHECK (Seats > 0),
EventID int NOT NULL,
foreign key(EventID)
references EVENT(EventID)
);

create table TICKET(
TicketID int primary key,
PatronID int NOT NULL,
foreign key(PatronID) references Patron(PatronID),
EventID int NOT NULL,
foreign key(EventID) references Event(EventID),
TicketTypeID int NOT NULL,
foreign key(TicketTypeID) references Ticket_Type(TicketTypeID)
);

create table STAFF(
StaffID int primary key,
phone varchar(11),
Role varchar(50) NOT NULL
check (Role IN ('Coordinator','Technican'))
);

create table Event_Staff(
EventID int NOT NULL,
StaffID int NOT NULL,
primary key (EventID, StaffID),
foreign key (EventID) references Event(EventID),
foreign key (StaffID) references Staff(StaffID)
);

select * from 
[dbo].[VENUE],
[dbo].[EVENT],
[dbo].[TICKET_TYPE],
[dbo].[TICKET],
[dbo].[STAFF],
[dbo].[Event_Staff]

SELECT * FROM VENUE;
SELECT * FROM EVENT;
SELECT * FROM PATRON;
SELECT * FROM TICKET_TYPE;
SELECT * FROM TICKET;
SELECT * FROM STAFF;
SELECT * FROM EVENT_STAFF;

ALTER TABLE STAFF
Add Staff_Name VARCHAR(50) NOT NULL;

insert into STAFF values (1,'01106569049','Coordinator','Mohamed');

SELECT Staff_Name
FROM STAFF

EXEC sp_help 'STAFF';
ALTER TABLE STAFF
DROP CONSTRAINT CK__STAFF__Role__5CD6CB2B;

ALTER TABLE STAFF
ADD CONSTRAINT CHK_Role
CHECK (Role IN ('Coordinator','Technician'));
