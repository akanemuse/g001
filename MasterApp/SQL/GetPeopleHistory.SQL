--declare
--	@InDevice varchar(50),
--	@OutDevice varchar(50)
--	set @InDevice='000206,'
--	set @OutDevice='000206,'
	
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
select r.*, isnull(ltrim(rtrim(c.Name)), 'GUEST'), ltrim(rtrim(k.Position_Desc)), isnull(ltrim(rtrim(v.Depart_Desc)), 'No Division') from --HRPAYROLL08.Matrix.dbo.
Trans_R r left join Cards c on c.Card_Number = r.Tr_Data
			left join Department v on v.Depart_Code = c.Depart_Code
			left join Job_Title k on k.Position_Code = c.Position_Code
where r.tr_date >= convert(varchar, GETDATE(), 112)
and CHARINDEX(LTRIM(RTRIM(r.EL5K_no)) + LTRIM(RTRIM(r.Dev_Id))+',',@InDevice) > 0
AND R.Tr_Code='0'

insert @tableKeluar
select r2.*, isnull(ltrim(rtrim(c.Name)), 'GUEST'), ltrim(rtrim(k.Position_Desc)), isnull(ltrim(rtrim(v.Depart_Desc)), 'No Division') from --HRPAYROLL08.Matrix.dbo.
Trans_R r2 left join Cards c on c.Card_Number = r2.Tr_Data
			left join Department v on v.Depart_Code = c.Depart_Code
			left join Job_Title k on k.Position_Code = c.Position_Code
where r2.tr_date >= convert(varchar, GETDATE(), 112)
and CHARINDEX(LTRIM(RTRIM(r2.EL5K_no)) + LTRIM(RTRIM(r2.Dev_Id))+',',@OutDevice) > 0
AND r2.Tr_Code='9'

select tableX.TrData, tableX.Name, tableX.Divisi, 
	SUBSTRING(tableX.TimeIn, 1, 2) +':' + SUBSTRING(tableX.TimeIn,3,2) as TimeIn, 
	SUBSTRING(tableY.TimeOut, 1, 2) +':' + SUBSTRING(tableY.TimeOut,3,2) as TimeOut 
from (
	select k.TrData, k.Name, k.Divisi, k.TrDate, min(k.TrTime) as TimeIn
	from @tableMasuk k group by k.TrData, k.Name, k.Divisi, k.TrDate
) tableX left join 
(
	select j.TrData, j.Name, j.Divisi, j.TrDate, max(j.TrTime) as TimeOut from @tableKeluar j
	group by j.TrData, j.Name, j.Divisi, j.TrDate
) tableY
on tableX.TrData = tableY.TrData

order by tableY.TimeOut
