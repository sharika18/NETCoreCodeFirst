using BLL.Kafka;
using DAL.Model;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class FakultasService
    {
        private IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly IKafkaSender _kafkaSender;
        //private IRedisService _redis;
        private readonly ILogger<FakultasService> _logger;

        
        public FakultasService(IUnitOfWork unitOfWork, IConfiguration config, IKafkaSender kafkaSender, ILogger<FakultasService> logger)
        {
            _unitOfWork = unitOfWork;
            _config = config;
            _kafkaSender = kafkaSender;
            _logger = logger;
        }

        public async Task<List<Fakultas>> GetAllFakultasAsync()
        {
            return await _unitOfWork.FakultasRepository.GetAll()
                .Include(a => a.ProgramStudis)
                .ToListAsync();
        }

        public async Task<Fakultas> GetFakultasByIdAsync(Guid fakultasId)
        {
            _logger.LogInformation("Getting All Data Fakultas");
            var result = await _unitOfWork.FakultasRepository.GetAll()
                .Where(x => x.FakultasId == fakultasId)
                .Include(a => a.ProgramStudis)
                .FirstOrDefaultAsync();
            return result;
        }

        public async Task CreateFakultasAsync(Fakultas data)
        {
            bool isExist = _unitOfWork.FakultasRepository.IsExist(x => x.NamaFakultas == data.NamaFakultas);
            if (isExist)
            {
                throw new Exception($"Fakultas with nama fakultas {data.NamaFakultas} already exist");
            }
            await _unitOfWork.FakultasRepository.AddAsync(data);
            await _unitOfWork.SaveAsync();
            await SendFakultasToKafka(data);
        }

        public async Task<Fakultas> UpdateFakultasAsync(Fakultas data)
        {
            bool isExist = _unitOfWork.FakultasRepository.IsExist(x => x.FakultasId == data.FakultasId);

            if (!isExist)
            {
                throw new Exception($"Fakultas with id {data.FakultasId} not exist");

            }
            _unitOfWork.FakultasRepository.Edit(data);
            await _unitOfWork.SaveAsync();
            return data;
        }

        public async Task DeleteFakultasAsync(Guid fakultasId)
        {
            var isExist = _unitOfWork.FakultasRepository.IsExist(x => x.FakultasId == fakultasId);
            if (!isExist)
            {
                throw new Exception($"Fakultas with id {fakultasId} not exist");
            }

            _unitOfWork.FakultasRepository.Delete(x => x.FakultasId == fakultasId);
            await _unitOfWork.SaveAsync();
        }

        private async Task SendFakultasToKafka(Fakultas data)
        {
            string topic = _config.GetValue<string>("Topic:Fakultas");
            await _kafkaSender.SendAsync(topic, data);
        }
    }
}
