using Invert.Api.Entities;

namespace Invert.Api.Repositories.Interface
{
    public interface IUnitOfWork
    {

        IUserRepository User { get; }
        IProjectRepository Project { get; }
        IArticleRepository Article { get; }
        IJobRepository Job { get; }
        INotificationRepository Notification { get; }
        Task<int> Save();

    }
}