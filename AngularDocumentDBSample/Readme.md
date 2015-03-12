# Simple AngularJS Sample with Azure DocumentDB

Author: Rainer Stropek

Blog: http://www.software-architects.com

## Content of the Sample

For my private hobby (keeping honey bees) I neede a small website where people
can register. I decided to implement it using the following technologies:

* HTML/JavaScript frontend with [AngularJS](https://angularjs.org/)
* [Google reCAPTCHA](https://www.google.com/recaptcha/intro/index.html) for making sure that only humans use the form
* JavaScript backend with [Node.js](https://nodejs.org/)
* [Azure Websites](http://azure.microsoft.com/en-us/services/websites/) for hosting the website
* [Azure DocumentDB](http://azure.microsoft.com/en-us/services/documentdb/) for storing registrations
* [Mandrill](https://mandrill.com/) for sending transactional emails

## The Code

[server.js](server.js) contains the code for the backend. If you are interested
in using Azure DocumentDB and Mandrill from Node.js, you might want to look at
it. Additionally, it contains the server-side checking of the reCAPTCHA.

[index.html](site/index.html) contains the frontend. It uses a quite simple
AngularJS form with some data binding. If you are interested in how to use
reCAPTCHA, you will find the corresponding code there.