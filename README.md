# com-danliris-purchasing
[![codecov](https://codecov.io/gh/danliris/com-danliris-service-purchasing/branch/dev/graph/badge.svg)](https://codecov.io/gh/danliris/com-danliris-service-purchasing) [![Build Status](https://travis-ci.org/danliris/com-danliris-service-purchasing.svg?branch=dev)](https://travis-ci.org/danliris/com-danliris-service-purchasing) [![Maintainability](https://api.codeclimate.com/v1/badges/bb2eab66433d41c445f8/maintainability)](https://codeclimate.com/github/danliris/com-danliris-service-purchasing/maintainability)



DanLiris Application is a enterprise project that aims to manage the business processes of a textile factory, PT. DanLiris.
This application is a microservices application consisting of services based on .NET Core and Aurelia Js which part of  NodeJS Frontend Framework. This application show how to implement microservice architecture principles. com-danliris-service-purchasing repository is part of service that will serve purchasing bussiness activity.

## Prerequisites
* Windows, Mac or Linux
* [Visual Studio Code](https://code.visualstudio.com/) or [Visual Studio](https://visualstudio.microsoft.com/vs/whatsnew/)
* [IIS Web Server](https://www.iis.net/) 
* [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
* [.NET Core SDK](https://www.microsoft.com/net/download/core#/current) (v2.0.9,  SDK 2.1.202, ASP.NET Core Runtime 2.0.9 )


## Getting Started

- Fork the repository and then clone the repository using command  `git clone https://github/YOUR-USERNAME/com-danliris-service-purchasing.git`  checkout the `dev` branch.


### Command Line

- Install the latest version of the .NET Core SDK from this page <https://www.microsoft.com/net/download/core>
- Next, navigate to root project or wherever your folder is on the command line in administrator mode.
- Create empty database.
- Setting connection to database using Connection Strings in appsettings.json. Your appsettings.json look like this:

```
{
  "Logging": {
    "IncludeScopes": false,
    "Debug": {
      "LogLevel": {
        "Default": "Warning"
      }
    },
    "Console": {
      "LogLevel": {
        "Default": "Warning"
      }
    }
  },

  "ConnectionStrings": {
    "DefaultConnection": "Server=your_db_server;Database=your_parent_database;Trusted_Connection=True;MultipleActiveResultSets=true",
  },
  "ClientId": "your ClientId",
  "Secret": "Your Secret",
  "ASPNETCORE_ENVIRONMENT": "Development"
}
```
and  Your appsettings.Developtment.json look like this :
```
{
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    }
  }
}
```
- Make sure port application has no conflict, setting port application in launchSettings.json
```
com-danliris-service-purchasing
 ┣ Com.DanLiris.Service.Purchasing.WebApi
    ┗ Properties
       ┗ launchSettings.json
```

file launchSettings.json look like this :
```
{
  "iisSettings": {
    "windowsAuthentication": false,
    "anonymousAuthentication": true,
    "iisExpress": {
      "applicationUrl": "http://localhost:17452/",
      "sslPort": 0
    }
  },
  "profiles": {
    "IIS Express": {
      "commandName": "IISExpress",
      "launchBrowser": true,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "Com.DanLiris.Service.Purchasing.WebApi": {
      "commandName": "Project",
      "launchBrowser": true,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      "applicationUrl": "http://localhost:5000"
    }
  }
}
```
- Call `dotnet run`.
- Then open the `http://localhost:22160/swagger/index.html` URL in your browser.

### Visual Studio

- Download Visual Studio 2019 (any edition) from https://www.visualstudio.com/downloads/ .
- Open `Com.DanLiris.Service.Purchasing.sln` and wait for Visual Studio to restore all Nuget packages.
- Create empty database.
- Setting connection to database using ConnectionStrings in appsettings.json and appsettings.Developtment.json.
- Make sure port application has no conflict, setting port application in launchSettings.json.
```
com-danliris-service-purchasing
 ┣ Com.DanLiris.Service.Purchasing.WebApi
    ┗ Properties
       ┗ launchSettings.json
```
- Ensure `Com.DanLiris.Service.Purchasing.WebApi` is the startup project and run it and the browser will launched in new tab http://localhost:22160/swagger/index.html


### Run Unit Tests in Visual Studio 
1. You can run all test suite, specific test suite or specific test case on test explorer.
2. Choose Tab Menu **Test** to select differnt menu test.
3. Select **Run All Test** or press (Ctrl + R, A ) to run all test suite.
4. Select **Test Explorer** or press (Ctrl + E, T ) to determine  test suite to run specifically.
5. Select **Analyze Code Coverage For All Test** to generate code coverage. 


## Knows More Details
### Root directory and description

```
com-danliris-service-purchasing
 ┣ Com.DanLiris.Service.Purchasing.Lib
 ┣ Com.DanLiris.Service.Purchasing.Test
 ┣ Com.DanLiris.Service.Purchasing.WebApi
 ┣ TestResults
 ┣ .dockerignore
 ┣ .gitignore
 ┣ .travis.yml
 ┣ codecov.yml
 ┣ Com.DanLiris.Service.Purchasing.sln
 ┣ docker-compose.test.yml
 ┣ Dockerfile.test
 ┣ Dockerfile.test.build
 ┣ Dockerfile.test.build.sh
 ┗ README.md
 ```

**1. Com.DanLiris.Service.Purchasing.Lib**

This folder consists of various libraries, domain Models, View Models, and Business Logic.The Model and View Models represents the data structure. Business Logic has responsibility  to organize, prepare, manipulate, and organize data. The tasks are include entering data into databases, updating data, deleting data, and so on. The model carries out its work based on instructions from the controller.


AutoMapperProfiles:

- Colecction class to setup mapping data 

Facades

- This contains class that implement Facades design pattern. This patern hides the complexities of the system and provides an interface to the client using which the client can access the system. This type of design pattern comes under structural pattern as this pattern adds an interface to existing system to hide its complexities.Classes that implement Facades have responsibility to prepare, manipulate, and organize data, including CRUD (Create, Read, Update, Delete ) on database.

Models:

- The Model is a collection of objects that Representation of data structure which hold the application data and it may contain the associated business logic.

ViewModels

- The View Model refers to the objects which hold the data that needs to be shown to the user.The View Model is related to the presentation layer of our application. They are defined based on how the data is presented to the user rather than how they are stored.

Configs

- Classes to setup entity model  that will be used in EF framework to generate schema database.

Migrations

- Collection of classes that generated by EF framework  to setup database and the tables.

Serializers

- This folders contains class to do serialization and deserialization. Serialization is the process of mapping an object to a BSON (Binary JSON) document that can be saved in MongoDB, and deserialization is the reverse process of reconstructing an object from a BSON (Binary JSON) document. For that reason the serialization process is also often referred to as “Object Mapping.”

Services

- Collection of classes and interfaces to validation and authentication user.

PDFTemplates

- Collection of classes to generate report in pdf format.

Utilities

- Collection of classes that frequently used as utility and  helper classes that frequently used in various cases.


The folder tree in this folder is:

```
com-danliris-service-purchasing
 ┣ Com.DanLiris.Service.Purchasing.Lib
 ┃ ┣ AutoMapperProfiles
 ┃ ┣ bin
 ┃ ┃ ┗ Debug
 ┃ ┃ ┃ ┗ netcoreapp2.0
 ┃ ┣ Configs
 ┃ ┃ ┗ Expedition
 ┃ ┣ Enums
 ┃ ┣ Facades
 ┃ ┃ ┣ BankExpenditureNoteFacades
 ┃ ┃ ┣ Expedition
 ┃ ┃ ┣ ExternalPurchaseOrderFacade
 ┃ ┃ ┃ ┣ Reports
 ┃ ┃ ┣ GarmentBeacukaiFacade
 ┃ ┃ ┣ GarmentCorrectionNoteFacades
 ┃ ┃ ┣ GarmentDailyPurchasingReportFacades
 ┃ ┃ ┣ GarmentDeliveryOrderFacades
 ┃ ┃ ┣ GarmentExternalPurchaseOrderFacades
 ┃ ┃ ┃ ┣ Reports
 ┃ ┃ ┣ GarmentInternalPurchaseOrderFacades
 ┃ ┃ ┣ GarmentInternNoteFacades
 ┃ ┃ ┣ GarmentInvoiceFacades
 ┃ ┃ ┣ GarmentPOMasterDistributionFacades
 ┃ ┃ ┣ GarmentPurchaseRequestFacades
 ┃ ┃ ┣ GarmentReceiptCorrectionFacades
 ┃ ┃ ┣ GarmentReports
 ┃ ┃ ┣ GarmentSupplierBalanceDebtFacades
 ┃ ┃ ┣ GarmentUnitDeliveryOrderFacades
 ┃ ┃ ┣ GarmentUnitDeliveryOrderReturFacades
 ┃ ┃ ┣ GarmentUnitExpenditureNoteFacade
 ┃ ┃ ┣ GarmentUnitReceiptNoteFacades
 ┃ ┃ ┣ InternalPO
 ┃ ┃ ┣ MonitoringCentralBillExpenditureFacades
 ┃ ┃ ┣ MonitoringCentralBillReceptionFacades
 ┃ ┃ ┣ MonitoringCorrectionNoteExpenditureFacades
 ┃ ┃ ┣ MonitoringCorrectionNoteReceptionFacades
 ┃ ┃ ┣ MonitoringUnitReceiptFacades
 ┃ ┃ ┣ PurchaseRequestFacades
 ┃ ┃ ┣ PurchasingDispositionFacades
 ┃ ┃ ┣ Report
 ┃ ┃ ┣ UnitPaymentCorrectionNoteFacade
 ┃ ┃ ┣ UnitReceiptNoteFacade
 ┃ ┣ Helpers
 ┃ ┣ Interfaces
 ┃ ┣ Migrations
 ┃ ┣ Models
 ┃ ┃ ┣ BankDocumentNumber
 ┃ ┃ ┣ BankExpenditureNoteModel
 ┃ ┃ ┣ DeliveryOrderModel
 ┃ ┃ ┣ Expedition
 ┃ ┃ ┣ ExternalPurchaseOrderModel
 ┃ ┃ ┣ GarmentBeacukaiModel
 ┃ ┃ ┣ GarmentCorrectionNoteModel
 ┃ ┃ ┣ GarmentDeliveryOrderModel
 ┃ ┃ ┣ GarmentExternalPurchaseOrderModel
 ┃ ┃ ┣ GarmentInternalPurchaseOrderModel
 ┃ ┃ ┣ GarmentInternNoteModel
 ┃ ┃ ┣ GarmentInventoryModel
 ┃ ┃ ┣ GarmentInvoiceModel
 ┃ ┃ ┣ GarmentPOMasterDistributionModels
 ┃ ┃ ┣ GarmentPurchaseRequestModel
 ┃ ┃ ┣ GarmentReceiptCorrectionModel
 ┃ ┃ ┣ GarmentSupplierBalanceDebtModel
 ┃ ┃ ┣ GarmentUnitDeliveryOrder
 ┃ ┃ ┣ GarmentUnitExpenditureNoteModel
 ┃ ┃ ┣ GarmentUnitReceiptNoteModel
 ┃ ┃ ┣ InternalPurchaseOrderModel
 ┃ ┃ ┣ PurchaseRequestModel
 ┃ ┃ ┣ PurchasingDispositionModel
 ┃ ┃ ┣ UnitPaymentCorrectionNoteModel
 ┃ ┃ ┣ UnitPaymentOrderModel
 ┃ ┃ ┗ UnitReceiptNoteModel
 ┃ ┣ obj
 ┃ ┃ ┣ Debug
 ┃ ┃ ┃ ┗ netcoreapp2.0
 ┃ ┣ PDFTemplates
 ┃ ┃ ┣ GarmentCorrectionNotePDFTemplates
 ┃ ┃ ┃ ┣ GarmentReturnCorrectionNote
 ┃ ┃ ┣ GarmentPurchaseRequestPDFTemplates
 ┃ ┃ ┣ GarmentUnitReceiptNotePDFTemplates
 ┃ ┣ Serializers
 ┃ ┣ Services
 ┃ ┣ Utilities
 ┃ ┃ ┣ CacheManager
 ┃ ┃ ┃ ┣ CacheData
 ┃ ┃ ┣ Currencies
 ┃ ┣ ViewModels
 ┃ ┃ ┣ BankExpenditureNote
 ┃ ┃ ┣ DebtBookReportViewModels
 ┃ ┃ ┣ DeliveryOrderViewModel
 ┃ ┃ ┣ Expedition
 ┃ ┃ ┣ ExternalPurchaseOrderViewModel
 ┃ ┃ ┃ ┣ Reports
 ┃ ┃ ┣ GarmentBeacukaiViewModel
 ┃ ┃ ┣ GarmentCorrectionNoteViewModel
 ┃ ┃ ┣ GarmentDailyPurchasingReportViewModel
 ┃ ┃ ┣ GarmentDeliveryOrderViewModel
 ┃ ┃ ┣ GarmentExternalPurchaseOrderViewModel
 ┃ ┃ ┃ ┣ Reports
 ┃ ┃ ┣ GarmentInternalPurchaseOrderViewModel
 ┃ ┃ ┣ GarmentInternNoteViewModel
 ┃ ┃ ┣ GarmentInventoryViewModels
 ┃ ┃ ┣ GarmentInvoiceViewModels
 ┃ ┃ ┣ GarmentPOMasterDistributionViewModels
 ┃ ┃ ┣ GarmentPurchaseRequestViewModel
 ┃ ┃ ┣ GarmentPurchasingBookReportViewModel
 ┃ ┃ ┣ GarmentReceiptCorrectionReportViewModel
 ┃ ┃ ┣ GarmentReceiptCorrectionViewModels
 ┃ ┃ ┣ GarmentReports
 ┃ ┃ ┣ GarmentSupplierBalanceDebtViewModel
 ┃ ┃ ┣ GarmentUnitDeliveryOrderViewModel
 ┃ ┃ ┣ GarmentUnitExpenditureNoteViewModel
 ┃ ┃ ┣ GarmentUnitReceiptNoteViewModels
 ┃ ┃ ┣ IntegrationViewModel
 ┃ ┃ ┣ InternalPurchaseOrderViewModel
 ┃ ┃ ┣ MonitoringCentralBillExpenditureViewModel
 ┃ ┃ ┣ MonitoringCentralBillReceptionViewModel
 ┃ ┃ ┣ MonitoringCorrectionNoteExpenditureViewModel
 ┃ ┃ ┣ MonitoringCorrectionNoteReceptionViewModel
 ┃ ┃ ┣ MonitoringUnitReceiptAllViewModel
 ┃ ┃ ┣ NewIntegrationViewModel
 ┃ ┃ ┃ ┣ CostCalculationGarment
 ┃ ┃ ┣ PRMasterValidationReportViewModel
 ┃ ┃ ┣ PurchaseOrder
 ┃ ┃ ┣ PurchaseRequestViewModel
 ┃ ┃ ┣ PurchasingDispositionViewModel
 ┃ ┃ ┃ ┣ EPODispositionLoader
 ┃ ┃ ┣ UnitPaymentCorrectionNoteViewModel
 ┃ ┃ ┣ UnitPaymentOrderViewModel
 ┃ ┃ ┣ UnitReceiptNote
 ┃ ┃ ┗ UnitReceiptNoteViewModel
 ┃ ┣ Com.DanLiris.Service.Purchasing.Lib.csproj
 ┃ ┣ LocalDbCashFlowDbContext.cs
 ┃ ┣ MongoDbContext.cs
 ┃ ┣ PurchasingDbContext.cs
 ┃ ┗ PurchasingLibClassDiagram.cd

 ```

**2. Com.DanLiris.Service.Purchasing.WebApi**

This folder consists of controller API. The controller has responsibility to processing data and  HTTP requests and then send it to a web page. All responses from the HTTP requests API are formatted as JSON (JavaScript Object Notation) objects containing information related to the request, and any status.

The folder tree in this folder is:

```
com-danliris-service-purchasing
 ┣ Com.DanLiris.Service.Purchasing.WebApi
 ┃ ┣ bin
 ┃ ┃ ┗ Debug
 ┃ ┃ ┃ ┗ netcoreapp2.0
 ┃ ┃ ┃ ┃ ┣ Properties
 ┃ ┣ Controllers
 ┃ ┃ ┗ v1
 ┃ ┃ ┃ ┣ BankExpenditureNote
 ┃ ┃ ┃ ┣ DeliveryOrderControllers
 ┃ ┃ ┃ ┣ Expedition
 ┃ ┃ ┃ ┣ ExternalPurchaseOrderControllers
 ┃ ┃ ┃ ┃ ┣ Reports
 ┃ ┃ ┃ ┣ GarmentBeacukaiControllers
 ┃ ┃ ┃ ┣ GarmentCorrectionNoteControllers
 ┃ ┃ ┃ ┣ GarmentDeliveryOrderControllers
 ┃ ┃ ┃ ┣ GarmentExternalPurchaseOrderControllers
 ┃ ┃ ┃ ┃ ┣ Reports
 ┃ ┃ ┃ ┣ GarmentInternalPurchaseOrderControllers
 ┃ ┃ ┃ ┣ GarmentInternNoteControllers
 ┃ ┃ ┃ ┣ GarmentInvoiceControllers
 ┃ ┃ ┃ ┣ GarmentPOMasterDistributionControllers
 ┃ ┃ ┃ ┣ GarmentPurchaseRequestControllers
 ┃ ┃ ┃ ┣ GarmentReceiptCorrectionControllers
 ┃ ┃ ┃ ┣ GarmentReports
 ┃ ┃ ┃ ┣ GarmentSupplierBalanceDebtControllers
 ┃ ┃ ┃ ┣ GarmentUnitDeliveryOrderControllers
 ┃ ┃ ┃ ┣ GarmentUnitDeliveryOrderReturnControllers
 ┃ ┃ ┃ ┣ GarmentUnitExpenditureNoteControllers
 ┃ ┃ ┃ ┣ GarmentUnitReceiptNoteControllers
 ┃ ┃ ┃ ┣ InternalPurchaseOrderControllers
 ┃ ┃ ┃ ┣ PurchaseRequestControllers
 ┃ ┃ ┃ ┣ PurchasingDispositionControllers
 ┃ ┃ ┃ ┣ Report
 ┃ ┃ ┃ ┣ UnitPaymentCorrectionNoteController
 ┃ ┃ ┃ ┣ UnitPaymentOrderControllers
 ┃ ┃ ┃ ┗ UnitReceiptNoteControllers
 ┃ ┣ Helpers
 ┃ ┣ obj
 ┃ ┃ ┣ Debug
 ┃ ┃ ┃ ┗ netcoreapp2.0
 ┃ ┣ Properties
 ┃ ┃ ┗ launchSettings.json
 ┃ ┣ SchedulerJobs
 ┃ ┃ ┗ MasterRegistry.cs
 ┃ ┣ Com.DanLiris.Service.Purchasing.WebApi.csproj
 ┃ ┣ Com.DanLiris.Service.Purchasing.WebApi.csproj.user
 ┃ ┣ Program.cs
 ┃ ┗ Startup.cs
 ```

**3. Com.DanLiris.Service.Purchasing.Test**

This folder is collection of classes to run code testing. The automation type testing used in this app is  a unit testing with using moq and xunit libraries.

DataUtils:

- Colecction class to seed data as data input in unit test 

The folder tree in this folder is:

```
com-danliris-service-purchasing
 ┣ Com.DanLiris.Service.Purchasing.Test
 ┃ ┣ bin
 ┃ ┃ ┗ Debug
 ┃ ┃ ┃ ┗ netcoreapp2.0
 ┃ ┃ ┃ ┃ ┣ Properties
 ┃ ┣ Controllers
 ┃ ┃ ┣ BankExpenditureNoteControllerTests
 ┃ ┃ ┣ DeliveryOrderControllerTests
 ┃ ┃ ┣ Expedition
 ┃ ┃ ┣ ExternalPurchaseOrderTests
 ┃ ┃ ┃ ┣ Reports
 ┃ ┃ ┣ GarmentBeacukaiTests
 ┃ ┃ ┣ GarmentCorrectionNoteControllerTests
 ┃ ┃ ┣ GarmentDeliveryOrderControllerTests
 ┃ ┃ ┣ GarmentExternalPurchaseOrderControllerTests
 ┃ ┃ ┃ ┣ Reports
 ┃ ┃ ┣ GarmentInternalPurchaseOrderControllerTests
 ┃ ┃ ┣ GarmentInternNoteTests
 ┃ ┃ ┣ GarmentInvoiceTests
 ┃ ┃ ┣ GarmentPOMasterDistributionControllerTests
 ┃ ┃ ┣ GarmentPurchaseRequestControllerTests
 ┃ ┃ ┣ GarmentReceiptCorrectionControllerTests
 ┃ ┃ ┣ GarmentReport
 ┃ ┃ ┣ GarmentSupplierBalanceDebtControllerTests
 ┃ ┃ ┣ GarmentUnitDeliveryOrderControllerTests
 ┃ ┃ ┣ GarmentUnitDeliveryOrderReturControllerTests
 ┃ ┃ ┣ GarmentUnitExpenditureNoteControllerTests
 ┃ ┃ ┣ GarmentUnitReceiptNoteControllerTests
 ┃ ┃ ┣ InternalPurchaseOrderTests
 ┃ ┃ ┣ MonitoringUnitReceiptControllerTests
 ┃ ┃ ┣ PurchaseRequestControllerTests
 ┃ ┃ ┣ PurchasingDispositionControllerTests
 ┃ ┃ ┣ Report
 ┃ ┃ ┣ UnitPaymentCorrectionNoteControllerTests
 ┃ ┃ ┣ UnitPaymentOrderControllerTests
 ┃ ┃ ┗ UnitReceiptNoteTests
 ┃ ┣ DataUtils
 ┃ ┃ ┣ BankExpenditureNoteDataUtils
 ┃ ┃ ┣ DeliveryOrderDataUtils
 ┃ ┃ ┣ ExpeditionDataUtil
 ┃ ┃ ┣ ExternalPurchaseOrderDataUtils
 ┃ ┃ ┣ GarmentBeacukaiDataUtils
 ┃ ┃ ┣ GarmentCorrectionNoteDataUtils
 ┃ ┃ ┣ GarmentDeliveryOrderDataUtils
 ┃ ┃ ┣ GarmentExternalPurchaseOrderDataUtils
 ┃ ┃ ┣ GarmentInternalPurchaseOrderDataUtils
 ┃ ┃ ┣ GarmentInternNoteDataUtils
 ┃ ┃ ┣ GarmentInvoiceDataUtils
 ┃ ┃ ┣ GarmentPOMasterDistributionDataUtils
 ┃ ┃ ┣ GarmentPurchaseRequestDataUtils
 ┃ ┃ ┣ GarmentReceiptCorrectionDataUtils
 ┃ ┃ ┣ GarmentUnitDeliveryOrderDataUtils
 ┃ ┃ ┣ GarmentUnitDeliveryOrderReturDataUtils
 ┃ ┃ ┣ GarmentUnitExpenditureDataUtils
 ┃ ┃ ┣ GarmentUnitReceiptNoteDataUtils
 ┃ ┃ ┣ InternalPurchaseOrderDataUtils
 ┃ ┃ ┣ NewIntegrationDataUtils
 ┃ ┃ ┣ PPHBankExpenditureNoteDataUtil
 ┃ ┃ ┣ PurchaseRequestDataUtils
 ┃ ┃ ┣ PurchasingDispositionDataUtils
 ┃ ┃ ┣ UnitPaymentCorrectionNoteDataUtils
 ┃ ┃ ┣ UnitPaymentOrderDataUtils
 ┃ ┃ ┣ UnitReceiptNote
 ┃ ┃ ┗ UnitReceiptNoteDataUtils
 ┃ ┣ Facades
 ┃ ┃ ┣ BankExpenditureNoteTest
 ┃ ┃ ┣ DeliveryOrderTests
 ┃ ┃ ┣ ExternalPurchaseOrderTests
 ┃ ┃ ┃ ┣ Reports
 ┃ ┃ ┣ GarmentBeacukaiTests
 ┃ ┃ ┣ GarmentCorrectionNotePriceTests
 ┃ ┃ ┣ GarmentCorrectionNoteQuantityTests
 ┃ ┃ ┣ GarmentDeliveryOrderTests
 ┃ ┃ ┣ GarmentExternalPurchaseOrderTests
 ┃ ┃ ┣ GarmentInternalPurchaseOrderTests
 ┃ ┃ ┣ GarmentInternNoteTests
 ┃ ┃ ┣ GarmentInvoiceTests
 ┃ ┃ ┣ GarmentPOMasterDistributionFacadeTests
 ┃ ┃ ┣ GarmentPurchaseRequestTests
 ┃ ┃ ┣ GarmentReceiptCorrectionTests
 ┃ ┃ ┣ GarmentReportTests
 ┃ ┃ ┣ GarmentReturnCorrectionNoteTests
 ┃ ┃ ┣ GarmentSupplierBalanceDebtTests
 ┃ ┃ ┣ GarmentUnitDeliveryOrderReturTests
 ┃ ┃ ┣ GarmentUnitDeliveryOrderTests
 ┃ ┃ ┣ GarmentUnitExpenditureNoteTests
 ┃ ┃ ┣ GarmentUnitReceiptNoteFacadeTests
 ┃ ┃ ┣ InternalPurchaseOrderTests
 ┃ ┃ ┣ MonitoringUnitReceiptTests
 ┃ ┃ ┣ PPHBankExpenditureNoteTest
 ┃ ┃ ┣ PurchaseRequestTests
 ┃ ┃ ┣ PurchasingDispositionTests
 ┃ ┃ ┣ PurchasingDocumentExpeditionTest
 ┃ ┃ ┣ ReportTest
 ┃ ┃ ┣ UnitPaymentOrderPaidStatusTests
 ┃ ┃ ┣ UnitPaymentOrderTests
 ┃ ┃ ┣ UnitPaymentPriceCorrectionNoteTests
 ┃ ┃ ┣ UnitPaymentQuantityCorrectionNoteTests
 ┃ ┃ ┣ UnitReceiptNoteTests
 ┃ ┣ Helpers
 ┃ ┣ obj
 ┃ ┃ ┣ Debug
 ┃ ┃ ┃ ┗ netcoreapp2.0
 ┃ ┣ Utils
 ┃ ┃ ┣ CoreDataTest.cs
 ┃ ┃ ┗ CurrencyProviderTest.cs
 ┃ ┣ Com.DanLiris.Service.Purchasing.Test.csproj
 ┃ ┣ ServiceProviderFixture.cs
 ┃ ┗ TestServerFixture.cs
```

**TestResults**

- Collections of files generated by the system for purposes of unit test code coverage.

**PurchasingDbContext.cs**

This file contain context class that derives from DbContext in entity framework. DbContext is an important class in Entity Framework API. It is a bridge between domain or entity classes and the database. DbContext and context class  is the primary class that is responsible for interacting with the database.

**File Program.cs**

Important class that contains the entry point to the application. The file has the Main() method used to run the application and it is used to create an instance of WebHostBuilder for creating a host for the application. The Startup class to be used by the application is specified in the Main method.

**File Startup.cs**

This file contains Startup class. The Startup class configures services and the app's request pipeline.Optionally includes a ConfigureServices method to configure the app's services. A service is a reusable component that provides app functionality. Services are registered in ConfigureServices and consumed across the app via dependency injection (DI) or ApplicationServices.This class also Includes a Configure method to create the app's request processing pipeline.

**File docker-compose.test.yml**

File that configure docker compose. Docker compose provides a way to orchestrate multiple containers that work together.

**File Dockerfile**

A Dockerfile is a text document that contains all the commands a user could call on the command line to assemble an image.

**File .travis.yml**

Travis CI (continuous integration) is configured by adding a file named .travis.yml. This file in a YAML format text file, located in root directory of the repository. This file specifies the programming language used, the desired building and testing environment (including dependencies which must be installed before the software can be built and tested), and various other parameters.

**File .codecov.yml**

This file is used to configure code coverage in unit tests.

**Com.DanLiris.Service.Purchasing.sln**

File .sln is extention for *solution* aka file solution for .Net Core, this file is used to manage all project by code editor.

 ### Validation
Data validation using **IValidatableObject**

Test Commit
