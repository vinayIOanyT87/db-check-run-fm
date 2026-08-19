-- This script is run by the ASC install, but it is attaching a new FuelsManager Defense core product report.
-- The ASC Uninstaller will NOT remove this entry.
use aviationdb;
if not exists (select * from REPORTS where REPORT_NAME = 'Automated Log Sheet Report.rpt' and DeleteFlag = 0)
begin
insert into REPORTS (REPORT_NAME, MODULE_NAME, CRC, CreatedBy, UpdatedBy)
values ('Automated Log Sheet Report.rpt','Dispatch',938718177,'ASC','ASC')
end