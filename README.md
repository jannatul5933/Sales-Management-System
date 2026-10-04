# Sales Management System

A Windows Forms desktop application for managing sales invoices, built with C# and SQL Server.

## Features
- Invoice entry with product details (name, quantity, unit price, expiry date, warranty)
- Master-details invoice view
- Crystal Reports for invoice and master-details reports

## Technologies
- C# (.NET Framework 4.8), Windows Forms
- MS SQL Server, ADO.NET
- Crystal Reports

## Database
`SQLQuery1.sql` contains the DDL script: creates the SalesDB database and the
Customer, Invoice and InvoiceDetails tables with primary and foreign keys.

## How to run
1. Run `SQLQuery1.sql` in SQL Server to create the database.
2. Open `SMS_Project/SMS_Project.sln` in Visual Studio.
3. Update the SQL Server name in the connection string, then run the project.
