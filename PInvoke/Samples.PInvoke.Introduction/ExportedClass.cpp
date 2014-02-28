#include "stdafx.h"
#include "ExportedClass.h"

// Method to create a CMiniVan.
extern "C" PINVOKE_API CMiniVan* CreateMiniVan()
{
	// Note that we use new to create the object.
	return new CMiniVan();
}

// Method accessing a CMiniVan
extern "C" PINVOKE_API int GetNumberOfSeats(CMiniVan* obj)
{
	if (NULL == obj)
	{
		return 0;
	}

	return obj->GetNumberOfSeats();
}

// Method to destroy a CMiniVan.
extern "C" PINVOKE_API void DeleteMiniVan(CMiniVan* obj)
{
	// Deletes a CMiniVan object that has been created with
	// CreateMiniVan.
	delete obj;
}

