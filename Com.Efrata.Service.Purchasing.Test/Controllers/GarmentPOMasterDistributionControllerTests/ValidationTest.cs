using Com.Efrata.Service.Purchasing.Lib.Interfaces;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.GarmentPOMasterDistributionViewModels;
using Com.Efrata.Service.Purchasing.Lib.ViewModels.NewIntegrationViewModel;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Efrata.Service.Purchasing.Test.Controllers.GarmentPOMasterDistributionControllerTests
{
    public class ValidationTest
    {
        [Fact]
        public void Validate_Data_Supplier_Null()
        {
            GarmentPOMasterDistributionViewModel viewModel = new GarmentPOMasterDistributionViewModel
            {
                Supplier = null
            };
            Assert.True(viewModel.Validate(null).Count() > 0);
        }

        [Fact]
        public void Validate_Data_DO_Invalid()
        {
            GarmentPOMasterDistributionViewModel viewModel = new GarmentPOMasterDistributionViewModel
            {
                Supplier = new SupplierViewModel { Id = 1 },
                DOId = 0,
                DONo = null
            };
            Assert.True(viewModel.Validate(null).Count() > 0);
        }

        [Fact]
        public void Validate_Data_Items_Null()
        {
            GarmentPOMasterDistributionViewModel viewModel = new GarmentPOMasterDistributionViewModel
            {
                Supplier = new SupplierViewModel { Id = 1 },
                DOId = 1,
                DONo = "DONo",
                Items = null
            };
            Assert.True(viewModel.Validate(null).Count() > 0);
        }

        [Fact]
        public void Validate_Data_Details_Empty()
        {
            GarmentPOMasterDistributionViewModel viewModel = new GarmentPOMasterDistributionViewModel
            {
                Supplier = new SupplierViewModel { Id = 1 },
                DOId = 1,
                DONo = "DONo",
                Items = new List<GarmentPOMasterDistributionItemViewModel>
                {
                    new GarmentPOMasterDistributionItemViewModel
                    {
                        Details = null
                    }
                }
            };
            Assert.True(viewModel.Validate(null).Count() > 0);
        }

        ValidationContext GetValidationContext(GarmentPOMasterDistributionViewModel viewModel, Dictionary<string, decimal> othersQuantity = null)
        {
            Mock<IGarmentPOMasterDistributionFacade> facade = new Mock<IGarmentPOMasterDistributionFacade>();
            facade.Setup(s => s.GetOthersQuantity(It.IsAny<GarmentPOMasterDistributionViewModel>()))
                .Returns(othersQuantity ?? new Dictionary<string, decimal>());

            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(s => s.GetService(typeof(IGarmentPOMasterDistributionFacade)))
                .Returns(facade.Object);

            ValidationContext validationContext = new ValidationContext(viewModel, serviceProvider.Object, null);

            return validationContext;
        }

        [Fact]
        public void Validate_Data_Details_CostCalculation_Empty()
        {
            GarmentPOMasterDistributionViewModel viewModel = new GarmentPOMasterDistributionViewModel
            {
                Supplier = new SupplierViewModel { Id = 1 },
                DOId = 1,
                DONo = "DONo",
                Items = new List<GarmentPOMasterDistributionItemViewModel>
                {
                    new GarmentPOMasterDistributionItemViewModel
                    {
                        Details = new List<GarmentPOMasterDistributionDetailViewModel>
                        {
                            new GarmentPOMasterDistributionDetailViewModel
                            {
                                CostCalculationId = 0,
                                RONo = null
                            }
                        }
                    }
                }
            };
            Assert.True(viewModel.Validate(GetValidationContext(viewModel)).Count() > 0);
        }

        [Fact]
        public void Validate_Data_Details_POSerialNumber_Null()
        {
            GarmentPOMasterDistributionViewModel viewModel = new GarmentPOMasterDistributionViewModel
            {
                Supplier = new SupplierViewModel { Id = 1 },
                DOId = 1,
                DONo = "DONo",
                Items = new List<GarmentPOMasterDistributionItemViewModel>
                {
                    new GarmentPOMasterDistributionItemViewModel
                    {
                        Details = new List<GarmentPOMasterDistributionDetailViewModel>
                        {
                            new GarmentPOMasterDistributionDetailViewModel
                            {
                                CostCalculationId = 1,
                                RONo = "RONo",
                                POSerialNumber = null
                            }
                        }
                    }
                }
            };
            Assert.True(viewModel.Validate(GetValidationContext(viewModel)).Count() > 0);
        }

        [Fact]
        public void Validate_Data_Details_Conversion_Null()
        {
            GarmentPOMasterDistributionViewModel viewModel = new GarmentPOMasterDistributionViewModel
            {
                Supplier = new SupplierViewModel { Id = 1 },
                DOId = 1,
                DONo = "DONo",
                Items = new List<GarmentPOMasterDistributionItemViewModel>
                {
                    new GarmentPOMasterDistributionItemViewModel
                    {
                        Details = new List<GarmentPOMasterDistributionDetailViewModel>
                        {
                            new GarmentPOMasterDistributionDetailViewModel
                            {
                                CostCalculationId = 1,
                                RONo = "RONo",
                                POSerialNumber = "POSerialNumber",
                                Conversion = 0
                            }
                        }
                    }
                }
            };
            Assert.True(viewModel.Validate(GetValidationContext(viewModel)).Count() > 0);
        }

        [Fact]
        public void Validate_Data_Details_Quantity_Null()
        {
            GarmentPOMasterDistributionViewModel viewModel = new GarmentPOMasterDistributionViewModel
            {
                Supplier = new SupplierViewModel { Id = 1 },
                DOId = 1,
                DONo = "DONo",
                Items = new List<GarmentPOMasterDistributionItemViewModel>
                {
                    new GarmentPOMasterDistributionItemViewModel
                    {
                        Details = new List<GarmentPOMasterDistributionDetailViewModel>
                        {
                            new GarmentPOMasterDistributionDetailViewModel
                            {
                                CostCalculationId = 1,
                                RONo = "RONo",
                                POSerialNumber = "POSerialNumber",
                                Conversion = 1,
                                Quantity = 0,
                                QuantityCC = 100
                            }
                        }
                    }
                }
            };
            Assert.True(viewModel.Validate(GetValidationContext(viewModel)).Count() > 0);
        }

        [Fact]
        public void Validate_Data_Details_QuantityCC_Used_All()
        {
            var othersQuantity = new KeyValuePair<string, decimal>("POSerialNumber", 2);

            GarmentPOMasterDistributionViewModel viewModel = new GarmentPOMasterDistributionViewModel
            {
                Supplier = new SupplierViewModel { Id = 1 },
                DOId = 1,
                DONo = "DONo",
                Items = new List<GarmentPOMasterDistributionItemViewModel>
                {
                    new GarmentPOMasterDistributionItemViewModel
                    {
                        Details = new List<GarmentPOMasterDistributionDetailViewModel>
                        {
                            new GarmentPOMasterDistributionDetailViewModel
                            {
                                CostCalculationId = 1,
                                RONo = "RONo",
                                POSerialNumber = othersQuantity.Key,
                                QuantityCC = othersQuantity.Value,
                                Conversion = 1,
                                Quantity = 1
                            }
                        }
                    }
                }
            };
            Assert.True(viewModel.Validate(GetValidationContext(viewModel, new Dictionary<string, decimal> { { othersQuantity.Key, othersQuantity.Value } })).Count() > 0);
        }

        [Fact]
        public void Validate_Data_Details_Quantity_Too_Much()
        {
            GarmentPOMasterDistributionViewModel viewModel = new GarmentPOMasterDistributionViewModel
            {
                Supplier = new SupplierViewModel { Id = 1 },
                DOId = 1,
                DONo = "DONo",
                Items = new List<GarmentPOMasterDistributionItemViewModel>
                {
                    new GarmentPOMasterDistributionItemViewModel
                    {
                        Details = new List<GarmentPOMasterDistributionDetailViewModel>
                        {
                            new GarmentPOMasterDistributionDetailViewModel
                            {
                                CostCalculationId = 1,
                                RONo = "RONo",
                                POSerialNumber = "POSerialNumber",
                                QuantityCC = 2,
                                Conversion = 1,
                                Quantity = 3
                            }
                        }
                    }
                }
            };
            Assert.True(viewModel.Validate(GetValidationContext(viewModel)).Count() > 0);
        }

        [Fact]
        public void Validate_Data_Details_POSerialNumber_Duplicate()
        {
            GarmentPOMasterDistributionViewModel viewModel = new GarmentPOMasterDistributionViewModel
            {
                Supplier = new SupplierViewModel { Id = 1 },
                DOId = 1,
                DONo = "DONo",
                Items = new List<GarmentPOMasterDistributionItemViewModel>
                {
                    new GarmentPOMasterDistributionItemViewModel
                    {
                        Details = new List<GarmentPOMasterDistributionDetailViewModel>
                        {
                            new GarmentPOMasterDistributionDetailViewModel
                            {
                                CostCalculationId = 1,
                                RONo = "RONo",
                                POSerialNumber = "POSerialNumber",
                                QuantityCC = 2,
                                Conversion = 1,
                                Quantity = 2
                            }
                        }
                    },
                    new GarmentPOMasterDistributionItemViewModel
                    {
                        Details = new List<GarmentPOMasterDistributionDetailViewModel>
                        {
                            new GarmentPOMasterDistributionDetailViewModel
                            {
                                CostCalculationId = 1,
                                RONo = "RONo",
                                POSerialNumber = "POSerialNumber",
                                QuantityCC = 2,
                                Conversion = 1,
                                Quantity = 2
                            }
                        }
                    }
                }
            };
            Assert.True(viewModel.Validate(GetValidationContext(viewModel)).Count() > 0);
        }

        [Fact]
        public void Validate_Data_Items_TotalQuantity_Too_Much()
        {
            GarmentPOMasterDistributionViewModel viewModel = new GarmentPOMasterDistributionViewModel
            {
                Supplier = new SupplierViewModel { Id = 1 },
                DOId = 1,
                DONo = "DONo",
                Items = new List<GarmentPOMasterDistributionItemViewModel>
                {
                    new GarmentPOMasterDistributionItemViewModel
                    {
                        Details = new List<GarmentPOMasterDistributionDetailViewModel>
                        {
                            new GarmentPOMasterDistributionDetailViewModel
                            {
                                CostCalculationId = 1,
                                RONo = "RONo",
                                POSerialNumber = "POSerialNumber",
                                QuantityCC = 2,
                                Conversion = 2,
                                Quantity = 4
                            }
                        },
                        DOQuantity = 2
                    }
                }
            };
            Assert.True(viewModel.Validate(GetValidationContext(viewModel)).Count() > 0);
        }
    }
}
