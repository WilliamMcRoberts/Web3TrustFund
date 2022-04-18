using MetaMask.Blazor;
using Microsoft.AspNetCore.Components.Web;
using Syncfusion.Blazor;
using Blazorise;
using Blazorise.Icons.FontAwesome;
using Blazorise.Bootstrap;

namespace Web3TrustFundUI
{
    public static class RegisterServices
    {
        public static void ConfigureServices(this WebApplicationBuilder builder)
        {
            // Add services to the container.

            // Add services to the container.
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NjEwODM5QDMyMzAyZTMxMmUzMEUzeFlOQUVQRTduUjQ1bFgzM1lIamY0TmhmL0FFSm54WEsyL1NObDFaRlk9");
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddMetaMaskBlazor();
            builder.Services.AddBlazorise(options =>
                                            {
                                                options.Immediate = true;
                                            })
                                            .AddBootstrapProviders()
                                            .AddFontAwesomeIcons();

                                                }
                                            }

}
