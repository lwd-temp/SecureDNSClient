﻿using System;

namespace MsmhToolsClass.ProxyServerPrograms;

public partial class ProxyProgram
{
    public class DontBypass
    {
        public enum Mode
        {
            File,
            Text,
            Disable
        }

        public Mode DontBypassMode { get; private set; } = Mode.Disable;
        public string PathOrText { get; private set; } = string.Empty;
        public string TextContent { get; private set; } = string.Empty;
        private List<string> DontBypassList { get; set; } = new();

        public DontBypass() { }

        /// <summary>
        /// Set DontBypass Database
        /// </summary>
        /// <param name="mode">Mode</param>
        /// <param name="filePathOrText">e.g. Each line: google.com</param>
        public void Set(Mode mode, string filePathOrText)
        {
            DontBypassMode = mode;
            PathOrText = filePathOrText;

            if (DontBypassMode == Mode.Disable) return;

            if (DontBypassMode == Mode.File)
            {
                try
                {
                    TextContent = File.ReadAllText(Path.GetFullPath(filePathOrText));
                }
                catch (Exception)
                {
                    // do nothing
                }
            }
            else if (DontBypassMode == Mode.Text)
                TextContent = filePathOrText;

            if (!string.IsNullOrEmpty(TextContent) || !string.IsNullOrWhiteSpace(TextContent))
            {
                TextContent += Environment.NewLine;
                DontBypassList = TextContent.SplitToLines();
            }
        }

        // If True Don't Bypass, If false Bypass
        public bool IsMatch(string destHostname)
        {
            string destHostnameNoWWW = destHostname;
            if (destHostnameNoWWW.StartsWith("www."))
                destHostnameNoWWW = destHostnameNoWWW.Replace("www.", string.Empty);

            if (DontBypassList.Any())
            {
                for (int n = 0; n < DontBypassList.Count; n++)
                {
                    string host = DontBypassList[n].Trim();
                    if (!string.IsNullOrEmpty(host) && !host.StartsWith("//")) // Add Support Comment //
                    {
                        if (host.StartsWith("www."))
                            host = host.Replace("www.", string.Empty);

                        // If Match
                        if (destHostnameNoWWW.Equals(host)) return true;
                    }
                }
            }

            // If Not Match
            return false;
        }
    }
}