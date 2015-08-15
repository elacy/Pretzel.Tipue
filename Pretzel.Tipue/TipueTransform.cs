using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotLiquid.FileSystems;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Pretzel.Logic.Extensibility;
using Pretzel.Logic.Templating.Context;

namespace Pretzel.Tipue
{
    public class TipueTransform: ITransform
    {
        public void Transform(SiteContext siteContext)
        {
            var tipueContent = new TipueContent();
            foreach (var post in siteContext.Posts)
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(post.Content);

                tipueContent.Pages.Add(new TipuePage
                {
                    Title = post.Title,
                    Tags = string.Join(" ", post.Tags),
                    Url = post.Url,
                    Text = doc.DocumentNode.InnerText
                });
            }

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var text = $"var tipuesearch = {JsonConvert.SerializeObject(tipueContent, settings)}";

            File.WriteAllText(Path.Combine(siteContext.OutputFolder, @"assets\js\tipuesearch_content.js"), text);
        }
    }
}
