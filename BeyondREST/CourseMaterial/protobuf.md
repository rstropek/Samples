# Material for Protocol Buffers (aka *ProtoBuf*)

## Introduction

* Language-neutral, platform-neutral, extensible way of serializing structured data for use in communications protocols, data storage, and more.
* XML or JSON, but smaller
* Generate code for all relevant target languages
* [Language guide](https://developers.google.com/protocol-buffers/docs/proto3)

## *Proto* files

* Syntax versions *proto2* or *proto3* (here we use *proto3*)
* Naming conventions
  * Proto files: `lower_snake_case.proto`
  * *CamelCase* (with an initial capital) for message names and Enums
  * *underscore_separated_names* for field names
  * *CAPITALS_WITH_UNDERSCORES* for enum values
* Split large *proto* files, [import *proto* files](https://developers.google.com/protocol-buffers/docs/proto3#importing-definitions)
* Namespaces with [packages](https://developers.google.com/protocol-buffers/docs/proto3#packages)

## Language

* Field types:
  * [Scalar types](https://developers.google.com/protocol-buffers/docs/proto3#scalar)
  * [Enumerations](https://developers.google.com/protocol-buffers/docs/proto3#enum)
  * Other [nested message types](https://developers.google.com/protocol-buffers/docs/proto3#nested)
* Each field has unique number
  * Identify field in [binary format](https://developers.google.com/protocol-buffers/docs/encoding)
  * Use 1-15 for frequently used fields (1 byte for ID), > 15 for less common fields (>= 2 bytes for ID)
* *Singular* (one once in a message) or *repeated* fields (0..n times in a message)
* [`Any`](https://developers.google.com/protocol-buffers/docs/proto3#any) for embedding types without having their definition
* Fields can share memory with [`Oneof`](https://developers.google.com/protocol-buffers/docs/proto3#oneof)

## Versioning

* [*Reserve*](https://developers.google.com/protocol-buffers/docs/proto3#reserved) field ID if you remove a field
* [Compatibility rules](https://developers.google.com/protocol-buffers/docs/proto3#updating) for updating message definitions

## Links

* [Documentation](https://developers.google.com/protocol-buffers)
