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
            var result = new PrescriptionMasterDto();
            try
            {
                long lastcount = await GetPrescriptionCountAsync();

                string prescSuffix = (lastcount + 1).ToString();

                input.RefferenceCode = input.DoctorCode + input.PatientCode + "00" + prescSuffix;
                input.PrescriptionDate = DateTime.Now;
                var newEntity = ObjectMapper.Map<PrescriptionMasterInputDto, PrescriptionMaster>(input);

                var prescriptionMaster = await _prescriptionMasterRepository.InsertAsync(newEntity);

                await _unitOfWorkManager.Current.SaveChangesAsync();

                result = ObjectMapper.Map<PrescriptionMaster, PrescriptionMasterDto>(prescriptionMaster);

            }
            catch (Exception ex)
            {
                return result;
            }
            return result;

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

            var diseaseHistory = await _prescriptionPatientDiseaseHistory.GetListAsync(h => h.PrescriptionMasterId == id);
            var patientDiseaseHistories = ObjectMapper.Map<List<PrescriptionPatientDiseaseHistory>, List<PrescriptionPatientDiseaseHistoryDto>>(diseaseHistory);
            var mainComplaints = await _prescriptionMainComplaint.GetListAsync(c => c.PrescriptionMasterId == id);
            var patientMainComplaints = ObjectMapper.Map<List<PrescriptionMainComplaint>, List<PrescriptionMainComplaintDto>>(mainComplaints);
            var findingObservations = await _prescriptionFindingsObservations.GetListAsync(f => f.PrescriptionMasterId == id);
            var findings = ObjectMapper.Map<List<PrescriptionFindingsObservations>, List<PrescriptionFindingsObservationsDto>>(findingObservations);
            var drugDetails = await _prescriptionDrugDetails.GetListAsync(m => m.PrescriptionMasterId == id);
            var medicnes = ObjectMapper.Map<List<PrescriptionDrugDetails>, List<PrescriptionDrugDetailsDto>>(drugDetails);
            var diagnosisTests = await _prescriptionMedicalCheckups.GetListAsync(t => t.PrescriptionMasterId == id);
            var tests = ObjectMapper.Map<List<PrescriptionMedicalCheckups>, List<PrescriptionMedicalCheckupsDto>>(diagnosisTests);



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
                result.DoctorProfileId = prescription.Appointment?.DoctorProfileId;
                result.DoctorName = prescription.Appointment?.DoctorName;
                result.DoctorCode = prescription.Appointment?.DoctorCode;
                result.PatientProfileId = prescription.PatientProfileId;
                result.PatientName = patientDetails?.PatientName;
                result.PatientCode = patientDetails?.PatientCode;
                result.PatientAge = patientDetails?.Age;
                result.PatientBloodGroup = patientDetails?.BloodGroup;
                result.PatientAdditionalInfo = prescription.PatientAdditionalInfo;
                result.ConsultancyType = prescription.ConsultancyType;
                result.ConsultancyTypeName = prescription.Appointment?.ConsultancyType > 0
                        ? ((ConsultancyType)prescription.Appointment.ConsultancyType).ToString()
                        : "N/A";
                result.AppointmentType = prescription.AppointmentType;
                result.AppointmentTypeName = prescription.Appointment?.AppointmentType > 0
                        ? ((AppointmentType)prescription.Appointment.AppointmentType).ToString()
                        : "N/A";
                result.AppointmentDate = prescription.AppointmentDate;
                result.PrescriptionDate = prescription.PrescriptionDate;
                result.PatientLifeStyle = prescription.PatientLifeStyle;
                result.ReportShowDate = prescription.ReportShowDate;
                result.FollowupDate = prescription.FollowupDate;
                result.Advice = prescription.Advice;
                result.PrescriptionPatientDiseaseHistory = patientDiseaseHistories;
                result.prescriptionMainComplaints = patientMainComplaints;
                result.PrescriptionFindingsObservations = findings;
                result.PrescriptionDrugDetails = medicnes;
                result.PrescriptionMedicalCheckups = tests;

                ////result.PrescriptionPatientDiseaseHistory = new List<PrescriptionPatientDiseaseHistoryDto>(); //prescription.PrescriptionPatientDiseaseHistory
                //if (prescription?.PrescriptionPatientDiseaseHistory?.Count > 0)
                //{
                //    foreach (var history in prescription.PrescriptionPatientDiseaseHistory)
                //    {
                //        var diseaseHistory = ObjectMapper.Map<PrescriptionPatientDiseaseHistory, PrescriptionPatientDiseaseHistoryDto>(history);
                //        result.PrescriptionPatientDiseaseHistory?.Add(diseaseHistory);
                //    }
                //}
                //if (prescription?.prescriptionMainComplaints?.Count > 0)
                //{
                //    foreach (var history in prescription.prescriptionMainComplaints)
                //    {
                //        var mainComplaints = ObjectMapper.Map<PrescriptionMainComplaint, PrescriptionMainComplaintDto>(history);
                //        result.prescriptionMainComplaints?.Add(mainComplaints);
                //    }
                //}
                //if (prescription?.PrescriptionFindingsObservations?.Count > 0)
                //{
                //    foreach (var history in prescription.PrescriptionFindingsObservations)
                //    {
                //        var findings = ObjectMapper.Map<PrescriptionFindingsObservations, PrescriptionFindingsObservationsDto>(history);
                //        result.PrescriptionFindingsObservations?.Add(findings);
                //    }
                //}
            }


            //var item = detailsPrescription.Where(x => x.Id == id);

            return result; //ObjectMapper.Map<PrescriptionMaster, PrescriptionMasterDto>(prescription);
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

        public async Task<List<PrescriptionMasterDto>> GetPrescriptionMasterListByDoctorIdPatientIdAsync(int doctorId, int patientId)
        {
            var doctDegrees = await _prescriptionMasterRepository.GetListAsync(dd => dd.DoctorProfileId == doctorId && dd.PatientProfileId == patientId);
            return ObjectMapper.Map<List<PrescriptionMaster>, List<PrescriptionMasterDto>>(doctDegrees);
        }

        public async Task<List<PrescriptionMasterDto>> GetPrescriptionMasterListByPatientIdAsync(int patientId)
        {
            var doctDegrees = await _prescriptionMasterRepository.GetListAsync(dd => dd.PatientProfileId == patientId);
            return ObjectMapper.Map<List<PrescriptionMaster>, List<PrescriptionMasterDto>>(doctDegrees);
        }

        public async Task<List<PrescriptionMasterDto>> GetPrescriptionListByAppointmentCreatorIdAsync(int patientId)
        {
            var prescriptionMaster = await _prescriptionMasterRepository.WithDetailsAsync(a => a.Appointment);
            var item = prescriptionMaster.Where(p => p.Appointment.AppointmentCreatorId == patientId).ToList();

            //var doctDegrees = await _prescriptionMasterRepository.GetListAsync(dd => dd.PatientProfileId == patientId);
            return ObjectMapper.Map<List<PrescriptionMaster>, List<PrescriptionMasterDto>>(item);
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

        public async Task<PrescriptionMasterDto> GetPrescriptionByAppointmentIdAsync(int appointmentId)
        {
            var detailsPrescription = await _prescriptionMasterRepository.WithDetailsAsync(a => a.Appointment
                                                                                              , doc => doc.Appointment.DoctorSchedule.DoctorProfile
                                                                                              , d => d.PrescriptionDrugDetails
                                                                                              , cd => cd.PrescriptionPatientDiseaseHistory
                                                                                              , c => c.prescriptionMainComplaints
                                                                                              , o => o.PrescriptionFindingsObservations
                                                                                              , mc => mc.PrescriptionMedicalCheckups);
            //var appprescription = detailsPrescription.Where(p=>p.AppointmentId == appointmentId).FirstOrDefault();

            var prescription = detailsPrescription.Where(p => p.AppointmentId == appointmentId).FirstOrDefault();
            if (prescription == null)
            {
                return null;
            }
            var diseaseHistory = await _prescriptionPatientDiseaseHistory.GetListAsync(h => h.PrescriptionMasterId == prescription.Id);
            var patientDiseaseHistories = ObjectMapper.Map<List<PrescriptionPatientDiseaseHistory>, List<PrescriptionPatientDiseaseHistoryDto>>(diseaseHistory);
            var mainComplaints = await _prescriptionMainComplaint.GetListAsync(c => c.PrescriptionMasterId == prescription.Id);
            var patientMainComplaints = ObjectMapper.Map<List<PrescriptionMainComplaint>, List<PrescriptionMainComplaintDto>>(mainComplaints);
            var findingObservations = await _prescriptionFindingsObservations.GetListAsync(f => f.PrescriptionMasterId == prescription.Id);
            var findings = ObjectMapper.Map<List<PrescriptionFindingsObservations>, List<PrescriptionFindingsObservationsDto>>(findingObservations);
            var drugDetails = await _prescriptionDrugDetails.GetListAsync(m => m.PrescriptionMasterId == prescription.Id);
            var medicnes = ObjectMapper.Map<List<PrescriptionDrugDetails>, List<PrescriptionDrugDetailsDto>>(drugDetails);
            var diagnosisTests = await _prescriptionMedicalCheckups.GetListAsync(t => t.PrescriptionMasterId == prescription.Id);
            var tests = ObjectMapper.Map<List<PrescriptionMedicalCheckups>, List<PrescriptionMedicalCheckupsDto>>(diagnosisTests);



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
                result.DoctorProfileId = prescription.Appointment?.DoctorProfileId;
                result.DoctorName = prescription.Appointment?.DoctorName;
                result.DoctorCode = prescription.Appointment?.DoctorCode;
                result.PatientProfileId = prescription.PatientProfileId;
                result.PatientName = patientDetails?.PatientName;
                result.PatientCode = patientDetails?.PatientCode;
                result.PatientAge = patientDetails?.Age;
                result.PatientBloodGroup = patientDetails?.BloodGroup;
                result.PatientAdditionalInfo = prescription.PatientAdditionalInfo;
                result.ConsultancyType = prescription.ConsultancyType;
                result.ConsultancyTypeName = prescription.Appointment?.ConsultancyType > 0
                        ? ((ConsultancyType)prescription.Appointment.ConsultancyType).ToString()
                        : "N/A";
                result.AppointmentType = prescription.AppointmentType;
                result.AppointmentTypeName = prescription.Appointment?.AppointmentType > 0
                        ? ((AppointmentType)prescription.Appointment.AppointmentType).ToString()
                        : "N/A";
                result.AppointmentDate = prescription.AppointmentDate;
                result.PrescriptionDate = prescription.PrescriptionDate;
                result.PatientLifeStyle = prescription.PatientLifeStyle;
                result.ReportShowDate = prescription.ReportShowDate;
                result.FollowupDate = prescription.FollowupDate;
                result.Advice = prescription.Advice;
                result.PrescriptionPatientDiseaseHistory = patientDiseaseHistories;
                result.prescriptionMainComplaints = patientMainComplaints;
                result.PrescriptionFindingsObservations = findings;
                result.PrescriptionDrugDetails = medicnes;
                result.PrescriptionMedicalCheckups = tests;

                ////result.PrescriptionPatientDiseaseHistory = new List<PrescriptionPatientDiseaseHistoryDto>(); //prescription.PrescriptionPatientDiseaseHistory
                //if (prescription?.PrescriptionPatientDiseaseHistory?.Count > 0)
                //{
                //    foreach (var history in prescription.PrescriptionPatientDiseaseHistory)
                //    {
                //        var diseaseHistory = ObjectMapper.Map<PrescriptionPatientDiseaseHistory, PrescriptionPatientDiseaseHistoryDto>(history);
                //        result.PrescriptionPatientDiseaseHistory?.Add(diseaseHistory);
                //    }
                //}
                //if (prescription?.prescriptionMainComplaints?.Count > 0)
                //{
                //    foreach (var history in prescription.prescriptionMainComplaints)
                //    {
                //        var mainComplaints = ObjectMapper.Map<PrescriptionMainComplaint, PrescriptionMainComplaintDto>(history);
                //        result.prescriptionMainComplaints?.Add(mainComplaints);
                //    }
                //}
                //if (prescription?.PrescriptionFindingsObservations?.Count > 0)
                //{
                //    foreach (var history in prescription.PrescriptionFindingsObservations)
                //    {
                //        var findings = ObjectMapper.Map<PrescriptionFindingsObservations, PrescriptionFindingsObservationsDto>(history);
                //        result.PrescriptionFindingsObservations?.Add(findings);
                //    }
                //}
            }


            //var item = detailsPrescription.Where(x => x.Id == id);

            return result; //ObjectMapper.Map<PrescriptionMaster, PrescriptionMasterDto>(prescription);
        }
    }
}
