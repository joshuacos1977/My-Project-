﻿CREATE PROCEDURE [dbo].[ContactViewOrSearch]
	@ContactName nvarchar(50)
AS
--Select Person's name to search person
  SELECT *
  FROM Persons
  WHERE PersonName LIKE @ContactName+'%'