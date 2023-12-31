﻿using AseIsthmusAPI.Data;
using AseIsthmusAPI.Data.AseIsthmusModels;
using AseIsthmusAPI.Data.DTOs;
using AseIsthmusAPI.Services;
using AseIsthmusAPI.Templates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AseIsthmusAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _service;
        private readonly EmailService _emailService;
        private readonly DocumentsService _documentService;

        #region Constructors 

        public UserController(UserService service, EmailService emailService, DocumentsService documentService)
        {
            _service = service;
            _emailService = emailService;
            _documentService = documentService;
        }

        #endregion

        #region Get

        [Authorize]
        [HttpGet]
        public async Task<IEnumerable<UserDtoOut>> Get()
        {
            return await _service.GetAll();
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDtoOut>> GetById([FromRoute] string id)
        {
            var user = await _service.GetDtoById(id);
            if (user is null) return UserNotFound(id);

            return user;
        }

        #endregion

        #region Create

        [HttpPost]
        public async Task<IActionResult> Insert(UserDtoIn user)
        {
            string validationResult = await _service.DuplicateAccount(user);

            if (validationResult.Equals("user exists by Id"))
                return BadRequest(new { error = "El usuario con el código de empleado ingresado ya existe en el sistema. Contacte al administrador." });
            else if (validationResult.Equals("user exists by numberId"))
                return BadRequest(new { error = "El usuario con la identificación ingresada ya existe en el sistema. Contacte al administrador." });
            else if (validationResult.Equals("user exists by email"))
                return BadRequest(new { error = "El usuario con el correo ingresado ya existe en el sistema. Contacte al administrador." });
            else if (!ModelState.IsValid)
                return BadRequest(new { error = "Faltan datos por ingresar." });
            else
            {
                var newUser = await _service.Create(user);

                return CreatedAtAction(nameof(GetById), new { id = newUser.PersonId }, newUser);
            }

        }

        #endregion

        #region Patch
        /// <summary>
        /// Update user by Admin
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [Authorize(Policy = "Administrator")]
        [HttpPatch("employee/{id}")]
        public async Task<IActionResult> Update([FromRoute] string id, UpdateProfileAdminDto user)
        {

            if (id != user.PersonId)
                return BadRequest(new { message = $"El código({id}) de la URL no coincide con el código({user.PersonId}) de los datos" });

            var existingClient = await _service.GetById(id);
            try
            {
                if (existingClient is not null)
                {
                    await _service.UpdateUserByAdmin(user);
                    return Ok(new { message = "successful" });
                }
                else
                {
                    return UserNotFound(id);
                }
            }
            catch (ArgumentException)
            {
                return BadRequest(new { error = "No se pudo procesar su pedido." });
            }
        }

        /// <summary>
        /// update user by user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateUserByUser([FromRoute] string id, UserUpdateDto user)
        {
            var existingClient = await _service.GetById(id);
            try
            {
                if (existingClient is not null)
                {
                    await _service.UpdateUserByUser(id, user);
                    return Ok(new { message = "successful" });
                }
                else
                {
                    return UserNotFound(id);
                }
            }
            catch (ArgumentException)
            {
                return BadRequest(new { error = "No se pudo procesar su pedido." });
            }
        }
        #endregion

        #region Delete

        [Authorize(Policy = "Administrator")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existingClient = await _service.GetById(id);

            if (existingClient is not null)
            {
                var result = await _service.DeleteUser(id);
                if (string.IsNullOrEmpty(result))
                {
                    return NoContent();
                }
                else
                {
                    return BadRequest(new { error = result });
                }
            }
            else
            {
                return UserNotFound(id);
            }
        }

        #endregion

        #region Patch user status

        //[Authorize(Policy = "Administrator")]
        //[HttpPatch("activateuser/{id}")]
        //public async Task<IActionResult> ManageUserStatus([FromRoute] string id)
        //{
        //    var user = await _service.GetById(id);
        //    HtmlContentProvider emailTemplate = new HtmlContentProvider();
        //    var result =  await _service.ManageUserStatus(id);

        //   if (user is not null && result is not null)
        //    {
        //        if (result.Equals("Activated"))
        //        {
        //            string pdfFilePath = @"Templates/G1_SC603_J_Requerimientos.pdf";
        //            _emailService.SendEmail(emailTemplate.ApprovalEmailContent(), "Activación de usuario", user.EmailAddress, pdfFilePath);
        //            return Ok(result);
        //        }
        //        else { 
        //            return Ok(result); 
        //        }
        //    }
        //    else
        //    {
        //        return BadRequest(new { error = "No se pudo procesar su pedido." });
        //    }

        //}

        [Authorize(Policy = "Administrator")]
        [HttpPatch("activateuser/{id}")]
        public async Task<IActionResult> ManageUserStatus([FromRoute] string id)
        {
            var user = await _service.GetById(id);
            HtmlContentProvider emailTemplate = new HtmlContentProvider();
            var result = await _service.ManageUserStatus(id);
           
            if (user is not null && result is not null)
            {
                if (result.Equals("Activated"))
                {
                    string welcome = "Bienvenida";
                    string googleDriveLink = await _documentService.FetchGoogleLink(welcome);


                    if (googleDriveLink != null)
                    {
                        _emailService.SendEmail(emailTemplate.ApprovalEmailContent(), "Activación de usuario", user.EmailAddress, googleDriveLink);
                        return Ok(result);
                    }
                    else {

                        _emailService.SendEmail(emailTemplate.ApprovalEmailContent(), "Activación de usuario", user.EmailAddress);
                        return Ok(result);
                    }

                   
                }
                else
                {
                    return Ok(result);
                }
            }
            else
            {
                return BadRequest(new { error = "No se pudo procesar su pedido." });
            }
        }


        #endregion

        #region Non Actions
        [NonAction]
        public NotFoundObjectResult UserNotFound(string id)
        {
            return NotFound(new { error = $"El usuario con código={id} no existe." });

        }

        #endregion
    }
}
