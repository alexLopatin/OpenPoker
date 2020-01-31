using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using OpenPoker.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace OpenPoker.Infrastructure
{
    public class UserManager : UserManager<User>
    {
        public UserManager(IUserStore<User> store)
            : base(store)
        { }

        public static UserManager Create(IdentityFactoryOptions<UserManager> options,
            IOwinContext context)
        {
            AppIdentityDbContext db = context.Get<AppIdentityDbContext>();
            UserManager manager = new UserManager(new UserStore<User>(db));
            return manager;
        }
    }
}
