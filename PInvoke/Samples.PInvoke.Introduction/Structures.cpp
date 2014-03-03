#include "stdafx.h"
#include "Structures.h"

// Just some demo data
const int numbOfCars = 3;
LPCWSTR wideCarMakes[numbOfCars] = { L"BMW", L"Ford", L"Viper" };
LPCWSTR wideCarColors[numbOfCars] = { L"Green", L"Pink", L"Red" };

// A method taking a struct.
extern "C" PINVOKE_API void DisplayBetterCar(BETTERCAR* theCar)
{
	// Print data about the car
	std::wcout << theCar->theCar.color << '\n';
	std::wcout << theCar->theCar.make << '\n';
	std::wcout << theCar->petName << '\n';
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
		currentCar->color = wideCarColors[i];
		currentCar->make = wideCarMakes[i];
	}

	// Note that the caller is responsible for freeing *theCars
}

// A Method returning an array of structs with embedded character arrays.
extern "C" PINVOKE_API void FillThreeBasicCars(CARFIXED* theCars)
{
	// Note that the callee assumes that the caller has allocated the
	// necessary memory.

	for (int i = 0; i < numbOfCars; i++, theCars++)
	{
		wcsncpy_s(theCars->make, sizeof(theCars->make) / sizeof(theCars->make[0]), wideCarMakes[i], wcslen(wideCarMakes[i]));
		wcsncpy_s(theCars->color, sizeof(theCars->color) / sizeof(theCars->color[0]), wideCarColors[i], wcslen(wideCarColors[i]));
	}
}

// Function returning an array of BSTRs with variable length
extern "C" PINVOKE_API void GiveMeMakes(BSTR** makes, int *length)
{
	// Allocate memory for receiving the array of BSTR*
	auto result = (BSTR*)CoTaskMemAlloc(numbOfCars * sizeof(BSTR));

	*length = numbOfCars;
	for (int i = 0; i < numbOfCars; i++)
	{
		// Allocate string for each car
		result[i] = SysAllocString(wideCarMakes[i]);
	}

	*makes = result;

	// Note that the caller is responsible for freeing the strings with
	// the car makes as well as the BSTR array.
}