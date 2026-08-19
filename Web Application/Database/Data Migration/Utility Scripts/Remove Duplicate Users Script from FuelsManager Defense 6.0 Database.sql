Use ConsolidatedDB6;

Create Table #AllDupIndex
( UserIndex int NOT NULL );

Create Table #MinUserIndex
(
	UserID nvarchar(100) NOT NULL,
	UserIndex int NOT NULL
)
Create Table #ExcludeDupIndex
( UserIndex int NOT NULL );


Insert into #AllDupIndex
Select UserIndex from tblUsers where UserID in(
Select UserID from tblUsers
group by UserID
having COUNT(UserID) > 1)
order by UserID,UserIndex

Insert into #MinUserIndex
Select UserID,Min(UserIndex) from tblUsers 
where UserID in(
Select UserID from tblUsers
group by UserID
having COUNT(UserID) > 1)
Group By UserID


Insert Into #ExcludeDupIndex
Select UserIndex from #AllDupIndex where UserIndex not in
(Select UserIndex from #MinUserIndex)


Delete from tblUserGroupMap 
where UserIndex in (Select UserIndex from #ExcludeDupIndex)

Delete from tblUsers where UserIndex in (Select UserIndex from #ExcludeDupIndex)

drop Table #AllDupIndex;
drop Table #MinUserIndex;
drop Table #ExcludeDupIndex;