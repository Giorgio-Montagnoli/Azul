using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Azul.Models.ViewModel
{
    public class GameListVM
    {
        public string Id { get; set; }

        public List<string> Players { get; set; }
    }
}