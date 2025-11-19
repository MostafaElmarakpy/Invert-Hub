using AutoMapper;
using Invert.Api.Data;
using Invert.Api.Dtos;
using Invert.Api.Dtos.Article;
using Invert.Api.Entities;
using Invert.Api.Repositories.Implementation;
using Invert.Api.Repositories.Interface;
using Invert.Api.Services.Interface;
using System.Text.Json;

namespace Invert.Api.Services.Implementation
{
    public class ArticleService : IArticleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ArticleService> _logger;

        public ArticleService(IUnitOfWork unitOfWork, ILogger<ArticleService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Guid> CreateArticleAsync(CreateArticleDto dto)
        {
            try
            {

                var title = dto.Title;
                if (string.IsNullOrWhiteSpace(title))
                {
                    throw new ArgumentException("Article must have a title (first header block)");
                }

                // Create entity
                var article = new Article(
                    title: title,
                    contentJson: dto.ContentJson,
                    author: dto.Author
                );

                await _unitOfWork.Article.Add(article);
                await _unitOfWork.Save();

                _logger.LogInformation("Article created with ID: {ArticleId}", article.Id);

                return article.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating article");
                throw;
            }
        }

        public async Task<IEnumerable<ArticleDto>> GetAllAsync()
        {
            try
            {
                var articles = await _unitOfWork.Article.GetAll();

                return articles.Select(ArticleDto.FromEntity).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all articles");
                throw;
            }
        }

        public async Task<ArticleDto?> GetByIdAsync(Guid id)
        {
            try
            {
                var article = await _unitOfWork.Article.Get(a => a.Id == id);

                if (article == null)
                {
                    _logger.LogWarning("Article with ID {ArticleId} not found", id);
                    return null;
                }

                return ArticleDto.FromEntity(article);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving article {ArticleId}", id);
                throw;
            }
        }

        public async Task UpdateArticleAsync(Guid id, UpdateArticleDto dto)
        {
            try
            {
                var article = await _unitOfWork.Article.Get(a => a.Id == id);

                if (article == null)
                {
                    throw new KeyNotFoundException($"Article with ID {id} not found");
                }

                // Update using entity method
                article.Update(
                    title: dto.Title,
                    contentJson: dto.ContentJson,
                    author: dto.Author
                );

                _unitOfWork.Article.Update(article);
                await _unitOfWork.Save();

                _logger.LogInformation("Article {ArticleId} updated successfully", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating article {ArticleId}", id);
                throw;
            }
        }

        public async Task DeleteArticleAsync(Guid id)
        {
            try
            {
                var article = await _unitOfWork.Article.Get(a => a.Id == id);

                if (article == null)
                {
                    throw new KeyNotFoundException($"Article with ID {id} not found");
                }

                _unitOfWork.Article.Remove(article);
                await _unitOfWork.Save();

                _logger.LogInformation("Article {ArticleId} deleted successfully", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting article {ArticleId}", id);
                throw;
            }
        }
    }
}