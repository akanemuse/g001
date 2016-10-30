	declare 
	--@InDevice varchar(50),
	--@OutDevice varchar(50),
	--@AssemblyDevice varchar(50)
	@LastIn varchar(255),
	@LastOut varchar(255)
	--set @InDevice='000101,'
	--set @OutDevice='000101,'
	--set @AssemblyDevice='000107,000108,000109,'
		
	
	select top 1 
		@LastIn = isnull(ltrim(rtrim(c.Name)),  'GUEST') + ' ('+ SUBSTRING(r.Tr_Time, 1, 2) +':' + SUBSTRING(r.Tr_Time,3,2) +')' from --HRPAYROLL08.Matrix.dbo.
		Trans_R r left join Cards c on c.Card_Number = r.Tr_Data
	where r.tr_date >= convert(varchar, GETDATE(), 112)
		and CHARINDEX(LTRIM(RTRIM(r.EL5K_no)) + LTRIM(RTRIM(r.Dev_Id))+',',@InDevice) > 0
		AND R.Tr_Code='0'
		order by seq_no desc
	
	select top 1
		@LastOut = isnull(ltrim(rtrim(c.Name)),  'GUEST') + ' ('+ SUBSTRING(r2.Tr_Time, 1, 2) +':' + SUBSTRING(r2.Tr_Time,3,2) +')' from --HRPAYROLL08.Matrix.dbo.
		Trans_R r2 left join Cards c on c.Card_Number = r2.Tr_Data
	where r2.tr_date >= convert(varchar, GETDATE(), 112)
		and CHARINDEX(LTRIM(RTRIM(r2.EL5K_no)) + LTRIM(RTRIM(r2.Dev_Id))+',',@OutDevice) > 0
		AND r2.Tr_Code='9'
		order by seq_no desc
	
	select @LastIn as LastIn, @LastOut as LastOut