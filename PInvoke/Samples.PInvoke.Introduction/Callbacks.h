#include "stdafx.h"

// Note that the first sample (CallMeBackToSayHello) uses __stdcall calling convention.
// The second sample shows how to use _cdecl.

// Function with a very simple callback
typedef void (__stdcall *SAYHELLOCALLBACK)();
extern "C" PINVOKE_API void CallMeBackToSayHello(SAYHELLOCALLBACK callback);

// Function with a callback receiving a structure
typedef struct
{
	double a;
	double b;
	double c;
} TRIANGLE;
typedef void (_cdecl *PYTHAGORASCALLBACK)(TRIANGLE result);
extern "C" PINVOKE_API void ReportPythagorasBack(double a, double b, PYTHAGORASCALLBACK callback);
