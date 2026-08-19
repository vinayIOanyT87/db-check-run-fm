//------------------------------------------------------------------------
// Copyright (C) Sewell Development Corporation, 1994 - 1999.
//     Web: www.sewelld.com      E-mail: support@sewelld.com
//
// LICENSE: Paid-up licensees are authorized to use this code on a site-wide
// basis and incorporate it into their software products, provided that the
// code is not resold as stand-alone source code or as part of a code library,
// and that this copyright notice and license agreement are not removed.
//------------------------------------------------------------------------

//  Implementation of SecureHashAlgorithm class.

#include "StdAfx.h"
#include "SecureHashAlgorithm.h"
#include <stdlib.h>

const int BLOCK_SIZE_IN_BITS  = 512;
const int BLOCK_SIZE_IN_BYTES = BLOCK_SIZE_IN_BITS / 8;


class _SecureHashAlgorithm1 
{
public:
	_SecureHashAlgorithm1();

	bool ComputeHash( const void* dataBuffer, DWORD dataBufferLength );
	bool GetHashValue( void* hashBuffer, DWORD hashBufferLength );
	void ComputeBlock( const BYTE* block );

	DWORD F0();
	DWORD F1();
	DWORD F2();
	DWORD F3();

	bool	m_hashComputed;
	__int64	m_messageLengthInBytes;

	DWORD	m_h[5];
	DWORD	m_w[80];
	DWORD	m_K[4];
	DWORD	m_A;
	DWORD	m_B;
	DWORD	m_C;
	DWORD	m_D;
	DWORD	m_E;

	DWORD	(_SecureHashAlgorithm1::*m_func[4])();
	DWORD	m_hashValue[5];

	int	m_bufferLength;
	BYTE	m_buffer[BLOCK_SIZE_IN_BYTES];
};

///////////////////////////////////////////////////////////////////////////////
//
///////////////////////////////////////////////////////////////////////////////
DWORD Swap( DWORD value )
{
	DWORD swappedValue;
	for( int i=0; i<4; i++ ) 
	{
		*(( BYTE* )&swappedValue + i ) = *(( const BYTE* )&value + 3 - i );
	}
	return swappedValue;
}

///////////////////////////////////////////////////////////////////////////////
//
///////////////////////////////////////////////////////////////////////////////
_SecureHashAlgorithm1::_SecureHashAlgorithm1()
{
	m_hashComputed	= false;
	m_messageLengthInBytes = 0;
	m_bufferLength	= 0;

	m_h[0]	= 0x67452301;
	m_h[1]	= 0xEFCDAB89;
	m_h[2]	= 0x98BADCFE;
	m_h[3]	= 0x10325476;
	m_h[4]	= 0xC3D2E1F0;
	m_K[0]	= 0x5A827999;
	m_K[1]	= 0x6ED9EBA1;
	m_K[2]	= 0x8F1BBCDC;
	m_K[3]	= 0xCA62C1D6;

	m_func[0] = &_SecureHashAlgorithm1::F0;
	m_func[1] = &_SecureHashAlgorithm1::F1;
	m_func[2] = &_SecureHashAlgorithm1::F2;
	m_func[3] = &_SecureHashAlgorithm1::F1;
}

///////////////////////////////////////////////////////////////////////////////
//
///////////////////////////////////////////////////////////////////////////////
DWORD _SecureHashAlgorithm1::F0()
{
	return (m_B & m_C) | (~m_B & m_D);
}

///////////////////////////////////////////////////////////////////////////////
//
///////////////////////////////////////////////////////////////////////////////
DWORD _SecureHashAlgorithm1::F1()
{
	return m_B ^ m_C ^ m_D;
}

///////////////////////////////////////////////////////////////////////////////
//
///////////////////////////////////////////////////////////////////////////////
DWORD _SecureHashAlgorithm1::F2()
{
	return (m_B & m_C) | (m_B & m_D) | (m_C & m_D);
}

///////////////////////////////////////////////////////////////////////////////
//
///////////////////////////////////////////////////////////////////////////////
void _SecureHashAlgorithm1::ComputeBlock(const BYTE* block)
{
	const DWORD* sourceWords = reinterpret_cast<const DWORD *>(block);

	for( int i=0; i<16; i++ ) 
	{
		m_w[i] = Swap( sourceWords[ i ]);
	}

	for( int t=16; t<=79; t++ )
	{
		DWORD Wt = m_w[t - 3] ^ m_w[t - 8] ^ m_w[t - 14] ^ m_w[t - 16];
		m_w[t] = _rotl(Wt, 1);
	}

	m_A = m_h[0];
	m_B = m_h[1];
	m_C = m_h[2];
	m_D = m_h[3];
	m_E = m_h[4];
	DWORD TEMP;

	for( int k=0; k<4; k++ )
	{
		for( t=k*20; t<=k*20+19; t++ ) 
		{
			TEMP = _rotl(m_A, 5) + (this->*m_func[k])() + m_E + m_w[t] + m_K[k];
			
			m_E = m_D;
			m_D = m_C;
			m_C = _rotl(m_B, 30);
			m_B = m_A;
			m_A = TEMP;
		}
	}
	m_h[0] += m_A;
	m_h[1] += m_B;
	m_h[2] += m_C;
	m_h[3] += m_D;
	m_h[4] += m_E;
}

