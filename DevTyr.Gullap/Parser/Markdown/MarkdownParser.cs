using System.Collections.Generic;
using MarkdownSharp;

namespace DevTyr.Gullap.Parser.Markdown
{
	public class MarkdownParser : IParser
	{
	    public IEnumerable<string> SupportedFileExtensions
	    {
	        get
	        {
	            yield return "md";
	            yield return "markdown";
	        } 
	    }

		public string Parse (string content)
		{
            Guard.NotNullOrEmpty(content, "content");

		    var markdownOptions = new MarkdownOptions
		    {
		        AutoHyperlink = true
		    };

		    var markdown = new MarkdownSharp.Markdown (markdownOptions);
		    return markdown.Transform (content);
		}
	}
}

