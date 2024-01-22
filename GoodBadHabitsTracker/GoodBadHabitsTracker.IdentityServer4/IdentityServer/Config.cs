// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;
using static System.Net.WebRequestMethods;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<ApiScope> ApiScopes
            => new List<ApiScope>
            {
                new ApiScope("GHBT.API", "GoodBadHabitsTracker.API")
            };
        public static IEnumerable<Client> Clients
            => new List<Client>
            {
                new Client
                {
                    ClientName = "GoodBadHabitsTracker.Client",
                    ClientId = "GBHT.Client",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    RedirectUris = {"https://localhost:8080/callback.html"},
                    PostLogoutRedirectUris = {"https://localhost:8080/index.html"},
                    AllowedCorsOrigins = {"https://localhost:8080"},

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "GoodBadHabitsTracker.API"
                    }
                }
            };
        public static IEnumerable<IdentityResource> IdentityResources
            => new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
    }
}