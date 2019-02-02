using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolSostituitore.Resources.Classes
{
    public class FileInformation
    {
        public FileInformation(string fileName,string path)
        {
            Path = path;
            FileName = fileName;
        }

        private string m_FileName;

        public string FileName
        {
            get { return m_FileName; }
            set { m_FileName = value; }
        }

        private string m_Path;

        public string Path
        {
            get { return m_Path; }
            set { m_Path = value; }
        }

    }
}
