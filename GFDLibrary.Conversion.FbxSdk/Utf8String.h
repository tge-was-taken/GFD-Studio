#pragma once
#include "pch.h"
#include <cassert>

using namespace System;
using namespace System::Text;

struct Utf8String
{
	u8 buffer[ 4096 ];

	const char* ToCStr() const
	{
		return (const char*)buffer;
	}

	Utf8String( String^ string )
	{
		const pin_ptr<const wchar_t> data = PtrToStringChars( string );
		const int length = Encoding::UTF8->GetBytes( (wchar_t*)data, string->Length, buffer, sizeof( buffer ) );
		assert( length < 4096);
		buffer[ length ] = NULL;
	}
};