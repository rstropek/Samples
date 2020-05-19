# Fitness Club Case Study

## Introduction

You work in software development at *Quality Fitness Inc.*. The company has **300 clubs in 8 different countries** in Europe. The largest presence is in France with 50 clubs.

*Quality Fitness* offers **12 different instructor-led fitness classes in each club** (e.g. spinning, yoga, stretching, etc.). On average, **each fitness club has 500 paying members** from which approximately **one third is regularly active in classes**.

In the past, reserving spots in classes upfront was not possible. You had to pick up course ticket in person at the reception desk a few minutes before the course. Because of Covid-19 and the limited space in training rooms, the **number of participants in classes have to be limited to on average 7 people per class**. *Quality Fitness* expects high demand for the limited number of spots. Therefore, a reservation system has to be implemented.

*Quality Fitness* already has a mobile-friendly website that includes a member area. Therefore, you do not have to consider building member management and authentication in this project. Existing mechanisms can be used.

## Functional Requirements

* Each paying member can participate in **max. 5 classes per week**.

* Starting on each **Thursday 6:00pm**, members can **reserve spots for classes for the upcoming calendar week** via *Quality Fitness*'s website. Thus, we expect a burst in traffic on Thursday evenings.

* On the website, members see an **overview over all courses of the week**. Each course is marked with a color, indicating whether
  * enough spots are avaiable (> ~3 spots available; green),
  * spots are getting rare (<= ~3 spots available; yellow),
  * or the class is fully booked (no spots available; red).

* Members can **request a spot** in a course by clicking on it.

* Once the system has processed a member's request for a reserved spot in a class, the member gets a **confirmation email** including
  * a QR code image (includes data about the member and the booked class)
  * and a link to a printable web page (includes QR code image and textual information about member and booked class).

* In the club, an employee **scans the QR code** on the member's phone (QR code in email) or printout to verify that the member has a reserved spot.

## Non-Functional Requirements

* *Quality Fitness* has decided to use **Microsoft Azure**.

* Operational costs per club must not exceed **25€/month**.

* *Quality Fitness* has no server administrators. Therefore, setting up and maintaining **VMs is not an option**.

* The system has to offer **decent performance**, even during traffic bursts:
  * Average response time on website on average <= 3 seconds
  * Average time between reserving a spot on the website and sending out confirmation email <= 1 minute

* *Quality Fitness* assumes that the solution will only be used until mid of 2021. Until then, the Covid-19 limitations should be obsolete and the class booking solution will no longer be needed. Therefore, **development costs should be low**. The software does not need to be functionally rich. It should just cover the features requirements mentioned above.

## Your Task

* Try to come up with a **solution architecture** for this system in Microsoft Azure.

* **Focus** in particular **on** using Azure components we discussed yesterday (**Azure Storage, Azure Service Bus**).
  * Where and how would you use them?
  * Which important API functions ([API reference](https://docs.microsoft.com/en-us/dotnet/api/overview/azure/service-bus)) would you use?
  * Make sure that your architecture considers Azure's [limits, quotas, and constraints](https://docs.microsoft.com/en-us/azure/azure-resource-manager/management/azure-subscription-service-limits).

* Try to do a **rough cost** estimation of your Azure solution.
