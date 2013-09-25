﻿using System;
using System.Collections.Generic;
using YamlDotNet.Dynamic;

namespace DevTyr.Gullap.Model
{
    public class ContentBase
    {
        private dynamic yaml;

        public ContentBase(DynamicYaml dynamicYaml, string unparsedContent)
        {
            yaml = dynamicYaml;
            Content = unparsedContent;
        }

        public string Title
        {
            get { return yaml.title; }
        }

        public string Category
        {
            get { return yaml.category; }
        }

        public string Author
        {
            get { return yaml.author; }
        }

        public DateTime Date
        {
            get { return DateTime.Parse((string)yaml.date); }
        }

        public List<string> Tags
        {
            get { return yaml.tags; }
        }

        public string Template
        {
            get { return yaml.template; }
        }

        public bool Draft
        {
            get
            {
                var draft = (string)yaml.draft;
                if (string.IsNullOrWhiteSpace(draft))
                    return false;
                return Boolean.Parse(draft);
            }
        }

        public string Content { get; set; }
        public string Url { get; set; }

        public dynamic Meta
        {
            get
            {
                return yaml;
            }
        }
    }
}
