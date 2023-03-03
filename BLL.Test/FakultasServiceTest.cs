using DAL.Repositories;
using System;
using System.Collections.Generic;
using Moq;
using BLL.Kafka;
using DAL.Model;
using BLL.Test.Common;
using System.Linq.Expressions;
using System.Linq;
using MockQueryable.Moq;
using System.Threading.Tasks;
using System.Threading;
using Xunit;
using FluentAssertions;
using BLL.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BLL.Test
{
    public class FakultasServiceTest
    {
        private IEnumerable<Fakultas> fakultas;
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<IKafkaSender> _IkafkaSender;
        private Mock<ILogger<FakultasService>>_loggerFakultasService;

        IConfiguration configuration = new ConfigurationBuilder()
                                       .AddInMemoryCollection()
                                       .Build();
        public FakultasServiceTest()
        {
            fakultas = CommonHelper.LoadDataFromFile<IEnumerable<Fakultas>>(@"MockData\Fakultas.json");
            _unitOfWork = MockUnitOfWork();
            _IkafkaSender = MockIkafkaSender();
            _loggerFakultasService = MockFakultasILogger();
            //redis = MockRedis();
        }

        private FakultasService CreateFakultasService()
        {
            return new FakultasService(_unitOfWork.Object, configuration, _IkafkaSender.Object, _loggerFakultasService.Object);
        }

        #region Mock Dependencies
        private Mock<IUnitOfWork> MockUnitOfWork()
        {
            var fakultasQueryable = fakultas.AsQueryable().BuildMock().Object;
            var mockUnitOfWork = new Mock<IUnitOfWork>();

            mockUnitOfWork
                .Setup(u => u.FakultasRepository.GetAll())
                .Returns(fakultasQueryable);

            mockUnitOfWork
                .Setup(u => u.FakultasRepository.IsExist(It.IsAny<Expression<Func<Fakultas, bool>>>()))
                .Returns((Expression<Func<Fakultas, bool>> condition) => fakultasQueryable.Any(condition));

            mockUnitOfWork
               .Setup(u => u.FakultasRepository.GetSingleAsync(It.IsAny<Expression<Func<Fakultas, bool>>>()))
               .ReturnsAsync((Expression<Func<Fakultas, bool>> condition) => fakultasQueryable.FirstOrDefault(condition));

            mockUnitOfWork
               .Setup(u => u.FakultasRepository.AddAsync(It.IsAny<Fakultas>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync((Fakultas author, CancellationToken token) =>
               {
                   author.FakultasId = Guid.NewGuid();
                   return author;
               });

            mockUnitOfWork
                .Setup(u => u.FakultasRepository.Delete(It.IsAny<Expression<Func<Fakultas, bool>>>()))
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

        private Mock<ILogger<FakultasService>> MockFakultasILogger()
        {
            var mockFakultasIlogger = new Mock<ILogger<FakultasService>>();
            return mockFakultasIlogger;
        }
        #endregion

        #region Unit Test Fakultas Service
        [Fact]
        public async Task GetAllAsync_Success()
        {
            //arrange
            var expected = fakultas;

            var svc = CreateFakultasService();

            // act
            var actual = await svc.GetAllFakultasAsync();

            // assert      
            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData("4fa85f64-5717-4562-b3fc-2c963f66afa6")]
        [InlineData("7fa85f64-5717-4562-b3fc-2c963f66afa6")]
        public async Task GetFakultasByIdAsync_Success(string fakultasId)
        {
            //arrange
            var id = Guid.Parse(fakultasId);
            var expected = fakultas.First(x => x.FakultasId == id);

            var svc = CreateFakultasService();

            //act
            var actual = await svc.GetFakultasByIdAsync(id);

            //assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task CreateFakultasAsync_Success()
        {
            //arrange
            var expected = new Fakultas
            {
                NamaFakultas = "Fakultas Unit Testing 1",
            };

            var svc = CreateFakultasService();

            //act
            Func<Task> act = async () => { await svc.CreateFakultasAsync(expected); };

            await act.Should().NotThrowAsync<Exception>();

            //assert

            string topic = "TopicTest";
            configuration["Topic:Fakultas"] = topic;
            _unitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
            _IkafkaSender.Verify(x => x.SendAsync(topic, It.IsAny<Fakultas>()), Times.Never);
        }

        [Theory]
        [InlineData("Fakultas Kedokteran")]
        public async Task CreateFakultasAsync_Error_FakultasAlreadyExist(string namaFakultas)
        {
            //arrange
            var expected = new Fakultas
            {
                NamaFakultas = namaFakultas,
            };

            var svc = CreateFakultasService();

            //act
            Func<Task> act = async () => { await svc.CreateFakultasAsync(expected); };

            await act.Should().ThrowAsync<Exception>().WithMessage($"Fakultas with nama fakultas {expected.NamaFakultas} already exist");
        }

        [Theory]
        [InlineData("7fa85f64-5717-4562-b3fc-2c963f66afa6", "Fakultas Kedokteran Updated")]
        public async Task UpdateFakultasAsync_Succees(string fakultasId, string namaFakultas)
        {


            Guid id = Guid.Parse(fakultasId);
            //arrange
            var expected = new Fakultas
            {
                FakultasId = id,
                NamaFakultas = namaFakultas,
            };

            var svc = CreateFakultasService();

            var oldData = await svc.GetFakultasByIdAsync(id);

            var newData = await svc.UpdateFakultasAsync(expected);
            oldData.Should().NotBeEquivalentTo(newData);
        }

        [Theory]
        [InlineData("53c8ddc4-8919-40b5-b660-ce38686a67be", "Fakultas Kedokteran Updated")]
        public async Task UpdateFakultasAsync_Error_FakultasIdNotExist(string fakultasId, string namaFakultas)
        {
            Guid id = Guid.Parse(fakultasId);
            //arrange
            var expected = new Fakultas
            {
                FakultasId = id,
                NamaFakultas = namaFakultas,
            };

            var svc = CreateFakultasService();

            var oldData = await svc.GetFakultasByIdAsync(id);

            //act
            Func<Task> act = async () => { await svc.UpdateFakultasAsync(expected); };
            await act.Should().ThrowAsync<Exception>($"Fakultas with id {fakultasId} not exist");
        }

        [Theory]
        [InlineData("7fa85f64-5717-4562-b3fc-2c963f66afa6")]
        public async Task DeleteFakultasAsync_Success(string fakultasId)
        {
            Guid id = Guid.Parse(fakultasId);
            //arrange
            
            var svc = CreateFakultasService();

            //act
            Func<Task> act = async () => { await svc.DeleteFakultasAsync(id); };
            await act.Should().NotThrowAsync<Exception>();
        }

        [Theory]
        [InlineData("53c8ddc4-8919-40b5-b660-ce38686a67be")]
        public async Task DeleteFakultasAsync_Error_FakultasIdNotExist(string fakultasId)
        {
            Guid id = Guid.Parse(fakultasId);
            //arrange

            var svc = CreateFakultasService();
            //act
            Func<Task> act = async () => { await svc.DeleteFakultasAsync(id); };
            await act.Should().ThrowAsync<Exception>($"Fakultas with id {fakultasId} not exist");
        }

        #endregion
    }
}
