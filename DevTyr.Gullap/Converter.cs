using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using DevTyr.Gullap.IO;
using DevTyr.Gullap.Model;
using DevTyr.Gullap.Parser;
using DevTyr.Gullap.Parser.Markdown;
using DevTyr.Gullap.Templating;
using DevTyr.Gullap.Templating.Nustache;
using DevTyr.Gullap.Extensions;

namespace DevTyr.Gullap
{
	public class Converter
	{
		private ConverterOptions Options { get; set; }
		private SitePaths Paths { get; set; }
		private IParser internalParser = new MarkdownParser();
		private ITemplater internalTemplater = new NustacheTemplater();

		public Converter (ConverterOptions options)
		{
			Guard.NotNull(options, "options");
			Guard.NotNullOrEmpty(options.SitePath, "options.SitePath");

			Options = options;

			Paths = new SitePaths(options.SitePath);
		}

		public void SetParser (IParser parser)
		{
			if (parser == null)
				throw new ArgumentNullException("parser");

			internalParser = parser;
		}

		public void SetTemplater (ITemplater templater)
		{
			if (templater == null)
				throw new ArgumentNullException("templater");

			internalTemplater = templater;
		}

		public void InitializeSite ()
		{
			var generator = new SiteGenerator();
			generator.Generate(new SitePaths(Options.SitePath));
		}

		public ConverterResult ConvertAll ()
		{
			try {
				CleanOutput();
				CopyAssets();
				return ConvertAllInternal();
			} catch (Exception ex) {
				return new ConverterResult(true, ex.Message, null);
			}
		}

		private void CleanOutput ()
		{
			var files = Directory.GetFiles(Paths.OutputPath, "*.*", SearchOption.AllDirectories);
			foreach(var file in files)
				File.Delete(file);
		}

		private void CopyAssets ()
		{
			DirectoryExtensions.DirectoryCopy(Paths.AssetsPath, Paths.OutputPath, true);
		}

		private ConverterResult ConvertAllInternal ()
		{
		    var workspaceInfo = new WorkspaceInfo(Paths.PagesPath);
		    var pages = workspaceInfo.GetPages().ToList();

		    ParseContents(pages);
		    var metadata = ParseTemplateData(pages);

			var successMessages = new List<string>();
			
			foreach (var page in pages) 
            {
				Export (page, metadata);
				successMessages.Add("Exported " + page.FileName);
			}
			return new ConverterResult(false, null, successMessages);
		}

        private object ParseTemplateData(IEnumerable<MetaPage> metaPages)
        {
            dynamic metadata = new
            {
                site = new
                {
                    time = DateTime.Now,
                    pages = metaPages.Where(page => !page.Page.Draft).Select(page => page.Page)
                }
            };

            return metadata;
        }

        private void ParseContents(IEnumerable<MetaPage> metaPages)
        {
            foreach (var page in metaPages)
            {
                internalParser.Parse(page.Page.Content);
            }
        }

		private void EnsureTargetPath (string targetPath)
		{
			if (!Directory.Exists (targetPath)) 
			{
				Directory.CreateDirectory(Path.Combine(Paths.OutputPath, targetPath));
			}
		}

		private void Export (MetaPage page, dynamic metadata)
		{
		    var targetFileName = page.FileName.Replace(Paths.PagesPath, Paths.OutputPath);

		    Console.WriteLine (targetFileName);
		    
            if (string.IsNullOrEmpty (targetFileName))
		        targetFileName = Environment.CurrentDirectory;

		    EnsureTargetPath (targetFileName);
		    
		    var result = internalTemplater.Transform (Paths.TemplatePath, page.Page.Template, metadata);

		    File.WriteAllText (targetFileName, result);
		}

	}
}

