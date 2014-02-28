#include "stdafx.h"
#include "Structures.h"

// Just some demo data
const int numbOfCars = 3;
LPCSTR carMakes[numbOfCars] = { "BMW", "Ford", "Viper" };
LPCSTR carColors[numbOfCars] = { "Green", "Pink", "Red" };
LPCWSTR wideCarMakes[numbOfCars] = { L"BMW", L"Ford", L"Viper" };

// A method taking a struct.
extern "C" PINVOKE_API void DisplayBetterCar(CAR2* theCar)
{
	// Print data about the car
	std::cout << theCar->theCar.color << '\n';
	std::cout << theCar->theCar.make << '\n';
	std::cout << theCar->petName << '\n';
}

// A Method returning an array of structs.
extern "C" PINVOKE_API void GiveMeThreeBasicCars(CAR** theCars)
{
	// Use CoTaskMemAlloc instead of new as .NET's P/Invoke uses
	// CoTaskMemFree. See also http://blogs.msdn.com/b/dsvc/archive/2009/06/22/troubleshooting-pinvoke-related-issues.aspx
	// and http://stackoverflow.com/questions/3614367/c-sharp-free-memory-allocated-by-operator-new-from-p-invoke-dll
	// for details.
	*theCars = (CAR*)CoTaskMemAlloc(numbOfCars * sizeof(CAR));

	// Fill the CAR structures
	auto currentCar = *theCars;
	for (int i = 0; i < numbOfCars; i++, currentCar++)
	{
		currentCar->color = carColors[i];
		currentCar->make = carMakes[i];
	}
}

// A Method returning an array of structs with embedded character arrays.
extern "C" PINVOKE_API void FillThreeBasicCars(CARFIXED* theCars)
{
	for (int i = 0; i < numbOfCars; i++, theCars++)
	{
		strncpy(theCars->make, carMakes[i], sizeof(theCars->make));
		strncpy(theCars->color, carColors[i], sizeof(theCars->color));
	}
}

// Function returning an array of BSTRs with variable length
extern "C" PINVOKE_API void GiveMeMakes(BSTR** makes, int *length)
{
	auto result = (BSTR*)CoTaskMemAlloc(numbOfCars * sizeof(BSTR));

	*length = numbOfCars;
	for (int i = 0; i < numbOfCars; i++)
	{
		result[i] = SysAllocString(wideCarMakes[i]);
	}

	*makes = result;
}