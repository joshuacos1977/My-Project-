﻿CREATE PROCEDURE [dbo].[PersonAddOrEdit]  
-- To add or update a record in the database table define columns for both tables (Persons and Emails)
@mode nvarchar(10),   
@Name nvarchar(250) = NULL,
@Age int  = NULL,
@PersonId int  = NULL,
@EmailId int  = NULL,
@PersonName nvarchar(250) = NULL,
@EmailAddress nvarchar(250)  = NULL  
AS  
-- ADD to save the data in database
  IF @mode='ADD'  
  BEGIN 
    DECLARE @Temp int;
    SET @Temp = (SELECT COUNT(*) FROM Persons WHERE  PersonName = @Name);
	IF @Temp <> 1                                      -- IF person is not present insert person and email data
	BEGIN
		INSERT INTO Persons (PersonName, Age) VALUES ( @Name, @Age);
		INSERT INTO Emails(PersonID, EmailAddress) 
		VALUES ((SELECT ID FROM Persons where PersonName = @Name) , @EmailAddress);
	END
	ELSE IF @Temp = 1                                    -- IF person is  present insert only email data
	BEGIN
		INSERT INTO Emails(PersonID, EmailAddress) 
		VALUES ((SELECT ID FROM Persons where PersonName = @Name) , @EmailAddress);
	END
  END  
  -- Below condition is for update Query for person and Email Data
  ELSE IF @mode='EDIT'  
  BEGIN  
	UPDATE Persons  SET  PersonName=@Name,  Age=@Age  WHERE ID=@PersonId;
	IF @EmailId <> 0
	BEGIN
		UPDATE Emails   SET  EmailAddress=@EmailAddress  WHERE PersonID=@PersonId AND EmailID = @EmailId; 
	END
  END
  -- Below condition to delete the email from tables 
  ELSE IF @mode='DELETE'
  BEGIN
   -- Delete the email ID first
    IF @EmailId <> 0
	    DELETE Emails Where EmailID = @EmailId
	DECLARE @TempVar INT;
	SET @TempVar = (SELECT COUNT(*) FROM Emails Where PersonID = @PersonId);
   
   -- If No other emails are present for the person delete the person data
   IF @TempVar = 0
		DELETE Persons WHERE Id=@PersonId;
  END
  -- Finally select data of a person
  ELSE 
   -- Drop temp table if it is present 
  BEGIN
    IF OBJECT_ID('tempdb..#TempPerson') IS NOT NULL
	BEGIN
		DROP TABLE #TempPerson
	END
	-- Create a temp table to store the join statement of 2 tables
	CREATE TABLE #TempPerson (PersonID INT, EmailID INT, PersonName Varchar(255), Age Int, Email VarChar(255))
	INSERT INTO #TempPerson SELECT P.ID, E.EmailID, P.PersonName,P.Age,E.EmailAddress FROM Persons P 
	     INNER JOIN Emails E ON P.ID = E.PersonID Where P.PersonName = @PersonName;
	--SELECT PersonName, Age, stuff ((SELECT '; ' + Email FROM #TempPerson 
	--	WHERE PersonName = T.PersonName FOR XML PATH('')), 1, 1, '') AS Email 
	--	FROM #TempPerson T GROUP BY PersonName, Age;
	SELECT PersonID, EmailID, PersonName, Age, Email FROM #TempPerson; 
  END