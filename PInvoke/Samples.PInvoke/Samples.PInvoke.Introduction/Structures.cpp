#include "stdafx.h"
#include "Structures.h"

// A method taking a struct.
extern "C" PINVOKE_API void DisplayBetterCar(CAR2* theCar)
{
	// Print data about the car
	std::cout << theCar->theCar.color;
	std::cout << theCar->theCar.make;
	std::cout << theCar->petName;
}

// A Method returning an array of structs.
extern "C" PINVOKE_API void GiveMeThreeBasicCars(CAR** theCars)
{
	auto numbOfCars = 3;

	// Use CoTaskMemAlloc instead of new as .NET's P/Invoke uses
	// CoTaskMemFree. See also http://blogs.msdn.com/b/dsvc/archive/2009/06/22/troubleshooting-pinvoke-related-issues.aspx
	// and http://stackoverflow.com/questions/3614367/c-sharp-free-memory-allocated-by-operator-new-from-p-invoke-dll
	// for details.
	*theCars = (CAR*)CoTaskMemAlloc(numbOfCars * sizeof(CAR));

	LPSTR carMakes[3] = { "BMW", "Ford", "Viper" };
	LPSTR carColors[3] = { "Green", "Pink", "Red" };

	auto pCurCar = *theCars;
	for (int i = 0; i < numbOfCars; i++, pCurCar++)
	{
		pCurCar->color = carColors[i];
		pCurCar->make = carMakes[i];
	}
}
