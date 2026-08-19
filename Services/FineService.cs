using System.Collections.Generic;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories;

namespace LibraryManagementSystem.Services
{
    public class FineService
    {
        private readonly IFineRepository _fineRepository;

        public FineService(IFineRepository fineRepository)
        {
            _fineRepository = fineRepository;
        }

        public List<Fine> GetAllFines() => _fineRepository.GetAll();

        public List<Fine> GetFinesForMember(int memberId) => _fineRepository.GetByMember(memberId);

        public List<Fine> FindFines(FineFilter filter) => _fineRepository.Find(filter ?? new FineFilter());

        public decimal GetTotalUnpaid() => _fineRepository.GetTotalUnpaid();

        public decimal GetTotalUnpaidForMember(int memberId) => _fineRepository.GetTotalUnpaidByMember(memberId);

        public void MarkFinePaid(int fineId) => _fineRepository.MarkPaid(fineId);
    }
}
