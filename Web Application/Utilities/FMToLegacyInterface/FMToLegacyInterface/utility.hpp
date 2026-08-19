#pragma once

// definitions for utility functions

// Define Constants for SubSystem Types

#define	FM_SYSTEM_NONE				0			// Undefined System
#define	FM_SYSTEM_DB				1			// DataManager
#define	FM_SYSTEM_CM				2			// CommunicationsManager
#define	FM_SYSTEM_REP				3			// ReportManager
#define	FM_SYSTEM_GUI				4			// Graphic User Interface
#define	FM_SYSTEM_FM				5			// Top Level FuelsManager System
#define	FM_SYSTEM_FMALARMGROUP	6			// Top Level FuelsManager Alarm Group config
#define	FM_SYSTEM_AM				7			// ArchiveManager	- Historical Database Subsystem
#define	FM_SYSTEM_AM_DM			8			// ArchiveManager	- Select Database for multiple conf
#define	FM_SYSTEM_FM_SECURITY	9			// Top Level FuelsManager System for security selection. does no validation of selection

//	Define Constants for RPC Transport Protocol EndPoints

//	Named Pipes Used for "ncacn_np" Transport

#define	AM_SERV_PIPE_NAME		TEXT("\\pipe\\am_mgr")
#define	CM_SERV_PIPE_NAME		TEXT("\\pipe\\fmcomm_mgr")
#define	DM_SERV_PIPE_NAME		TEXT("\\pipe\\fmdata_mgr" )
#define	FM_SERV_PIPE_NAME		TEXT("\\pipe\\fm_mgr" )
#define	RM_SERV_PIPE_NAME		TEXT("\\pipe\\rm_mgr" )

//	Socket Ports for "ncacn_ip_tcp" TCP/IP Transport

#define	DM_SERV_SOCK_NAME		TEXT("10101")
#define	CM_SERV_SOCK_NAME		TEXT("10102")
#define	RM_SERV_SOCK_NAME		TEXT("10103")
#define	FM_SERV_SOCK_NAME		TEXT("10104")
#define	AM_SERV_SOCK_NAME		TEXT("10105")

// function prototypes
RPC_STATUS	FuelManagerBind(RPC_BINDING_HANDLE* pBind,RPC_IF_HANDLE* pClientIf,DWORD dwSystemType,LPTSTR	pServer);

BOOL ConvertStringToLevelNames(PLEVELNAMES Name, LPTSTR szName);

int TrimSpaces(LPTSTR pString, BYTE bLead, BYTE bTrail);

DWORD		GetDefaultLevelS(PBYTE pbLevels);

BOOL bGetRegistryStringValue(LPTSTR lpszKey, LPTSTR lpszValue, LPTSTR lpszOutput);

BOOL	MvmntBind(RPC_BINDING_HANDLE* phBinding, LPTSTR	lpszServer);



