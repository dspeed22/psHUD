using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.PowerShell;
using System.Management.Automation.Runspaces;

namespace psHUD
{
    class Program
    {
        static void Main(string[] args)
        {
            new Hud().Init(args);
        }
    }
}
