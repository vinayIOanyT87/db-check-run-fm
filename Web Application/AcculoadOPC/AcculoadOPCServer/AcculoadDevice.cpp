/******************************************************************************

	FILE NAME:		AcculoadDevice.cpp


	PURPOSE:			Implementation of the CAcculoadDevice


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.1  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		-----------	----------  -------------------------------------------
		07/14/2008	W.Gray		7.4.5.0 - Revised to set OPC_QUALITY_BAD on Keypad Data tag
										and False on Keypad Data Pending after WritePrompt

		11/05/2008	W.Gray		7.4.6.0 - Revised PrepareRequest to set m_dwCommunicationsTimeout 
										default timeout to 2000

		11/25/2008	W.Gray		7.4.6.1 - Change PerformSerialIO to delay 5 seconds on OpenCommPort error (CSI 6319)

		11/25/2008	W.Gray		7.4.6.2 - Change PerformSerialIO to call SignalCommunicationsFailure
										when OpenCommPort fails

		12/07/2008	W.Gray		7.4.6.3 - Revised to ignore error NO39 on RR - Read Recipe Command (CSI 6328)

		12/29/2008	W.Gray		7.4.6.4 - Correction to error introduced 7.4.6.3 (CSI 6328)

		01/16/2009	W.Gray		7.4.6.5 - Revised to ignore error NO05 on RR

		01/23/2009	W.Gray		7.4.6.6 - Revised default communications timeout from 2 to 4 seconds
										also increased the minimum timeout on Card Reader awaiting CRC from
										10 to 20 milliseconds

		01/27/2009	W.Gray		7.4.6.7 - Correction to PerformSerialIO to check port handle prior to
										CloseHandle

		01/27/2009	W.Gray		7.4.6.7 - Change to call CloseComPort

		03/03/2009	W.Gray		7.4.6.8 - Changed to support CTPL (CSI 1794)

		03/03/2009	W.Gray		7.4.6.9 - Changed to force false on Released and Batch Done and
										force true on Authorized and Presetting In Progress on Authorize

		06/17/2009	W.Gray		7.4.6.10 - Change to not ignore errors on read keypad data (CSI 4042)

		06/18/2009	W.Gray		7.4.6.11 - Change to remove change made in 7.4.6.9 for forcing Presetting
										in progress to true.  Determined this change is not instantaneous (CSI 4042)

		07/17/2009	W.Gray		7.4.6.12 - Change Process Response to process WA - WG
										as far as resetting keypad data and keypad data pending.

		08/12/2009	W.Gray		7.4.6.13 - Added support for Accuload III-SA, all change
										to not report error on NO37 response to CD (CSI-5640)

		12/1/2009	W.Gray		7.4.6.14 - Revised timeout on failure of OpenCommPort from 5 to 1 second because
										it was taking to long to timeout.

		12/10/2009	W.Gray		7.5.1.0 - Revised to handle error on WaitCommEvent (WI 9947)

		01/06/2009	W.Gray		7.4.6.15 - Revised ProcessResponse to send changes to KeypadData,
										KeypadDataPending, and DisplayMessageTimeout only when changed by
										by successful Write command.

*******************************************************************************/

#include "StdAfx.h"
#include "AcculoadDevice.h"
#include "DeviceManager.h"


extern CDeviceManager*		g_pDeviceManager;

