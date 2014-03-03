#include "stdafx.h"
#include "CalculationFunctions.h"

// A very simple DLL export.
extern "C" PINVOKE_API int AddNumbers(int x, int y)
{
	return x + y;
}

// A method taking an array.
extern "C" PINVOKE_API int AddArray(int numbers[], int size)
{
	auto result = 0;
	for (int i = 0; i < size; i++)
	{
		result += numbers[i];
	}

	return result;
}
