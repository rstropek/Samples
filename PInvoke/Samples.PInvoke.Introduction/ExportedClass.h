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
// Create object
extern "C" PINVOKE_API CMiniVan* CreateMiniVan();

// Work with object
extern "C" PINVOKE_API void DeleteMiniVan(CMiniVan* obj);

// Destroy object
extern "C" PINVOKE_API int GetNumberOfSeats(CMiniVan* obj);
