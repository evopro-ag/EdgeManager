using System;
using System.Collections.Generic;
using System.Text;
using EdgeManager.Interfaces.Services;
using Microsoft.PowerShell.Commands;

namespace EdgeManager.Interfaces.Models
{
    
    public class JsonCommand
    {
        private string _command;
        public JsonCommand(string command)
        {
            _command = command;
        }

        public string GetCommand()
        {
            return _command;
        } 
        

    }
}
