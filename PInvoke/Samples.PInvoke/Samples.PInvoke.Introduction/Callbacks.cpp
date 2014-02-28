#include "stdafx.h"
#include "Callbacks.h"

// Function receiving a very simple callback
extern "C" PINVOKE_API void CallMeBackToSayHello(SAYHELLOCALLBACK callback)
{
	std::cout << "About to call back\n";
	(*callback)();
	std::cout << "Called back\n";
}

// Function with a more complex callback
extern "C" PINVOKE_API void ReportPythagorasBack(double a, double b, PYTHAGORASCALLBACK callback)
{
	// Calculate result
	TRIANGLE triangle;
	triangle.a = a;
	triangle.b = b;
	triangle.c = sqrt(a * a + b * b);

	callback(triangle);
}
