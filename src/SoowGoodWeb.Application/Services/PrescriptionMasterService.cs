using SoowGoodWeb.DtoModels;
using SoowGoodWeb.InputDto;
using SoowGoodWeb.Interfaces;
using SoowGoodWeb.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace SoowGoodWeb.Services
{
    public class PrescriptionMasterService : SoowGoodWebAppService, IPrescriptionMasterService
    {
        private readonly IRepository<PrescriptionMaster> _prescriptionMasterRepository;
        private readonly IRepository<PrescriptionMainComplaint> _prescriptionMainComplaint;
        private readonly IRepository<PrescriptionFindingsObservations> _prescriptionFindingsObservations;
        private readonly IRepository<PrescriptionMedicalCheckups> _prescriptionMedicalCheckups;
        private readonly IRepository<PrescriptionPatientDiseaseHistory> _prescriptionPatientDiseaseHistory;
        private readonly IRepository<PrescriptionDrugDetails> _prescriptionDrugDetails;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public PrescriptionMasterService(IRepository<PrescriptionMaster> prescriptionMasterRepository,
                                         IUnitOfWorkManager unitOfWorkManager,
                                         IRepository<PrescriptionMainComplaint> prescriptionMainComplaint,
                                         IRepository<PrescriptionFindingsObservations> prescriptionFindingsObservations,
                                         IRepository<PrescriptionMedicalCheckups> prescriptionMedicalCheckups,
                                         IRepository<PrescriptionPatientDiseaseHistory> prescriptionPatientDiseaseHistory,
                                         IRepository<PrescriptionDrugDetails> prescriptionDrugDetails)
        {
            _prescriptionMasterRepository = prescriptionMasterRepository;

            _unitOfWorkManager = unitOfWorkManager;
            _prescriptionMainComplaint = prescriptionMainComplaint;
            _prescriptionFindingsObservations = prescriptionFindingsObservations;
            _prescriptionMedicalCheckups = prescriptionMedicalCheckups;
            _prescriptionPatientDiseaseHistory = prescriptionPatientDiseaseHistory;
            _prescriptionDrugDetails = prescriptionDrugDetails;
        }
        public async Task<PrescriptionMasterDto> CreateAsync(PrescriptionMasterInputDto input)
        {
            var newEntity = ObjectMapper.Map<PrescriptionMasterInputDto, PrescriptionMaster>(input);

            var prescriptionMaster = await _prescriptionMasterRepository.InsertAsync(newEntity);

            await _unitOfWorkManager.Current.SaveChangesAsync();

            return ObjectMapper.Map<PrescriptionMaster, PrescriptionMasterDto>(prescriptionMaster);
        }

        public async Task<PrescriptionMasterDto> GetAsync(int id)
        {
            var item = await _prescriptionMasterRepository.GetAsync(x => x.Id == id);

            return ObjectMapper.Map<PrescriptionMaster, PrescriptionMasterDto>(item);
        }

        //public async Task<PrescriptionMasterDto> GetPrescriptionAsync(int id)
        //{
        //    //var detailsPrescription = await _prescriptionMasterRepository.WithDetailsAsync(a => a.Appointment
        //    //                                                                                  , doc => doc.Appointment.DoctorSchedule.DoctorProfile
        //    //                                                                                  , d => d.PrescriptionDrugDetails
        //    //                                                                                  , cd => cd.PrescriptionPatientDiseaseHistory
        //    //                                                                                  , c => c.prescriptionMainComplaints
        //    //                                                                                  , o => o.PrescriptionFindingsObservations
        //    //                                                                                  , mc => mc.PrescriptionMedicalCheckups);

        //    var item = await _prescriptionMasterRepository.GetAsync(x => x.Id == id);

        //    return ObjectMapper.Map<PrescriptionMaster, PrescriptionMasterDto>(item);
        //}
        public async Task<List<PrescriptionMasterDto>> GetListAsync()
        {
            var degrees = await _prescriptionMasterRepository.GetListAsync();
            return ObjectMapper.Map<List<PrescriptionMaster>, List<PrescriptionMasterDto>>(degrees);
        }
        public async Task<List<PrescriptionMasterDto>> GetPrescriptionMasterListByDoctorIdAsync(int doctorId)
        {
            var doctDegrees = await _prescriptionMasterRepository.GetListAsync(dd => dd.DoctorProfileId == doctorId);
            return ObjectMapper.Map<List<PrescriptionMaster>, List<PrescriptionMasterDto>>(doctDegrees);
        }

        public async Task<List<PrescriptionMasterDto>> GetPrescriptionMasterListByPatientIdAsync(int patientId)
        {
            var doctDegrees = await _prescriptionMasterRepository.GetListAsync(dd => dd.PatientProfileId == patientId);
            return ObjectMapper.Map<List<PrescriptionMaster>, List<PrescriptionMasterDto>>(doctDegrees);
        }
        public async Task<PrescriptionMasterDto> UpdateAsync(PrescriptionMasterInputDto input)
        {
            var updateItem = ObjectMapper.Map<PrescriptionMasterInputDto, PrescriptionMaster>(input);

            var item = await _prescriptionMasterRepository.UpdateAsync(updateItem);

            return ObjectMapper.Map<PrescriptionMaster, PrescriptionMasterDto>(item);
        }
    }
}
