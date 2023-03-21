using BLL.Cache;
using BLL.DTO;
using BLL.Kafka;
using BLL.Services;
using BLL.Test.Common;
using DAL.Interfaces;
using DAL.Models;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace BLL.Test
{
    public class SalesServiceTest
    {
        private IEnumerable<Sales> sales;
        private Mock<IUnitOfWork> _unitOfWork;
        IConfiguration _configuration = new ConfigurationBuilder()
                                       .AddInMemoryCollection()
                                       .Build();
        private Mock<IKafkaSender> _kafkaSender;
        private Mock<IRedisService> _redis;
        private Mock<ILogger<SalesService>> _logger;

        ITestOutputHelper output;
        public SalesServiceTest(ITestOutputHelper output)
        {
            sales = CommonHelper.LoadDataFromFile<IEnumerable<Sales>>(@"MockData\Sales.json");
            _unitOfWork = MockUnitOfWork();
            _kafkaSender = MockIkafkaSender();
            _redis = MockRedis();
            _logger = MockSalesILogger();
            this.output = output;
        }

        #region Mock Dependencies
        private Mock<IUnitOfWork> MockUnitOfWork()
        {
            var salesQueryable = sales.AsQueryable().BuildMock().Object;
            var mockUnitOfWork = new Mock<IUnitOfWork>();

            mockUnitOfWork
                .Setup(u => u.SalesRepository.GetAll())
                .Returns(salesQueryable);

            mockUnitOfWork
                .Setup(u => u.SalesRepository.IsExist(It.IsAny<Expression<Func<Sales, bool>>>()))
                .Returns((Expression<Func<Sales, bool>> condition) => salesQueryable.Any(condition));

            mockUnitOfWork
               .Setup(u => u.SalesRepository.GetSingleAsync(It.IsAny<Expression<Func<Sales, bool>>>()))
               .ReturnsAsync((Expression<Func<Sales, bool>> condition) => salesQueryable.FirstOrDefault(condition));

            mockUnitOfWork
               .Setup(u => u.SalesRepository.AddAsync(It.IsAny<Sales>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync((Sales author, CancellationToken token) =>
               {
                   author.SalesId = Guid.NewGuid();
                   return author;
               });

            mockUnitOfWork
                .Setup(u => u.SalesRepository.Delete(It.IsAny<Expression<Func<Sales, bool>>>()))
                .Verifiable();

            mockUnitOfWork
                .Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            return mockUnitOfWork;
        }

        private Mock<IKafkaSender> MockIkafkaSender()
        {
            var mockIkafkaSender = new Mock<IKafkaSender>();

            mockIkafkaSender
                .Setup(x => x.SendAsync(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            return mockIkafkaSender;
        }

        private Mock<IRedisService> MockRedis()
        {
            var mockRedis = new Mock<IRedisService>();

            mockRedis
                .Setup(x => x.GetAsync<Sales>(It.Is<string>(x => x.Equals($"{PrefixRedisKey.SalesKey}:4fa85f64-5717-4562-b3fc-2c963f66afa6"))))
                .ReturnsAsync(sales.FirstOrDefault(x => x.SalesId == Guid.Parse("4fa85f64-5717-4562-b3fc-2c963f66afa6")))
                .Verifiable();

            mockRedis
                .Setup(x => x.SaveAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            mockRedis
              .Setup(x => x.DeleteAsync(It.IsAny<string>())).Verifiable();

            return mockRedis;
        }

        private Mock<ILogger<SalesService>> MockSalesILogger()
        {
            var mockSalesIlogger = new Mock<ILogger<SalesService>>();
            return mockSalesIlogger;
        }
        #endregion

        private SalesService CreateSalesService()
        {
            return new SalesService(_unitOfWork.Object, _configuration, _kafkaSender.Object, _redis.Object, _logger.Object);
        }

        #region UNIT TEST - SALES SERVICE
        [Fact]
        public async Task GetAllAsync_Success()
        {
            //Expected
            var expected = sales;

            var svc = CreateSalesService();

            //Actual
            var actual = await svc.GetAllSalesAsync();

            //Assert      
            actual.Should().BeEquivalentTo(expected);
        }


        
        [Theory]
        [InlineData("4fa85f64-5717-4562-b3fc-2c963f66afa6")]
        [InlineData("3fa85f64-5717-4562-b3fc-2c963f66afa6")]
        public async Task GetSalesByIdAsync_Success(string salesId)
        {
            //Expected
            var id = Guid.Parse(salesId);
            var expected = sales.First(x => x.SalesId == id);

            var svc = CreateSalesService();

            //Actual
            var actual = await svc.GetSalesByIdAsync(id);
            //Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData("4fa85f64-5717-4562-b3fc-2c963f66afa6")]
        public async Task GetSalesByIdAsync_Redis_Success(string salesId)
        {
            //Expected
            var id = Guid.Parse(salesId);
            var expected = sales.First(x => x.SalesId == id);

            var svc = CreateSalesService();

            //Actual
            var actual = await svc.GetSalesByIdAsync(id);
            //Assert
            actual.Should().BeEquivalentTo(expected);

            _redis.Verify(x => x.GetAsync<Sales>($"{PrefixRedisKey.SalesKey}:{salesId}"), Times.Once);
            _redis.Verify(x => x.SaveAsync($"{PrefixRedisKey.SalesKey}:{salesId}", It.IsAny<Sales>(), It.IsAny<TimeSpan>()), Times.Never);
        }

        [Fact]
        public async Task CreateSalesAsync_Success()
        {
            //Arrange
            var expected = new Sales()
            {
                ProductId = Guid.Parse("10827462-769A-6627-120E-04709C00D27A"),
                CustomerId = Guid.Parse("8D02546F-5839-B1B2-AFE4-002C344F99A9"),
                TerritoriesId = Guid.Parse("CB181A14-1A4D-5D32-E777-19AE8CAE0666"),
                OrderQuantity = 1,
                UnitPrice = 101,
                SalesAmount = 101,
                OrderDate = DateTime.Now
            };

            var svc = CreateSalesService();

            //Actual
            Func<Task> act = async () => { await svc.CreateSalesAsync(expected); };
            await act.Should().NotThrowAsync<Exception>();

            //assert
            _unitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
            _redis.Verify(x => x.SaveAsync($"{PrefixRedisKey.SalesKey}:{expected.SalesId}", It.IsAny<Sales>(), It.IsAny<TimeSpan>()), Times.Once);
        }
        //[Theory]
        //[InlineData("7fa85f64-5717-4562-b3fc-2c963f66afa6")]
        //public async Task UpdateFakultasAsync_Succees(string salesId)
        //{


        //    Guid id = Guid.Parse(salesId);
            
        //    //Expected
        //    var expected = new Sales
        //    {
        //        SalesId = id,
        //        CustomerId = namaFakultas,,
        //        ProductId = ,

        //    };

        //    var svc = CreateFakultasService();

        //    var oldData = await svc.GetFakultasByIdAsync(id);

        //    var newData = await svc.UpdateFakultasAsync(expected);
        //    oldData.Should().NotBeEquivalentTo(newData);
        //}
        #endregion
    }
}
