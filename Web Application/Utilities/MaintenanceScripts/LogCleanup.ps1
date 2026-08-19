<#
.SYNOPSIS
Cleanup script for IIS logs

.PARAMETER MaxAge
Maximum age (in days) of log files to retain; files older than this will be deleted 
#>
[CmdletBinding(SupportsShouldProcess)]
param ([int]$MaxAge = 30)
$WebLogFolder = "C:\inetpub\logs\LogFiles"
if ($MaxAge -lt 0) {
    exit 1
}
$DeleteBefore = (Get-Date).AddDays(-$MaxAge)
Get-ChildItem -Path $WebLogFolder -Recurse -File -Force | 
    Where-Object { $_.LastWriteTime -lt $DeleteBefore } | 
    Remove-Item -Force

