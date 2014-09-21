DECLARE @customerName NVARCHAR(50)
SET @customername = 'Smith'

DECLARE @AddressTypeID INT
SELECT @AddressTypeID = AddressTypeID FROM Person.AddressType WHERE Name = 'Main Office';

PRINT 'Execution start time: ' + CAST(GETDATE() AS VARCHAR(50));

SELECT	p.LastName, p.FirstName, a.AddressLine1, a.AddressLine2, a.City, cr.Name as CountryRegionName
FROM	Person.Person p
		INNER JOIN Person.BusinessEntityContact bec on p.BusinessEntityID = bec.PersonID
		INNER JOIN Person.BusinessEntity be on bec.BusinessEntityID = be.BusinessEntityID
		LEFT JOIN Person.BusinessEntityAddress bea on bea.BusinessEntityID = be.BusinessEntityID
			AND bea.AddressTypeID = @AddressTypeID
		LEFT JOIN Person.Address a on bea.AddressID = a.AddressID
		LEFT JOIN Person.StateProvince sp on a.StateProvinceID = sp.StateProvinceID
		LEFT JOIN Person.CountryRegion cr on sp.CountryRegionCode = cr.CountryRegionCode
WHERE	p.FirstName LIKE '%' + @customerName + '%' OR p.LastName LIKE '%' + @customerName + '%' AND
		5000 < (
			SELECT	SUM(sod.OrderQty * sod.UnitPrice * (1 - sod.UnitPriceDiscount)) AS Revenue
			FROM	Sales.Customer c
					INNER JOIN Sales.SalesOrderHeader soh on c.CustomerID = soh.CustomerID
					INNER JOIN Sales.SalesOrderDetail sod on soh.SalesOrderID = sod.SalesOrderID
			WHERE	c.PersonID = p.BusinessEntityID)
ORDER BY p.LastName, p.FirstName, cr.Name, a.City;

PRINT 'Execution start time: ' + CAST(GETDATE() AS VARCHAR(50));
