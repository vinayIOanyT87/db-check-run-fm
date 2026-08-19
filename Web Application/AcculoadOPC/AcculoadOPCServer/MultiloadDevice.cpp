/******************************************************************************

	FILE NAME:		MultiloadDevice.cpp


	PURPOSE:			Implementation of the CMultiloadDevice


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.1  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		-----------	----------  -------------------------------------------
		11/25/2008	W.Gray		7.4.6.1 - Change PerformSerialIO to delay 5 seconds on OpenCommPort error (CSI 6319)

		11/25/2008	W.Gray		7.4.6.2 - Change PerformSerialIO to call SignalCommunicationsFailure
										when OpenCommPort fails

		01/27/2009	W.Gray		7.4.6.3 - Change to call CloseComPort

		12/10/2009	W.Gray		7.5.1.0 - Revised to handle error on WaitCommEvent (WI 9947)

*******************************************************************************/
#include "StdAfx.h"
#include "MultiloadDevice.h"
#include "DeviceManager.h"

extern CDeviceManager*		g_pDeviceManager;


HRESULT CMultiloadDevice::PrepareRequest(CTag* pTag,BOOL bWrite)
{
	CIO* pIO=pTag->m_pIO;

	pIO->m_bXmtBuffer[0]=0x00;
	pIO->m_bXmtBuffer[1]=STX;
	pIO->m_bXmtBuffer[2]=0x30+(pTag->m_bAddress / 10);
	pIO->m_bXmtBuffer[3]=0x30+(pTag->m_bAddress % 10);

	int iCommandLength=strlen(pTag->m_pszCommand);

	CStringA	oData;
	int iDataLength;

	if(pTag->m_dwAccessRights && OPC_WRITEABLE
	&& pTag->m_Value.vt == VT_BSTR)
	{
		oData=(LPTSTR) pTag->m_Value.bstrVal;
		iDataLength=pIO->m_wXmtLength=oData.GetLength();

		if(iDataLength > BUFFER_MAX-6)
		{
			CString oError;
			oError.Format(_T("IO Error : Prepare Request Maximum String Length Exceeded"));
			theApp.LogError(oError);
			return E_FAIL;
		}
	}
	else
		iDataLength=0;

	strncpy((LPSTR) &pIO->m_bXmtBuffer[4],pTag->m_pszCommand,iCommandLength);
	strncpy((LPSTR) &pIO->m_bXmtBuffer[4+iCommandLength],oData,iDataLength);
	pIO->m_bXmtBuffer[4+iCommandLength+iDataLength]=ETX;
	pIO->m_bXmtBuffer[5+iCommandLength+iDataLength]=pIO->LRC(&pIO->m_bXmtBuffer[2],3+iCommandLength+iDataLength);
	pIO->m_bXmtBuffer[6+iCommandLength+iDataLength]=0x7F;
	pIO->m_wXmtLength=7+iCommandLength+iDataLength;

	return S_OK;
}

