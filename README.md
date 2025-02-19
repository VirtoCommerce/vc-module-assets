# Virto Commerce Assets Module
[![CI status](https://github.com/VirtoCommerce/vc-module-assets/workflows/Module%20CI/badge.svg?branch=dev)](https://github.com/VirtoCommerce/vc-module-assets/actions?query=workflow%3A"Module+CI") [![Quality gate](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-assets&metric=alert_status&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-assets) [![Reliability rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-assets&metric=reliability_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-assets) [![Security rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-assets&metric=security_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-assets) [![Sqale rating](https://sonarcloud.io/api/project_badges/measure?project=VirtoCommerce_vc-module-assets&metric=sqale_rating&branch=dev)](https://sonarcloud.io/dashboard?id=VirtoCommerce_vc-module-assets)

## Overview
The Virto Commerce Assets module is a base module that provides a powerful, flexible, and extensible platform for managing assets in Virto Commerce. 
With this module, you can easily search, organize, and retrieve assets stored in different locations, including file systems, Azure storage, and other custom asset storage solutions.

The module is designed to be highly configurable and extensible, allowing developers to easily add new asset storage providers or customize existing ones to meet the unique needs of their business. It defines a set of common abstractions for asset search, retrieval, and manipulation, making it easy for developers to work with assets regardless of their underlying storage location.

***Note:*** *You cannot work with the Platform files directly; the only way to do so is through the Assets abstraction.*

Virto Commerce has out-of-the box providers:
* [File System](https://github.com/VirtoCommerce/vc-module-filesystem-assets)
* [Azure Blob Storage](https://github.com/VirtoCommerce/vc-module-azureblob-assets)

## Key features

* Upload files
* Read files
* Search files

The Asset module also includes a user interface, which serves as both a File Manager and a File Dictionary. All Platform modules have access to this dictionary through the programming interface, and all folders in the Assets are created with Platform modules.
Each Platform module works with its own folder located in the Assets.


![Assets](docs/media/screen-assets.png)

## Uploading file to Assets

To upload a file, select a folder from the list of assets (e.g., Catalog) and then click the *Upload* button, as the screen capture below shows:

![Upload file](docs/media/screen-upload-file.png)

You can upload the file using one of the following ways:

1. Drag and drop the file
1. Browse the file
1. Enter an external URL for the file

***Note:*** *You cannot upload your file into the root.*

## Asset modules

The Assets module uses one of the following providers for assets management:

1. [vc-module-azureblob-assets](https://github.com/VirtoCommerce/vc-module-azureblob-assets): Provides Azure Blob Storage implementation.
2. [vc-module-filesystem-assets](https://github.com/VirtoCommerce/vc-module-filesystem-assets): Provides File System implementation.


To switch between the implementations, follow these steps:
1. Open **appsettings.json** for the Virto Commerce Platform instance.
2. Navigate to the **Assets** node:
```json
    "Assets": {
        "Provider": "FileSystem",
        "FileSystem": {
            "RootPath": "~/assets",
            "PublicUrl": "http://localhost:10645/assets/"
        },
        "AzureBlobStorage": {
            "ConnectionString": "",
            "CdnUrl": ""
        }
    }
```
3. Modify the following settings:
    - Set the **Provider** value to **FileSystem** or **AzureBlobStorage**
    - Provide **ConnectionString** in case you are going to use the **AzureBlobStorage** implementation option

## Documentation
* [Assets module user documentation](https://docs.virtocommerce.org/platform/user-guide/assets/overview/)
* [REST API](https://virtostart-demo-admin.govirto.com/docs/index.html?urls.primaryName=VirtoCommerce.Assets)
* [Assets management providers configuration](https://docs.virtocommerce.org/platform/developer-guide/Configuration-Reference/appsettingsjson/#assets)
* [View on GitHub](https://github.com/VirtoCommerce/vc-module-assets/)

## References

* [Deployment](https://docs.virtocommerce.org/platform/developer-guide/Tutorials-and-How-tos/Tutorials/deploy-module-from-source-code/)
* [Installation](https://docs.virtocommerce.org/platform/user-guide/modules-installation/)
* [Home](https://virtocommerce.com)
* [Community](https://www.virtocommerce.org)
* [Download latest release](https://github.com/VirtoCommerce/vc-module-assets/releases/latest)

## License

Copyright (c) Virto Solutions LTD.  All rights reserved.

Licensed under the Virto Commerce Open Software License (the "License"); you
may not use this file except in compliance with the License. You may
obtain a copy of the License at

http://virtocommerce.com/opensourcelicense

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
implied.

