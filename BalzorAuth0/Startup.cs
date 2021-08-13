using BalzorAuth0.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using RestSharp;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ScopeLibrary.ScopeRequirement;
using ScopeLibrary.ScopeHandler;
using TokenLibrary.Token;

namespace BalzorAuth0
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Set variables
            string domain = Configuration["Auth0:Domain"];
            string clientId = Configuration["Auth0:ClientId"];
            string clientSecret = Configuration["Auth0:ClientSecret"];
            string audience = Configuration["Auth0:Audience"];

            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            // Add authentication services
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOpenIdConnect("Auth0", options =>
            {
                // Set the authority to your Auth0 domain
                options.Authority = $"https://{domain}";
                // Configure the Auth0 Client ID and Client Secret
                options.ClientId = clientId;
                options.ClientSecret = clientSecret;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "https://schemas.quickstarts.com/roles"
                };
                // Set response type to code
                options.ResponseType = "code";

                // Configure the scope
                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");   //?? new code
                options.Scope.Add("email");
                // Set the callback path, so Auth0 will call back to http://localhost:3000/callback
                // Also ensure that you have added the URL as an Allowed Callback URL in your Auth0 dashboard
                options.CallbackPath = new PathString("/callback");

                // Configure the Claims Issuer to be Auth0
                options.ClaimsIssuer = "Auth0";
                // Saves tokens to the AuthenticationProperties
                options.SaveTokens = true;
                // Add Events
                options.Events = new OpenIdConnectEvents
                {
                    //Add audience parameter
                    OnRedirectToIdentityProvider = context =>
                    {
                        context.ProtocolMessage.SetParameter("audience", audience);
                        return Task.FromResult(0);
                    },
                    //Add event when token were validated
                    OnTokenValidated = context =>
                    {
                        //set security token
                        string secJwt = context.SecurityToken.RawData;
                        //set access token
                        string accessJwt = context.TokenEndpointResponse.AccessToken;
                        //read access token
                        var handler = new JwtSecurityTokenHandler();
                        var jwt = handler.ReadJwtToken(accessJwt);
                        //get permissions from access token
                        List<Claim> permissions = jwt.Claims.Where(x => x.Type == "permissions").ToList();
                        //add permission to user claims (in this case add new identity)
                        context.Principal.AddIdentity(new ClaimsIdentity(permissions));
                        //return
                        return Task.FromResult(0);
                    },
                    // handle the logout redirection
                    OnRedirectToIdentityProviderForSignOut = (context) =>
                    {
                        var logoutUri = $"https://{domain}/v2/logout?client_id={clientId}";

                        var postLogoutUri = context.Properties.RedirectUri;
                        if (!string.IsNullOrEmpty(postLogoutUri))
                        {
                            if (postLogoutUri.StartsWith("/"))
                            {
                                // transform to absolute
                                var request = context.Request;
                                postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + postLogoutUri;
                            }
                            logoutUri += $"&returnTo={ Uri.EscapeDataString(postLogoutUri)}";
                        }

                        context.Response.Redirect(logoutUri);
                        context.HandleResponse();

                        return Task.FromResult(0);
                    }
                };
            });
            //Add policies
            services.AddAuthorization(options =>
            {
                options.AddPolicy("read:cuestion", policy => policy.Requirements.Add(new HasScopeRequirement("read:cuestion", $"https://{domain}/")));
            });
            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
            //
            services.AddHttpContextAccessor();
            
            services.AddHttpClient();
            services.AddScoped<TokenProvider>();
            services.AddScoped<WeatherForecastService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
