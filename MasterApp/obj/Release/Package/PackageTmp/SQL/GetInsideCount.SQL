select count(1) as 'INSIDE' from Trans_R r
where r.tr_date >= convert(varchar, GETDATE()-1, 112)
and CHARINDEX(LTRIM(RTRIM(r.EL5K_no)) + LTRIM(RTRIM(r.Dev_Id))+',',@INDEVICE) > 0
and r.tr_data not in (
	select r2.Tr_Data as 'OUT' from Trans_R r2
	where r2.tr_date >= convert(varchar, GETDATE()-1, 112)
	and CHARINDEX(LTRIM(RTRIM(r2.EL5K_no)) + LTRIM(RTRIM(r2.Dev_Id))+',',@OUTDEVICE) > 0
	and ltrim(rtrim(r.Tr_Data)) = ltrim(rtrim(r2.Tr_Data))
)