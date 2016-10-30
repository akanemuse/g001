	--declare 
	--@InDevice varchar(50),
	--@OutDevice varchar(50)
	--set @InDevice='000101,'
	--set @OutDevice='000101,'
	
	declare @tableMasuk table (
		SeqNo varchar(6),
		EL5KNo varchar(8),
		DevType varchar(1),
		Devid varchar(4),
		TrDate varchar(8),
		TrTime varchar(8),
		TrData varchar(30),
		TrCode varchar(1),
		Extra varchar(4),
		TrUser varchar(12),
		StaffNumber varchar(12),
		Name varchar(255),
		Jabatan varchar(128),
		Divisi varchar(128)
	)

	declare @tableKeluar table (
		SeqNo varchar(6),
		EL5KNo varchar(8),
		DevType varchar(1),
		Devid varchar(4),
		TrDate varchar(8),
		TrTime varchar(8),
		TrData varchar(30),
		TrCode varchar(1),
		Extra varchar(4),
		TrUser varchar(12),
		StaffNumber varchar(12),
		Name varchar(255),
		Jabatan varchar(128),
		Divisi varchar(128)
	)

	insert @tableMasuk 
	select r.*, isnull(ltrim(rtrim(c.Name)), 'GUEST'), isnull(ltrim(rtrim(k.Position_Desc)), 'GUEST'), isnull(ltrim(rtrim(v.Depart_Desc)), 'No Division') from --HRPAYROLL08.Matrix.dbo.
	Trans_R r left join Cards c on c.Card_Number = r.Tr_Data
	left join Department v on v.Depart_Code = c.Depart_Code
	left join Job_Title k on k.Position_Code = c.Position_Code
	where r.tr_date >= convert(varchar, GETDATE(), 112)
	and CHARINDEX(LTRIM(RTRIM(r.EL5K_no)) + LTRIM(RTRIM(r.Dev_Id))+',',@InDevice) > 0
	AND R.Tr_Code='0' 

	insert @tableKeluar
	select r2.*, isnull(ltrim(rtrim(c.Name)), 'GUEST'), isnull(ltrim(rtrim(k.Position_Desc)), 'GUEST'), isnull(ltrim(rtrim(v.Depart_Desc)), 'No Division') from --HRPAYROLL08.Matrix.dbo.
	Trans_R r2 left join Cards c on c.Card_Number = r2.Tr_Data
	left join Department v on v.Depart_Code = c.Depart_Code
	left join Job_Title k on k.Position_Code = c.Position_Code
	where r2.tr_date >= convert(varchar, GETDATE(), 112)
	and CHARINDEX(LTRIM(RTRIM(r2.EL5K_no)) + LTRIM(RTRIM(r2.Dev_Id))+',',@OutDevice) > 0
	AND r2.Tr_Code='9'

	select 
		SeqNo,
		EL5KNo,
		DevType,
		Devid,
		TrDate,
		--TrTime,
		SUBSTRING(TrTime, 1, 2) +':' + SUBSTRING(TrTime,3,2) as TrTime,
		TrData,
		TrCode,
		Extra,
		TrUser,
		StaffNumber,
		Name,
		ltrim(rtrim(Jabatan)) as Jabatan,
		ltrim(rtrim(Divisi)) as Divisi
	from (
		select * from @tableMasuk
		union all
		select * from @tableKeluar
		) total
	order by TrData, trdate, TrTime