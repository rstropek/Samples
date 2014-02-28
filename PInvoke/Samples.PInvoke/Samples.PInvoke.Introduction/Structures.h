#include "stdafx.h"

// Functions Receiving Structures
typedef struct 
{
	LPCSTR make;
	LPCSTR color;
} CAR;

// Variant of CAR with embedded character arrays
typedef struct 
{
	char make[256];
	char color[256];
} CARFIXED;

// Structure containing a structure
typedef struct _CAR2
{
	CAR theCar;
	LPCSTR petName;
} CAR2;

// Function returning an array of three cars
extern "C" PINVOKE_API void GiveMeThreeBasicCars(CAR** theCars);

// Function receiving a complex structure
extern "C" PINVOKE_API void DisplayBetterCar(CAR2* theCar);

// Function filling an array of three CARFIXED structures
extern "C" PINVOKE_API void FillThreeBasicCars(CARFIXED* theCars);

// Function returning an array of BSTRs with variable length
extern "C" PINVOKE_API void GiveMeMakes(BSTR** makes, int *length);
