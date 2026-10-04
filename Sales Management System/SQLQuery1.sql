USE master
CREATE DATABASE SalesDB
Go
USE SalesDB
Go

CREATE TABLE Customer(
  CustomerID INT PRIMARY KEY IDENTITY(1,1), 
  CustomerEmail NVARCHAR(100),
  CustomerPhone NVARCHAR(20)
);
GO

CREATE TABLE Invoice (
    InvoiceID INT IDENTITY(1,1) PRIMARY KEY, 
    CustomerName NVARCHAR(100),              
    InvoiceDate DATE                         
);

CREATE TABLE InvoiceDetails (
    InvoiceDetailID INT IDENTITY(1,1) PRIMARY KEY, 
    InvoiceID INT,                        
    ProductName NVARCHAR(100),                     
    ProductQuantity INT,                           
    ProductImage NVARCHAR(MAX),                   
    UnitPrice DECIMAL(18,2),                      
    ExpiryDate DATE,                               
    IsWarrantyAvailable BIT DEFAULT 1,             
    
    -- Foreign Key Constraint
    CONSTRAINT FK_Invoice_Details FOREIGN KEY (InvoiceID) 
    REFERENCES Invoice(InvoiceID) ON DELETE CASCADE
);

SELECT*FROM  Invoice

SELECT*FROM InvoiceDetails