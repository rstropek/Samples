#include "stdafx.h"

// Note that all callbacks are defined with CALLBACK here. This means they used
// __stdcall calling convention.

// Function with a very simple callback
typedef void (CALLBACK *SAYHELLOCALLBACK)();
extern "C" PINVOKE_API void CallMeBackToSayHello(SAYHELLOCALLBACK callback);

// Function with a callback receiving a structure
typedef struct
{
	double a;
	double b;
	double c;
} TRIANGLE;
typedef void (CALLBACK *PYTHAGORASCALLBACK)(TRIANGLE result);
extern "C" PINVOKE_API void ReportPythagorasBack(double a, double b, PYTHAGORASCALLBACK callback);
