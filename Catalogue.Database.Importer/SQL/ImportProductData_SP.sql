create procedure ImportProductData_SP 
	@CategoryName nvarchar(50),
	@CategoryIsActive bit,
	@ProductName nvarchar(100),
	@ProductCode varchar(10),
	@ProductPrice money,
	@ProductIsActive bit
as
begin
	declare @CategoryID int, @ImportProductResult int;

	select @CategoryID = CategoryID
	from Categories 
	where CategoryName = @CategoryName;

	if @CategoryID is null
	begin
		insert into Categories(CategoryName, IsActive)
		values(@CategoryName, @CategoryIsActive);
		set @CategoryID = SCOPE_IDENTITY();
	end
	else
	begin
		update Categories
		set IsActive = @CategoryIsActive,
			UpdateDate = GETDATE()
		where CategoryID = @CategoryID and IsActive != @CategoryIsActive;
	end

	exec @ImportProductResult = ImportProduct_SP @CategoryID, @ProductName, @ProductCode, @ProductPrice, @ProductIsActive;
	if @ImportProductResult != 0
	begin
		-- Throw error
		return @ImportProductResult;
	end
	return 0;
end
go

create procedure ImportProduct_SP
	@CategoryID int,
	@ProductName nvarchar(100),
	@ProductCode varchar(10),
	@ProductPrice money,
	@ProductIsActive bit
as
begin
	if not exists (select * from Products where ProductCode = @ProductCode)
	begin
		insert into Products(ProductName, ProductCode, Price, IsActive, CategoryID)
		values(@ProductName, @ProductCode, @ProductPrice, @ProductIsActive, @CategoryID);
	end
	else
	begin
		update Products
		set ProductName = @ProductName,
			Price = @ProductPrice,
			IsActive = @ProductIsActive
		where ProductCode = @ProductCode and 
			 (ProductName != @ProductName or Price != @ProductPrice or IsActive != @ProductIsActive);
	end

	return 0;
end