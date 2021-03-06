﻿using System.Collections.Generic;
using DevTyr.Gullap.Model;

namespace DevTyr.Gullap.IO
{
    public class WorkspaceFiles
    {
        private readonly List<MetaContent> filesToParse = new List<MetaContent>();
        private readonly List<string> filesNotToParse = new List<string>();

        public List<MetaContent> FilesToParse 
        {
            get
            {
                return filesToParse;
            } 
        }

        public List<string> FilesNotToParse
        {
            get
            {
                return filesNotToParse;
            }
        }
    }
}
