using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Controllers
{
    // Versionamento
    [Route("v1/Categories")]
    public class CategoryController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        // Cache
        [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any, Duration = 30)]
        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<ActionResult<List<Category>>> Get(
            [FromServices] DataContext context
        )
        {
            var category = await context.Categories.AsNoTracking().ToListAsync();
            return Ok(category);
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Category>> GetById(
            int id,
            [FromServices] DataContext context
        )
        {
            var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return Ok(category);
        }
        
        [HttpPost]
        [Route("")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Category>> Post(
            [FromBody]Category model,
            [FromServices]DataContext context)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            try
            {
                context.Categories.Add(model);
                await context.SaveChangesAsync();
                
                return Ok(model);
            }
            catch 
            {
                return BadRequest(new { Message = "Não foi possível criar a categoria" });
            }
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Category>> Put(
            int id, 
            [FromBody] Category model,
            [FromServices] DataContext context)
        {
            // Verifica se o id informado é o mesmo do modelo
            if(id != model.Id)
                return NotFound(new {message = "Categoria não encontrada"});
            
            // Verifica se os dados são válidos
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
                
            try 
            {
                context.Entry<Category>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new {Message = "Este registro já foi atualizado!"});
            }
            catch (Exception)
            {
                return BadRequest(new {Message = "Não foi possível atualizar a categoria!"});
            }
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<List<Category>>> Delete(
            int id,
            [FromServices] DataContext context
        )
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if(category == null)
                return NotFound(new {Message = "Categoria não encontrada!"});
            
            try
            {
                context.Categories.Remove(category);
                await context.SaveChangesAsync();
                return Ok(new { Message = "Categoria removida com sucesso!" });
            }
            catch(Exception)
            {
                return BadRequest(new { Message = "Não foi possível remover uma categoria" });
            }
        }
    }
}