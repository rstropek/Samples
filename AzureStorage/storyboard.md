# Tooling

* Visual Studio Code
  * [Azure CLI Tools](https://marketplace.visualstudio.com/items?itemName=ms-vscode.azurecli)
  * [Azure Account](https://marketplace.visualstudio.com/items?itemName=ms-vscode.azure-account)
  * [ARM Tools](https://marketplace.visualstudio.com/items?itemName=msazurermtools.azurerm-vscode-tools)
* Visual Studio
  * *Azure Development* workload
* [Azure Storage Explorer](https://azure.microsoft.com/en-us/features/storage-explorer/)


# Documentation Starting Points

* Getting started: [Microsoft Cloud Adoption Framework](https://docs.microsoft.com/en-us/azure/cloud-adoption-framework/overview)
* Keeping up to date
  * [Azure updates](https://azure.microsoft.com/en-us/updates/)
  * [Azure blogs](https://azure.microsoft.com/en-us/blog/)
  * Follow important product managers on Twitter (e.g. [Scott Guthrie](https://twitter.com/scottgu))


# Azure Resource Manager

## Concepts

![ARM Concept](https://docs.microsoft.com/en-us/azure/azure-resource-manager/management/media/overview/consistent-management-layer.png)


![ARM Scope](https://docs.microsoft.com/en-us/azure/azure-resource-manager/management/media/overview/scope-levels.png)

* Carefully define naming concept upfront ([Microsoft's tips](https://docs.microsoft.com/en-us/azure/cloud-adoption-framework/ready/azure-best-practices/naming-and-tagging))
* List of all [Azure Resource Providers](https://docs.microsoft.com/en-us/azure/azure-resource-manager/management/azure-services-resource-providers)
* [ARM template reference](https://docs.microsoft.com/en-us/azure/templates/microsoft.resources/2019-10-01/resourcegroups) for resource groups

## Resource-based Access Control (RBAC)

![RBAC](https://docs.microsoft.com/en-us/azure/role-based-access-control/media/overview/rbac-security-principal.png)

![Role assignments](https://docs.microsoft.com/en-us/azure/role-based-access-control/media/overview/rbac-overview.png)

* [More about RBAC](https://docs.microsoft.com/en-us/azure/role-based-access-control/overview)

## ARM Templates

* Automate deployments
* Use the practice of *infrastructure as code*
* Declarative language using JSON
  * Powerful when done, hard to develop and debug
  * Feature-rich [function library](https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/template-functions)
* [*What if* feature](https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/template-deploy-what-if?tabs=azure-powershell) (currently in preview)
* [Docs](https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/overview)
* Often combined with or sometimes replaced by:
  * [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/what-is-azure-cli?view=azure-cli-latest)
  * [Azure PowerShell](https://docs.microsoft.com/en-us/powershell/azure/?view=azps-3.8.0)

> [Sample for creating resource group with ARM template](azuredeploy-resource-group.json)


# Azure Storage

## Types

* [Blobs](https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blobs-introduction)
  * Massively scalable object store for text and binary data
* [Files](https://docs.microsoft.com/en-us/azure/storage/files/storage-files-introduction)
  * SMB file shares as a service
  * Especially relevant for existing apps that rely on SMB
* [Queues](https://docs.microsoft.com/en-us/azure/storage/queues/storage-queues-introduction)
  * For really *large* queues
  * Consider [Service Bus](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-messaging-overview) as an alternative ([how to choose](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-azure-and-service-bus-queues-compared-contrasted))
* [Tables](https://docs.microsoft.com/en-us/azure/storage/tables/table-storage-overview)
  * Not very relevant anymore, consider [CosmosDB](https://docs.microsoft.com/en-us/azure/cosmos-db/table-introduction) instead
* [Disks](https://docs.microsoft.com/en-us/azure/virtual-machines/windows/managed-disks-overview)
  * Disks for VMs
  * Not covered here

## Blob Storage

![Blob Storage Structure](https://docs.microsoft.com/en-us/azure/storage/blobs/media/storage-blobs-introduction/blob1.png)

### Tools

* [Azure Storage Explorer](https://azure.microsoft.com/en-us/features/storage-explorer/)
* [Data movement tools](https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blobs-introduction#move-data-to-blob-storage)

### Security and Compliance Features

* **Limit anonymous access**, enable only if absolutely necessary (e.g. [static website hosting](https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blob-static-website))
* Choose proper [level of redundancy](https://docs.microsoft.com/en-us/azure/storage/common/storage-account-overview?toc=/azure/storage/blobs/toc.json#redundancy)
  * Recommendation for default level if no specific requirements are given: *[Zone](https://docs.microsoft.com/en-us/azure/storage/common/storage-account-overview?toc=/azure/storage/blobs/toc.json#redundancy)-redundant storage* ([docs](https://docs.microsoft.com/en-us/azure/storage/common/storage-redundancy?toc=/azure/storage/blobs/toc.json#zone-redundant-storage))
* [Immutability option](https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blob-immutable-storage)
* Always encrypted, [MS-managed or customer-managed keys](https://docs.microsoft.com/en-us/azure/storage/common/storage-service-encryption)
* [Secure transfer](https://docs.microsoft.com/en-us/azure/storage/common/storage-require-secure-transfer) (*HTTPS*) should be required
* [*Advanced threat protection*](https://docs.microsoft.com/en-us/azure/storage/common/storage-advanced-threat-protection?tabs=azure-portal) is available
* [*Soft delete*](https://docs.microsoft.com/en-us/azure/storage/blobs/soft-delete-overview) is available based on [blob snapshots](https://docs.microsoft.com/en-us/azure/storage/blobs/snapshots-overview) (alternative: [Blob versioning](https://docs.microsoft.com/en-us/azure/storage/blobs/versioning-overview?tabs=powershell), currently in preview)
* Limit access using [*shared access signatures*](https://docs.microsoft.com/en-us/azure/storage/common/storage-sas-overview)
* Use Azure AD to secure access to blob storage (in particular [managed identities](https://docs.microsoft.com/en-us/azure/active-directory/managed-identities-azure-resources/overview) are useful for that)
  * If not possible, store access secrets in [key vault](https://docs.microsoft.com/en-us/azure/key-vault/general/overview) or at least protect them properly (e.g. ASP.NET Core [Data Protection](https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/introduction?view=aspnetcore-3.1))
* Limit access using [IP firewall rules](https://docs.microsoft.com/en-us/azure/storage/common/storage-network-security#grant-access-from-an-internet-ip-range)
* Use [*private endpoints*](https://docs.microsoft.com/en-us/azure/private-link/create-private-endpoint-storage-portal) to limit access to certain virtual networks
* [Enable logging](https://docs.microsoft.com/en-us/azure/storage/common/storage-analytics-logging?tabs=dotnet) and/or [monitoring](https://docs.microsoft.com/en-us/azure/storage/common/monitor-storage) for detailed access tracking

### Storage Account

* *http://<storage-account-name>.blob.core.windows.net*
  * Be careful with naming concept! Storage account names must be globally unique and can *only contain numbers and lowercase letters* (e.g. no dashes or underscores; historical reasons)
* Recommended [type](https://docs.microsoft.com/en-us/azure/storage/common/storage-account-overview?toc=/azure/storage/blobs/toc.json#types-of-storage-accounts): *General-purpose v2*
* [*Hot*, *cool*, *archive*](https://docs.microsoft.com/en-us/azure/storage/common/storage-account-overview?toc=/azure/storage/blobs/toc.json#access-tiers-for-block-blob-data)
  * Keep [costs](https://azure.microsoft.com/en-us/pricing/details/storage/blobs/) in mind
  * Avoid archive if not absolutely necessary
  * Consider setting up [storage lifecycle](https://docs.microsoft.com/en-us/azure/storage/blobs/storage-lifecycle-management-concepts?tabs=azure-portal)
* [*Standard* or *Premium* performance tiers](https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blob-performance-tiers)
* ARM References
  * [Storage accounts](https://docs.microsoft.com/en-us/azure/templates/microsoft.storage/2019-06-01/storageaccounts)
  * [Blob storage](https://docs.microsoft.com/en-us/azure/templates/microsoft.storage/2019-06-01/storageaccounts/blobservices)
  * [Containers](https://docs.microsoft.com/en-us/azure/templates/microsoft.storage/2019-06-01/storageaccounts/blobservices/containers)

> [Sample for creating storage account with ARM template](azuredeploy-storage.json)

### Concurrency

* [*Optimistic Concurrency*](https://docs.microsoft.com/en-us/azure/storage/common/storage-concurrency?toc=/azure/storage/blobs/toc.json#optimistic-concurrency-for-blobs-and-containers)
  * Based on *HTTP ETag* and *If-Match* headers
* [*Pessimistic Concurrency*](https://docs.microsoft.com/en-us/azure/storage/common/storage-concurrency?toc=/azure/storage/blobs/toc.json#pessimistic-concurrency-for-blobs)
  * Based on [Blob *Leases*](https://docs.microsoft.com/en-us/rest/api/storageservices/Lease-Blob)

### Performance Considerations

* *Premium* performance only available for *BlockBlobStorage*, limited for *General-purpose V2* ([details](https://docs.microsoft.com/en-us/azure/storage/common/storage-account-overview#types-of-storage-accounts))
* [Batching](https://docs.microsoft.com/en-us/dotnet/api/azure.storage.blobs.specialized.blobbatchclient)
  * [REST reference](https://docs.microsoft.com/en-us/rest/api/storageservices/blob-batch)
* [Append blobs](https://docs.microsoft.com/en-us/rest/api/storageservices/understanding-block-blobs--append-blobs--and-page-blobs#about-append-blobs)
* [Copy blob from URL](https://docs.microsoft.com/en-us/rest/api/storageservices/copy-blob-from-url) to move/copy existing blobs
* [Build blobs incrementally](https://docs.microsoft.com/en-us/rest/api/storageservices/put-block-list)
* Use [Event Grid integration](https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blob-event-overview) to prevent polling

### Samples

* [.NET Storage Client Library](https://docs.microsoft.com/en-us/dotnet/api/overview/azure/storage?view=azure-dotnet)
* Code Samples see *How to* section in [blob storage docs](https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blobs-introduction)
* Samples on GitHub
  * [Upload and download of blobs](https://github.com/Azure/azure-sdk-for-net/blob/master/sdk/storage/Azure.Storage.Blobs/samples/Sample01b_HelloWorldAsync.cs)
  * [Authentication](https://github.com/Azure/azure-sdk-for-net/blob/master/sdk/storage/Azure.Storage.Blobs/samples/Sample02_Auth.cs)
* [Tests on GitHub](https://github.com/Azure/azure-sdk-for-net/tree/master/sdk/storage/Azure.Storage.Blobs/tests)

### Upcoming Features (Preview)

* [Blob versioning](https://docs.microsoft.com/en-us/azure/storage/blobs/versioning-overview?tabs=powershell)
* [Change feeds](https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blob-change-feed?tabs=azure-portal)
* [Point-in-time restore](https://docs.microsoft.com/en-us/azure/storage/blobs/point-in-time-restore-overview)
* [Blob index tags](https://docs.microsoft.com/en-us/azure/storage/blobs/storage-manage-find-blobs)


# Azure Messaging and Eventing

## Comparison

* [Service Bus Messaging](https://docs.microsoft.com/en-us/azure/service-bus-messaging/)
* [Event Grid](https://docs.microsoft.com/en-us/azure/event-grid/)
* [Event Hub](https://docs.microsoft.com/en-us/azure/event-hubs/) (not covered here)
* [How to choose](https://docs.microsoft.com/en-us/azure/event-grid/compare-messaging-services#comparison-of-services)

## Azure Service Bus

![Queues](https://docs.microsoft.com/en-us/azure/service-bus-messaging/media/service-bus-messaging-overview/about-service-bus-queue.png)
![Topics and Subscriptions](https://docs.microsoft.com/en-us/azure/service-bus-messaging/media/service-bus-messaging-overview/about-service-bus-topic.png)

* Perfectly suited for high-value business transactions
* Stable service, no major new features announced (see also [team blog](https://azure.microsoft.com/en-us/blog/tag/azure-service-bus/))
* [Supports](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-amqp-overview) the [*Advanced Message Queuing Protocol (AMQP)*](https://en.wikipedia.org/wiki/Advanced_Message_Queuing_Protocol)
  * ISO/IEC Standard
  * Possible to reduce lock-in effect (e.g. [RabbitMQ with Plugin](https://github.com/rabbitmq/rabbitmq-amqp1.0/blob/v3.7.x/README.md))
* [Many advanced features](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-messaging-overview#advanced-features)
* Recommendation: Use *Premium* tier for production environments ([docs](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-premium-messaging))
  * Predictable performance
  * Larger messages
  * Keep [pricing](https://azure.microsoft.com/en-us/pricing/details/service-bus/) in mind
* ARM References
  * [Azure Service Bus Namespaces](https://docs.microsoft.com/en-us/azure/templates/microsoft.servicebus/2017-04-01/namespaces)

> [Sample for creating resource group with ARM template](azuredeploy-resource-group.json)

### Security Features

* Limit access using [*shared access signatures*](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-sas)
* Use Azure AD to secure access to Service Bus (in particular [managed identities](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-managed-service-identity) are useful for that)
  * If not possible, store access secrets in [key vault](https://docs.microsoft.com/en-us/azure/key-vault/general/overview) or at least protect them properly (e.g. ASP.NET Core [Data Protection](https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/introduction?view=aspnetcore-3.1))
* Limit access using [IP firewall rules](https://docs.microsoft.com/en-us/azure/service-bus-messaging/network-security#ip-firewall)
* Use [*private endpoints*](https://docs.microsoft.com/en-us/azure/service-bus-messaging/network-security#private-endpoints) to limit access to certain virtual networks
* Uses Azure Storage in the background, therefore always encrypted, MS-managed or [customer-managed keys](https://docs.microsoft.com/en-us/azure/service-bus-messaging/configure-customer-managed-key#enable-customer-managed-keys)

### Programming with Service Bus

* Collection of [.NET Samples](https://github.com/Azure/azure-service-bus/tree/master/samples/DotNet/Microsoft.ServiceBus.Messaging) on GitHub
* [`Message`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.servicebus.message) consisting of...
  * `Body`
  * Properties (`SystemProperties` and `UserProperties`)
* [Receive modes](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-queues-topics-subscriptions#receive-modes)
  * Receive and Delete
  * Peek Lock (consider checking [`DeliveryCount`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.servicebus.messaging.brokeredmessage.deliverycount) or setting [`MaxDeliveryCount`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.servicebus.messaging.queuedescription.maxdeliverycount) for poisoned message handling)
* [Filtered topics](https://docs.microsoft.com/en-us/azure/service-bus-messaging/topic-filters)
* [Message correlation](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-messages-payloads#message-routing-and-correlation)
  * Request-Reply-Pattern using [`ReplyTo`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.servicebus.message.replyto)
* Payload serialization (see also [`ContentType`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.servicebus.message.contenttype)); e.g.:
  * JSON
  * ProtoBuf
* Message [sequencing](https://docs.microsoft.com/en-us/azure/service-bus-messaging/message-sequencing) and [lifetime control](https://docs.microsoft.com/en-us/azure/service-bus-messaging/message-expiration)
  * [Dead-letter queue](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-dead-letter-queues)
  * [Sessions](https://docs.microsoft.com/en-us/azure/service-bus-messaging/message-sessions#first-in-first-out-fifo-pattern)
* Send events via Event Grid when messages are available ([docs](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-to-event-grid-integration-concept)) or messages are delivered to dead-letter queue
  * Prevents polling
* Consider [monitoring with Application Insights](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-end-to-end-tracing#tracking-with-azure-application-insights)
* Consider [Azure Serverless Functions](https://docs.microsoft.com/en-us/azure/azure-functions/) for message-driven applications

### Performance Considerations

* Use AMQP protocol if possible ([docs](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-performance-improvements?tabs=net-standard-sdk#protocols))
* Reuse Service Bus client objects (e.g. queue, topic, and subscription clients), use DI for that
* Enable [prefetching](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-performance-improvements?tabs=net-standard-sdk#prefetching)


# Azure Event Grid

## Concepts

![Event Grid](https://docs.microsoft.com/en-us/azure/event-grid/media/overview/functional-model.png)

* Built-in [Event Sources](https://docs.microsoft.com/en-us/azure/event-grid/overview#event-sources) (*Topics*)
  * [Storage integration](https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blob-event-overview)
  * [Service Bus integration](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-to-event-grid-integration-concept#events-and-event-schemas)
  * List of [system topics](https://docs.microsoft.com/en-us/azure/event-grid/system-topics)
* Built-in [Event Handlers](https://docs.microsoft.com/en-us/azure/event-grid/overview#event-handlers)
  * Note: [Azure Relay Hybrid Connections](https://docs.microsoft.com/en-us/azure/azure-relay/relay-what-is-it#hybrid-connections) is a built-in target, useful for integrating Event Grid with on-premise resources
* Custom [sources (topics)](https://docs.microsoft.com/en-us/azure/event-grid/custom-topics) and handlers
  * [Event domains](https://docs.microsoft.com/en-us/azure/event-grid/event-domains) for large-scale deployments
  * [Handline events in an HTTP endpoint](https://docs.microsoft.com/en-us/azure/event-grid/receive-events)
* Useful for learning: [*Event Grid Viewer* Sample](https://github.com/Azure-Samples/azure-event-grid-viewer)
  * For this training deployed at [https://storage-demo-event-grid-viewer.azurewebsites.net/](https://storage-demo-event-grid-viewer.azurewebsites.net/)
* Build hybrid solutions based on [Azure Relay](https://docs.microsoft.com/en-us/azure/azure-relay/relay-what-is-it)
  * [Docs](https://docs.microsoft.com/en-us/azure/event-grid/custom-event-to-hybrid-connection)
* Support for [CloudEvents](https://cloudevents.io/) specification is [in preview](https://docs.microsoft.com/en-us/azure/event-grid/cloudevents-schema)
  * Potential to reduce lock-in effect (see [contributors](https://github.com/cloudevents/spec/blob/master/community/contributors.md))

### Security Features

* Limit access using [*shared access signatures*](https://docs.microsoft.com/en-us/azure/event-grid/security-authentication#authenticate-publishing-clients-using-sas-or-key)
* Use Azure AD to [secure Event Grid webhook endpoint](https://docs.microsoft.com/en-us/azure/event-grid/security-authentication#authenticate-event-delivery-to-webhook-endpoints)
* Store access secrets in [key vault](https://docs.microsoft.com/en-us/azure/key-vault/general/overview) or at least protect them properly (e.g. ASP.NET Core [Data Protection](https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/introduction?view=aspnetcore-3.1))
* Use [RBAC to control access to Event Grid](https://docs.microsoft.com/en-us/azure/event-grid/security-authorization)
* Limit possitibility to publish events using [IP firewall rules](https://docs.microsoft.com/en-us/azure/event-grid/network-security#ip-firewall)
* Use [*private endpoints*](https://docs.microsoft.com/en-us/azure/event-grid/network-security#private-endpoints) to keep events inside your virtual network

### Programming with Event Grid

* Tip: [*ngrok*](https://ngrok.com/) is a useful tool during development and testing
* [Azure Event Grid event schema](https://docs.microsoft.com/en-us/azure/event-grid/event-schema)
  * General schema with metadata
  * Source-specific data (e.g. [storage](https://docs.microsoft.com/en-us/azure/event-grid/event-schema-blob-storage))
* Implement [endpoint validation](https://docs.microsoft.com/en-us/azure/event-grid/receive-events#endpoint-validation)
* [Event filtering](https://docs.microsoft.com/en-us/azure/event-grid/event-filtering)
* Manage events that could not be delivered using [Dead-lettering](https://docs.microsoft.com/en-us/azure/event-grid/manage-event-delivery)
  * Note [retry schedule](https://docs.microsoft.com/en-us/azure/event-grid/delivery-and-retry#retry-schedule-and-duration)


# Multi-Tenant Data Storage

* Design a system so that each tenant’s data is isolated from one another
* Divide storage into compartments so one tenant breach cannot flow into another tenant
* No service should have access to all tenant data
* Always keep general [service limits, quotas, and constraints](https://docs.microsoft.com/en-us/azure/azure-resource-manager/management/azure-subscription-service-limits) in mind.

## Relational

* [SQL Database elastic pools](https://docs.microsoft.com/en-us/azure/sql-database/sql-database-elastic-pool)
  * [Pricing](https://azure.microsoft.com/en-us/pricing/details/sql-database/elastic/)
  * One DB (cluster) for each tenant
* [Azure Database for PostgresSQL - Hyperscale](https://docs.microsoft.com/en-us/azure/postgresql/overview#azure-database-for-postgresql---hyperscale-citus)
  * [Sharding](https://docs.microsoft.com/en-us/azure/postgresql/concepts-hyperscale-choose-distribution-column#multi-tenant-apps)

## NoSQL

* Azure Storage
  * *Storage account* per tenant
  * *Container* per tenant
* Azure CosmosDB
  * [Container](https://docs.microsoft.com/en-us/azure/cosmos-db/databases-containers-items#azure-cosmos-containers) per tenant
  * Tenant as partition key


# Hybrid Solutions

## Potential Services

* Azure Stack
* [Azure Virtual Network (VPN) Gateway](https://docs.microsoft.com/en-us/azure/vpn-gateway/vpn-gateway-about-vpngateways)
  * In combination with [Azure Networking](https://docs.microsoft.com/en-us/azure/networking/networking-overview)
  * [Azure Private Links/Endpoints](https://docs.microsoft.com/en-us/azure/private-link/private-link-overview) can be useful
* [Azure Service Bus](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-messaging-overview)
* [Azure Relay Hybrid Connections](https://docs.microsoft.com/en-us/azure/azure-relay/relay-what-is-it#hybrid-connections)
  * WebSockets with rendezvous point in the cloud
* [Azure App Service Hybrid Connections](https://docs.microsoft.com/en-us/azure/app-service/app-service-hybrid-connections)

## Azure Networking

* [Services overview](https://docs.microsoft.com/en-us/azure/networking/networking-overview#connect)
* On-premise integration with [Azure VPN Gateway](https://docs.microsoft.com/en-us/azure/vpn-gateway/vpn-gateway-about-vpngateways) and potentially [Azure ExpressRoute](https://docs.microsoft.com/en-us/azure/expressroute/expressroute-introduction)
* Discuss [*Private Endpoint* sample](https://github.com/rstropek/Samples/tree/master/AzurePrivateEndpoints)
