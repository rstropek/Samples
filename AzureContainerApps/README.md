# Azure Container Apps Demo with Infrastructure as Code

This project demonstrates how to deploy a Next.js application to Azure Container Apps using Infrastructure as Code (IaC) with Bicep templates. The sample showcases best practices for deploying containerized applications on Azure with proper security, monitoring, and scalability considerations.

## Project Overview

The demo includes:
- **Application**: A simple Next.js calculator app containerized with Docker
- **Infrastructure**: Complete Azure infrastructure defined using Bicep templates
- **Security**: Managed Identity for secure access to Azure Container Registry
- **Monitoring**: Log Analytics workspace for application and infrastructure monitoring
- **CI/CD Ready**: Structure suitable for automated deployment pipelines

## Infrastructure Components

### Core Infrastructure (`central-infra.bicep`)
- **Azure Container Registry (ACR)**: Stores container images
- **Log Analytics Workspace**: Centralized logging and monitoring
- **Container Apps Environment**: Hosting environment for container apps

### Security (`managed-identity.bicep`)
- **User-Assigned Managed Identity**: Provides secure access to ACR without storing credentials

### Application Infrastructure (`project-infra.bicep`)
- **Container App**: Hosts the Next.js application with auto-scaling capabilities

## Prerequisites

Before deploying this sample, ensure you have:

1. **Azure CLI** installed and configured
2. **Azure Resource Group** created
3. **Required permissions**

## Deployment Instructions

### 1. Clone and Navigate to Project
```bash
git clone <repository-url>
cd AzureContainerApps
```

### 2. Configure Deployment Variables

Edit the variables in `deploy.azcli`:

```bash
RG=container-apps-demo           # Your resource group name
LOCATION=westeurope              # Your preferred Azure region
PROJECT_NAME=container-apps-demo # Project identifier
STAGE=dev                        # Environment stage
```

### 3. Deploy Infrastructure

Or execute step by step (see [deploy.azcli](deploy.azcli)):

* Deploy Managed Identity
* Deploy Central Infrastructure
* Build and Push Container Image
* Deploy Application

### 4. Access Your Application

After deployment, find your application URL (e.g. using portal) and access it.
