using SoowGoodWeb.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace SoowGoodWeb.SpRepository
{
    public class DegreeSpRepository
    {
        private readonly IRepository<Degree> _degreeRepository;
        public DegreeSpRepository(IRepository<Degree> degreeRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _degreeRepository = degreeRepository;
        }
        //public List<Degree> GetDegreeList()
        //{
        //    List<SqlParams> _params = new List<SqlParams>();
        //    _params.Add(new SqlParam("@providerid", SqlDbType.NVarChar, providerid));
        //    _params.Add(new SqlParam("@appointmenttype", SqlDbType.NVarChar, appointmenttype));
        //    var items = Executor.ExecuteStoredProcedure<ScheduleAppointment>("pr_booking_getclinicappointmentslot", _params);
        //    return items;
        //}
    }
}
