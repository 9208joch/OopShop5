USE WebShopDb;

DELETE FROM dbo.Customers;

SET IDENTITY_INSERT dbo.Customers ON;

INSERT INTO dbo.Customers 
(Id, Name, Email, Address, City, Country, phone, Age, Gender, MaskedCreditCard, OtherContactInfo, PreferredPaymentMethod)
VALUES 
(1, 'Anna Andersson', 'anna@mail.se', 'Storgatan 1', 'Stockholm', 'Sverige', '070-1234567', 28, 'Female', '1111', '-', 'Card'),
(2, 'Erik Karlsson', 'erik@mail.se', 'Lilla Torget 5', 'Göteborg', 'Sverige', '073-9876543', 45, 'Male', '2222', '-', 'Swish'),
(3, 'Sara Nilsson', 'sara@mail.se', 'Trädgårdsgatan 12', 'Malmö', 'Sverige', '076-5554433', 32, 'Female', '3333', '-', 'Swish');

SET IDENTITY_INSERT dbo.Customers OFF;