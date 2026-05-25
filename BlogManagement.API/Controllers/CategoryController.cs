using BlogManagement.API.Models.Common;
using BlogManagement.API.Models.Domain;
using BlogManagement.API.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<Category>>.Ok(categories));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound(ApiResponse<Category>.Fail("Category not found."));

            return Ok(ApiResponse<Category>.Ok(category));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
        {
            try
            {
                var category = await _categoryService.CreateAsync(request.Name, request.Description);
                return CreatedAtAction(nameof(GetById), new { id = category.Id },
                    ApiResponse<Category>.Ok(category, "Category created successfully."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<Category>.Fail(ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateCategoryRequest request)
        {
            try
            {
                var category = await _categoryService.UpdateAsync(id, request.Name, request.Description);
                return Ok(ApiResponse<Category>.Ok(category, "Category updated successfully."));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<Category>.Fail("Category not found."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<Category>.Fail(ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _categoryService.DeleteAsync(id);
                return Ok(ApiResponse.Ok("Category deleted successfully."));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse.Fail("Category not found."));
            }
        }
        public class CreateCategoryRequest
        {
            [System.ComponentModel.DataAnnotations.Required]
            [System.ComponentModel.DataAnnotations.MaxLength(100)]
            public string Name { get; set; } = string.Empty;
            public string? Description { get; set; }
        }
    }
}
