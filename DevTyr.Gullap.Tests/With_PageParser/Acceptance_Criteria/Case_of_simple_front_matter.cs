using System;
using DevTyr.Gullap.Yaml;
using FluentAssertions;
using NUnit.Framework;

namespace DevTyr.Gullap.Tests.With_PageParser.Acceptance_Criteria
{
    [TestFixture]
	public class Case_of_simple_front_matter
    {
        readonly PageParser parser = new PageParser ();
		readonly SampleYamlFrontMatter sample = new SampleYamlFrontMatter 
        {
			Text = 
				"---" + Environment.NewLine +
				"title: Test" + Environment.NewLine +
				"---" + Environment.NewLine +
				" " + Environment.NewLine +
				"This is the content",
			Title = "Test", 
			Content = "This is the content"
		};

        readonly SampleYamlFrontMatter sampleOnlyTitle = new SampleYamlFrontMatter
        {
            Text = "---" + Environment.NewLine +
                    "title: Test" + Environment.NewLine +
                    "---" + Environment.NewLine,
            Title = "Test"
        };

        private readonly SampleYamlFrontMatter sampleOnlyTitleHavingHyphensInContent = new SampleYamlFrontMatter
        {
            Text = "---" + Environment.NewLine +
                   "title: Test" + Environment.NewLine +
                   "---" + Environment.NewLine +
                   "This is having ----- inside",
            Title = "Test",
            Content = "This is having ----- inside"
        };

        [Test]
        public void Can_parse_content()
        {
            var page = parser.Parse(sample.Text);

            page.Content
				.Should ()
				.BeEquivalentTo (sample.Content);
        }

		[Test]
		public void Can_parse_title()
		{
		    var page = parser.Parse(sample.Text);

			page.Title
				.Should ()
				.BeEquivalentTo (sample.Title);
		}

        [Test]
        public void Can_parse_having_only_title()
        {
            var page = parser.Parse(sampleOnlyTitle.Text);

            page.Title
                .Should()
                .BeEquivalentTo(sampleOnlyTitle.Title);
        }

        [Test]
        public void Can_parse_having_hyphens_within_content()
        {
            var page = parser.Parse(sampleOnlyTitleHavingHyphensInContent.Text);

            page.Title
                .Should()
                .BeEquivalentTo(sampleOnlyTitleHavingHyphensInContent.Title);

            page.Content
                .Should()
                .BeEquivalentTo(sampleOnlyTitleHavingHyphensInContent.Content);
        }

		private class SampleYamlFrontMatter 
		{
			public string Text { get; set; }
			public string Title { get; set; }
			public string Content { get; set; }
		}
	}
}
