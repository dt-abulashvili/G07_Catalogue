create view vw_ProductCatalogue as
select C.CategoryName,
	   1 as CategoryIsActive,
	   P.ProductName,
	   'P' + RIGHT(CONCAT('00000', P.ProductID), 5) as ProductCode,
	   P.UnitPrice,
	   1 as ProductIsActive
from Categories C 
	inner join Products P on C.CategoryID = P.CategoryID
order by C.CategoryName