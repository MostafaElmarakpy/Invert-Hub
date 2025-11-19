using Invert.Api.Data;
using Invert.Api.Entities;
using Invert.Api.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace Invert.Api.Repositories.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;

        public IUserRepository User { get; private set; }
        public IProjectRepository Project { get; private set; }

        public IArticleRepository Article { get; private set; }
        public IJobRepository Job { get; private set; }

        public INotificationRepository Notification { get; }



        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            Project = new ProjectRepository(_dbContext);
            User = new UserRepository(_dbContext);
            Article = new ArticleRepository(_dbContext);
            Job = new JobRepository(_dbContext);
            Notification = new NotificationRepository(_dbContext);
        }



        public void Dispose()
        {
            _dbContext.Dispose();
        }


        public async Task<int> Save()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}