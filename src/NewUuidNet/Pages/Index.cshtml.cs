using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NewUuidNet.Pages;

public class IndexModel : PageModel
{
    public Guid Uuid { get; } = Guid.NewGuid();

    public void OnGet()
    {
        Response.Headers.CacheControl = "no-store, no-cache, max-age=0";
        Response.Headers.Pragma = "no-cache";
        Response.Headers.Expires = "0";
    }
}
