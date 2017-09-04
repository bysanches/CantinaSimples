using CantinaSimples.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CantinaSimples.Web.Infrastructure
{
    public class UsuarioManager : UserManager<Usuario>
    {
        public UsuarioManager(IUserStore<Usuario> store)
            : base(store)
        {
        }

        public static UsuarioManager Create(IdentityFactoryOptions<UsuarioManager> options, IOwinContext context)
        {
            var appDbContext = context.Get<CantinaContext>();
            var appUserManager = new UsuarioManager(new UserStore<Usuario>(appDbContext));

            appUserManager.EmailService = new AspNetIdentity.WebApi.Services.EmailService();

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                appUserManager.UserTokenProvider = new DataProtectorTokenProvider<Usuario>(dataProtectionProvider.Create("ASP.NET Identity"))
                {
                    //Code for email confirmation and reset password life time
                    TokenLifespan = TimeSpan.FromHours(24)
                };
            }

            appUserManager.UserValidator = new UserValidator<Usuario>(appUserManager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            appUserManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6
            };

            return appUserManager;
        }
    }
}