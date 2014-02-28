#include "stdafx.h"

// Functions Receiving Structures
typedef struct _CAR
{
	LPSTR make;
	LPSTR color;
} CAR;

typedef struct _CAR2
{
	CAR theCar;
	LPSTR petName;
} CAR2;

extern "C" PINVOKE_API void GiveMeThreeBasicCars(CAR** theCars);
extern "C" PINVOKE_API void DisplayBetterCar(CAR2* theCar);
