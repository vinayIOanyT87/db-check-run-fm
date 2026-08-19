CREATE FUNCTION [dbo].[udf_IsUserGroupAssignedToModifyTransactionAlias]
                       (@SiteGuid uniqueidentifier, 
                        @userGuid uniqueidentifier, 
                        @aliasGuid uniqueidentifier )
RETURNs bit
BEGIN
IF EXISTS( SELECT TOP 1 1 FROM 
   [map].[tblGroupToTransactionAlias] m WITH(NOLOCK)
   JOIN [dbo].tblTransactionAliases a WITH(NOLOCK) 
        ON a.TransactionAliasGuid=m.TransactionAliasGuid
     JOIN [dbo].tblgroups g WITH(NOLOCK) 
        ON m.GroupGuid=g.GroupGuid
   JOIN map.tblUserToGroup ug WITH(NOLOCK) 
        ON g.GroupGuid=ug.GroupGuid
   JOIN map.tblEntityTransactionAliasToSite e WITH(NOLOCK) 
        ON e.TransactionAliasGuid=m.TransactionAliasGuid
   JOIN map.tblEntityUserGroupToSite eg WITH(NOLOCK)
        ON eg.GroupGuid=g.GroupGuid
   JOIN [dbo].tblsites s WITH(NOLOCK) 
        ON s.siteguid=e.siteguid AND s.siteguid=eg.siteguid
   WHERE ug.UserGuid=@userGuid AND s.SiteGuid=@SiteGuid 
         AND a.TransactionAliasGuid=@aliasGuid 
         AND lookuprightindex = 1
)
   RETURN 1
RETURN 0
END
