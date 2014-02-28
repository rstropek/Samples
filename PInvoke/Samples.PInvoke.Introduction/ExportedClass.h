#include "stdafx.h"

// A class to be exported.
class PINVOKE_API CMiniVan
{
private:
	int m_numbSeats;

public:
	CMiniVan() 
	{ 
		m_numbSeats = 9; 
	}

	int GetNumberOfSeats()
	{
		return m_numbSeats;
	}
};

// Functions for class marshaling.
extern "C" PINVOKE_API CMiniVan* CreateMiniVan();
extern "C" PINVOKE_API void DeleteMiniVan(CMiniVan* obj);
extern "C" PINVOKE_API int GetNumberOfSeats(CMiniVan* obj);
