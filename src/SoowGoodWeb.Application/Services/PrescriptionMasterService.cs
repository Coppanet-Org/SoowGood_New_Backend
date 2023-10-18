using SoowGoodWeb.DtoModels;
using SoowGoodWeb.Enums;
using SoowGoodWeb.InputDto;
using SoowGoodWeb.Interfaces;
using SoowGoodWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
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
        private readonly IRepository<DoctorProfile> _doctorDetails;
        private readonly IRepository<PatientProfile> _patientDetails;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public PrescriptionMasterService(IRepository<PrescriptionMaster> prescriptionMasterRepository,
                                         IUnitOfWorkManager unitOfWorkManager,
                                         IRepository<PrescriptionMainComplaint> prescriptionMainComplaint,
                                         IRepository<PrescriptionFindingsObservations> prescriptionFindingsObservations,
                                         IRepository<PrescriptionMedicalCheckups> prescriptionMedicalCheckups,
                                         IRepository<PrescriptionPatientDiseaseHistory> prescriptionPatientDiseaseHistory,
                                         IRepository<PrescriptionDrugDetails> prescriptionDrugDetails,
                                         IRepository<DoctorProfile> doctorDetails,
                                         IRepository<PatientProfile> patientDetails)
        {
            _prescriptionMasterRepository = prescriptionMasterRepository;

            _unitOfWorkManager = unitOfWorkManager;
            _prescriptionMainComplaint = prescriptionMainComplaint;
            _prescriptionFindingsObservations = prescriptionFindingsObservations;
            _prescriptionMedicalCheckups = prescriptionMedicalCheckups;
            _prescriptionPatientDiseaseHistory = prescriptionPatientDiseaseHistory;
            _prescriptionDrugDetails = prescriptionDrugDetails;
            _doctorDetails = doctorDetails;
            _patientDetails = patientDetails;
        }
        public async Task<PrescriptionMasterDto> CreateAsync(PrescriptionMasterInputDto input)
        {
            long lastcount = await GetPrescriptionCountAsync();

            string prescSuffix = (lastcount + 1).ToString();

            input.RefferenceCode = input.DoctorCode + '_' + input.PatientCode + '_' + prescSuffix;
            input.PrescriptionDate = DateTime.Now;
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

        public async Task<PrescriptionMasterDto> GetPrescriptionAsync(int id)
        {
            var detailsPrescription = await _prescriptionMasterRepository.WithDetailsAsync(a => a.Appointment
                                                                                              , doc => doc.Appointment.DoctorSchedule.DoctorProfile
                                                                                              , d => d.PrescriptionDrugDetails
                                                                                              , cd => cd.PrescriptionPatientDiseaseHistory
                                                                                              , c => c.prescriptionMainComplaints
                                                                                              , o => o.PrescriptionFindingsObservations
                                                                                              , mc => mc.PrescriptionMedicalCheckups);

            var prescription = detailsPrescription.Where(p => p.Id == id).FirstOrDefault();
            //var doctorDetails = await _doctorDetails.WithDetailsAsync(d => d.Id == prescription.DoctorProfileId);
            var patientDetails = await _patientDetails.GetAsync(p => p.Id == prescription.PatientProfileId);

            var result = new PrescriptionMasterDto();
            if (prescription != null)
            {
                result.Id = prescription.Id;
                result.RefferenceCode = prescription.RefferenceCode;
                result.AppointmentId = prescription.AppointmentId;
                result.AppointmentSerial = prescription.Appointment?.AppointmentSerial;
                result.AppointmentType = prescription.Appointment?.AppointmentType;
                result.AppointmentTypeName = prescription.Appointment?.AppointmentType > 0
                        ? ((AppointmentType)prescription.Appointment.AppointmentType).ToString()
                        : "N/A";

            }


            //var item = detailsPrescription.Where(x => x.Id == id);

            return ObjectMapper.Map<PrescriptionMaster, PrescriptionMasterDto>(prescription);
        }
        public async Task<List<PrescriptionMasterDto>> GetListAsync()
        {
            var degrees = await _prescriptionMasterRepository.GetListAsync();
            return ObjectMapper.Map<List<PrescriptionMaster>, List<PrescriptionMasterDto>>(degrees);
        }
        public async Task<int> GetPrescriptionCountAsync()
        {
            var prescriptions = await _prescriptionMasterRepository.GetListAsync();
            var prescriptionCount = prescriptions.Count();
            return prescriptionCount;
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

        public async Task<List<PrescriptionPatientDiseaseHistoryDto>> GetPatientDiseaseListAsync(long patientId)
        {
            List<PrescriptionPatientDiseaseHistoryDto>? result = null;

            var prescriptionMaster = await _prescriptionMasterRepository.GetListAsync();
            var patientPrescription = prescriptionMaster.Where(p => p.PatientProfileId == patientId).OrderByDescending(d => d.Id).FirstOrDefault();
            var item = await _prescriptionPatientDiseaseHistory.GetListAsync(p => p.PatientProfileId == patientId
                                                                            && p.PrescriptionMasterId == patientPrescription.Id);
            //var diseaseItem = item.OrderByDescending(d => d.PrescriptionMasterId).FirstOrDefault();

            //result = new List<PrescriptionPatientDiseaseHistoryDto>();
            //foreach (var disease in item)
            //{
            //    result.Add(new PrescriptionPatientDiseaseHistoryDto()
            //    {
            //        Id = disease.CommonDisease.Id,
            //        DiseaseName = disease. //drug.DosageForm + " " + drug.BrandName

            //    });
            //}
            //return result;


            return ObjectMapper.Map<List<PrescriptionPatientDiseaseHistory>, List<PrescriptionPatientDiseaseHistoryDto>>(item);
        }
    }
}
