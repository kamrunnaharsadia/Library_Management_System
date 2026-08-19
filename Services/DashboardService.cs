using LibraryManagementSystem.Repositories;

namespace LibraryManagementSystem.Services
{
    public class DashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardService(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public DashboardStats GetStats() => _dashboardRepository.GetStats();
    }
}
