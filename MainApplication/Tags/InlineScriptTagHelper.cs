using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;

namespace MainApplication.Tags
{
    public class InlineScriptTagHelper : InlineTagHelper
    {
        [HtmlAttributeName("src")]
        public string Src { get; set; }

        public InlineScriptTagHelper(IWebHostEnvironment hostingEnvironment, IMemoryCache cache)
            : base(hostingEnvironment, cache)
        {
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var fileContent = await GetFileContentAsync(Src);
            if (fileContent == null)
            {
                output.SuppressOutput();
                return;
            }

            output.TagName = "script";
            output.Attributes.RemoveAll("src");
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.AppendHtml(fileContent);
        }
    }
}
