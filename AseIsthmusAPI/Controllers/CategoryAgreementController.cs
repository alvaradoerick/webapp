﻿using Microsoft.AspNetCore.Mvc;
using AseIsthmusAPI.Data.DTOs;
using AseIsthmusAPI.Services;
using System.Collections;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AseIsthmusAPI.Data.AseIsthmusModels;

namespace AseIsthmusAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CategoryAgreementController : ControllerBase
    {
        private readonly CategoryAgreementService _service;


        public CategoryAgreementController(CategoryAgreementService service)
        {
            _service = service;
        }

        // [Authorize]
        [HttpGet]
        public async Task<IEnumerable<CategoryAgreement>> Get()
        {
            return await _service.GetAll();
        }

        [Authorize]
        [HttpGet("active-categories")]
        public async Task<IEnumerable<CategoryAgreement>> GetAllActiveCategories()
        {
            return await _service.GetAllActiveCategories();
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryAgreement>> GetById(int id)
        {
            var categoryAgreement = await _service.GetById(id);

            if (categoryAgreement is null)
            {
                return NotFound();
            }
            else
            {
                return categoryAgreement;
            }

        }

        [HttpPost]
        // [Authorize]
        public async Task<IActionResult> Create(CategoryAgreement categoryAgreement)
        {
            var newCategoryAgreement = await _service.Create(categoryAgreement);

            return CreatedAtAction(nameof(GetById), new { id = newCategoryAgreement.CategoryAgreementId }, newCategoryAgreement);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, CategoryAgreement categoryAgreement)
        {
            if (id != categoryAgreement.CategoryAgreementId)
            {
                return BadRequest(new { error = "El ID de la URL no coincecide con el ID del cuerpo de la solicitud" });
            }
            var categoryAgreementToUpdate = await _service.GetById(id);

            if (categoryAgreementToUpdate is not null)
            {
                await _service.Update(id, categoryAgreement);
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        //[Authorize]
        public async Task<IActionResult> Delete(int id)
        {

            bool hasAgreements = await _service.HasAgreements(id);

            if (hasAgreements)
            {
                return BadRequest(new { error = "No se puede eliminar la categoría porque tiene convenios asociados." });
            }
            else
            {
                var categoryAgreementToDelete = await _service.GetById(id);

                if (categoryAgreementToDelete is not null)
                {
                    await _service.Delete(id);
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
        }

    }

}
