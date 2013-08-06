using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using DevTyr.Gullap.IO;
using DevTyr.Gullap.Menu;
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
		    var mapping = workspaceInfo.GetPathContentMapping();

		    var fileInfos = new List<ParsedFileInfo>();

		    foreach (var key in mapping.Keys)
		    {
		        var fileInfo = internalParser.Parse(mapping[key]);
		        fileInfo.FileName = key;
                fileInfos.Add(fileInfo);
		    }

		    var menuBuilder = new MenuBuilder ();
			var mainMenu = menuBuilder.Build (fileInfos);
			
			var successMessages = new List<string>();
			
			foreach (var info in fileInfos) {
				Export (info, mainMenu);
				if (string.IsNullOrWhiteSpace(info.TargetFileName)) {
					successMessages.Add("Handled (not exported) " + Path.GetFileName(info.FileName));
				} else {
					successMessages.Add("Exported " + info.TargetFileName);
				}
			}
			return new ConverterResult(false, null, successMessages);
		}

		private void EnsureTargetPath (string targetPath)
		{
			if (!Directory.Exists (targetPath)) 
			{
				Directory.CreateDirectory(Path.Combine(Paths.OutputPath, targetPath));
			}
		}

		private void Export (ParsedFileInfo info, MainMenu mainMenu)
		{
		    if (!info.ShouldBeGenerated) return;

		    var directoryName = Path.GetDirectoryName (info.TargetFileName);
		    Console.WriteLine (info.TargetFileName);
		    if (string.IsNullOrEmpty (directoryName))
		        directoryName = Environment.CurrentDirectory;
		    EnsureTargetPath (directoryName);

		    var sidebarItems = mainMenu.GetSidebarItems(info.Sidebar);

		    var templateData = new { content = info.ParsedContent, menu = mainMenu, title = info.Title, date = info.Date, description = info.Description, author = info.Author, sidebarheader = info.Sidebar, sidebaritems = sidebarItems, keywords = info.Keywords };

		    var result = internalTemplater.Transform (Paths.TemplatePath, info.Template, templateData);

		    var target = CalculateTargetFileName(info);
		    File.WriteAllText (target, result);
		}

		private string CalculateTargetFileName (ParsedFileInfo info)
		{
			return Path.Combine(Paths.OutputPath, info.TargetFileName);
		}
	}
}