HRESULT CAcculoadDevice::PrepareRequest(CTag* pTag,BOOL bWrite)
{
	CIO* pIO=pTag->m_pIO;

	// Set default communications timeout
	pTag->m_pIO->m_dwCommunicationsTimeOut = 4000;

	if(!pTag->m_pIO->m_bNetworkCommunications)
		pIO->m_bXmtBuffer[0]=STX;
	else
		pIO->m_bXmtBuffer[0]='*';

	pIO->m_bXmtBuffer[1]=0x30+(pTag->m_bAddress / 10);
	pIO->m_bXmtBuffer[2]=0x30+(pTag->m_bAddress % 10);

	if(pTag->m_pszCommand)
	{
		// Enquire Alarms
		if(!strcmp(pTag->m_pszCommand,"EA"))
		{
			pIO->m_bXmtBuffer[3]=pTag->m_pszCommand[0];
			pIO->m_bXmtBuffer[4]=pTag->m_pszCommand[1];
			pIO->m_bXmtBuffer[5]=' ';
			pIO->m_bXmtBuffer[6]=pTag->m_pszSection[0];
			pIO->m_bXmtBuffer[7]=pTag->m_pszSection[1];
			if(!pTag->m_pIO->m_bNetworkCommunications)
			{
				pIO->m_bXmtBuffer[8]=ETX;
				pIO->m_bXmtBuffer[9]=pIO->LRC(&pIO->m_bXmtBuffer[1],8);
			}
			else
			{
				pIO->m_bXmtBuffer[8]=0x0D;
				pIO->m_bXmtBuffer[9]=0x0A;
			}
			pIO->m_wXmtLength=10;
		}

		// Request Prompt Data
		else if(!strcmp(pTag->m_pszCommand,"TI"))
		{
			pIO->m_bXmtBuffer[3]=pTag->m_pszCommand[0];
			pIO->m_bXmtBuffer[4]=pTag->m_pszCommand[1];
			pIO->m_bXmtBuffer[5]=' ';
			pIO->m_bXmtBuffer[6]=pTag->m_pszSection[0];

			if(!pTag->m_pIO->m_bNetworkCommunications)
			{
				pIO->m_bXmtBuffer[7]=ETX;
				pIO->m_bXmtBuffer[8]=pIO->LRC(&pIO->m_bXmtBuffer[1],7);
			}
			else
			{
				pIO->m_bXmtBuffer[7]=0x0D;
				pIO->m_bXmtBuffer[8]=0x0A;
			}
			pIO->m_wXmtLength=9;
		}

		// Output Relay
		else if(!strcmp(pTag->m_pszCommand,"OR"))
		{
			pIO->m_bXmtBuffer[3]=pTag->m_pszCommand[0];
			pIO->m_bXmtBuffer[4]=pTag->m_pszCommand[1];
			pIO->m_bXmtBuffer[5]=' ';
			pIO->m_bXmtBuffer[6]=pTag->m_pszSection[0];
			pIO->m_bXmtBuffer[7]=pTag->m_pszSection[1];
			pIO->m_bXmtBuffer[8]=' ';
			if(pTag->m_Value.boolVal == VARIANT_TRUE)
				pIO->m_bXmtBuffer[9]='1';
			else
				pIO->m_bXmtBuffer[9]='0';

			if(!pTag->m_pIO->m_bNetworkCommunications)
			{
				pIO->m_bXmtBuffer[10]=ETX;
				pIO->m_bXmtBuffer[11]=pIO->LRC(&pIO->m_bXmtBuffer[1],10);
			}
			else
			{
				pIO->m_bXmtBuffer[10]=0x0D;
				pIO->m_bXmtBuffer[11]=0x0A;
			}
			pIO->m_wXmtLength=12;
		}

		// Dynamic Values
		else if(!strcmp(pTag->m_pszCommand,"DY"))
		{
			pIO->m_bXmtBuffer[3]=pTag->m_pszCommand[0];
			pIO->m_bXmtBuffer[4]=pTag->m_pszCommand[1];
			pIO->m_bXmtBuffer[5]=' ';
			pIO->m_bXmtBuffer[6]=pTag->m_pszSection[0];
			pIO->m_bXmtBuffer[7]=pTag->m_pszSection[1];
			pIO->m_bXmtBuffer[8]=0x30+(BYTE) (pTag->m_dwItem / 10);
			pIO->m_bXmtBuffer[9]=0x30+(BYTE) (pTag->m_dwItem % 10);

			if(!pTag->m_pIO->m_bNetworkCommunications)
			{
				pIO->m_bXmtBuffer[10]=ETX;
				pIO->m_bXmtBuffer[11]=pIO->LRC(&pIO->m_bXmtBuffer[1],10);
			}
			else
			{
				pIO->m_bXmtBuffer[10]=0x0D;
				pIO->m_bXmtBuffer[11]=0x0A;
			}
			pIO->m_wXmtLength=12;
		}

		// End Transaction
		// End Batch
		// Stop Arm
		// Stop All Arms
		// Start Arm
		// Clear Transactions in Standby Mode
		// Enquire Status
		// Swing Arm Position
		// Read Keypad
		// Release Keypad and Display
		// Read Preset Amount
		// Log Out of Program Mode
		// Get Batch Number
		// Get Key
		// Force Full Screen View
		// Request Recipe
		else if(!strcmp(pTag->m_pszCommand,"ET")
		|| !strcmp(pTag->m_pszCommand,"EB")
		|| !strcmp(pTag->m_pszCommand,"ST")
		|| !strcmp(pTag->m_pszCommand,"SP")
		|| !strcmp(pTag->m_pszCommand,"SA")
		|| !strcmp(pTag->m_pszCommand,"CT")
		|| !strcmp(pTag->m_pszCommand,"EQ")
		|| !strcmp(pTag->m_pszCommand,"SW")
		|| !strcmp(pTag->m_pszCommand,"RK")
		|| !strcmp(pTag->m_pszCommand,"DA")
		|| !strcmp(pTag->m_pszCommand,"RP")
		|| !strcmp(pTag->m_pszCommand,"GK")
		|| !strcmp(pTag->m_pszCommand,"FS")
		|| !strcmp(pTag->m_pszCommand,"RR")
		|| !strcmp(pTag->m_pszCommand, "OA")
		)
		{
			pIO->m_bXmtBuffer[3]=pTag->m_pszCommand[0];
			pIO->m_bXmtBuffer[4]=pTag->m_pszCommand[1];

			if(!pTag->m_pIO->m_bNetworkCommunications)
			{
				pIO->m_bXmtBuffer[5]=ETX;
				pIO->m_bXmtBuffer[6]=pIO->LRC(&pIO->m_bXmtBuffer[1],5);
			}
			else
			{
				pIO->m_bXmtBuffer[5]=0x0D;
				pIO->m_bXmtBuffer[6]=0x0A;
			}
			pIO->m_wXmtLength=7;
		}

		else if ( !strcmp(pTag->m_pszCommand,"LO") )
		{
			pIO->m_bXmtBuffer[3]=pTag->m_pszCommand[0];
			pIO->m_bXmtBuffer[4]=pTag->m_pszCommand[1];

			if(!pTag->m_pIO->m_bNetworkCommunications)
			{
				pIO->m_bXmtBuffer[5]=ETX;
				pIO->m_bXmtBuffer[6]=pIO->LRC(&pIO->m_bXmtBuffer[1],5);
			}
			else
			{
				pIO->m_bXmtBuffer[5]=0x0D;
				pIO->m_bXmtBuffer[6]=0x0A;
			}
			pIO->m_wXmtLength=7;

			// LO requires a higher timeout
			pTag->m_pIO->m_dwCommunicationsTimeOut = 2000;
		}

		else if (!strcmp(pTag->m_pszCommand,"RB"))
		{
			pIO->m_bXmtBuffer[3]=pTag->m_pszCommand[0];
			pIO->m_bXmtBuffer[4]=pTag->m_pszCommand[1];

			if(!pTag->m_pIO->m_bNetworkCommunications)
			{
				pIO->m_bXmtBuffer[5]=ETX;
				pIO->m_bXmtBuffer[6]=pIO->LRC(&pIO->m_bXmtBuffer[1],5);
			}
			else
			{
				pIO->m_bXmtBuffer[5]=0x0D;
				pIO->m_bXmtBuffer[6]=0x0A;
			}
			pIO->m_wXmtLength=7;
		}

		// Authorize Preset
		// Allocate Blend Recipes
		// Authorize And Set Batch Amount
		else if(!strcmp(pTag->m_pszCommand,"AP")
		|| !strcmp(pTag->m_pszCommand,"AB")
		|| !strcmp(pTag->m_pszCommand,"SB"))
		{
			pIO->m_bXmtBuffer[3]=pTag->m_pszCommand[0];
			pIO->m_bXmtBuffer[4]=pTag->m_pszCommand[1];
			if(pTag->m_Value.vt == VT_BSTR)
			{
				CStringA	oString((LPTSTR) pTag->m_Value.bstrVal);
				int iLen=oString.GetLength();
				if(iLen != 0)
				{
					pIO->m_bXmtBuffer[5]=' ';
					strncpy((LPSTR) &pIO->m_bXmtBuffer[6],oString,iLen);
					iLen++;
				}

				if(!pTag->m_pIO->m_bNetworkCommunications)
				{
					pIO->m_bXmtBuffer[5+iLen]=ETX;
					pIO->m_bXmtBuffer[6+iLen]=pIO->LRC(&pIO->m_bXmtBuffer[1],5+iLen);
				}
				else
				{
					pIO->m_bXmtBuffer[5+iLen]=0x0D;
					pIO->m_bXmtBuffer[6+iLen]=0x0A;
				}
				pIO->m_wXmtLength=7+iLen;
			}
			else
				return E_FAIL;
		}

		// Write Second Line
		// Write Third Line
		// Write Fourth Line
		// Write First Line With Prompt
		// Write First Line With Prompt On Set
		// Write First Line With Prompt On Set No Echo
		// Write First Line With Prompt No Echo
		else if(!strcmp(pTag->m_pszCommand,"WA")
		|| !strcmp(pTag->m_pszCommand,"WB")
		|| !strcmp(pTag->m_pszCommand,"WC")
		|| !strcmp(pTag->m_pszCommand,"WD")
		|| !strcmp(pTag->m_pszCommand,"WE")
		|| !strcmp(pTag->m_pszCommand,"WF")
		|| !strcmp(pTag->m_pszCommand,"WG")
		|| !strcmp(pTag->m_pszCommand,"WP")
		|| !strcmp(pTag->m_pszCommand,"WQ")
		|| !strcmp(pTag->m_pszCommand,"WX"))
		{
			pIO->m_bXmtBuffer[3]=pTag->m_pszCommand[0];
			pIO->m_bXmtBuffer[4]=pTag->m_pszCommand[1];
			if(pTag->m_Value.vt == VT_BSTR)
			{
				CStringA	oString((LPTSTR) pTag->m_Value.bstrVal);
				int iLen=oString.GetLength();
				strncpy((LPSTR) &pIO->m_bXmtBuffer[5],oString,iLen);

				if(!pTag->m_pIO->m_bNetworkCommunications)
				{
					pIO->m_bXmtBuffer[5+iLen]=ETX;
					pIO->m_bXmtBuffer[6+iLen]=pIO->LRC(&pIO->m_bXmtBuffer[1],5+iLen);
				}
				else
				{
					pIO->m_bXmtBuffer[5+iLen]=0x0D;
					pIO->m_bXmtBuffer[6+iLen]=0x0A;
				}
				pIO->m_wXmtLength=7+iLen;
			}
			else
				return E_FAIL;
		}


		// Reset
		else if(!strcmp(pTag->m_pszCommand,"RE"))
		{
			pIO->m_bXmtBuffer[3]=pTag->m_pszCommand[0];
			pIO->m_bXmtBuffer[4]=pTag->m_pszCommand[1];
			pIO->m_bXmtBuffer[5]=' ';
			pIO->m_bXmtBuffer[6]=pTag->m_pszSection[0];
			pIO->m_bXmtBuffer[7]=pTag->m_pszSection[1];

			if(!pTag->m_pIO->m_bNetworkCommunications)
			{
				pIO->m_bXmtBuffer[8]=ETX;
				pIO->m_bXmtBuffer[9]=pIO->LRC(&pIO->m_bXmtBuffer[1],8);
			}
			else
			{
				pIO->m_bXmtBuffer[8]=0x0D;
				pIO->m_bXmtBuffer[9]=0x0A;
			}
			
			pIO->m_wXmtLength=10;
		}

		// Card Data - Command is combined with control outputs
		else if(!strcmp(pTag->m_pszCommand,"CD"))
		{
			pIO->m_bXmtBuffer[3]=pTag->m_pszCommand[0];
			pIO->m_bXmtBuffer[4]=pTag->m_pszCommand[1];
			if(pTag->m_pDevice->m_Type != ACCULOAD_III_SA)
			{
				pIO->m_bXmtBuffer[5]=' ';
				CTagList*	pLeaf=&pTag->m_pParent->m_Leaf;
				POSITION	pos=pLeaf->GetHeadPosition();

				// First Entry is the Data Tag
				pLeaf->GetNext(pos);

				// Remaining Tags are Control Tags
				BYTE	bControl=0;
				while(pos)
				{
					CTag* pControlTag=pLeaf->GetNext(pos);
					if(pControlTag->m_Value.vt == VT_BOOL
					&& pControlTag->m_Value.boolVal)
						bControl|=(1 << pControlTag->m_dwItem);
				}
				pIO->m_bXmtBuffer[6]=0x30+((bControl & 0xF0) >> 4);
				pIO->m_bXmtBuffer[7]=0x30+(bControl & 0xF);

				if(!pTag->m_pIO->m_bNetworkCommunications)
				{
					pIO->m_bXmtBuffer[8]=ETX;
					pIO->m_bXmtBuffer[9]=pIO->LRC(&pIO->m_bXmtBuffer[1],8);
				}
				else
				{
					pIO->m_bXmtBuffer[8]=0x0D;
					pIO->m_bXmtBuffer[9]=0x0A;
				}
				pIO->m_wXmtLength=10;
			}

			else
			{
				if(!pTag->m_pIO->m_bNetworkCommunications)
				{
					pIO->m_bXmtBuffer[5]=ETX;
					pIO->m_bXmtBuffer[6]=pIO->LRC(&pIO->m_bXmtBuffer[1],5);
				}
				else
				{
					pIO->m_bXmtBuffer[5]=0x0D;
					pIO->m_bXmtBuffer[6]=0x0A;
				}
				pIO->m_wXmtLength=7;
			}
		}

		// Analog Inputs
		else if(!strcmp(pTag->m_pszCommand,"RD"))
		{
			pIO->m_bXmtBuffer[3]=pTag->m_pszCommand[0];
			pIO->m_bXmtBuffer[4]=pTag->m_pszCommand[1];
			pIO->m_bXmtBuffer[5]=' ';

			switch(pTag->m_dwItem)
			{
				case 0:
					pIO->m_bXmtBuffer[6]='T';
					break;
				case 1:
					pIO->m_bXmtBuffer[6]='P';
					break;
				case 2:
					pIO->m_bXmtBuffer[6]='D';
					break;
			}

			// Product
			if(pTag->m_pszSection[0] == 'P')
			{
				pIO->m_bXmtBuffer[7]=pTag->m_pszSection[0];
				pIO->m_bXmtBuffer[8]=pTag->m_pszSection[1];

				if(!pTag->m_pIO->m_bNetworkCommunications)
				{
					pIO->m_bXmtBuffer[9]=ETX;
					pIO->m_bXmtBuffer[10]=pIO->LRC(&pIO->m_bXmtBuffer[1],9);
				}
				else
				{
					pIO->m_bXmtBuffer[9]=0x0D;
					pIO->m_bXmtBuffer[10]=0x0A;
				}
				pIO->m_wXmtLength=11;
			}

			// Arm
			else
			{
				if(!pTag->m_pIO->m_bNetworkCommunications)
				{
					pIO->m_bXmtBuffer[7]=ETX;
					pIO->m_bXmtBuffer[8]=pIO->LRC(&pIO->m_bXmtBuffer[1],7);
				}
				else
				{
					pIO->m_bXmtBuffer[7]=0x0D;
					pIO->m_bXmtBuffer[8]=0x0A;
				}
				pIO->m_wXmtLength=9;
			}
		}

		// Non-resetable totals
		else if(!strcmp(pTag->m_pszCommand,"VT"))
		{
			pIO->m_bXmtBuffer[3]=pTag->m_pszCommand[0];
			pIO->m_bXmtBuffer[4]=pTag->m_pszCommand[1];
			pIO->m_bXmtBuffer[5]=' ';

			// Additive
			if(pTag->m_pszSection[0] == 'A')
			{
				int nSectionLength = strlen( pTag->m_pszSection );
				strncpy( (LPSTR) &pIO->m_bXmtBuffer[6], pTag->m_pszSection, nSectionLength );
				
				if(!pTag->m_pIO->m_bNetworkCommunications)
				{
					pIO->m_bXmtBuffer[6 + nSectionLength + 0]=ETX;
					pIO->m_bXmtBuffer[6 + nSectionLength + 1]=pIO->LRC(&pIO->m_bXmtBuffer[1],6 + nSectionLength + 0);
				}
				else
				{
					pIO->m_bXmtBuffer[6 + nSectionLength + 0]=0x0D;
					pIO->m_bXmtBuffer[6 + nSectionLength + 1]=0x0A;
				}

				pIO->m_wXmtLength=6 + nSectionLength + 2;
			}

			// Product
			else
			{
				switch(pTag->m_dwItem)
				{
					case 0:		//Raw Volume
						pIO->m_bXmtBuffer[6]='R';
						break;
					case 1:		// Gross Volume
						pIO->m_bXmtBuffer[6]='G';
						break;
					case 2:		// GST Volume
						pIO->m_bXmtBuffer[6]='N';
						break;
					case 3:		// GSV Volume
						pIO->m_bXmtBuffer[6]='P';
						break;
					case 4:		// Mass
						pIO->m_bXmtBuffer[6]='R';
						break;
				}

				int nSectionLength = strlen( pTag->m_pszSection );
				if ( nSectionLength == 0 )
				{
					if(!pTag->m_pIO->m_bNetworkCommunications)
					{
						pIO->m_bXmtBuffer[7]=ETX;
						pIO->m_bXmtBuffer[8]=pIO->LRC(&pIO->m_bXmtBuffer[1],7);
					}
					else
					{
						pIO->m_bXmtBuffer[7]=0x0D;
						pIO->m_bXmtBuffer[8]=0x0A;
					}

					pIO->m_wXmtLength=9;
				}
				else
				{
					pIO->m_bXmtBuffer[7]=' ';
					strncpy( (LPSTR) &pIO->m_bXmtBuffer[8], pTag->m_pszSection, nSectionLength );
					if(!pTag->m_pIO->m_bNetworkCommunications)
					{
						pIO->m_bXmtBuffer[8 + nSectionLength + 0]=ETX;
						pIO->m_bXmtBuffer[8 + nSectionLength + 1]=pIO->LRC(&pIO->m_bXmtBuffer[1],8 + nSectionLength + 0);
					}
					else
					{
						pIO->m_bXmtBuffer[8 + nSectionLength + 0]=0x0D;
						pIO->m_bXmtBuffer[8 + nSectionLength + 1]=0x0A;
					}

					pIO->m_wXmtLength=8 + nSectionLength + 2;
				}
			}
		}

		// PC - Program Code Change
		else if(!strcmp(pTag->m_pszCommand,"PC"))
		{
			pIO->m_bXmtBuffer[3]=pTag->m_pszCommand[0];
			pIO->m_bXmtBuffer[4]=pTag->m_pszCommand[1];
			pIO->m_bXmtBuffer[5]=' ';
			if(pTag->m_Value.vt == VT_BSTR)
			{
				CStringA	oString((LPTSTR) pTag->m_Value.bstrVal);
				int iLen=oString.GetLength();
				strncpy((LPSTR) &pIO->m_bXmtBuffer[6],oString,iLen);

				if(!pTag->m_pIO->m_bNetworkCommunications)
				{
					pIO->m_bXmtBuffer[6+iLen]=ETX;
					pIO->m_bXmtBuffer[7+iLen]=pIO->LRC(&pIO->m_bXmtBuffer[1],6+iLen);
				}
				else
				{
					pIO->m_bXmtBuffer[6+iLen]=0x0D;
					pIO->m_bXmtBuffer[7+iLen]=0x0A;
				}

				pIO->m_wXmtLength=8+iLen;
			}
			else
				return E_FAIL;
		}

		// PV - Program Code Value
		// PC - Program Code Change
		else if(!strcmp(pTag->m_pszCommand,"PV"))
		{
			pIO->m_bXmtBuffer[3]=pTag->m_pszCommand[0];
			if(bWrite)
				pIO->m_bXmtBuffer[4]='C';
			else
				pIO->m_bXmtBuffer[4]='V';
			pIO->m_bXmtBuffer[5]=' ';
			pIO->m_bXmtBuffer[6]=pTag->m_pszSection[0];
			pIO->m_bXmtBuffer[7]=pTag->m_pszSection[1];
			pIO->m_bXmtBuffer[8]=' ';
			
			char szParameter[4];
			sprintf(szParameter,"%03d",pTag->m_dwItem);
			pIO->m_bXmtBuffer[9]=szParameter[0];
			pIO->m_bXmtBuffer[10]=szParameter[1];
			pIO->m_bXmtBuffer[11]=szParameter[2];

			if(bWrite)
			{
				int iLength=0;
				pIO->m_bXmtBuffer[12]=' ';
				if(pTag->m_Value.vt == VT_BSTR)
				{
					// Recipe Name
					if(pTag->m_pszSection[0] >= '0'
					&& pTag->m_pszSection[0] <= '5')
					{
						CStringA strValue(pTag->m_Value.bstrVal);
						strncpy((char*) &pIO->m_bXmtBuffer[13],strValue.GetBuffer(0),9);
						iLength=strValue.GetLength();						
					}
				}
				else if(pTag->m_Value.vt == VT_R8)
				{
					CStringA strValue;
					strValue.Format("%f",pTag->m_Value.dblVal);
					strcpy((char*) &pIO->m_bXmtBuffer[13],strValue.GetBuffer(0));
					iLength=strValue.GetLength();
				}
				else
				{
					CStringA strValue;
					strValue.Format("%d",pTag->m_Value.iVal);
					strcpy((char*) &pIO->m_bXmtBuffer[13],strValue.GetBuffer(0));
					iLength=strValue.GetLength();
				}

				if(!pTag->m_pIO->m_bNetworkCommunications)
				{
					pIO->m_bXmtBuffer[13+iLength]=ETX;
					pIO->m_bXmtBuffer[14+iLength]=pIO->LRC(&pIO->m_bXmtBuffer[1],13+iLength);
				}
				else
				{
					pIO->m_bXmtBuffer[13+iLength]=0x0D;
					pIO->m_bXmtBuffer[14+iLength]=0x0A;
				}
				pIO->m_wXmtLength=15+iLength;
			}
			else
			{
				if(!pTag->m_pIO->m_bNetworkCommunications)
				{
					pIO->m_bXmtBuffer[12]=ETX;
					pIO->m_bXmtBuffer[13]=pIO->LRC(&pIO->m_bXmtBuffer[1],12);
				}
				else
				{
					pIO->m_bXmtBuffer[12]=0x0D;
					pIO->m_bXmtBuffer[13]=0x0A;
				}

				pIO->m_wXmtLength=14;
			}
		}

		else if (!strcmp(pTag->m_pszCommand,"SD"))
		{
			pIO->m_bXmtBuffer[3]=pTag->m_pszCommand[0];
			pIO->m_bXmtBuffer[4]=pTag->m_pszCommand[1];

			if ( bWrite )	
			{
				if ( pTag->m_Value.vt == VT_BSTR )
				{
					CStringA oString( (LPTSTR) pTag->m_Value.bstrVal );
					int iLen = oString.GetLength();

					if ( iLen != 0 )
					{
						pIO->m_bXmtBuffer[5] = ' ';
						strncpy( (LPSTR) &pIO->m_bXmtBuffer[6], oString, iLen );
						++iLen;
					}

					if(!pTag->m_pIO->m_bNetworkCommunications)
					{
						pIO->m_bXmtBuffer[5+iLen] = ETX;
						pIO->m_bXmtBuffer[6+iLen] = pIO->LRC(&pIO->m_bXmtBuffer[1], 5 + iLen );
					}
					else
					{
						pIO->m_bXmtBuffer[5+iLen]=0x0D;
						pIO->m_bXmtBuffer[6+iLen]=0x0A;
					}

					pIO->m_wXmtLength = 7 + iLen;

				}
				else
					return E_FAIL;
			}
			else
				return E_FAIL;

		}

		else if (!strcmp(pTag->m_pszCommand,"AR"))
		{
			pIO->m_bXmtBuffer[3]=pTag->m_pszCommand[0];
			pIO->m_bXmtBuffer[4]=pTag->m_pszCommand[1];
			pIO->m_bXmtBuffer[5]=' ';
			pIO->m_bXmtBuffer[6]=pTag->m_pszSection[0];
			pIO->m_bXmtBuffer[7]=pTag->m_pszSection[1];
			pIO->m_bXmtBuffer[8]=' ';
			
			switch ( pTag->m_dwItem )
			{
				case 0:
					pIO->m_bXmtBuffer[9] = 'S';
					pIO->m_bXmtBuffer[10] = 'Y';
					break;

				case 1:
					pIO->m_bXmtBuffer[9] = 'D';
					pIO->m_bXmtBuffer[10] = 'D';
					break;

			}

			if(!pTag->m_pIO->m_bNetworkCommunications)
			{
				pIO->m_bXmtBuffer[11]=ETX;
				pIO->m_bXmtBuffer[12]=pIO->LRC(&pIO->m_bXmtBuffer[1],11);
			}
			else
			{
				pIO->m_bXmtBuffer[11]=0x0D;
				pIO->m_bXmtBuffer[12]=0x0A;
			}

			pIO->m_wXmtLength=13;

		}

		else
			return E_FAIL;
	}	
	else
		return E_FAIL;

	return S_OK;
}



