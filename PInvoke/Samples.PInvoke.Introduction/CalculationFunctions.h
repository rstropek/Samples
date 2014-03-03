#include "stdafx.h"

// Some quite simple functions to begin with

// PInvoke "Hello World"
extern "C" PINVOKE_API int AddNumbers(int x, int y);

// Adds up all numbers in numbers[]
extern "C" PINVOKE_API int AddArray(int numbers[], int size);
