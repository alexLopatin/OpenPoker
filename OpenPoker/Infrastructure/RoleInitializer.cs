using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using OpenPoker.Models;
using System.Xml;

namespace OpenPoker.Infrastructure
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            XmlDocument settings = new XmlDocument();
            settings.Load("IdentityInit.xml");
            XmlElement xRoot = settings.DocumentElement;
            if (xRoot.Name == "Identity")
            {
                foreach (XmlNode xnode in xRoot)
                {
                    if (xnode.Name == "Roles")
                    {
                        foreach (XmlNode roleNode in xnode)
                        {
                            string name = roleNode.Attributes["name"].Value;
                            if (await roleManager.FindByNameAsync(name) == null)
                            {
                                await roleManager.CreateAsync(new IdentityRole(name));
                            }
                        }
                    }
                    if (xnode.Name == "Users")
                    {
                        foreach (XmlNode userNode in xnode)
                        {
                            string email = userNode.Attributes["email"].Value;
                            string password = userNode.Attributes["password"].Value;
                            List<string> roles = new List<string>();
                            foreach(XmlNode roleNode in userNode)
                            {
                                if (roleNode.Name == "User.Role")
                                    roles.Add(roleNode.Attributes["name"].Value);
                            }
                            if (await userManager.FindByNameAsync(email) == null)
                            {
                                User admin = new User { Email = email, UserName = email };
                                userManager.Options.Password.RequireNonAlphanumeric = false;
                                IdentityResult result = await userManager.CreateAsync(admin, password);
                            }
                            var user = await userManager.FindByNameAsync(email);
                            if(user != null)
                                foreach (string role in roles)
                                {
                                    if (await userManager.IsInRoleAsync(user, role) == false)
                                        await userManager.AddToRoleAsync(user, "admin");
                                }
                        }
                    }
                }
            }
        }
    }
}