///////////////////////////////////////////////////////////////////////////////
//
///////////////////////////////////////////////////////////////////////////////
bool _SecureHashAlgorithm1::ComputeHash(const void* dataBuffer, DWORD dataBufferLength)
{
	bool result = !m_hashComputed;	// Return false if hash value was already computed.

	if( result )
	{
		m_messageLengthInBytes += dataBufferLength;
		const BYTE* data = static_cast<const BYTE *>(dataBuffer);

		if( m_bufferLength ) 
		{		
			// We have some left over characters
			DWORD addBuffLen = __min(sizeof(m_buffer) - m_bufferLength, dataBufferLength);

			memcpy(m_buffer + m_bufferLength, data, addBuffLen);

			data += addBuffLen;
			dataBufferLength -= addBuffLen;
			m_bufferLength   += addBuffLen;

			if( m_bufferLength == sizeof( m_buffer ))
			{
				ComputeBlock( m_buffer );
				m_bufferLength = 0;
			}
		}
		while( dataBufferLength >= BLOCK_SIZE_IN_BYTES )
		{
			ComputeBlock( data );

			data += BLOCK_SIZE_IN_BYTES;
			dataBufferLength -= BLOCK_SIZE_IN_BYTES;
		}
		if( dataBufferLength )
		{
			m_bufferLength = dataBufferLength;
			memcpy( m_buffer, data, dataBufferLength );
		}
	}
	return result;
}

///////////////////////////////////////////////////////////////////////////////
//
///////////////////////////////////////////////////////////////////////////////
bool _SecureHashAlgorithm1::GetHashValue( void* hashBuffer, DWORD hashBufferLength )
{
	bool result = false;

	if( hashBufferLength >= 20 )
	{
		result = true;
		if( !m_hashComputed )
		{
			m_hashComputed = true;
			__int64 messageLengthInBits = m_messageLengthInBytes * 8;
			m_buffer[m_bufferLength++] = 0x80;		// Add the required 1 bit

			if( BLOCK_SIZE_IN_BYTES == m_bufferLength )
			{
				ComputeBlock( m_buffer );
				m_bufferLength = 0;
			}
			if( m_bufferLength > BLOCK_SIZE_IN_BYTES - 8 )
			{
				// We can't fit in this buffer.
				memset( m_buffer + m_bufferLength, 0, BLOCK_SIZE_IN_BYTES - m_bufferLength );
				ComputeBlock( m_buffer );
				m_bufferLength = 0;
			}
			memset( m_buffer+m_bufferLength, 0, BLOCK_SIZE_IN_BYTES - 8 - m_bufferLength );

			// Store the bit length in the last 64-bits.  Since we are little endian,
			// and SHA expects big-endian, reverse the order of the bytes.
			const BYTE* bitLength = ( const BYTE* )&messageLengthInBits;

			for( int i=0; i<8; i++ )
			{
				m_buffer[ BLOCK_SIZE_IN_BYTES - i - 1] = *(bitLength + i);
			}
			ComputeBlock( m_buffer );
			for( i=0; i<5; i++ )
			{
				m_hashValue[ i ] = Swap( m_h[ i ]);
			}
		}
		memcpy( hashBuffer, m_hashValue, 20 );
	}
	return result;
}

//---- SecureHashAlogorithm1 ----

///////////////////////////////////////////////////////////////////////////////
//
///////////////////////////////////////////////////////////////////////////////
SecureHashAlgorithm1::SecureHashAlgorithm1()
{
	m_sha = new _SecureHashAlgorithm1;
}

///////////////////////////////////////////////////////////////////////////////
//
///////////////////////////////////////////////////////////////////////////////
SecureHashAlgorithm1::~SecureHashAlgorithm1()
{
	delete m_sha;
}

///////////////////////////////////////////////////////////////////////////////
//
///////////////////////////////////////////////////////////////////////////////
bool SecureHashAlgorithm1::ComputeHash(const void* dataBuffer, DWORD dataBufferLength)
{
	return m_sha->ComputeHash( dataBuffer, dataBufferLength );
}

///////////////////////////////////////////////////////////////////////////////
//
///////////////////////////////////////////////////////////////////////////////
bool SecureHashAlgorithm1::GetHashValue( void* hashBuffer, DWORD hashBufferLength )
{
	return m_sha->GetHashValue( hashBuffer, hashBufferLength );
}
