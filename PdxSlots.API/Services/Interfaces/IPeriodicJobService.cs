using PdxSlots.API.Dtos.PeriodicJobs;

namespace PdxSlots.API.Services.Interfaces
{
    public interface IPeriodicJobService
    {
        Task<PeriodicJobGetDto> StartJob();
        Task<IEnumerable<PeriodicJobGetDto>> GetJobs();
        Task<PeriodicJobEmailsGetDto> GetEmails();
        Task<PeriodicJobEmailsGetDto> UpdateEmails(PeriodicJobEmailsPostDto periodicJobEmailsPostDto);
    }
}