HRESULT CAcculoadDevice::ProcessResponse(CTag* pTag,HRESULT hr)
{
	CIO* pIO=pTag->m_pIO;

	// Replace the ETX or CR with '\0' to ensure string is null terminated
	if(SUCCEEDED(hr)
	&& pIO->m_wRcvLength > 0)
		pIO->m_bRcvBuffer[pIO->m_wRcvLength-1]='\0';

	if(pTag->m_pszCommand)
	{
		// Enquire Alarms - group of alarms are received
		// iterate through the m_Leaf collection for all
		// Tags associated with response data.
		if(!strcmp(pTag->m_pszCommand,"EA"))
		{
			WORD wQuality=pTag->m_wQuality;
			CTag*	pParent=pTag->m_pParent;
			POSITION pos=pParent->m_Leaf.GetHeadPosition();
			while(pos)
			{
				CTag* pTag=pParent->m_Leaf.GetNext(pos);
				if(FAILED(hr))
					pTag->m_wQuality=wQuality;
				else
				{
					pTag->m_Value.vt=VT_BOOL;
					pTag->m_Value.boolVal=((pIO->m_bRcvBuffer[4+pTag->m_dwItem/4]-0x30) & (1 << (pTag->m_dwItem % 4))) ? VARIANT_TRUE : VARIANT_FALSE;	
					pTag->m_wQuality=OPC_QUALITY_GOOD;
				}
				CoFileTimeNow(&pTag->m_Timestamp);
				pTag->m_bCurrent=TRUE;
			}
		}

		// Enquire Status - group of status are received,
		// iterate through the m_Leaf collection for all
		// Tags associated with response data
		else if(!strcmp(pTag->m_pszCommand,"EQ"))
		{
			WORD wQuality=pTag->m_wQuality;
			CTag*	pParent=pTag->m_pParent;
			POSITION pos=pParent->m_Leaf.GetHeadPosition();
			BOOL	bMessageIsBad = false;

			// verify that the message is valid based on the Accuload documentation
			// this should be a specific number of characters for an EQ response, depending on device type
			if(!FAILED(hr))
			{
				int iSTXPosition = 0;
				int iETXPosition = 0;
				for(int iLoop = 0; iLoop < pIO->m_wRcvLength;iLoop++)
				{
					if(pIO->m_bRcvBuffer[iLoop] == 0x02 ||
						pIO->m_bRcvBuffer[iLoop] == 0x2A) // Ethernet response begins with CD 2A, serial begins with 00 02
						iSTXPosition = iLoop;
					if(pIO->m_bRcvBuffer[iLoop] == 0x03 ||
						pIO->m_bRcvBuffer[iLoop] == 0x0D || 
						pIO->m_bRcvBuffer[iLoop] == 0x00) // Ethernet response ends with 0D, serial ends with 03.  Also defend against end of string.
						iETXPosition = iLoop;
					// the accuload does not always send an ETX sometimes it is a 0b???????? so check for the 7f and set based on that
					if(pIO->m_bRcvBuffer[iLoop] == 0x7f)
						iETXPosition = iLoop - 1;
				}
				if(iETXPosition == 0)
					iETXPosition = pIO->m_wRcvLength - 1;

				// offset etx by -1 for the null and stx by 3 for the accuload address
				int iResponseLength = iETXPosition - (iSTXPosition + 3);
				switch (this->m_Type)
				{
				case ACCULOAD_III_Q:
				case ACCULOAD_III_SA:
					//if (iResponseLength != 16) 
					//{
					//	bMessageIsBad = true;
					//}
					break;
				case MICROLOAD_NET:
					//if (iResponseLength != 6)
					//{
					//	bMessageIsBad = true;
					//}
					break;
				case ACCULOAD_2_STD:
					if (iResponseLength != 6)
					{
						bMessageIsBad = true;
					}
					break;
				case ACCULOAD_2_SEQ:
				case ACCULOAD_2_RBM:
					// unable to find documentation giving the format of the EQ response for these two devices
					break;
				default:
					// Should not come here; Multiloads should go through the MultiloadDevice class
					bMessageIsBad = true;
				}
			}
			while(pos)
			{
				CTag* pTag=pParent->m_Leaf.GetNext(pos);

				// Only do the "EQ" tags
				if ( pTag->m_pszCommand[0] == 'E' && pTag->m_pszCommand[1] == 'Q' )
				{
					if(FAILED(hr))
					{
						if(pTag->m_wQuality != wQuality)
						{
							pTag->m_wQuality=wQuality;
							pTag->m_dwUpdateSequence++;
						}
					}
					else if(bMessageIsBad)
					{
						pTag->m_wQuality=OPC_QUALITY_BAD;
						pTag->m_dwUpdateSequence++;
					}
					else
					{
						BOOL boolVal=((pIO->m_bRcvBuffer[4+pTag->m_dwItem/4]-0x30) & (1 << (pTag->m_dwItem % 4))) ? VARIANT_TRUE : VARIANT_FALSE;
						if(pTag->m_Value.boolVal != boolVal
						|| pTag->m_wQuality != OPC_QUALITY_GOOD)
						{
							pTag->m_Value.vt=VT_BOOL;
							pTag->m_Value.boolVal=((pIO->m_bRcvBuffer[4+pTag->m_dwItem/4]-0x30) & (1 << (pTag->m_dwItem % 4))) ? VARIANT_TRUE : VARIANT_FALSE;	
							pTag->m_wQuality=OPC_QUALITY_GOOD;
							pTag->m_dwUpdateSequence++;
						}
					}
					CoFileTimeNow(&pTag->m_Timestamp);
					pTag->m_bCurrent=TRUE;
				}
			}
		}

		// Reset Power-fail Alarm
		else if (!strcmp(pTag->m_pszCommand,"AR"))
		{
			if(SUCCEEDED(hr))
			{
				if(pIO->m_bRcvBuffer[4] == 'N'
				&& pIO->m_bRcvBuffer[5] == 'O')
				{
					// NO03 - We get this one if the alarm is not active - just ignore
					if((pIO->m_bRcvBuffer[6] != '0'	|| pIO->m_bRcvBuffer[7] != '3'))
						ReportError(pTag);
					pTag->m_wQuality=OPC_QUALITY_BAD;
				}
				else
					pTag->m_wQuality=OPC_QUALITY_GOOD;
			}
		}

		// Reset
		// Release Keypad and Display
		// Output Relay
		else if(!strcmp(pTag->m_pszCommand,"RE")
		|| !strcmp(pTag->m_pszCommand,"DA")
		|| !strcmp(pTag->m_pszCommand,"OR"))
		{
			if(SUCCEEDED(hr))
			{
				if(pIO->m_bRcvBuffer[4] == 'N'
				&& pIO->m_bRcvBuffer[5] == 'O')
				{
					// NO06 - Operation Not Allowed is expected
					if((pIO->m_bRcvBuffer[6] != '0'	|| pIO->m_bRcvBuffer[7] != '6')
					&& pTag->m_pszSection != NULL
					&& (pTag->m_pszSection[0] != 'C'	|| pTag->m_pszSection[1] != 'D'))
						ReportError(pTag);
					pTag->m_wQuality=OPC_QUALITY_BAD;
				}
				else
					pTag->m_wQuality=OPC_QUALITY_GOOD;
			}
		}

		else if(!strcmp(pTag->m_pszCommand,"RB"))
		{
			int iFields;
			
			if ( SUCCEEDED(hr) )
			{
				CString strData((LPSTR) &pIO->m_bRcvBuffer[4]);

				if(pIO->m_bRcvBuffer[4] == 'N'
				&& pIO->m_bRcvBuffer[5] == 'O')
				{
					ReportError(pTag);
					pTag->m_wQuality=OPC_QUALITY_BAD;
				}
				else
				{
					TCHAR	szBatchNumber[3];
					pTag->m_Value.vt=VT_BSTR;
					iFields = swscanf(strData,_T("RB %2s"),szBatchNumber);
					if ( iFields == 1 )
					{
						VariantClear(&pTag->m_Value);
						pTag->m_Value=szBatchNumber;
						pTag->m_wQuality=OPC_QUALITY_GOOD;
					}
					else
					{
						pTag->m_wQuality=OPC_QUALITY_BAD;
					}
				}
			}
		}

		// Swing Arm Position
		else if(!strcmp(pTag->m_pszCommand,"SW"))
		{
			if(SUCCEEDED(hr))
			{
				if(pIO->m_bRcvBuffer[4] == 'N'
				&& pIO->m_bRcvBuffer[5] == 'O')
				{
					if(pIO->m_bRcvBuffer[6] == '1'
					&& pIO->m_bRcvBuffer[7] == '9')
					{
						if(pTag->m_wQuality != OPC_QUALITY_NOT_CONNECTED)
						{
							ReportError(pTag);
							pTag->m_wQuality=OPC_QUALITY_NOT_CONNECTED;
						}
					}
					else
					{
						ReportError(pTag);
						pTag->m_wQuality=OPC_QUALITY_BAD;
					}
				}
				else
				{
					CString strData((LPSTR) &pIO->m_bRcvBuffer[4]);

					int iFields;
					TCHAR	szPosition[10];
					iFields=swscanf(strData,_T("SW %9s"),szPosition);
					if(iFields == 1)
					{
						VariantClear(&pTag->m_Value);
						pTag->m_Value=szPosition;
						pTag->m_wQuality=OPC_QUALITY_GOOD;
					}
					else
						pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
				}
				CoFileTimeNow(&pTag->m_Timestamp);
			}
		}
	
		// Read Keypad
		else if(!strcmp(pTag->m_pszCommand,"RK"))
		{
			if(SUCCEEDED(hr))
			{
				if(pIO->m_bRcvBuffer[4] == 'N'
				&& pIO->m_bRcvBuffer[5] == 'O')
				{
					ReportError(pTag);

					if(pTag->m_wQuality != OPC_QUALITY_BAD)
					{
						pTag->m_wQuality=OPC_QUALITY_BAD;
						pTag->m_dwUpdateSequence++;
					}
				}
				else
				{
					CString strData((LPSTR) &pIO->m_bRcvBuffer[4]);

					int iFields;
					TCHAR	szData[26];
					iFields=swscanf(strData,_T("RK %25s"),szData);
					if(iFields == 1)
					{
						// Remove Last Character which is function key
						szData[lstrlen(szData)-1]=_T('\0');
						if(pTag->m_wQuality != OPC_QUALITY_GOOD)
						{
							VariantClear(&pTag->m_Value);
							pTag->m_Value=szData;
							pTag->m_wQuality=OPC_QUALITY_GOOD;
							pTag->m_dwUpdateSequence++;
						}
					}
					else
					{
						if(pTag->m_wQuality != OPC_QUALITY_COMM_FAILURE)
						{
							pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
							pTag->m_dwUpdateSequence++;
						}
					}
				}
				CoFileTimeNow(&pTag->m_Timestamp);
			}
		}

		// Read Prompt
		else if(!strcmp(pTag->m_pszCommand,"TI"))
		{
			if(SUCCEEDED(hr))
			{
				if(pIO->m_bRcvBuffer[4] == 'N'
				&& pIO->m_bRcvBuffer[5] == 'O')
				{
					ReportError(pTag);
					pTag->m_wQuality=OPC_QUALITY_BAD;
				}
				else
				{
					CString strData((LPSTR) &pIO->m_bRcvBuffer[4]);

					int iFields;
					int iPrompt;
					pTag->m_Value.vt=VT_I4;
					iFields=swscanf(strData,_T("TI %d %d"),&iPrompt,&pTag->m_Value.iVal);
					if(iFields == 2)
						pTag->m_wQuality=OPC_QUALITY_GOOD;
					else
						pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
				}
				CoFileTimeNow(&pTag->m_Timestamp);
			}
		}


		// Read Card Reader
		else if(!strcmp(pTag->m_pszCommand,"CD"))
		{
			if(SUCCEEDED(hr))
			{
				CTagList*	pLeaf=&pTag->m_pParent->m_Leaf;
				POSITION	pos=pLeaf->GetHeadPosition();
				CTag* pData=pLeaf->GetNext(pos);

				VariantClear(&pData->m_Value);
				if(pTag->m_pDevice->m_Type != SMITH_PROXIMITY
				&& pIO->m_bRcvBuffer[4] == 'N'
				&& pIO->m_bRcvBuffer[5] == 'O')
				{
					if(pIO->m_bRcvBuffer[6] == '1'
					&& pIO->m_bRcvBuffer[7] == '9')
					{
						if(pData->m_wQuality != OPC_QUALITY_NOT_CONNECTED)
						{
							ReportError(pTag);
							pData->m_wQuality=OPC_QUALITY_NOT_CONNECTED;
						}
					}
					else if(pIO->m_bRcvBuffer[6] == '3'
					&& pIO->m_bRcvBuffer[7] == '7')
					{
						pData->m_wQuality=OPC_QUALITY_OUT_OF_SERVICE;
					}
					else
					{
						ReportError(pTag);
						pData->m_wQuality=OPC_QUALITY_BAD;
					}
				}
				else
				{
					CString strData((pTag->m_pDevice->m_Type == SMITH_PROXIMITY) ? (LPSTR) &pIO->m_bRcvBuffer[3] : (LPSTR) &pIO->m_bRcvBuffer[4]);

					int iFields;
					int iStatus;
					TCHAR	szData[26];
					iFields=swscanf(strData,_T("CD %d %25s"),&iStatus,szData);
					if(iFields >= 1)
					{
						if(iStatus == 0
						&& iFields == 2)
						{
							pData->m_Value=szData;
							pData->m_wQuality=OPC_QUALITY_GOOD;
							// The Accuload Holds the data until reset
							// the Proximity Card does not so force a group update
							// to ensure clients see the new data
							if(pTag->m_pDevice->m_Type == SMITH_PROXIMITY)
								g_pDeviceManager->UpdateGroups();
						}
						else
						{
							pData->m_Value=_T("");
							pData->m_wQuality=OPC_QUALITY_GOOD;
						}
					}
					else
						pData->m_wQuality=OPC_QUALITY_COMM_FAILURE;
				}
				
				while(pos)
				{
					CTag* pControlTag=pLeaf->GetNext(pos);
					if(pData->m_wQuality == OPC_QUALITY_COMM_FAILURE
					|| pData->m_wQuality == OPC_QUALITY_NOT_CONNECTED)
						pControlTag->m_wQuality=pData->m_wQuality;
					else
						pControlTag->m_wQuality=OPC_QUALITY_GOOD;
				}
				CoFileTimeNow(&pTag->m_Timestamp);
			}
		}

		else if(!strcmp(pTag->m_pszCommand,"GK"))
		{
			if ( SUCCEEDED( hr ) )
			{
				if(pIO->m_bRcvBuffer[4] == 'N'
				&& pIO->m_bRcvBuffer[5] == 'O')
				{
					// Don't report NO32
					if ( ! ( pIO->m_bRcvBuffer[6] == '3' && pIO->m_bRcvBuffer[7] == '2' ) )
					{
						ReportError( pTag );
					}
					pTag->m_wQuality = OPC_QUALITY_BAD;
				}
				else
				{
					CString strData((LPSTR) &pIO->m_bRcvBuffer[4]);

					int iFields;
					TCHAR	szData[4];
					iFields=swscanf(strData,_T("GK%1s%1s%1s"),&szData[0],&szData[1],&szData[2]);
					if(iFields > 0)
					{
						pTag->m_Value=szData;
						pTag->m_wQuality=OPC_QUALITY_GOOD;
					}
					else
					{
						pTag->m_wQuality=OPC_QUALITY_BAD;
					}
				}
				CoFileTimeNow(&pTag->m_Timestamp);
			}
		}
		
		// Reset Preset Amount
		else if(!strcmp(pTag->m_pszCommand,"RP"))
		{
			if(SUCCEEDED(hr))
			{
				if(pIO->m_bRcvBuffer[4] == 'N'
				&& pIO->m_bRcvBuffer[5] == 'O')
				{
					ReportError(pTag);
					pTag->m_wQuality=OPC_QUALITY_BAD;
				}
				else
				{
					CString strData((LPSTR) &pIO->m_bRcvBuffer[4]);

					pTag->m_Value.vt=VT_R8;
					int iFields=swscanf(strData,_T("RP %lf"),&pTag->m_Value.dblVal);
					if(iFields == 1)
						pTag->m_wQuality=OPC_QUALITY_GOOD;
					else
						pTag->m_wQuality=OPC_QUALITY_BAD;
				}
				CoFileTimeNow(&pTag->m_Timestamp);
			}
		}

		// Dynamic Values
		else if(!strcmp(pTag->m_pszCommand,"DY"))
		{
			if(SUCCEEDED(hr))
			{
				if(pIO->m_bRcvBuffer[4] == 'N'
				&& pIO->m_bRcvBuffer[5] == 'O')
				{
					ReportError(pTag);
					pTag->m_wQuality=OPC_QUALITY_BAD;
				}
				else if(pIO->m_bRcvBuffer[4] == 'D'
				&& pIO->m_bRcvBuffer[5] == 'Y')
				{
					CString strData((LPSTR) &pIO->m_bRcvBuffer[4]);

					if(-1 != strData.Find(_T("(not available)")))
						pTag->m_wQuality=OPC_QUALITY_OUT_OF_SERVICE;
					else if ( -1 != strData.Find(_T("(not used)")))
						// Microload
						pTag->m_wQuality=OPC_QUALITY_OUT_OF_SERVICE;
					else if ( -1 != strData.Find(_T("(transaction not in progress)")))
						pTag->m_wQuality=OPC_QUALITY_OUT_OF_SERVICE;
					else
					{
						if(!strcmp(pTag->m_pszSection,"SY"))
						{ 
							int iFields;
							int iRequiredFields;
							int iArm;
							TCHAR	szUnits[15];

							if(pTag->m_pDevice->m_Type != MICROLOAD_NET)
							{

								switch(pTag->m_dwItem)
								{
									case 0:
									case 1:
									case 2:
									case 3:
									case 4:
									case 5:
									case 6:
									case 7:
									case 8:
									case 9:
									case 10:
									case 11:
										pTag->m_Value.vt=VT_R8;
										iFields=swscanf(strData,_T("DY Flow (Arm %d) %lf %14s"),&iArm,&pTag->m_Value.dblVal,szUnits);
										if(iFields == 3)
											pTag->m_wQuality=OPC_QUALITY_GOOD;

										break;

									case 12:
									case 13:
									case 14:
									case 15:
									case 16:
									case 17:
									{
										TCHAR	szRecipe[15];
										iFields=swscanf(strData,_T("DY Recipe (Arm %d) %14s"),&iArm,szRecipe);
										if(iFields == 2)
										{
											VariantClear(&pTag->m_Value);
											pTag->m_Value=szRecipe;
											pTag->m_wQuality=OPC_QUALITY_GOOD;
										}

										break;
									}

									case 18:
									case 19:
									case 20:
									case 21:
									case 22:
									case 23:
										pTag->m_Value.vt=VT_R8;
										iFields=swscanf(strData,_T("DY Preset (Arm %d) %lf %14s"),&iArm,&pTag->m_Value.dblVal,szUnits);
										if(iFields == 3)
											pTag->m_wQuality=OPC_QUALITY_GOOD;
										else
											pTag->m_wQuality=OPC_QUALITY_BAD;
										break;

									case 24:
									case 25:
									case 26:
									case 27:
									case 28:
									case 29:
										pTag->m_Value.vt=VT_R8;
										iFields=swscanf(strData,_T("DY Deliv. (Arm %d) %lf %14s"),&iArm,&pTag->m_Value.dblVal,szUnits);
										if(iFields == 3)
											pTag->m_wQuality=OPC_QUALITY_GOOD;
										else
											pTag->m_wQuality=OPC_QUALITY_BAD;
										break;

									case 30:
									case 31:
									case 32:
									case 33:
									case 34:
									case 35:
										pTag->m_Value.vt=VT_R8;
										iFields=swscanf(strData,_T("DY Remain. (Arm %d) %lf %14s"),&iArm,&pTag->m_Value.dblVal,szUnits);
										if(iFields == 3)
											pTag->m_wQuality=OPC_QUALITY_GOOD;
										else
											pTag->m_wQuality=OPC_QUALITY_BAD;
										break;

									default:
										pTag->m_wQuality=OPC_QUALITY_BAD;
										break;
								}
							}
	
							else
							{
								switch ( pTag->m_dwItem )
								{
									case 0:
									case 1:
										pTag->m_Value.vt = VT_R8;
										iFields = swscanf( strData, _T("DY Flow %lf %14s"), &pTag->m_Value.dblVal, szUnits );
										iRequiredFields = 2;
										break;

									case 2:
										TCHAR szRecipe[15];
										iFields = swscanf( strData, _T("DY Recipe %14s"), szRecipe );
										if ( iFields == 1 )
										{
											VariantClear( &pTag->m_Value );
											pTag->m_Value = szRecipe;
										}

										iRequiredFields = 1;
										break;

									case 3:
										pTag->m_Value.vt = VT_R8;
										iFields = swscanf( strData, _T("DY Preset %lf %14s"), &pTag->m_Value.dblVal, szUnits );
										iRequiredFields = 2;
										break;

									case 4:
										pTag->m_Value.vt = VT_R8;
										iFields = swscanf( strData, _T("DY Delivered %lf %14s"), &pTag->m_Value.dblVal, szUnits );
										iRequiredFields = 2;
										break;

									case 5:
										pTag->m_Value.vt = VT_R8;
										iFields = swscanf( strData, _T("DY Remaining %lf %14s"), &pTag->m_Value.dblVal, szUnits );
										iRequiredFields = 2;
										break;

									case 6:
										pTag->m_Value.vt = VT_R8;
										iFields = swscanf( strData, _T("DY Cur Mtr Factor %lf"), &pTag->m_Value.dblVal );
										iRequiredFields = 1;
										break;

									case 7:
										pTag->m_Value.vt = VT_R8;
										iFields = swscanf( strData, _T("DY Temperature %lf %s"), &pTag->m_Value.dblVal, szUnits );
										iRequiredFields = 2;
										break;

									case 8:
										pTag->m_Value.vt = VT_R8;
										iFields = swscanf( strData, _T("DY Density %lf %14s"), &pTag->m_Value.dblVal, szUnits );
										iRequiredFields = 2;
										break;

									case 9:
										pTag->m_Value.vt = VT_R8;
										iFields = swscanf( strData, _T("DY Pressure %lf %14s"), &pTag->m_Value.dblVal, szUnits );
										iRequiredFields = 2;
										break;

									case 10:
										pTag->m_Value.vt = VT_R8;
										iFields = swscanf( strData, _T("DY Vapr Pressure %lf %14s"), &pTag->m_Value.dblVal, szUnits );
										iRequiredFields = 2;
										break;

									case 11:
										TCHAR szPosition[15];
										pTag->m_Value.vt = VT_R8;
										iFields = swscanf( strData, _T("DY Valve Req. %14s"), szPosition );
										if ( iFields == 1 )
										{
											VariantClear( &pTag->m_Value );
											pTag->m_Value = szPosition;
										}
										iRequiredFields = 1;
										break;

									case 12:
									{
										CString Combined;
										TCHAR szDate[15], szTime[15];
     									pTag->m_Value.vt = VT_R8;
										iFields = swscanf( strData, _T("DY Pwr Fail %8s %8s %2s"), szDate, szTime, szUnits );
										Combined = CString(szDate) + " " + CString(szTime) + " " + CString(szUnits);
										if ( iFields == 3 )
										{
											VariantClear( &pTag->m_Value );
											pTag->m_Value = Combined;
										}
										iRequiredFields = 3;
										break;
									}

									default:
										return OPC_QUALITY_BAD;

								}

								if ( iFields == iRequiredFields )
									pTag->m_wQuality=OPC_QUALITY_GOOD;
							}
						}

						else if(!strcmp(pTag->m_pszSection,"P1")
						|| !strcmp(pTag->m_pszSection,"P2")
						|| !strcmp(pTag->m_pszSection,"P3")
						|| !strcmp(pTag->m_pszSection,"P4")
						|| !strcmp(pTag->m_pszSection,"P5")
						|| !strcmp(pTag->m_pszSection,"P6"))
						{
							int iFields;
							int iRequiredFields;
						
							TCHAR	szUnits[15];

							pTag->m_Value.vt=VT_R8;
							switch(pTag->m_dwItem)
							{
								case 0:
								case 1:
									iFields=swscanf(strData,_T("DY Flow %lf %14s"),&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=2;
									break;
		
								case 2:
									iFields=swscanf(strData,_T("DY Batch Avg Temp %lf %14s"),&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=2;
									break;

								case 3:
									iFields=swscanf(strData,_T("DY Batch Avg Dens %lf %14s"),&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=2;
									break;

								case 4:
									iFields=swscanf(strData,_T("DY Avg API %lf"),&pTag->m_Value.dblVal);
									iRequiredFields=1;
									break;

								case 5:
									iFields=swscanf(strData,_T("DY Avg Ref Dens %lf"),&pTag->m_Value.dblVal);
									iRequiredFields=1;
									break;

								case 6:
									iFields=swscanf(strData,_T("DY Avg Rel Dens %lf"),&pTag->m_Value.dblVal);
									iRequiredFields=1;
									break;

								case 7:
									iFields=swscanf(strData,_T("DY Batch Avg Press %lf %14s"),&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=2;
									break;

								case 8:
									iFields=swscanf(strData,_T("DY Avg Vapor Press %lf %14s"),&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=2;
									break;

								case 9:
									iFields=swscanf(strData,_T("DY Batch Avg Mtr Factor %lf"),&pTag->m_Value.dblVal);
									iRequiredFields=1;
									break;

								case 10:
									iFields=swscanf(strData,_T("DY Batch Avg CTL %lf"),&pTag->m_Value.dblVal);
									iRequiredFields=1;
									break;

								case 11:
									iFields=swscanf(strData,_T("DY Batch Avg CPL %lf"),&pTag->m_Value.dblVal);
									iRequiredFields=1;
									break;

								case 12:
									iFields=swscanf(strData,_T("DY Temperature %lf %14s"),&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=2;
									break;

								case 13:
									iFields=swscanf(strData,_T("DY Density %lf %14s"),&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=2;
									break;

								case 14:
									iFields=swscanf(strData,_T("DY Cur Meter Factor %lf"),&pTag->m_Value.dblVal);
									iRequiredFields=1;
									break;

								case 15:
								{
									TCHAR	szPosition[15];
									iFields=swscanf(strData,_T("DY Valve Requested %14s"),szPosition);
									if(iFields == 1)
									{
										VariantClear(&pTag->m_Value);
										pTag->m_Value=szPosition;
									}
									iRequiredFields=1;
									break;
								}

								case 16:
									iFields=swscanf(strData,_T("DY Current Percentage %lf %14s"),&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=2;
									break;

								case 17:
									iFields=swscanf(strData,_T("DY Desired Percentage %lf %14s"),&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=2;
									break;

								case 18:
									iFields=swscanf(strData,_T("DY IV Batch %lf %14s"),&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=2;
									break;

								case 19:
									iFields=swscanf(strData,_T("DY GV Batch %lf %14s"),&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=2;
									break;

								case 20:
									iFields=swscanf(strData,_T("DY GST Batch %lf %14s"),&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=2;
									break;

								case 21:
									iFields=swscanf(strData,_T("DY GSV Batch %lf %14s"),&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=2;
									break;

								case 22:
									iFields=swscanf(strData,_T("DY Mass Batch %lf %14s"),&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=2;
									break;

								case 23:
									iFields=swscanf(strData,_T("DY IV Trans %lf %14s"),&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=2;
									break;

								case 24:
									iFields=swscanf(strData,_T("DY GV Trans %lf %14s"),&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=2;
									break;

								case 25:
									iFields=swscanf(strData,_T("DY GST Trans %lf %14s"),&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=2;
									break;

								case 26:
									iFields=swscanf(strData,_T("DY GSV Trans %lf %14s"),&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=2;
									break;

								case 27:
									iFields=swscanf(strData,_T("DY Mass Trans %lf %14s"),&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=2;
									break;

								case 28:
									iFields=swscanf(strData,_T("DY Cur Ref Dens %lf %14s"),&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=2;
									break;

								case 29:
									iFields=swscanf(strData,_T("DY Batch Avg CTPL %lf"),&pTag->m_Value.dblVal);
									iRequiredFields=1;
									break;


								default:
									iFields=0;
									iRequiredFields=2;
									break;
							}

							if(iFields == iRequiredFields)
								pTag->m_wQuality=OPC_QUALITY_GOOD;
							else
								pTag->m_wQuality=OPC_QUALITY_BAD;

						}

						else if(!strcmp(pTag->m_pszSection,"TR"))
						{
							int iFields;
							int iRequiredFields;
							TCHAR	szUnits[15];
							CString oName;

							pTag->m_Value.vt=VT_R8;
							switch(pTag->m_dwItem)
							{
								case 0:
								{
									TCHAR szReport[15];
									pTag->m_Value.vt=VT_I4;
									iFields=swscanf(strData,_T("DY Recipe %d = %14s"),&pTag->m_Value.iVal,szReport);
									iRequiredFields=2;
									break;
								}
								case 1:
									iFields=swscanf(strData,_T("DY IV Trans %lf %14s"),&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=2;
									break;

								case 2:
									iFields=swscanf(strData,_T("DY GV Trans %lf %14s"),&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=2;
									break;

								case 3:
									iFields=swscanf(strData,_T("DY GST Trans %lf %14s"),&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=2;
									break;

								case 4:
									iFields=swscanf(strData,_T("DY GSV Trans %lf %14s"),&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=2;
									break;

								case 5:
									iFields=swscanf(strData,_T("DY Mass Trans %lf %14s"),&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=2;
									break;

								case 6:
									if(pTag->m_oName == oName.LoadString(IDS_AVERAGE_METER_FACTOR))
									{
										iFields=swscanf(strData,_T("DY Trans Avg Mtr Factor %lf"),&pTag->m_Value.dblVal);
										iRequiredFields=1;
									}
									else
									{
										iFields=swscanf(strData,_T("DY Trans Avg Temp %lf %14s"),&pTag->m_Value.dblVal,szUnits);
										iRequiredFields=2;
									}
									break;

								case 7:
									oName.LoadString(IDS_AVERAGE_TEMPERATURE);
                           if(0 == pTag->m_oName.Compare(oName))
										iFields=swscanf(strData,_T("DY Trans Avg Temp %lf %14s"),&pTag->m_Value.dblVal,szUnits);
									else
										iFields=swscanf(strData,_T("DY Trans Avg Dens %lf %14s"),&pTag->m_Value.dblVal,szUnits);

									iRequiredFields=2;
									break;

								case 8:
									if(pTag->m_oName == oName.LoadString(IDS_AVERAGE_DENSITY))
										iFields=swscanf(strData,_T("DY Trans Avg Dens %lf %14s"),&pTag->m_Value.dblVal,szUnits);
									else
										iFields=swscanf(strData,_T("DY Trans Avg Press %lf %14s"),&pTag->m_Value.dblVal,szUnits);

									iRequiredFields=2;
									break;

								case 9:
									oName.LoadString(IDS_AVERAGE_PRESSURE);
									if(0 == pTag->m_oName.Compare(oName))
									{
										iFields=swscanf(strData,_T("DY Trans Avg Press %lf %14s"),&pTag->m_Value.dblVal,szUnits);
										iRequiredFields=2 ;
									}
									else
									{
										iFields=swscanf(strData,_T("DY Trans Avg Meter Factor %lf"),&pTag->m_Value.dblVal);
										iRequiredFields=1;
									}

									break;

								case 10:
									iFields=swscanf(strData,_T("DY Trans Avg CTL %lf"),&pTag->m_Value.dblVal);
									iRequiredFields=1;
									break;

								case 11:
									iFields=swscanf(strData,_T("DY Trans Avg CPL %lf"),&pTag->m_Value.dblVal);
									iRequiredFields=1;
									break;

								case 12:
								case 13:
								case 14:
								case 15:
								case 16:
								case 17:
								case 18:
								case 19:
								case 20:
								case 21:
								case 22:
								case 23:
								case 24:
								case 25:
								case 26:
								case 27:
								case 28:
								case 29:
								case 30:
								case 31:
								case 32:
								case 33:
								case 34:
								case 35:
								{
									int iAdditive;
									iFields=swscanf(strData,_T("DY Add %d Trans %lf"),&iAdditive,&pTag->m_Value.dblVal);
									iRequiredFields=2;
									break;
								}

								default:
									iFields=0;
									iRequiredFields=1;
									break;
							}	

							if(iFields == iRequiredFields)
								pTag->m_wQuality=OPC_QUALITY_GOOD;
							else
								pTag->m_wQuality=OPC_QUALITY_BAD;
						}

						else if(!strcmp(pTag->m_pszSection,"CB")
						|| !strcmp(pTag->m_pszSection,"B1")
						|| !strcmp(pTag->m_pszSection,"B2")
						|| !strcmp(pTag->m_pszSection,"B3")
						|| !strcmp(pTag->m_pszSection,"B4")
						|| !strcmp(pTag->m_pszSection,"B5")
						|| !strcmp(pTag->m_pszSection,"B6")
						|| !strcmp(pTag->m_pszSection,"B7")
						|| !strcmp(pTag->m_pszSection,"B8")
						|| !strcmp(pTag->m_pszSection,"B9")
						|| !strcmp(pTag->m_pszSection,"BA"))
						{
							int iFields;
							int iRequiredFields;
							TCHAR	szUnits[15];
							CString oName;

							pTag->m_Value.vt=VT_R8;
							switch(pTag->m_dwItem)
							{
								case 0:
								{
									TCHAR szReport[15];
									pTag->m_Value.vt=VT_I4;
									iFields=swscanf(strData,_T("DY Recipe %d - %14s"),&pTag->m_Value.iVal,szReport);
									iRequiredFields=2;
									break;
								}
								case 1:
									iFields=swscanf(strData,_T("DY IV Batch %lf %14s"),&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=2;
									break;

								case 2:
									iFields=swscanf(strData,_T("DY GV Batch %lf %14s"),&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=2;
									break;

								case 3:
									iFields=swscanf(strData,_T("DY GST Batch %lf %14s"),&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=2;
									break;

								case 4:
									iFields=swscanf(strData,_T("DY GSV Batch %lf %14s"),&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=2;
									break;

								case 5:
									iFields=swscanf(strData,_T("DY Mass Batch %lf %14s"),&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=2;
									break;

								case 6:
									oName.LoadString(IDS_AVERAGE_METER_FACTOR);
									if(0 == pTag->m_oName.Compare(oName))
									{
										iFields=swscanf(strData,_T("DY Batch Avg Mtr Factor %lf"),&pTag->m_Value.dblVal);
										iRequiredFields=1;
									}
									else
									{
										iFields=swscanf(strData,_T("DY Batch Avg Temp %lf %14s"),&pTag->m_Value.dblVal,szUnits);
										iRequiredFields=2;
									}

									break;

								case 7:
									oName.LoadString(IDS_AVERAGE_TEMPERATURE);
									if(0 == pTag->m_oName.Compare(oName))
										iFields=swscanf(strData,_T("DY Batch Avg Temp %lf %14s"),&pTag->m_Value.dblVal,szUnits);
									else
										iFields=swscanf(strData,_T("DY Batch Avg Dens %lf %14s"),&pTag->m_Value.dblVal,szUnits);
									
									iRequiredFields=2;
									break;

								case 8:
									iFields=swscanf(strData,_T("DY Batch Avg Press %lf %14s"),&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=2;
									break;

								case 9:
									iFields=swscanf(strData,_T("DY Batch Avg Meter Factor %lf"),&pTag->m_Value.dblVal);
									iRequiredFields=1;
									break;

								case 10:
									oName.LoadString(IDS_AVERAGE_REFERENCE_DENSITY);
									if(0 == pTag->m_oName.Compare(oName))
										iFields=swscanf(strData,_T("DY Avg Ref Dens %lf"),&pTag->m_Value.dblVal);
									else
										iFields=swscanf(strData,_T("DY Batch Avg CTL %lf"),&pTag->m_Value.dblVal);

									iRequiredFields=1;
									break;

								case 11:
									iFields=swscanf(strData,_T("DY Batch Avg CPL %lf"),&pTag->m_Value.dblVal);
									iRequiredFields=1;
									break;

								case 12:
									oName.LoadString(IDS_AVERAGE_PRESSURE);
									if(0 == pTag->m_oName.Compare(oName))
									{
										iFields=swscanf(strData,_T("DY Batch Avg Press %lf %14s"),&pTag->m_Value.dblVal,szUnits);
										iRequiredFields=2;
										break;
									}
									goto CheckAdditives;

								case 13:
									goto CheckAdditives;

								case 14:
									oName.LoadString(IDS_AVERAGE_CTL);
									if(0 == pTag->m_oName.Compare(oName))
									{
										iFields=swscanf(strData,_T("DY Batch Avg CTL %lf"),&pTag->m_Value.dblVal);
										iRequiredFields=1;
										break;
									}
									goto CheckAdditives;

								case 15:
									oName.LoadString(IDS_AVERAGE_CPL);
									if(0 == pTag->m_oName.Compare(oName))
									{
										iFields=swscanf(strData,_T("DY Batch Avg CPL %lf"),&pTag->m_Value.dblVal);
										iRequiredFields=1;
										break;
									}
									goto CheckAdditives;

								case 16:
								case 17:
								case 18:
								case 19:
								case 20:
								case 21:
								case 22:
								case 23:
								case 24:
								case 25:
								case 26:
								case 27:
								case 28:
								case 29:
								case 30:
								case 31:
								case 32:
								case 33:
								case 34:
								case 35:
CheckAdditives:
								{
									int iAdditive;
									iFields=swscanf(strData,_T("DY Add %d Batch %lf"),&iAdditive,&pTag->m_Value.dblVal);
									iRequiredFields=2;
									break;
								}

								default:
									iFields=0;
									iRequiredFields=1;
									break;
							}	

							if(iFields == iRequiredFields)
								pTag->m_wQuality=OPC_QUALITY_GOOD;
							else
								pTag->m_wQuality=OPC_QUALITY_BAD;
						}

						else if(!strcmp(pTag->m_pszSection,"FA"))
						{
							int iFields;
							int iRequiredFields;
						
							TCHAR	szUnits[15];
							TCHAR szAdditiveNumber[2];

							pTag->m_Value.vt=VT_R8;
							switch(pTag->m_dwItem)
							{
								case 0:
								case 12:
								case 24:
								case 36:
									iFields=swscanf(strData,_T("DY A%1s IV Batch %lf %14s"),szAdditiveNumber,&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=3;
									break;

								case 1:
								case 13:
								case 25:
								case 37:
									iFields=swscanf(strData,_T("DY A%1s GV Batch %lf %14s"),szAdditiveNumber,&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=3;
									break;

								case 2:
								case 14:
								case 26:
								case 38:
									iFields=swscanf(strData,_T("DY A%1s GST Batch %lf %14s"),szAdditiveNumber,&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=3;
									break;

								case 3:
								case 15:
								case 27:
								case 39:
									iFields=swscanf(strData,_T("DY A%1s Mass Batch %lf %14s"),szAdditiveNumber,&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=3;
									break;

								case 4:
								case 16:
								case 28:
								case 40:
									iFields=swscanf(strData,_T("DY A%1s Current Temp %lf %14s"),szAdditiveNumber,&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=3;
									break;

								case 5:
								case 17:
								case 29:
								case 41:
									iFields=swscanf(strData,_T("DY A%1s Batch Avg Temp %lf %14s"),szAdditiveNumber,&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=3;
									break;

								case 6:
								case 18:
								case 30:
								case 42:
									iFields=swscanf(strData,_T("DY A%1s Batch Avg Dens %lf %14s"),szAdditiveNumber,&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=3;
									break;

								case 7:
								case 19:
								case 31:
								case 43:
									iFields=swscanf(strData,_T("DY A%1s Batch Avg CTL %lf"),szAdditiveNumber,&pTag->m_Value.dblVal);
									iRequiredFields=2;
									break;

								case 8:
								case 20:
								case 32:
								case 44:
									iFields=swscanf(strData,_T("DY A%1s IV Trans %lf %14s"),szAdditiveNumber,&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=3;
									break;

								case 9:
								case 21:
								case 33:
								case 45:
									iFields=swscanf(strData,_T("DY A%1s GV Trans %lf %14s"),szAdditiveNumber,&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=3;
									break;

								case 10:
								case 22:
								case 34:
								case 46:
									iFields=swscanf(strData,_T("DY A%1s GST Trans %lf %14s"),szAdditiveNumber,&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=3;
									break;

								case 11:
								case 23:
								case 35:
								case 47:
									iFields=swscanf(strData,_T("DY A%1s Mass Trans %lf %14s"),szAdditiveNumber,&pTag->m_Value.dblVal,szUnits);
									iRequiredFields=3;
									break;

								default:
									iFields=0;
									break;
							}

							if(iFields == iRequiredFields)
								pTag->m_wQuality=OPC_QUALITY_GOOD;
							else
								pTag->m_wQuality=OPC_QUALITY_BAD;

						}
					}
				}
				else		
					pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;

				CoFileTimeNow(&pTag->m_Timestamp);
			}
		}

		// End Transaction
		// End Batch
		// Stop Arm
		// Stop All Arms
		// Start Arm
		// Write Second Line
		// Write Third Line
		// Write Fourth Line
		// Write First Line With Prompt
		// Write First Line With Prompt On Set
		// Write First Line With Prompt On Set No Echo
		// Write First Line With Prompt No Echo
		// Clear Transactions
		// Set Date And Time
		// Log Out of Program Mode
		// Authorize And Set Batch Amount
		// Force Full Screen View
		else if(!strcmp(pTag->m_pszCommand,"ET")
		|| !strcmp(pTag->m_pszCommand,"EB")
		|| !strcmp(pTag->m_pszCommand,"ST")
		|| !strcmp(pTag->m_pszCommand,"SP")
		|| !strcmp(pTag->m_pszCommand,"ST")
		|| !strcmp(pTag->m_pszCommand,"AP")
		|| !strcmp(pTag->m_pszCommand,"AB")
		|| !strcmp(pTag->m_pszCommand,"WA")
		|| !strcmp(pTag->m_pszCommand,"WB")
		|| !strcmp(pTag->m_pszCommand,"WC")
		|| !strcmp(pTag->m_pszCommand,"WD")
		|| !strcmp(pTag->m_pszCommand,"WE")
		|| !strcmp(pTag->m_pszCommand,"WF")
		|| !strcmp(pTag->m_pszCommand,"WG")
		|| !strcmp(pTag->m_pszCommand,"WP")
		|| !strcmp(pTag->m_pszCommand,"WQ")
		|| !strcmp(pTag->m_pszCommand,"WX")
		|| !strcmp(pTag->m_pszCommand,"CT")
		|| !strcmp(pTag->m_pszCommand,"SD")
		|| !strcmp(pTag->m_pszCommand,"LO")
		|| !strcmp(pTag->m_pszCommand,"SB")
		|| !strcmp(pTag->m_pszCommand,"FS")
		|| !strcmp(pTag->m_pszCommand,"AU")
		|| !strcmp(pTag->m_pszCommand,"SF")
		)
		{
			if(SUCCEEDED(hr))
			{
				if(pIO->m_bRcvBuffer[4] == 'N'
				&& pIO->m_bRcvBuffer[5] == 'O')
				{
					// NO06 is acceptable for ET - End Transaction
					// it is the only way to remove Authorization
					// but an NO06 will occur if transaction has not
					// begun.
					if(!strcmp(pTag->m_pszCommand,"ET")
					&& pIO->m_bRcvBuffer[6] == '0'
					&& pIO->m_bRcvBuffer[7] == '6')
					{
						pTag->m_wQuality=OPC_QUALITY_GOOD;
						return S_OK;
					}

					// NO94 is acceptable for an LO - Log Out of Program Mode
					// the Accumate does this automatically after a brief time delay
					if(!strcmp(pTag->m_pszCommand,"LO")
					&& pIO->m_bRcvBuffer[6] == '9'
					&& pIO->m_bRcvBuffer[7] == '4')
					{
						pTag->m_wQuality=OPC_QUALITY_GOOD;
						return S_OK;
					}


					ReportError(pTag);
					pTag->m_wQuality=OPC_QUALITY_DEVICE_FAILURE;
					return E_FAIL;
				}
				else
				{
					pTag->m_wQuality=OPC_QUALITY_GOOD;

					if(!strcmp(pTag->m_pszCommand,"WA")
					|| !strcmp(pTag->m_pszCommand,"WB")
					|| !strcmp(pTag->m_pszCommand,"WC")
					|| !strcmp(pTag->m_pszCommand,"WD")
					|| !strcmp(pTag->m_pszCommand,"WE")
					|| !strcmp(pTag->m_pszCommand,"WF")
					|| !strcmp(pTag->m_pszCommand,"WG")
					|| !strcmp(pTag->m_pszCommand,"WP")
					|| !strcmp(pTag->m_pszCommand,"WQ")
					|| !strcmp(pTag->m_pszCommand,"WX"))
					{
						CTag* pKeypadDataTag=g_pDeviceManager->FindTag((pTag->m_pParent->GetPathName()+_T("Keypad Data")).GetBuffer());
						if(pKeypadDataTag != NULL
						&& pKeypadDataTag->m_wQuality == OPC_QUALITY_GOOD)
						{
							pKeypadDataTag->m_wQuality=OPC_QUALITY_BAD;
							pKeypadDataTag->m_dwUpdateSequence++;
							CoFileTimeNow(&pKeypadDataTag->m_Timestamp);
						}

						CTag* pKeypadDataPending=g_pDeviceManager->FindTag((pTag->m_pParent->GetPathName()+_T("Status.Keypad Data Pending")).GetBuffer());
						if(pKeypadDataPending != NULL
						&& pKeypadDataPending->m_wQuality == OPC_QUALITY_GOOD
						&& pKeypadDataPending->m_Value.boolVal == VARIANT_TRUE)
						{
							pKeypadDataPending->m_Value.vt=VT_BOOL;
							pKeypadDataPending->m_Value.boolVal=VARIANT_FALSE;
							pKeypadDataPending->m_dwUpdateSequence++;
							CoFileTimeNow(&pKeypadDataPending->m_Timestamp);
						}

						CTag* pDisplayMessageTimeout=g_pDeviceManager->FindTag((pTag->m_pParent->GetPathName()+_T("Status.Display Message Time-out")).GetBuffer());
						if(pDisplayMessageTimeout != NULL
						&& pDisplayMessageTimeout->m_wQuality == OPC_QUALITY_GOOD
						&& pDisplayMessageTimeout->m_Value.boolVal == VARIANT_TRUE)
						{
							pDisplayMessageTimeout->m_Value.vt=VT_BOOL;
							pDisplayMessageTimeout->m_Value.boolVal=VARIANT_FALSE;
							pDisplayMessageTimeout->m_dwUpdateSequence++;
							CoFileTimeNow(&pDisplayMessageTimeout->m_Timestamp);
						}

					}

					if(!strcmp(pTag->m_pszCommand,"AU")
					|| !strcmp(pTag->m_pszCommand,"AP")
					|| !strcmp(pTag->m_pszCommand,"SB")
					|| !strcmp(pTag->m_pszCommand,"SF"))
					{
						CTag* pAuthorized=g_pDeviceManager->FindTag((pTag->m_pParent->GetPathName()+_T("Status.Authorized")).GetBuffer());
						if(pAuthorized != NULL
						&& pAuthorized->m_Value.boolVal != VARIANT_TRUE)
						{
							pAuthorized->m_wQuality=OPC_QUALITY_GOOD;
							pAuthorized->m_Value.vt=VT_BOOL;
							pAuthorized->m_Value.boolVal=VARIANT_TRUE;
							pAuthorized->m_dwUpdateSequence++;
							CoFileTimeNow(&pAuthorized->m_Timestamp);
						}

						CTag* pReleased=g_pDeviceManager->FindTag((pTag->m_pParent->GetPathName()+_T("Status.Released")).GetBuffer());
						if(pReleased != NULL
						&& pReleased->m_Value.boolVal != VARIANT_FALSE)
						{
							pReleased->m_wQuality=OPC_QUALITY_GOOD;
							pReleased->m_Value.vt=VT_BOOL;
							pReleased->m_Value.boolVal=VARIANT_FALSE;
							pReleased->m_dwUpdateSequence++;
							CoFileTimeNow(&pReleased->m_Timestamp);
						}

						CTag* pBatchDone=g_pDeviceManager->FindTag((pTag->m_pParent->GetPathName()+_T("Status.Batch Done")).GetBuffer());
						if(pBatchDone != NULL
						&& pBatchDone->m_Value.boolVal != VARIANT_FALSE)
						{
							pBatchDone->m_wQuality=OPC_QUALITY_GOOD;
							pBatchDone->m_Value.vt=VT_BOOL;
							pBatchDone->m_Value.boolVal=VARIANT_FALSE;
							pBatchDone->m_dwUpdateSequence++;
							CoFileTimeNow(&pBatchDone->m_Timestamp);
						}
					}

					if(!strcmp(pTag->m_pszCommand,"AB"))
					{
						CTag* pRecipe=g_pDeviceManager->FindTag((pTag->m_pParent->GetPathName()+_T("Recipe")).GetBuffer());
						if(pRecipe != NULL
						&& pRecipe->m_Value.bVal != 0)
						{
							pRecipe->m_wQuality=OPC_QUALITY_GOOD;
							pRecipe->m_Value.vt=VT_UI2;
							pRecipe->m_Value.uiVal=0;
							pRecipe->m_dwUpdateSequence++;
							CoFileTimeNow(&pRecipe->m_Timestamp);
						}
					}

					return S_OK;
				}
			}
			else
				return hr;
		}

		// Request Recipe
		else if(!strcmp(pTag->m_pszCommand,"RR"))
		{
			if(SUCCEEDED(hr))
			{
				if(pIO->m_bRcvBuffer[4] == 'N'
				&& pIO->m_bRcvBuffer[5] == 'O')
				{
					// Ignore NO05, this is a normal response
					// when No Transaction Ever done

					// Ignore NO06, this is a normal response when
					// no presetting has been performed on arm

					// Ignore NO39, this is a normal response when
					// no current batch on arm
					if(!((pIO->m_bRcvBuffer[6] == '0'
					&& pIO->m_bRcvBuffer[7] == '5')
					|| (pIO->m_bRcvBuffer[6] == '0'
					&& pIO->m_bRcvBuffer[7] == '6')
					|| (pIO->m_bRcvBuffer[6] == '3'
					&& pIO->m_bRcvBuffer[7] == '9')))
						ReportError(pTag);

					pTag->m_wQuality=OPC_QUALITY_BAD;
				}

				else if(pIO->m_bRcvBuffer[4] == 'R'
				&& pIO->m_bRcvBuffer[5] == 'R')
				{
					int iFields;
					int iRequiredFields;
					unsigned short uiPreviousRecipe;

					CString strData((LPSTR) &pIO->m_bRcvBuffer[4]);
					
					pTag->m_Value.vt=VT_UI2;

					uiPreviousRecipe = pTag->m_Value.uiVal;
					iFields=swscanf(strData,_T("RR %2hd"),&pTag->m_Value.uiVal);
					if (uiPreviousRecipe != pTag->m_Value.uiVal)
					{
						pTag->m_dwUpdateSequence++;
					}

					iRequiredFields=1;

					if(iFields == iRequiredFields)
						pTag->m_wQuality=OPC_QUALITY_GOOD;
					else
						pTag->m_wQuality=OPC_QUALITY_BAD;

				}
				else		
					pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;

				CoFileTimeNow(&pTag->m_Timestamp);

			}
		}

		// Analog Inputs
		else if(!strcmp(pTag->m_pszCommand,"RD"))
		{
			if(SUCCEEDED(hr))
			{
				if(pIO->m_bRcvBuffer[4] == 'N'
				&& pIO->m_bRcvBuffer[5] == 'O')
				{
					ReportError(pTag);
					pTag->m_wQuality=OPC_QUALITY_BAD;
				}

				else if(pIO->m_bRcvBuffer[4] == 'R'
				&& pIO->m_bRcvBuffer[5] == 'D')
				{
					int iFields;
					int iRequiredFields;

					CString strData((LPSTR) &pIO->m_bRcvBuffer[4]);

					pTag->m_Value.vt=VT_R8;
					TCHAR szType[2];

					iFields=swscanf(strData,_T("RD %1s %lf"),szType,&pTag->m_Value.dblVal);
					iRequiredFields=2;

					if(iFields == iRequiredFields)
						pTag->m_wQuality=OPC_QUALITY_GOOD;
					else
						pTag->m_wQuality=OPC_QUALITY_BAD;

				}
				else		
					pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;

				CoFileTimeNow(&pTag->m_Timestamp);
			}
		}

		// Non-resettable Totals
		else if(!strcmp(pTag->m_pszCommand,"VT"))
		{
			if(SUCCEEDED(hr))
			{
				if(pIO->m_bRcvBuffer[4] == 'N'
				&& pIO->m_bRcvBuffer[5] == 'O')
				{
					ReportError(pTag);
					pTag->m_wQuality=OPC_QUALITY_BAD;
				}

				else if(pIO->m_bRcvBuffer[4] == 'V'
				&& pIO->m_bRcvBuffer[5] == 'T')
				{
					int iFields;
					int iRequiredFields;

					CString strData((LPSTR) &pIO->m_bRcvBuffer[4]);

					pTag->m_Value.vt=VT_R8;
					if(pIO->m_bRcvBuffer[7] == 'A')
					{
						int iAdditive;
						iFields=swscanf(strData,_T("VT A%d %lf"),&iAdditive,&pTag->m_Value.dblVal);
						iRequiredFields=2;
					}
					else
					{
						TCHAR szType[2];
						int iProduct;
	
						if(pTag->m_pDevice->m_Type != MICROLOAD_NET)
						{
							iFields=swscanf(strData,_T("VT %1s P%d %lf"),szType,&iProduct,&pTag->m_Value.dblVal);
							iRequiredFields = 3;
						}
						else
						{
							iFields = swscanf(strData, _T("VT %1s %lf"), szType, &pTag->m_Value.dblVal);
							iRequiredFields = 2;
						}

					}

					if (iFields == iRequiredFields)
						pTag->m_wQuality = OPC_QUALITY_GOOD;
					else
						pTag->m_wQuality = OPC_QUALITY_BAD;

				}
				else
					pTag->m_wQuality = OPC_QUALITY_COMM_FAILURE;

					CoFileTimeNow(&pTag->m_Timestamp);
			}
		}

		// Program Code Values
		// Program Code Change
		else if (!strcmp(pTag->m_pszCommand, "PC")
		|| !strcmp(pTag->m_pszCommand, "PV"))
		{
			if (SUCCEEDED(hr))
			{
				if (pIO->m_bRcvBuffer[4] == 'N'
					&& pIO->m_bRcvBuffer[5] == 'O')
				{
					ReportError(pTag);
					pTag->m_wQuality = OPC_QUALITY_BAD;
					hr = E_FAIL;
				}

				else if (pIO->m_bRcvBuffer[4] == 'P'
					&& (pIO->m_bRcvBuffer[5] == 'V'
						|| pIO->m_bRcvBuffer[5] == 'C'))
				{
					int iFields;
					TCHAR szSection[255];
					TCHAR szItem[255];
					TCHAR szValue[255];
					int iPos;

					pIO->m_bRcvBuffer[254] = 0;

					CString strData((LPSTR)&pIO->m_bRcvBuffer[7]);

					iFields = swscanf(strData, _T("%254s %254s %n"), szSection, szItem, &iPos);
					if (strData.GetLength() > iPos)
					{
						_tcsncpy(szValue, &(((LPCTSTR)strData)[iPos]), sizeof(szValue) / sizeof(TCHAR));
						if (pTag->m_NativeType == VT_BSTR)
						{
							VariantClear(&pTag->m_Value);
							pTag->m_Value = szValue;
							pTag->m_wQuality = OPC_QUALITY_GOOD;
						}

						else if (pTag->m_NativeType == VT_R8)
						{
							iFields = swscanf(szValue, _T("%lf"), &pTag->m_Value.dblVal);
							if (iFields == 1)
							{
								pTag->m_wQuality = OPC_QUALITY_GOOD;
								pTag->m_Value.vt = pTag->m_NativeType;
							}
							else
								pTag->m_wQuality = OPC_QUALITY_BAD;

						}
						else
						{
							iFields = swscanf(szValue, _T("%d"), &pTag->m_Value.iVal);
							if (iFields == 1)
							{
								pTag->m_wQuality = OPC_QUALITY_GOOD;
								pTag->m_Value.vt = pTag->m_NativeType;
							}
							else
								pTag->m_wQuality = OPC_QUALITY_BAD;

						}
					}
					else
						pTag->m_wQuality = OPC_QUALITY_BAD;
				}
				else
					pTag->m_wQuality = OPC_QUALITY_COMM_FAILURE;
			}
		}

		// Other Arm Addresses (OA)
		else if (!strcmp(pTag->m_pszCommand, "OA"))
		{
			if (SUCCEEDED(hr))
			{
				if (pIO->m_bRcvBuffer[4] == 'N'
					&& pIO->m_bRcvBuffer[5] == 'O')
				{
					ReportError(pTag);
					pTag->m_wQuality = OPC_QUALITY_BAD;
					hr = E_FAIL;
				}

				else if (pIO->m_bRcvBuffer[4] == 'O'
					&& pIO->m_bRcvBuffer[5] == 'A')
				{
					int iFields;
					int iAddresses[6]; // Accuload addresses up to six arms; unused/unlicenced arms will have address 00

					pIO->m_bRcvBuffer[254] = 0;

					CString strData((LPSTR)&pIO->m_bRcvBuffer[7]);

					iFields = swscanf(strData, _T("%u %u %u %u %u %u"), &iAddresses[0], &iAddresses[1], &iAddresses[2], &iAddresses[3], &iAddresses[4], &iAddresses[5]);
					if (6 == iFields) // Accuload communications guide specifies that OA command will return 6 addresses for Accuload III/IV
					{
						pTag->m_wQuality = OPC_QUALITY_BAD;
						pTag->m_Value.vt = pTag->m_NativeType;
						pTag->m_Value.intVal = 0;

						for (int iArmNumber = 1; iArmNumber <= 6; iArmNumber++)
						{
							if ((iAddresses[iArmNumber - 1] & 0xFF) == pTag->m_bAddress)
							{
								pTag->m_wQuality = OPC_QUALITY_GOOD;
								pTag->m_Value.intVal = iArmNumber;
							}
						}
					}
					else
						pTag->m_wQuality = OPC_QUALITY_BAD;
				}
				else
					pTag->m_wQuality = OPC_QUALITY_COMM_FAILURE;
			}
		}

		else
			return E_FAIL;
	}
	else
		return E_FAIL;

	return hr;
}


void CAcculoadDevice::ReportError(CTag* pTag)
{
	CIO* pIO=pTag->m_pIO;

	CString oCode;
	
	// Start with code in case we cannot translate it.  If we can translate it below,
	// the value will be overwritten with the translation.
	oCode.Format( _T("NO%c%c"), pIO->m_bRcvBuffer[6], pIO->m_bRcvBuffer[7]);

	if(pIO->m_bRcvBuffer[6] == '0')
	{
		switch(pIO->m_bRcvBuffer[7])
		{
			case '0':
				oCode="NO00 - Invalid Command";
				break;
			case '1':
				oCode="NO01 - In Program Mode";
				break;
			case '2':
				oCode="NO02 - Accuload III Released";
				break;
			case '3':
				oCode="NO03 - Value out of Range";
				break;
			case '4':
				oCode="NO04 - Flow Active";
				break;
			case '5':
				oCode="NO05 - No Transaction Ever Done";
				break;
			case '6':
				oCode="NO06 - Operation Not Allowed";
				break;
			case '7':
				oCode="NO07 - Wrong Control Mode";
				break;
			case '8':
				oCode="NO08 - Transaction In Progress";
				break;
			case '9':
				oCode="NO09 - Alarm Condition";
				break;
		}
	}

	else if(pIO->m_bRcvBuffer[6] == '1')
	{
		switch(pIO->m_bRcvBuffer[7])
		{
			case '0':
				oCode="NO10 - Storage Full";
				break;
			case '1':
				oCode="NO11 - Operation Out of Sequence";
				break;
			case '2':
				oCode="NO12 - Power Failed During Transaction";
				break;
			case '3':
				oCode="NO13 - Comm Authorized";
				break;
			case '4':
				oCode="NO14 - Program Code Not Used";
				break;
			case '5':
				oCode="NO15 - Keypad/Display in Use";
				break;
			case '6':
				oCode="NO16 - Ticket Not In Printer";
				break;
			case '7':
				oCode="NO17 - No Keypad Data Pending";
				break;
			case '8':
				oCode="NO18 - No Transaction In Progress";
				break;
			case '9':
				oCode="NO19 - Option Not Installed";
				break;
		}
	}			 

	else if(pIO->m_bRcvBuffer[6] == '2')
	{
		switch(pIO->m_bRcvBuffer[7])
		{
			case '0':
				oCode="NO20 - Start after Stop Delay";
				break;
			case '1':
				oCode="NO21 - Permissive Delay Active";
				break;
			case '2':
				oCode="NO22 - Print Request Pending";
				break;
			case '3':
				oCode="NO23 - No Meter Enabled";
				break;
			case '4':
				oCode="NO24 - Must Be In Program Mode";
				break;
			case '5':
				oCode="NO25 - Ticket Alarm During Transaction";
				break;
			case '6':
				oCode="NO26 - Volume Type Not Selected";
				break;
			case '7':
				oCode="NO27 - Exactly 1 Recipe Must Be Enabled";
				break;
			case '8':
				oCode="NO28 - Batch Limit Reached";
				break;
			case '9':
				oCode="NO29 - Checking Entries";
				break;
		}
	}			 

	else if(pIO->m_bRcvBuffer[6] == '3')
	{
		switch(pIO->m_bRcvBuffer[7])
		{
			case '0':
				oCode="NO30 - Invalid Product/Recipe/Additive";
				break;
			case '1':
				oCode="NO31 - Invalid Argument for Configuration";
				break;
			case '2':
				oCode="NO32 - No Key Ever Pressed";
				break;
			case '3':
				oCode="NO33 - Max Active Arms Reached";
				break;
			case '4':
				oCode="NO34 - Transaction Not Standby";
				break;
			case '5':
				oCode="NO35 - Swing Arm Out of Position";
				break;
			case '6':
				oCode="NO36 - Card-in Required";
				break;
			case '7':
				oCode="NO37 - Data Not Available";
				break;
			case '8':
				oCode="NO38 - Invalid Additive Combination";
				break;
			case '9':
				oCode="NO39 - No Current Batch on this Arm";
				break;
		}

	}			 

	else if(pIO->m_bRcvBuffer[6] == '9')
	{
		switch(pIO->m_bRcvBuffer[7])
		{
			case '0':
				oCode="NO90 - Microcomputer Protocol Required";
				break;

			case '1':
				oCode="NO91 - Buffer Allocation Failure";
				break;

			case '2':
				oCode="NO92 - Keypad Locked";
				break;

			case '3':
				oCode="NO93 - Data Recall Failure";
				break;

			case '4':
				oCode="NO94 - Not in Program Mode";
				break;

			case '5':
				oCode="NO95 - Security Access Not Available";
				break;

			case '9':
				oCode="NO99 - Device Internal Error";
				break;

		}

	}

	CString oError;
	oError.Format(_T("IO Error : %s for %s"),oCode,pTag->GetPathName());
	theApp.LogError(oError);
}


HRESULT CAcculoadDevice::PerformNetworkIO(CTag* pTag)
{
	CIO* pIO=pTag->m_pIO;
	CString csMessage;

	if(pIO->m_pSocket == NULL)
	{
		HRESULT hr=pIO->OpenSocket(pTag);
		if(FAILED(hr))
		{
			csMessage.Format(_T("PerformNetworkIO - OpenSocket failed with hresult %u"), hr);
			pIO->LogMessage(csMessage.GetString());
			pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
			return E_FAIL;
		}
	}

	for(INT iTry=0;iTry < 3;iTry++)
	{
		pIO->LogWrite(pIO->m_bXmtBuffer,pIO->m_wXmtLength);
		if(SOCKET_ERROR == pIO->m_pSocket->Send(pIO->m_bXmtBuffer,pIO->m_wXmtLength))
		{
			CString oError;
			oError.Format(_T("IO Error = %s : CAsyncSocket.SendTo"),pIO->SocketError(pIO->m_pSocket->GetLastError()));
			theApp.LogError(oError);
			pIO->LogMessage(oError.GetString());

			delete pIO->m_pSocket;
			pIO->m_pSocket=NULL;
			pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
			return E_FAIL;
		}

		// Start with 1 to align receive data with Minicomp Host Protocol
		pIO->m_wRcvLength=1;
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
				pIO->LogMessage(oError.GetString());

				delete pIO->m_pSocket;
				pIO->m_pSocket=NULL;
				pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;

				pIO->SignalCommunicationsFailure(pTag);

				return E_FAIL;
			}

			pIO->m_wRcvLength+=(WORD) dwNumberOfBytesRead; 

			// Receipt is complete on LF
			if(pIO->m_bRcvBuffer[pIO->m_wRcvLength-1] == 0x0A)
				break;
		}

		pIO->LogRead(pIO->m_bRcvBuffer,dwNumberOfBytesRead);

		// Minimum 5 bytes
		if(dwNumberOfBytesRead < 5)
		{
			csMessage.Format(_T("PerformNetworkIO - received too few bytes(%d)"), dwNumberOfBytesRead);
			pIO->LogMessage(csMessage.GetString());
			continue;
		}

		if(dwNumberOfBytesRead > BUFFER_MAX-1)
		{
			csMessage.Format(_T("PerformNetworkIO - received too many bytes(%d)"), dwNumberOfBytesRead);
			pIO->LogMessage(csMessage.GetString());
			continue;
		}

		// First Byte is *
		if(pIO->m_bRcvBuffer[1] != '*')
		{
			csMessage.Format(_T("PerformNetworkIO - unexpected first byte (%x)"), pIO->m_bRcvBuffer[1]);
			pIO->LogMessage(csMessage.GetString());
			continue;
		}

		// Third and Forth are Address
		if(pIO->m_bRcvBuffer[2] != pIO->m_bXmtBuffer[1]
		|| pIO->m_bRcvBuffer[3] != pIO->m_bXmtBuffer[2])
		{
			csMessage.Format(_T("PerformNetworkIO - unexpected received address (%c%c)"), pIO->m_bRcvBuffer[2], pIO->m_bRcvBuffer[3]);
			pIO->LogMessage(csMessage.GetString());
			continue;
		}


		// Second to Last is CR
		if(pIO->m_bRcvBuffer[pIO->m_wRcvLength-2] != 0x0D)
		{
			csMessage.Format(_T("PerformNetworkIO - unexpected end response character(%c)"), pIO->m_bRcvBuffer[pIO->m_wRcvLength-2]);
			pIO->LogMessage(csMessage.GetString());
			continue;
		}

		pIO->m_wRcvLength--;

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

HRESULT CAcculoadDevice::PerformSerialIO(CTag* pTag)
{
	CIO* pIO=pTag->m_pIO;
	DWORD dwError = 0;
	CString csMessage;

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
			WaitForSingleObject(pIO->m_hKillEvent,1000);			
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
		{
			dwError = GetLastError();
			csMessage.Format(_T("PerformSerialIO - ClearCommError [write] failed with error %u"), dwError);
			pIO->LogMessage(csMessage.GetString());
			continue;
		}

		if (dwCommErrFlags != 0)
		{
			csMessage.Format(_T("PerformSerialIO - ClearCommError [write] cleared error flags %x"), dwCommErrFlags);
			pIO->LogMessage(csMessage.GetString());
		}

		if(!PurgeComm( pIO->m_hPort,
							PURGE_RXCLEAR |
							PURGE_RXABORT |
							PURGE_TXCLEAR |
							PURGE_TXABORT))
		{
			dwError = GetLastError();
			csMessage.Format(_T("PerformSerialIO - PurgeComm failed with error %u"), dwError);
			pIO->LogMessage(csMessage.GetString());
			continue;
		}

		// Read the response
	 	if(!SetCommMask(pIO->m_hPort,EV_ERR | EV_RXFLAG))
		{
			dwError = GetLastError();
			csMessage.Format(_T("PerformSerialIO - SetCommMask failed with error %u"), dwError);
			pIO->LogMessage(csMessage.GetString());
			continue;
		}

		// Write the request
		pIO->m_WriteOverLapped.Offset=0;
		pIO->m_WriteOverLapped.OffsetHigh=0;
		//pIO->LogWrite(pIO->m_bXmtBuffer,pIO->m_wXmtLength); // duplicates the call in CIO::ReadTag and CIO::WriteTag
		if(!WriteFile(pIO->m_hPort,pIO->m_bXmtBuffer,pIO->m_wXmtLength,&dwNumberOfBytesWritten,&pIO->m_WriteOverLapped))
		{
			if((dwError = GetLastError()) != ERROR_IO_PENDING)
			{
				csMessage.Format(_T("PerformSerialIO - WriteFile failed with error %u"), dwError);
				pIO->LogMessage(csMessage.GetString());
				continue;
			}

			if(!GetOverlappedResult(pIO->m_hPort,&pIO->m_WriteOverLapped,&dwNumberOfBytesWritten,TRUE))
			{
				dwError = GetLastError();
				csMessage.Format(_T("PerformSerialIO - GetOverlappedResult [write] failed with error %u"), dwError);
				pIO->LogMessage(csMessage.GetString());
				continue;
			}
		}

		if(pIO->m_wXmtLength != dwNumberOfBytesWritten)
		{
			csMessage.Format(_T("PerformSerialIO - WriteFile tried to write %u bytes but only successfully wrote %u"), pIO->m_wXmtLength, dwNumberOfBytesWritten);
			pIO->LogMessage(csMessage.GetString());
			continue;
		}

		if(!WaitCommEvent(pIO->m_hPort,&dwCommEvtFlags,&pIO->m_CommOverLapped)
		&& (dwError = GetLastError()) != ERROR_IO_PENDING)
		{
			csMessage.Format(_T("PerformSerialIO - WaitCommEvent failed with error %u"), dwError);
			pIO->LogMessage(csMessage.GetString());
			if(pTag->m_wQuality == OPC_QUALITY_COMM_FAILURE)
			{
				pIO->CloseComPort();
				pIO->m_bPortParametersChanged=FALSE;
				pIO->SignalCommunicationsFailure(pTag, dwError);

				return E_FAIL;
			}
			else
				continue;
		}

		switch(WaitForSingleObject(pIO->m_CommOverLapped.hEvent,pIO->m_dwCommunicationsTimeOut))
		{
			case WAIT_OBJECT_0:
	   		if((dwCommEvtFlags & EV_ERR ) == EV_ERR)
			{
				csMessage.Format(_T("PerformSerialIO - WaitCommEvent indicated event flag EV_ERR"));
				pIO->LogMessage(csMessage.GetString());
				continue;
			}

   			else if((dwCommEvtFlags & EV_RXFLAG ) == EV_RXFLAG )
				{
					// For Card Reader wait for LRC
					if(pTag->m_pDevice->m_Type == SMITH_PROXIMITY)
					{
						float fBaudRate;					
						switch(pIO->m_Baud)
						{
							case ACCULOAD_BAUD_1200:
								fBaudRate=CBR_1200;
								break;
							case ACCULOAD_BAUD_2400:
								fBaudRate=CBR_2400;
								break;
							case ACCULOAD_BAUD_4800:
								fBaudRate=CBR_4800;
								break;
							case ACCULOAD_BAUD_9600:
								fBaudRate=CBR_9600;
								break;
							case ACCULOAD_BAUD_19200:
								fBaudRate=CBR_19200;
								break;
							case ACCULOAD_BAUD_38400:
								fBaudRate=CBR_38400;
								break;
							default:
								fBaudRate=CBR_1200;
								break;
						}

						float fTimeoutMult = 2000 * 11 / fBaudRate;
						if(fTimeoutMult < 20)
							fTimeoutMult=20;

						Sleep((int) fTimeoutMult);
					}

					if(!ClearCommError(pIO->m_hPort,&dwCommErrFlags,&ComStat))
					{
						dwError = GetLastError();
						csMessage.Format(_T("PerformSerialIO - ClearCommError [read] failed with error %u"), dwError);
						pIO->LogMessage(csMessage.GetString());
						continue;
					}

					if (dwCommErrFlags != 0)
					{
						csMessage.Format(_T("PerformSerialIO - ClearCommError [read] cleared error flags %x"), dwCommErrFlags);
						pIO->LogMessage(csMessage.GetString());
					}

					if(ComStat.cbInQue < 5)
					{
						csMessage.Format(_T("PerformSerialIO - ClearCommError [read] Comstat shows too few bytes(%x)"), ComStat.cbInQue);
						pIO->LogMessage(csMessage.GetString());
						continue;
					}

					if(ComStat.cbInQue > BUFFER_MAX-1)
					{
						csMessage.Format(_T("PerformSerialIO - ClearCommError [read] Comstat shows too many bytes(%x)"), ComStat.cbInQue);
						pIO->LogMessage(csMessage.GetString());
						continue;
					}

					pIO->m_ReadOverLapped.Offset=0;
					pIO->m_ReadOverLapped.OffsetHigh=0;
					if(!ReadFile(pIO->m_hPort,pIO->m_bRcvBuffer,ComStat.cbInQue,&dwNumberOfBytesRead,&pIO->m_ReadOverLapped)
					&& (dwError = GetLastError()) != ERROR_IO_PENDING )
					{
						csMessage.Format(_T("PerformSerialIO - ReadFile failed with error %u"), dwError);
						pIO->LogMessage(csMessage.GetString());
						continue;
					}

				 	if(!GetOverlappedResult(pIO->m_hPort,&pIO->m_ReadOverLapped,&dwNumberOfBytesRead,TRUE))
					{
						dwError = GetLastError();
						csMessage.Format(_T("PerformSerialIO - GetOverlappedResult [read] failed with error %u"), dwError);
						pIO->LogMessage(csMessage.GetString());
						continue;
					}

					break;
				}
				else
					continue;

	      case WAIT_TIMEOUT:
				csMessage.Format(_T("PerformSerialIO - wait timed out - %d ms"), pIO->m_dwCommunicationsTimeOut);
				pIO->LogMessage(csMessage.GetString());
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
			{
				csMessage.Format(_T("PerformSerialIO - WaitForSingleObject on read failed"));
				pIO->LogMessage(csMessage.GetString());
				continue;
			}
		}

		// pIO->LogRead(pIO->m_bRcvBuffer,dwNumberOfBytesRead); // duplicates the call in CIO::ReadTag and CIO::WriteTag

		// Minimum 5 bytes
		if(dwNumberOfBytesRead < 5)
		{
			csMessage.Format(_T("PerformSerialIO - less than minimum bytes read"));
			pIO->LogMessage(csMessage.GetString());
			continue;
		}

		if(dwNumberOfBytesRead > BUFFER_MAX-1)
		{
			csMessage.Format(_T("PerformSerialIO - more than maximum bytes read"));
			pIO->LogMessage(csMessage.GetString());
			continue;
		}

		pIO->m_wRcvLength=(WORD) dwNumberOfBytesRead; 

		if(pTag->m_pDevice->m_Type == SMITH_PROXIMITY)
		{
			// First Byte is STX
			if(pIO->m_bRcvBuffer[0] != STX)
			{
				csMessage.Format(_T("PerformSerialIO - missing STX read"));
				pIO->LogMessage(csMessage.GetString());
				continue;
			}

			// Second and Third are Address
			if(pIO->m_bRcvBuffer[1] != pIO->m_bXmtBuffer[1]
			|| pIO->m_bRcvBuffer[2] != pIO->m_bXmtBuffer[2])
			{
				csMessage.Format(_T("PerformSerialIO - mismatched address read"));
				pIO->LogMessage(csMessage.GetString());
				continue;
			}

			BYTE bLRC;
		
			// Last Character is LRC
			bLRC=pIO->m_bRcvBuffer[pIO->m_wRcvLength-1];
			pIO->m_wRcvLength--;
				
			// Next to last is ETX
			if(pIO->m_bRcvBuffer[pIO->m_wRcvLength-1] != ETX)
			{
				csMessage.Format(_T("PerformSerialIO - missing ETX read"));
				pIO->LogMessage(csMessage.GetString());
				continue;
			}

			// Skip STX prefix
			if(bLRC != pIO->LRC(&pIO->m_bRcvBuffer[1],pIO->m_wRcvLength-1))
			{
				csMessage.Format(_T("PerformSerialIO - bad LRC read"));
				pIO->LogMessage(csMessage.GetString());
				continue;
			}
		}
		else
		{
			// First Byte is NL
			if(pIO->m_bRcvBuffer[0] != '\0')
			{
				csMessage.Format(_T("PerformSerialIO - missing initial NULL read"));
				pIO->LogMessage(csMessage.GetString());
				continue;
			}

			// Second Byte is STX
			if(pIO->m_bRcvBuffer[1] != STX)
			{
				csMessage.Format(_T("PerformSerialIO - missing STX read"));
				pIO->LogMessage(csMessage.GetString());
				continue;
			}

			// Third and Fourth are Address
			if(pIO->m_bRcvBuffer[2] != pIO->m_bXmtBuffer[1]
			|| pIO->m_bRcvBuffer[3] != pIO->m_bXmtBuffer[2])
			{
				csMessage.Format(_T("PerformSerialIO - mismatched address read"));
				pIO->LogMessage(csMessage.GetString());
				continue;
			}

			BYTE bLRC;
		
			// Last character should be 0x7F, it may be PAD or LRC
			if(pIO->m_bRcvBuffer[pIO->m_wRcvLength-1] != 0x7F)
			{
				csMessage.Format(_T("PerformSerialIO - unexpected last character received. %x"), (BYTE)pIO->m_bRcvBuffer[pIO->m_wRcvLength-1]);
				pIO->LogMessage(csMessage.GetString());
				continue;
			}

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
				{
					csMessage.Format(_T("PerformSerialIO - mismatched address read"));
					pIO->LogMessage(csMessage.GetString());
					continue;
				}

				bLRC=pIO->m_bRcvBuffer[pIO->m_wRcvLength-1];
				pIO->m_wRcvLength--;
			}

			// Skip NULL,STX prefix
			if(bLRC != pIO->LRC(&pIO->m_bRcvBuffer[2],pIO->m_wRcvLength-2))
			{
				csMessage.Format(_T("PerformSerialIO - bad LRC read"));
				pIO->LogMessage(csMessage.GetString());
				continue;
			}
		}
		break;
	}

	if(iTry == 3
	|| pIO->m_bPortParametersChanged)
	{
		pIO->CloseComPort();
		pIO->m_bPortParametersChanged=FALSE;
		pTag->m_wQuality=OPC_QUALITY_COMM_FAILURE;
		pIO->SignalCommunicationsFailure(pTag, dwError);

		return E_FAIL;
	}

	pIO->SignalCommunicationsRestored(pTag);

	return S_OK;
}

