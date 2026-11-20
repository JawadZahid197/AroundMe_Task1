-- Run once on AroundMeIdentityDB if it already existed in D2.
-- Back up the database before running this script.
USE [AroundMeIdentityDB];
GO
IF COL_LENGTH('dbo.AspNetUsers', 'City') IS NULL
BEGIN
    ALTER TABLE dbo.AspNetUsers ADD City nvarchar(100) NULL;
END;
GO
