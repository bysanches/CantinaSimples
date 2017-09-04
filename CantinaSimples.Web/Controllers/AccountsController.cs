using CantinaSimples.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace CantinaSimples.Web.Controllers
{
    [Authorize(Roles="Administrador")]
    [RoutePrefix("api/accounts")]
    public class AccountsController : BaseApiController
    {
        [Route("users")]
        public IHttpActionResult GetUsers()
        {
            return Ok(this.AppUserManager.Users.ToList().Select(u => this.TheModelFactory.Create(u)));
        }

        [Route("user/{id:guid}", Name="user")]
        public async Task<IHttpActionResult> GetUser(string Id)
        {
            var user = await this.AppUserManager.FindByIdAsync(Id);

            if (user != null)
            {
                return Ok(this.TheModelFactory.Create(user));
            }

            return NotFound();
        }

        [Route("userByEmail")]
        public async Task<IHttpActionResult> GetUserByEmail([Required] string email)
        {
            var user = await this.AppUserManager.FindByEmailAsync(email);

            if (user != null)
            {
                return Ok(this.TheModelFactory.Create(user));
            }

            return NotFound();

        }

        [Route("usersByRole")]
        public async Task<IHttpActionResult> GetUsersByRole([Required] string roleName)
        {
            var role = await this.AppRoleManager.FindByNameAsync(roleName);

            if (role == null)
                throw new Exception("Perfil não encontrado.");

            var users = this.AppUserManager.Users.Where(u => u.Roles.Select(r => r.RoleId).Contains(role.Id));

            return Ok(users.ToList().Select(u => this.TheModelFactory.Create(u)));
        }

        [Route("create")]
        public async Task<IHttpActionResult> CreateUser(UsuarioBindingModel userModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new Usuario()
            {
                UserName = userModel.Email,
                Email = userModel.Email,
                FirstName = userModel.FirstName,
                LastName = userModel.LastName
            };

            IdentityResult addUserResult = await this.AppUserManager.CreateAsync(user, userModel.Password);

            if (!addUserResult.Succeeded)
            {
                return GetErrorResult(addUserResult);
            }

            try
            {
                string code = await this.AppUserManager.GenerateEmailConfirmationTokenAsync(user.Id);

                var callbackUrl = new Uri(Url.Link("ConfirmEmailRoute", new { userId = user.Id, code = code }));

                await this.AppUserManager.SendEmailAsync(user.Id, "Verifique sua conta", "Por favor verifique a sua conta clicando <a href=\"" + callbackUrl + "\">aqui</a>");
            }
            catch(Exception)
            {

            }

            if (!string.IsNullOrEmpty(userModel.RoleName))
            {
                await this.AppUserManager.AddToRoleAsync(user.Id, userModel.RoleName);
            }

            Uri locationHeader = new Uri(Url.Link("user", new { id = user.Id }));

            return Created(locationHeader, TheModelFactory.Create(user));
        }

        [Route("edit")]
        [HttpPut]
        public async Task<IHttpActionResult> EditUser(UsuarioEditModel userModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Usuario user = await this.AppUserManager.FindByIdAsync(userModel.Id.ToString());

            if (user != null)
            {
                user.FirstName = userModel.FirstName;
                user.LastName = userModel.LastName;
                user.Email = userModel.Email;

                IdentityResult editResult = await this.AppUserManager.UpdateAsync(user);

                if (!editResult.Succeeded)
                {
                    return GetErrorResult(editResult);
                }

                List<string> removeRoles = new List<string>() { "Administrador", "Gerente", "Atendente", "Responsavel" };
                
                removeRoles.Remove(userModel.RoleName);

                await this.AppUserManager.RemoveFromRolesAsync(user.Id, removeRoles.ToArray());

                await this.AppUserManager.AddToRoleAsync(user.Id, userModel.RoleName);

                Uri locationHeader = new Uri(Url.Link("user", new { id = user.Id }));

                return Created(locationHeader, TheModelFactory.Create(user));
            }
            else
            {
                throw new Exception("Usuário não encontrado.");
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("ConfirmEmail", Name = "ConfirmEmailRoute")]
        public async Task<IHttpActionResult> ConfirmEmail(string userId = "", string code = "")
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                ModelState.AddModelError("", "User Id and Code are required");
                return BadRequest(ModelState);
            }

            IdentityResult result = await this.AppUserManager.ConfirmEmailAsync(userId, code);

            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return GetErrorResult(result);
            }
        }

        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await this.AppUserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        [Route("user/{id:guid}")]
        public async Task<IHttpActionResult> DeleteUser(string id)
        {

            var appUser = await this.AppUserManager.FindByIdAsync(id);

            if (appUser != null)
            {
                IdentityResult result = await this.AppUserManager.DeleteAsync(appUser);

                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }

                return Ok();

            }

            return NotFound();

        }
    }
}
