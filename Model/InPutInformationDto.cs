﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 子端.Model
{
    public class InPutInformationDto
    {

        public List<int> type { get; set; }

        public string YamlPath { get; set; }
        public string Ip { get; set; }

        public InPutYamlTextDto yaml { get; set; }
    }
}
