#include "stdafx.h"

// Various structure to demonstrate different aspects of marshaling

// Structure with embedded pointers to wide strings
typedef struct 
{
	LPCWSTR make;
	LPCWSTR color;
} CAR;

// Different flavor of CAR with embedded character arrays
typedef struct 
{
	WCHAR make[256];
	WCHAR color[256];
} CARFIXED;

// Structure containing a structure plus an embedded character array
typedef struct
{
	CAR theCar;
	LPCWSTR petName;
} BETTERCAR;



// Function returning an array of exactly three cars
extern "C" PINVOKE_API void GiveMeThreeBasicCars(CAR** theCars);

// Function receiving a complex structure
extern "C" PINVOKE_API void DisplayBetterCar(BETTERCAR* theCar);

// Function filling an array of exactly three CARFIXED structures
extern "C" PINVOKE_API void FillThreeBasicCars(CARFIXED* theCars);

// Function returning an array of BSTRs with variable length
extern "C" PINVOKE_API void GiveMeMakes(BSTR** makes, int *length);
