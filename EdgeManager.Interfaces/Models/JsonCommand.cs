using System;
using System.Collections.Generic;
using System.Text;
using EdgeManager.Interfaces.Services;
using Microsoft.PowerShell.Commands;

namespace EdgeManager.Interfaces.Models
{
    
    public class JsonCommand
    {
        public JsonCommand(string command, string result)
        {
            Command = command;
            ResultJson = result;
        }
        public string Command { get; }
        public string ResultJson { get; set; }
    }
}
