﻿using SoowGoodWeb.DtoModels;
using SoowGoodWeb.InputDto;
using SoowGoodWeb.Interfaces;
using SoowGoodWeb.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;

namespace SoowGoodWeb.Services
{
    public class DiagonsticPackageTestService : SoowGoodWebAppService, IDiagonsticPackageTestService
    {
        private readonly IRepository<DiagonsticPackageTest> _diagonsticPackageTestRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<DoctorProfile> _doctorProfileRepository;

        public DiagonsticPackageTestService(IRepository<DiagonsticPackageTest> diagonsticPackageTestRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _diagonsticPackageTestRepository = diagonsticPackageTestRepository;

            _unitOfWorkManager = unitOfWorkManager;
        }
        public async Task<DiagonsticPackageTestDto> CreateAsync(DiagonsticPackageTestInputDto input)
        {
            var newEntity = ObjectMapper.Map<DiagonsticPackageTestInputDto, DiagonsticPackageTest>(input);

            var diagonsticPackageTest = await _diagonsticPackageTestRepository.InsertAsync(newEntity);

            await _unitOfWorkManager.Current.SaveChangesAsync();

            return ObjectMapper.Map<DiagonsticPackageTest, DiagonsticPackageTestDto>(diagonsticPackageTest);
        }

        public async Task<DiagonsticPackageTestDto> UpdateAsync(DiagonsticPackageTestInputDto input)
        {
            var updateItem = ObjectMapper.Map<DiagonsticPackageTestInputDto, DiagonsticPackageTest>(input);

            var item = await _diagonsticPackageTestRepository.UpdateAsync(updateItem);

            await _unitOfWorkManager.Current.SaveChangesAsync();

            return ObjectMapper.Map<DiagonsticPackageTest, DiagonsticPackageTestDto>(item);
        }


        public async Task<DiagonsticPackageTestDto> GetAsync(int id)
        {
            var item = await _diagonsticPackageTestRepository.GetAsync(x => x.Id == id);

            return ObjectMapper.Map<DiagonsticPackageTest, DiagonsticPackageTestDto>(item);
        }
        public async Task<List<DiagonsticPackageTestDto>> GetListAsync()
        {
            List<DiagonsticPackageTestDto>? result = null;
            var alldiagonsticPackageTestwithDetails = await _diagonsticPackageTestRepository.WithDetailsAsync(d => d.DiagonsticPackage, pc=>pc.PathologyCategory, pt=>pt.PathologyTest);
            //var list = allsupervisorwithDetails.ToList();

            if (!alldiagonsticPackageTestwithDetails.Any())
            {
                return result;
            }
            result = new List<DiagonsticPackageTestDto>();
            foreach (var item in alldiagonsticPackageTestwithDetails)
            {
                result.Add(new DiagonsticPackageTestDto()
                {
                   Id = item.Id,
                   DiagonsticPackageId = item.DiagonsticPackageId,
                   DiagonsticPackageName=item.DiagonsticPackageId>0?item.DiagonsticPackage.PackageName:null,
                   PathologyCategoryId=item.PathologyCategoryId,
                   PathologyCategoryName=item.PathologyCategoryId>0?item.PathologyCategory.PathologyCategoryName:null,
                   PathologyTestId=item.PathologyTestId,
                   PathologyTestName=item.PathologyTestId>0?item.PathologyTest.PathologyTestName:null,
                });
            }
            return result;
            //var diagonsticPackageTests = await _diagonsticPackageTestRepository.GetListAsync();
            //return ObjectMapper.Map<List<DiagonsticPackageTest>, List<DiagonsticPackageTestDto>>(diagonsticPackageTests).OrderByDescending(a=>a.Id).ToList();


            //result = result.OrderByDescending(a => a.AppointmentDate).ToList();
            //var list = result.OrderBy(item => item.AppointmentSerial)
            //    .GroupBy(item => item.AppointmentDate)
            //    .OrderBy(g => g.Key).Select(g => new { g }).ToList();


            //return result;
        }

        
    }
}