HRESULT CMultiloadDevice::ProcessResponse(CTag* pTag,HRESULT hr)
{
	CIO* pIO=pTag->m_pIO;

	if(SUCCEEDED(hr)
	&& pIO->m_wRcvLength > 0)
		pIO->m_bRcvBuffer[pIO->m_wRcvLength-1]='\0';

	// Received buffer is base upon Serial I/O with Accuload Mini Comp Host Protocol
	// NULL STS A1 A2 DATA


	// RCU Status is returned for Multiload request
	if(m_pRcuStatusTag != NULL)
	{
		if(hr == S_OK)
		{
			m_pRcuStatusTag->m_Value.vt=VT_UI1;
			if(m_pRcuStatusTag->m_Value.bVal != pIO->m_bRcvBuffer[5])
			{
				m_pRcuStatusTag->m_Value.bVal=pIO->m_bRcvBuffer[5];
				m_pRcuStatusTag->m_dwUpdateSequence++;
			}
			m_pRcuStatusTag->m_wQuality=OPC_QUALITY_GOOD;
		}
		else
			m_pRcuStatusTag->m_wQuality=OPC_QUALITY_BAD;

	
		CoFileTimeNow(&m_pRcuStatusTag->m_Timestamp);
		m_pRcuStatusTag->m_bCurrent=true;
	

		// Card Status is returned for Multiload requests
		if(hr == S_OK)
		{
			m_pCardStatusTag->m_Value.vt=VT_UI1;
			m_pCardStatusTag->m_Value.bVal=pIO->m_bRcvBuffer[6];
			m_pCardStatusTag->m_wQuality=OPC_QUALITY_GOOD;
		}
		else
			m_pCardStatusTag->m_wQuality=OPC_QUALITY_BAD;

		CoFileTimeNow(&m_pCardStatusTag->m_Timestamp);
		m_pCardStatusTag->m_bCurrent=true;
	}

	// Check for Command or Parameter Error
	if(hr == S_OK)
	{
		if(m_Type == RCU_II_OPEN)
		{
			if(pIO->m_bRcvBuffer[4] != 'A')
			{
				if(pTag->m_wQuality != OPC_QUALITY_BAD)
				{
					pTag->m_wQuality=OPC_QUALITY_BAD;
					pTag->m_bCurrent=true;
					CoFileTimeNow(&pTag->m_Timestamp);
				
					CString oData(pTag->m_pszCommand);
					CString oError;
					oError.Format(_T("IO Error : Invalid Command or Parameter - %s"),oData);
					theApp.LogError(oError);
				}
				return E_FAIL;
			}
		}

		else
		{
			if(pIO->m_bRcvBuffer[4] != '0')
			{
				if(pTag->m_wQuality != OPC_QUALITY_BAD)
				{
					pTag->m_wQuality=OPC_QUALITY_BAD;
					pTag->m_bCurrent=true;
					CoFileTimeNow(&pTag->m_Timestamp);
					CString oData(pTag->m_pszCommand);
					CString oError;
					if (pIO->m_bRcvBuffer[4] == '1')
					{
						oError.Format(_T("IO Error : Invalid Parameter for command - %s"),oData);
					}
					else
					{
						oError.Format(_T("IO Error : Invalid Command - %s"),oData);
					}
					theApp.LogError(oError);
				}
				return E_FAIL;
			}
		}
	}

	int iCommandLength=strlen(pTag->m_pszCommand);

	// Terminal Command
	// Read Register - but not R112
	// Update Register
	if(strncmp(pTag->m_pszCommand,"R962",4)
	&&((!strncmp(pTag->m_pszCommand,"T",1)
	|| (!strncmp(pTag->m_pszCommand,"R",1)
	&&	strncmp(pTag->m_pszCommand,"R112",4))
	|| !strncmp(pTag->m_pszCommand,"U",1))))
	{
		if(FAILED(hr))
			pTag->m_wQuality=OPC_QUALITY_BAD;
		else
		{

			// On successful Setup Data Entry Terminal Command			if(!strncmp(pTag->m_pszCommand,"T",1))			
			{
				CStringA	oData;
				oData=(LPTSTR) pTag->m_Value.bstrVal;
				int iDataLength=pIO->m_wXmtLength=oData.GetLength();
				for(int i=0;i < iDataLength;i++)
				{
					if(oData[i] == 27
					&& i+1 < iDataLength
					&& oData[i+1] == 'E')
					{
						// Force a change of state on the m_pRcuStatusTag
						if(m_Type == RCU_II_RCU)
						{
							if(m_pRcuStatusTag != NULL)
								m_pRcuStatusTag->m_dwUpdateSequence++;
						}

						// Clear the Input Done to insure that a change of state is detected
						CTag* pInputDone=g_pDeviceManager->FindTag((pTag->m_pParent->GetPathName()+_T("Status.Input Done")).GetBuffer());
						if(pInputDone != NULL
						&& pInputDone->m_wQuality == OPC_QUALITY_GOOD
						&& pInputDone->m_Value.boolVal == VARIANT_TRUE)
						{
							pInputDone->m_Value.vt=VT_BOOL;
							pInputDone->m_Value.boolVal=VARIANT_FALSE;
							pInputDone->m_dwUpdateSequence++;
							CoFileTimeNow(&pInputDone->m_Timestamp);
						}
						break;
					}
				}
			}

			CString strData((LPSTR) &pIO->m_bRcvBuffer[6+iCommandLength]);
			pTag->m_Value=strData;
			pTag->m_wQuality=OPC_QUALITY_GOOD;

		}

		pTag->m_bCurrent=true;
		CoFileTimeNow(&pTag->m_Timestamp);
	}

	// Query Command
	// Bay Alarms
	// Trip Preset Alarms
	// Request SMP Status (Preset Status)
	else if(!strcmp(pTag->m_pszCommand,"Q")
	|| !strncmp(pTag->m_pszCommand,"MRBA",4)
	|| !strncmp(pTag->m_pszCommand,"MTPA",4)
	|| ((!strncmp(pTag->m_pszCommand,"MSS",3) || !strncmp(pTag->m_pszCommand,"MRS",3))
	&& !strcmp(&pTag->m_pszCommand[6],"0002")))
	{
		// iterate through the m_Leaf collection for all
		// Tags associated with response data.
		WORD wQuality=pTag->m_wQuality;
		CTag*	pParent;

		int iBaseIndex=0;
		if(!strcmp(pTag->m_pszCommand,"Q"))
			iBaseIndex=7;
		else if(!strncmp(pTag->m_pszCommand,"MRBA",4))
			iBaseIndex=10;
		else if(!strncmp(pTag->m_pszCommand,"MTPA",4))
			iBaseIndex=13;
		else if(!strncmp(pTag->m_pszCommand,"MSS",3))
			iBaseIndex=12;
		else if(!strncmp(pTag->m_pszCommand,"MRS",3))
			iBaseIndex=12;

		// The following is a bit unusual.  The RcuStatusTag
		// and the CardStatusTag are under the MultiloadTag
		// because they are returned with every response
		// but the m_pszCommand for them is "Q"
		if(pTag == m_pRcuStatusTag
		|| pTag == m_pCardStatusTag)
			pParent=m_pStatusTag;
		else
			pParent=pTag->m_pParent;

		POSITION pos=pParent->m_Leaf.GetHeadPosition();
		while(pos)
		{
			CTag* pTag=pParent->m_Leaf.GetNext(pos);
			if(FAILED(hr))
				pTag->m_wQuality=wQuality;
			else
			{
				BYTE bValue=pIO->m_bRcvBuffer[iBaseIndex+pTag->m_dwItem/4];
				if(bValue < 58)
					bValue-=48;
				else if(bValue < 71)
					bValue-=55;
				else
					bValue-=87;

				pTag->m_Value.vt=VT_BOOL;
				pTag->m_Value.boolVal=(bValue & (1 << (pTag->m_dwItem % 4))) ? VARIANT_TRUE : VARIANT_FALSE;	

				pTag->m_wQuality=OPC_QUALITY_GOOD;
			}

			CoFileTimeNow(&pTag->m_Timestamp);
			pTag->m_bCurrent=TRUE;
		}
	}

	// Additive Alarms
	// Meter Alarms
	// Component Alarms
	else if(!strncmp(pTag->m_pszCommand,"MRAA",4)
	|| !strncmp(pTag->m_pszCommand,"MRMA",4)
	|| !strncmp(pTag->m_pszCommand,"MRCA",4))
	{
		// Tags associated with response data.
		WORD wQuality=pTag->m_wQuality;
		CTag*	pAlarms=pTag->m_pParent->m_pParent;

		POSITION pos=pAlarms->m_Branch.GetHeadPosition();
		while(pos)
		{
			CTag* pSubAlarms=pAlarms->m_Branch.GetNext(pos);
			POSITION pos=pSubAlarms->m_Leaf.GetHeadPosition();
			while(pos)
			{
				CTag* pTag=pSubAlarms->m_Leaf.GetNext(pos);
				if(FAILED(hr))
					pTag->m_wQuality=wQuality;
				else
				{
					BYTE bValue=pIO->m_bRcvBuffer[13+pTag->m_dwItem/4];
					if(bValue < 58)
						bValue-=48;
					else if(bValue < 71)
						bValue-=55;
					else
						bValue-=87;

					pTag->m_Value.vt=VT_BOOL;
					pTag->m_Value.boolVal=(bValue & (1 << (pTag->m_dwItem % 4))) ? VARIANT_TRUE : VARIANT_FALSE;	

					pTag->m_wQuality=OPC_QUALITY_GOOD;
				}

				CoFileTimeNow(&pTag->m_Timestamp);
				pTag->m_bCurrent=TRUE;
			}
		}
	}

	// Request Status (State)
	else if((!strncmp(pTag->m_pszCommand,"MSS",3) || !strncmp(pTag->m_pszCommand,"MRS",3))
	&& (!strcmp(&pTag->m_pszCommand[6],"0001")))
	{
		if(FAILED(hr))
			pTag->m_wQuality=OPC_QUALITY_BAD;
		else
		{
			USHORT sValue;
			int iFields=sscanf((LPSTR) &pIO->m_bRcvBuffer[12],"%4hx",&sValue);
			if(iFields == 1)
			{
				pTag->m_Value.vt=VT_UI2;
				pTag->m_Value.iVal=sValue;
				pTag->m_wQuality=OPC_QUALITY_GOOD;
			}
			else
				pTag->m_wQuality=OPC_QUALITY_BAD;
		}

		CoFileTimeNow(&pTag->m_Timestamp);
		pTag->m_bCurrent=TRUE;
	}

	// Request Status (Batch Preset, Batch Gross, Batch Net, Avg Temp, Avg Press, Avg Density)
	// Request Status (Batch Component Gross, Net, Avg Temp, Avg Press, Avg Density)

	// Note: for Multiload II the 07C0 will require multiple components and should be
	//       processed like the 8000.  A test will need to be added for device type
	//       when suppport for the Multiload II is added
	else if((!strncmp(pTag->m_pszCommand,"MRS",3)
	&& (!strcmp(&pTag->m_pszCommand[6],"01F8")))
	|| (!strncmp(pTag->m_pszCommand,"MSS",3)
	&& (!strcmp(&pTag->m_pszCommand[6],"01F8")
	|| !strcmp(&pTag->m_pszCommand[6],"7C00"))))
	{
		WORD wQuality=pTag->m_wQuality;
		CTag* pGroup=pTag->m_pParent;
		POSITION pos=pGroup->m_Leaf.GetHeadPosition();
		while(pos)
		{
			CTag* pTag=pGroup->m_Leaf.GetNext(pos);
			if(FAILED(hr))
				pTag->m_wQuality=wQuality;
			else
			{
				if(pTag->m_NativeType == VT_UI4)
				{
					ULONG uValue;
					int iFields=sscanf((LPSTR) &pIO->m_bRcvBuffer[12+pTag->m_dwItem],"%9d",&uValue);
					if(iFields == 1)
					{
						pTag->m_Value.vt=VT_UI4;
						pTag->m_Value.ulVal=uValue;
						pTag->m_wQuality=OPC_QUALITY_GOOD;
					}
					else
						pTag->m_wQuality=OPC_QUALITY_BAD;
				}
				
				else if(pTag->m_NativeType == VT_UI2)
				{
					USHORT usValue;
					int iFields=sscanf((LPSTR) &pIO->m_bRcvBuffer[12+pTag->m_dwItem],"%6hd",&usValue);
					if(iFields == 1)
					{
						pTag->m_Value.vt=VT_UI2;
						pTag->m_Value.uiVal=usValue;
						pTag->m_wQuality=OPC_QUALITY_GOOD;
					}
					else
						pTag->m_wQuality=OPC_QUALITY_BAD;
				}

				else if(pTag->m_NativeType == VT_I2)
				{
					SHORT sValue;
					int iFields=sscanf((LPSTR) &pIO->m_bRcvBuffer[12+pTag->m_dwItem],"%6hd",&sValue);
					if(iFields == 1)
					{
						pTag->m_Value.vt=VT_UI2;
						pTag->m_Value.iVal=sValue;
						pTag->m_wQuality=OPC_QUALITY_GOOD;
					}
					else
						pTag->m_wQuality=OPC_QUALITY_BAD;
				}
			}

			CoFileTimeNow(&pTag->m_Timestamp);
			pTag->m_bCurrent=TRUE;
		}
	}
	else if(!strncmp(pTag->m_pszCommand,"MRS",3)	// multiload II
	&& (!strcmp(&pTag->m_pszCommand[6],"01F8")
	|| !strcmp(&pTag->m_pszCommand[6],"7C00")))
	{
		WORD wQuality=pTag->m_wQuality;
		CTag* pGroup=pTag->m_pParent;
		POSITION pos=pGroup->m_Leaf.GetHeadPosition();

		while(pos)
		{
			CTag* pTag=pGroup->m_Leaf.GetNext(pos);
			if(FAILED(hr))
				pTag->m_wQuality=wQuality;
			else
			{
				if(pTag->m_NativeType == VT_UI4)	// gross net vol
				{
					pTag->m_Value.ulVal = 0;
					for(int iLoop = 0;iLoop < 8;iLoop++)
					{
						ULONG uValue;
						int iFields=sscanf((LPSTR) &pIO->m_bRcvBuffer[12+pTag->m_dwItem],"%9d",&uValue);
						if(iFields == 1)
						{
							pTag->m_Value.vt=VT_UI4;
							pTag->m_Value.ulVal=uValue;
							pTag->m_wQuality=OPC_QUALITY_GOOD;
						}
						else
							pTag->m_wQuality=OPC_QUALITY_BAD;
					}
				}
				
				else if(pTag->m_NativeType == VT_UI2)	// temp and pressure
				{
					pTag->m_Value.uiVal=0;
					for(int iLoop = 0;iLoop < 8;iLoop++)
					{
						USHORT usValue;
						int iFields=sscanf((LPSTR) &pIO->m_bRcvBuffer[12+pTag->m_dwItem],"%6hd",&usValue);
						if(iFields == 1)
						{
							pTag->m_Value.vt=VT_UI2;
							pTag->m_Value.uiVal=usValue;
							pTag->m_wQuality=OPC_QUALITY_GOOD;
						}
						else
							pTag->m_wQuality=OPC_QUALITY_BAD;
					}
				}

				else if(pTag->m_NativeType == VT_I2)	// density
				{
					pTag->m_Value.iVal=0;
					for(int iLoop = 0;iLoop < 8;iLoop++)
					{
						SHORT sValue;
						int iFields=sscanf((LPSTR) &pIO->m_bRcvBuffer[12+pTag->m_dwItem],"%6hd",&sValue);
						if(iFields == 1)
						{
							pTag->m_Value.vt=VT_UI2;
							pTag->m_Value.iVal=sValue;
							pTag->m_wQuality=OPC_QUALITY_GOOD;
						}
						else
							pTag->m_wQuality=OPC_QUALITY_BAD;
					}
				}
			}

			CoFileTimeNow(&pTag->m_Timestamp);
			pTag->m_bCurrent=TRUE;
		}
	}

	// Request Status (Flow)
	else if((!strncmp(pTag->m_pszCommand,"MSS",3) || !strncmp(pTag->m_pszCommand,"MRS",3))
	&& (!strcmp(&pTag->m_pszCommand[6],"0200")))
	{
		if(FAILED(hr))
			pTag->m_wQuality=OPC_QUALITY_BAD;
		else
		{
			SHORT sValue;
			int iFields=sscanf((LPSTR) &pIO->m_bRcvBuffer[12+pTag->m_dwItem],"%5hd",&sValue);
			if(iFields == 1)
			{
				pTag->m_Value.vt=VT_UI2;
				pTag->m_Value.iVal=sValue;
				pTag->m_wQuality=OPC_QUALITY_GOOD;
			}
			else
				pTag->m_wQuality=OPC_QUALITY_BAD;
		}

		CoFileTimeNow(&pTag->m_Timestamp);
		pTag->m_bCurrent=TRUE;
	}

	// Request Status (Batch Additive Gross)
	else if((!strncmp(pTag->m_pszCommand,"MSS",3) || !strncmp(pTag->m_pszCommand,"MRS",3))
	&& !strcmp(&pTag->m_pszCommand[6],"8000"))
	{
		// Tags associated with response data.
		WORD wQuality=pTag->m_wQuality;
		CTag*	pGroup=pTag->m_pParent->m_pParent;

		POSITION pos=pGroup->m_Branch.GetHeadPosition();
		while(pos)
		{
			CTag* pNumber=pGroup->m_Branch.GetNext(pos);
			POSITION pos=pNumber->m_Leaf.GetHeadPosition();
			while(pos)
			{
				CTag* pTag=pNumber->m_Leaf.GetNext(pos);
				if(FAILED(hr))
					pTag->m_wQuality=wQuality;
				else
				{
					if(pTag->m_NativeType == VT_UI4)
					{
						ULONG uValue;
						int iFields=sscanf((LPSTR) &pIO->m_bRcvBuffer[12+pTag->m_dwItem],"%9d",&uValue);
						if(iFields == 1)
						{
							pTag->m_Value.vt=VT_UI4;
							pTag->m_Value.ulVal=uValue;
							pTag->m_wQuality=OPC_QUALITY_GOOD;
						}
						else
							pTag->m_wQuality=OPC_QUALITY_BAD;
					}
					
					else if(pTag->m_NativeType == VT_UI2)
					{
						USHORT usValue;
						int iFields=sscanf((LPSTR) &pIO->m_bRcvBuffer[12+pTag->m_dwItem],"%6hd",&usValue);
						if(iFields == 1)
						{
							pTag->m_Value.vt=VT_UI2;
							pTag->m_Value.uiVal=usValue;
							pTag->m_wQuality=OPC_QUALITY_GOOD;
						}
						else
							pTag->m_wQuality=OPC_QUALITY_BAD;
					}

					else if(pTag->m_NativeType == VT_I2)
					{
						SHORT sValue;
						int iFields=sscanf((LPSTR) &pIO->m_bRcvBuffer[12+pTag->m_dwItem],"%6hd",&sValue);
						if(iFields == 1)
						{
							pTag->m_Value.vt=VT_I2;
							pTag->m_Value.iVal=sValue;
							pTag->m_wQuality=OPC_QUALITY_GOOD;
						}
						else
							pTag->m_wQuality=OPC_QUALITY_BAD;
					}
				}
			}

			CoFileTimeNow(&pTag->m_Timestamp);
			pTag->m_bCurrent=TRUE;
		}
	}

	// Request Gross Totalizer
	else if(!strncmp(pTag->m_pszCommand,"R112",3))
	{
		// Tags associated with response data.
		WORD wQuality=pTag->m_wQuality;
		CTag*	pTotalizers=pTag->m_pParent->m_pParent;

		POSITION pos=pTotalizers->m_Branch.GetHeadPosition();
		while(pos)
		{
			CTag* pType=pTotalizers->m_Branch.GetNext(pos);
			POSITION pos=pType->m_Leaf.GetHeadPosition();
			while(pos)
			{
				CTag* pTag=pType->m_Leaf.GetNext(pos);
				if(FAILED(hr))
					pTag->m_wQuality=wQuality;
				else
				{
					ULONG uValue;
					int iFields=sscanf((LPSTR) &pIO->m_bRcvBuffer[13+pTag->m_dwItem],"%9d",&uValue);
					if(iFields == 1)
					{
						pTag->m_Value.vt=VT_UI4;
						pTag->m_Value.ulVal=uValue;
						pTag->m_wQuality=OPC_QUALITY_GOOD;
					}
					else
						pTag->m_wQuality=OPC_QUALITY_BAD;
				}

				CoFileTimeNow(&pTag->m_Timestamp);
				pTag->m_bCurrent=TRUE;
			}
		}
	}
	else if(!strncmp(pTag->m_pszCommand,"R962",3))	// card number
	{
		if(FAILED(hr))
			m_pCardNumberTag->m_wQuality=OPC_QUALITY_BAD;
		else
		{
			if(m_pCardStatusTag->m_Value.bVal == '1')
			{
				// determine if the card is a standard or TWIC card
				// for a standard the response is always
				// 0=yy=0000000=xxxxxxxx=01 where yy = facility number and xxxxxxxx is the card number
				if(pIO->m_bRcvBuffer[10] == '0' &&
					pIO->m_bRcvBuffer[11] == '=')
				{
					BYTE* pCardPointer = &(pIO->m_bRcvBuffer[10]); // We've already determined that this is valid.  We are at the start of the card data.

					// Move to the third equals sign
					// Note that we've already established that pIO->m_bRcvBuffer and pIO->wRcvLength have reasonable values by the time we get here.
					int delimCount = 0;
					for (; pCardPointer < (pIO->m_bRcvBuffer + pIO->m_wRcvLength); pCardPointer++)
					{
						if ('=' == *pCardPointer)
						{
							delimCount++;
						}

						if (3 <= delimCount)
						{
							// First delimiter starts the facility number
							// Second delimiter starts the fixed 7 zeroes
							// Third delimiter stars the card number
							break;
						}
					}

					if (3 == delimCount) // we found the third delimiter; desired card number is up to the next delimiter
					{
						// advance to next byte; this will put us at the beginning of the card number
						pCardPointer++;

						// find the next delimiter, for the end of the card number
						BYTE* pCardEnd = pCardPointer;
						for (; pCardEnd < (pIO->m_bRcvBuffer + pIO->m_wRcvLength); pCardEnd++)
						{
							if ('=' == *pCardEnd)
							{
								// Found the end delimter of the card number
								*pCardEnd = 0x00;
								CString strData((LPSTR) pCardPointer);
								m_pCardNumberTag->m_Value=strData;
								m_pCardNumberTag->m_wQuality=OPC_QUALITY_GOOD;
								break;
							}
						}
					}
				}
				// check for twic card format
				//1111,2222,333333,4,5,6666666666788889 – TWIC Card FASC-N that was read from the card.
				//Where,
				//1111 = Agency Code
				//2222 = System Code
				//333333 = Credential Number
				//4 = Credential Series
				//5 = Individual Credential Issue
				//6666666666 = Person Identifier (Used as Card Number in Access ID Database) this is the only field we are interested in at this time
				//7 = Organizational Category
				//8888 = Organizational Identifier
				//9 = Person/Organization Association
				else if(pIO->m_bRcvBuffer[14] == ',' &&
					pIO->m_bRcvBuffer[19] == ',')
				{
					CString strData((LPSTR) &pIO->m_bRcvBuffer[31]);
					m_pCardNumberTag->m_Value=strData;
					m_pCardNumberTag->m_wQuality=OPC_QUALITY_GOOD;
				}
			}
			else
			{
				CString strData("");
				m_pCardNumberTag->m_Value=strData;
				m_pCardNumberTag->m_wQuality=OPC_QUALITY_GOOD;
			}
		}

		m_pCardNumberTag->m_bCurrent=true;
		CoFileTimeNow(&m_pCardNumberTag->m_Timestamp);
	}

	// Query Card
	// Query Input
	else if(!strcmp(pTag->m_pszCommand,"QC")
	|| !strcmp(pTag->m_pszCommand,"QI"))
	{
		if(FAILED(hr))
			pTag->m_wQuality=OPC_QUALITY_BAD;
		else
		{
			CString strData((LPSTR) &pIO->m_bRcvBuffer[4]);
			pTag->m_Value=strData;
			pTag->m_wQuality=OPC_QUALITY_GOOD;
		}

		pTag->m_bCurrent=true;
		CoFileTimeNow(&pTag->m_Timestamp);
	}

	else
	{
		CoFileTimeNow(&pTag->m_Timestamp);
		pTag->m_bCurrent=TRUE;
	}

	return S_OK;
}

