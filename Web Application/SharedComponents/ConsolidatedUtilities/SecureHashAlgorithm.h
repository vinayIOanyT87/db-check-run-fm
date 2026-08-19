//------------------------------------------------------------------------
// Copyright (C) Sewell Development Corporation, 1994 - 2001.
//     Web: www.sewelld.com      E-mail: support@sewelld.com
//
// LICENSE: Paid-up licensees are authorized to use this code on a site-wide
// basis and incorporate it into their software products, provided that the
// code is not resold as stand-alone source code or as part of a code library,
// and that this copyright notice and license agreement are not removed.
//------------------------------------------------------------------------

//  Interface definition for SecureHashAlgorithm class.

#pragma once

class SecureHashAlgorithm1 
{
public:
	SecureHashAlgorithm1();
	~SecureHashAlgorithm1();

	// This call may be repeated as many times as desired to accumulate the hash value for a sequence of buffers.
	// Returns true (success) if GetHashValue() has not been called yet for this object, otherwise false.
	bool ComputeHash( const void* dataBuffer, DWORD dataBufferLength );

	// To get the 160-bit hash value, pass a pointer to a buffer that is 20 bytes long (at least).
	// Once this function has been called to get a result, ComputeHash() may no longer be called successfully.
	// After calling this function, if a new hash value computation is desired, instantiate a new object.
	// In other words, there is no "reset" function to allow re-using the object.
	bool GetHashValue( void* hashBuffer, DWORD hashBufferLength );

private:
	class _SecureHashAlgorithm1* m_sha;
};
