using Invert.Api.Dtos;
using Invert.Api.Dtos.Article;
using Invert.Api.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Invert.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticlesController : ControllerBase
{
    private readonly IArticleService _service;
    private readonly IMemoryCache _cache;
    private readonly ILogger<ArticlesController> _logger;

    public ArticlesController(
        IArticleService service,
        IMemoryCache cache,
        ILogger<ArticlesController> logger)
    {
        _service = service;
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Get all articles
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            const string cacheKey = "all_articles";

            if (!_cache.TryGetValue(cacheKey, out IEnumerable<ArticleDto> articles))
            {
                articles = await _service.GetAllAsync();
                _cache.Set(cacheKey, articles, TimeSpan.FromMinutes(5));
                _logger.LogInformation("Articles loaded from database and cached");
            }
            else
            {
                _logger.LogInformation("Articles loaded from cache");
            }

            // ✅ Return format that frontend expects: { articles: [] }
            return Ok(new { articles });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all articles");
            return StatusCode(500, new { error = "Failed to retrieve articles" });
        }
    }

    /// <summary>
    /// Create new article
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateArticleDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var articleId = await _service.CreateArticleAsync(dto);

            // Clear cache
            _cache.Remove("all_articles");

            _logger.LogInformation("Article created successfully with ID: {ArticleId}", articleId);

            return CreatedAtAction(
                nameof(GetById),
                new { id = articleId },
                new { id = articleId, success = true, message = "Article created successfully" }
            );
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error creating article");
            return BadRequest(new { success = false, error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating article");
            return StatusCode(500, new { success = false, error = "Failed to create article" });
        }
    }

    /// <summary>
    /// Get article by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var cacheKey = $"article_{id}";

            if (!_cache.TryGetValue(cacheKey, out ArticleDto article))
            {
                article = await _service.GetByIdAsync(id);
                if (article != null)
                {
                    _cache.Set(cacheKey, article, TimeSpan.FromMinutes(10));
                }
            }

            if (article == null)
            {
                _logger.LogWarning("Article with ID {ArticleId} not found", id);
                return NotFound(new { error = $"Article with ID {id} not found" });
            }

            return Ok(article);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving article {ArticleId}", id);
            return StatusCode(500, new { error = "Failed to retrieve article" });
        }
    }

    /// <summary>
    /// Update article
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateArticleDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingArticle = await _service.GetByIdAsync(id);
            if (existingArticle == null)
            {
                _logger.LogWarning("Article with ID {ArticleId} not found for update", id);
                return NotFound(new { success = false, error = $"Article with ID {id} not found" });
            }

            await _service.UpdateArticleAsync(id, dto);

            // Clear cache
            _cache.Remove($"article_{id}");
            _cache.Remove("all_articles");

            _logger.LogInformation("Article {ArticleId} updated successfully", id);

            // ✅ Return format that frontend expects: { success: true }
            return Ok(new { success = true, message = "Article updated successfully" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { success = false, error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating article {ArticleId}", id);
            return StatusCode(500, new { success = false, error = "Failed to update article" });
        }
    }

    /// <summary>
    /// Delete article
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var existingArticle = await _service.GetByIdAsync(id);
            if (existingArticle == null)
            {
                _logger.LogWarning("Article with ID {ArticleId} not found for deletion", id);
                return NotFound(new { success = false, error = $"Article with ID {id} not found" });
            }

            await _service.DeleteArticleAsync(id);

            // Clear cache
            _cache.Remove($"article_{id}");
            _cache.Remove("all_articles");

            _logger.LogInformation("Article {ArticleId} deleted successfully", id);

            return Ok(new { success = true, message = "Article deleted successfully" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { success = false, error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting article {ArticleId}", id);
            return StatusCode(500, new { success = false, error = "Failed to delete article" });
        }
    }

    /// <summary>
    /// Delete multiple articles
    /// </summary>
    [HttpPost("delete-multiple")]
    public async Task<IActionResult> DeleteMultiple([FromBody] List<Guid> ids)
    {
        try
        {
            if (ids == null || ids.Count == 0)
            {
                return BadRequest(new { success = false, error = "No IDs provided" });
            }

            var deletedCount = 0;
            foreach (var id in ids)
            {
                try
                {
                    await _service.DeleteArticleAsync(id);
                    _cache.Remove($"article_{id}");
                    deletedCount++;
                }
                catch (KeyNotFoundException)
                {
                    _logger.LogWarning("Article with ID {ArticleId} not found", id);
                }
            }

            _cache.Remove("all_articles");

            return Ok(new
            {
                success = true,
                message = $"{deletedCount} article(s) deleted successfully",
                deletedCount
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting multiple articles");
            return StatusCode(500, new { success = false, error = "Failed to delete articles" });
        }
    }
}