HRESULT CMultiloadDevice::PerformNetworkIO(CTag* pTag)
{
	CIO* pIO=pTag->m_pIO;
	pIO->m_wRcvLength=0;

	if(pIO->m_pSocket == NULL)
	{
		HRESULT hr=pIO->OpenSocket(pTag);
		if(FAILED(hr))
		{
			pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
			return E_FAIL;
		}
	}

	for(INT iTry=0;iTry < 3;iTry++)
	{

		if(SOCKET_ERROR == pIO->m_pSocket->Send(pIO->m_bXmtBuffer,pIO->m_wXmtLength))
		{
			CString oError;
			oError.Format(_T("IO Error = %s : CAsyncSocket.SendTo"),pIO->SocketError(pIO->m_pSocket->GetLastError()));
			theApp.LogError(oError);

			delete pIO->m_pSocket;
			pIO->m_pSocket=NULL;
			pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
			return E_FAIL;
		}

		DWORD dwNumberOfBytesRead=0;
		while(true)
		{
			dwNumberOfBytesRead=pIO->m_pSocket->Receive(	&pIO->m_bRcvBuffer[pIO->m_wRcvLength],
																	sizeof(pIO->m_bRcvBuffer)-pIO->m_wRcvLength-1);

			if(dwNumberOfBytesRead == SOCKET_ERROR
			|| dwNumberOfBytesRead == 0)
			{
				CString oError;
				oError.Format(_T("IO Error = %s : CAsyncSocket.Receive"),pIO->SocketError(pIO->m_pSocket->GetLastError()));
				theApp.LogError(oError);

				delete pIO->m_pSocket;
				pIO->m_pSocket=NULL;
				pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;

				pIO->SignalCommunicationsFailure(pTag);

				return E_FAIL;
			}

			pIO->m_wRcvLength+=(WORD) dwNumberOfBytesRead; 

			// Receipt is complete on &7F
			if(pIO->m_bRcvBuffer[pIO->m_wRcvLength-1] == 0x7F)
				break;
		}

		// Minimum 5 bytes
		if(dwNumberOfBytesRead < 5)
			continue;

		pIO->m_wRcvLength=(WORD) dwNumberOfBytesRead; 

		// First Byte is NL
		if(pIO->m_bRcvBuffer[0] != '\0')
			continue;

		// Second Byte is STX
		if(pIO->m_bRcvBuffer[1] != STX)
			continue;

		// Third and Fourth are Address
		if(pIO->m_bRcvBuffer[2] != pIO->m_bXmtBuffer[2]
		|| pIO->m_bRcvBuffer[3] != pIO->m_bXmtBuffer[3])
			continue;

		BYTE bLRC;
	
		// Last character should be 0x7F, it may be PAD or LRC
		if(pIO->m_bRcvBuffer[pIO->m_wRcvLength-1] != 0x7F)
			continue;

		bLRC=pIO->m_bRcvBuffer[pIO->m_wRcvLength-1];
		pIO->m_wRcvLength--;
			
		// Possible ETX or LRC
		if(pIO->m_bRcvBuffer[pIO->m_wRcvLength-1] == ETX)
		{
			if(pIO->m_bRcvBuffer[pIO->m_wRcvLength-2] == ETX)
			{
				bLRC=pIO->m_bRcvBuffer[pIO->m_wRcvLength-1];
				pIO->m_wRcvLength--;
			}
		}
		else
		{
			// Protocol Error
			if(pIO->m_bRcvBuffer[pIO->m_wRcvLength-2] != ETX)
				continue;

			bLRC=pIO->m_bRcvBuffer[pIO->m_wRcvLength-1];
			pIO->m_wRcvLength--;
		}

		// Skip NULL,STX prefix
		if(bLRC != pIO->LRC(&pIO->m_bRcvBuffer[2],pIO->m_wRcvLength-2))
			continue;

		break;
	}

	if(iTry == 3)
	{
		delete pIO->m_pSocket;
		pIO->m_pSocket=NULL;
		pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;

		pIO->SignalCommunicationsFailure(pTag);

		return E_FAIL;
	}

	// PerformNetworkIO called from multiple threads so must detach after each use
	pIO->m_hSocket=pIO->m_pSocket->Detach();
	delete pIO->m_pSocket;
	pIO->m_pSocket=NULL;

	pIO->SignalCommunicationsRestored(pTag);

	return S_OK; 
}

