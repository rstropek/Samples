# Azure Private Endpoint Sample

## Introduction

This sample was originally created for [*Techorama 2021*](https://techorama.be/) conference. It demonstrates the use of various security concepts in order to build applications that can "survive" in the public cloud (i.e. live in a zero-trust environment):

* Infrastructure-as-Code with *Bicep*
* 100% PaaS and serverless services
* AAD-backed Managed Identities for Azure SQL Database and Azure Storage
* Network segmentation with Private Endpoints
* Managed Certificates for Azure App Service
* Application Insights for logging

## Usage

[*deploy.azcli*](deploy.azcli) contains an Azure CLI scrapbook with all the commands necessary to deploy the Bicep scripts.
