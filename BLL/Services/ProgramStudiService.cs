using DAL.Model;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace BLL.Services
{
    public class ProgramStudiService
    {
        private IUnitOfWork _unitOfWork;
        //private IRedisService _redis;
        private readonly ILogger<ProgramStudiService> _logger;
        public ProgramStudiService(IUnitOfWork unitOfWork, ILogger<ProgramStudiService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<List<ProgramStudi>> GetAllProgramStudiAsync()
        {
            return await _unitOfWork.ProgramStudiRepository.GetAll()
                .Include(a => a.Fakultas)
                .ToListAsync();
        }

        public async Task<ProgramStudi> GetProgramStudiByIdAsync(Guid id)
        {
            _logger.LogInformation("Getting All Data Fakultas");
            var result = await _unitOfWork.ProgramStudiRepository.GetAll()
                .Where(x => x.ProgramStudiId == id)
                .FirstOrDefaultAsync();
            return result;
        }

        public async Task CreateProgramStudiAsync(ProgramStudi data)
        {
            bool isExist = _unitOfWork.ProgramStudiRepository.IsExist(x => x.NamaProgramStudi == data.NamaProgramStudi);
            bool isFakultasExits = _unitOfWork.FakultasRepository.IsExist(x => x.FakultasId == data.FakultasId);

            if(!isFakultasExits)
            {
                throw new Exception($"Fakultas with id {data.Fakultas.FakultasId} doesn't exist");
            }
            if (isExist)
            {
                throw new Exception($"Fakultas with nama program studi {data.NamaProgramStudi} already exist");
            }
            await _unitOfWork.ProgramStudiRepository.AddAsync(data);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateProgramStudiAsync(ProgramStudi data)
        {
            bool isExist = _unitOfWork.ProgramStudiRepository.IsExist(x => x.ProgramStudiId == data.ProgramStudiId);
            bool isFakultasExits = _unitOfWork.FakultasRepository.IsExist(x => x.FakultasId == data.FakultasId);

            if (!isExist)
            {
                throw new Exception($"Fakultas with nama program studi {data.NamaProgramStudi} doesn't exist");
            }
            if (!isFakultasExits)
            {
                throw new Exception($"Fakultas with id {data.Fakultas.FakultasId} doesn't exist");
            }

            data.NamaProgramStudi = data.NamaProgramStudi;
            data.FakultasId = data.FakultasId;
            _unitOfWork.ProgramStudiRepository.Edit(data);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteProgramStudiAsync(Guid programStudiId)
        {
            bool isExist = _unitOfWork.ProgramStudiRepository.IsExist(x => x.ProgramStudiId == programStudiId);
            if (!isExist)
            {
                throw new Exception($"Fakultas with id {programStudiId} doesn't exist");
            }
            _unitOfWork.ProgramStudiRepository.Delete(x => x.ProgramStudiId == programStudiId);
            await _unitOfWork.SaveAsync();
        }
    }
}