HRESULT CMultiloadDevice::PerformSerialIO(CTag* pTag)
{
	CIO* pIO=pTag->m_pIO;

	if ( pIO->m_bPortParametersChanged )
	{
		pIO->CloseComPort();
		pIO->m_bPortParametersChanged=FALSE;
	}
	
	if ( pIO->m_hPort == INVALID_HANDLE_VALUE )
	{
		HRESULT hr=pIO->OpenComPort(pTag->m_pDevice->m_Type);
		if(FAILED(hr))
		{
			pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
			pIO->SignalCommunicationsFailure(pTag);
			return E_FAIL;
		}
	}

	for(INT iTry=0;iTry < 3;iTry++)
	{
		DWORD		dwNumberOfBytesWritten=0;
		DWORD		dwNumberOfBytesRead=0;
		DWORD		dwCommErrFlags=0;
		DWORD		dwCommEvtFlags=0;
		COMSTAT	ComStat;

		pIO->m_wRcvLength=0;

		if(!ClearCommError(pIO->m_hPort,&dwCommErrFlags,&ComStat))
			continue;

		if(!PurgeComm( pIO->m_hPort,
							PURGE_RXCLEAR |
							PURGE_RXABORT |
							PURGE_TXCLEAR |
							PURGE_TXABORT))
			continue;

		// Read the response
	 	if(!SetCommMask(pIO->m_hPort,EV_ERR | EV_RXFLAG))
			continue;

		// Write the request
		pIO->m_WriteOverLapped.Offset=0;
		pIO->m_WriteOverLapped.OffsetHigh=0;
		if(!WriteFile(pIO->m_hPort,pIO->m_bXmtBuffer,pIO->m_wXmtLength,&dwNumberOfBytesWritten,&pIO->m_WriteOverLapped))
		{
			if(GetLastError() != ERROR_IO_PENDING)
				continue;

			if(!GetOverlappedResult(pIO->m_hPort,&pIO->m_WriteOverLapped,&dwNumberOfBytesWritten,TRUE))
				continue;
		}

		if(pIO->m_wXmtLength != dwNumberOfBytesWritten)
			continue;

	 	if(!WaitCommEvent(pIO->m_hPort,&dwCommEvtFlags,&pIO->m_CommOverLapped)
		&& GetLastError() != ERROR_IO_PENDING)
		{
			if(pTag->m_wQuality == OPC_QUALITY_COMM_FAILURE)
			{
				pIO->CloseComPort();
				pIO->m_bPortParametersChanged=FALSE;
				pIO->SignalCommunicationsFailure(pTag);

				return E_FAIL;
			}
			else
				continue;
		}

		switch(WaitForSingleObject(pIO->m_CommOverLapped.hEvent,pIO->m_dwCommunicationsTimeOut))
		{
			case WAIT_OBJECT_0:
	   		if((dwCommEvtFlags & EV_ERR ) == EV_ERR)
					continue;

   			else if((dwCommEvtFlags & EV_RXFLAG ) == EV_RXFLAG )
				{
					if(!ClearCommError(pIO->m_hPort,&dwCommErrFlags,&ComStat))
						continue;

					if(ComStat.cbInQue < 5)
						continue;

					if(ComStat.cbInQue > BUFFER_MAX-1)
						continue;

					pIO->m_ReadOverLapped.Offset=0;
					pIO->m_ReadOverLapped.OffsetHigh=0;
					if(!ReadFile(pIO->m_hPort,pIO->m_bRcvBuffer,ComStat.cbInQue,&dwNumberOfBytesRead,&pIO->m_ReadOverLapped)
					&& GetLastError() != ERROR_IO_PENDING )
						continue;

				 	if(!GetOverlappedResult(pIO->m_hPort,&pIO->m_ReadOverLapped,&dwNumberOfBytesRead,TRUE))
						continue;

					break;
				}
				else
					continue;

	      case WAIT_TIMEOUT:
				if(pTag->m_wQuality == OPC_QUALITY_COMM_FAILURE)
				{
					pIO->CloseComPort();
					pIO->m_bPortParametersChanged=FALSE;
					pIO->SignalCommunicationsFailure(pTag);

					return E_FAIL;
				}
				else
					continue;

	      case WAIT_FAILED:
			default:
				continue;
		}

		// Minimum 5 bytes
		if(dwNumberOfBytesRead < 5)
			continue;

		if(dwNumberOfBytesRead > BUFFER_MAX-1)
			continue;

		pIO->m_wRcvLength=(WORD) dwNumberOfBytesRead; 

		// First Byte is NL
		if(pIO->m_bRcvBuffer[0] != '\0')
			continue;

		// Second Byte is STX
		if(pIO->m_bRcvBuffer[1] != STX)
			continue;

		// Third and Fourth are Address
		if(pIO->m_bRcvBuffer[2] != pIO->m_bXmtBuffer[2]
		|| pIO->m_bRcvBuffer[3] != pIO->m_bXmtBuffer[3])
			continue;

		BYTE bLRC;
	
		// Last character should be 0x7F, it may be PAD or LRC
		if(pIO->m_bRcvBuffer[pIO->m_wRcvLength-1] != 0x7F)
			continue;

		bLRC=pIO->m_bRcvBuffer[pIO->m_wRcvLength-1];
		pIO->m_wRcvLength--;
			
		// Possible ETX or LRC
		if(pIO->m_bRcvBuffer[pIO->m_wRcvLength-1] == ETX)
		{
			if(pIO->m_bRcvBuffer[pIO->m_wRcvLength-2] == ETX)
			{
				bLRC=pIO->m_bRcvBuffer[pIO->m_wRcvLength-1];
				pIO->m_wRcvLength--;
			}
		}
		else
		{
			// Protocol Error
			if(pIO->m_bRcvBuffer[pIO->m_wRcvLength-2] != ETX)
				continue;

			bLRC=pIO->m_bRcvBuffer[pIO->m_wRcvLength-1];
			pIO->m_wRcvLength--;
		}

		// Skip NULL,STX prefix
		if(bLRC != pIO->LRC(&pIO->m_bRcvBuffer[2],pIO->m_wRcvLength-2))
			continue;

		break;
	}

	if(iTry == 3
	|| pIO->m_bPortParametersChanged)
	{
		pIO->CloseComPort();
		pIO->m_bPortParametersChanged=FALSE;
		pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;

		pIO->SignalCommunicationsFailure(pTag);

		return E_FAIL;
	}

	pIO->SignalCommunicationsRestored(pTag);

	return S_OK;
}

