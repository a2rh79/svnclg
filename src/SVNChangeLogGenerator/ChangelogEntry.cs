// Copyright © CharlieFirpo 2009 <Firpo.Charlie@googlemail.com>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SVNChangeLogGenerator
{
    public class ChangelogEntry
    {
        public int StartRevision { get; set; }
        public int EndRevision { get; set; }
        private List<string> m_Version = new List<string>();
        public List<string> Version { get { return m_Version; } set { m_Version = value; } }
        private List<string> m_Authors = new List<string>();
        public List<string> Authors { get { return m_Authors; } set { m_Authors = value; } }
        private List<string> m_Dates = new List<string>();
        public List<string> Dates { get { return m_Dates; } set { m_Dates = value; } }
        private List<List<string>> m_Msg = new List<List<string>>();
        public List<List<string>> Msg { get { return m_Msg; } set { m_Msg = value; } }
        private List<SvnPath> m_Paths = new List<SvnPath>();
        public List<SvnPath> Paths { get { return m_Paths; } set { m_Paths = value; } }
        private List<int> m_Revisions = new List<int>();
        public List<int> Revisions { get { return m_Revisions; } set { m_Revisions = value; } }

        public string VersionString
        {
            get
            {
                string version = String.Empty;

                for (int i = 0; i < m_Version.Count; i++)
                {
                    version += m_Version[i];

                    if (i != m_Version.Count - 1)
                    {
                        version += ".";
                    }
                }

                return version;
            }
        }
    }

    public class SvnPath
    {
        public string Name { get; set; }
        public string Kind { get; set; }
        public string FromPath { get; set; }
        public string FromRevision { get; set; }
        private List<string> m_Actions = new List<string>();
        public List<string> Actions { get { return m_Actions; } set { m_Actions = value; } }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(Name);

            sb.Append(" (");

            for (int i = m_Actions.Count - 1; i >= 0; i--) // Count backwards to have the right order
            {
                sb.Append(m_Actions[i]);

                if (i != 0)
                {
                    sb.Append(", ");
                }
            }

            sb.Append(")");

            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            return Name.Equals(((SvnPath)obj).Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ Kind.GetHashCode() ^ FromPath.GetHashCode()
                ^ FromRevision.GetHashCode() ^ Actions.GetHashCode();
        }
    }
}
