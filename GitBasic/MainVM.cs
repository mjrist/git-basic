using Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitBasic
{
    public class MainVM : Observable
    {
        public Prop<string> WorkingDirectory { get; set; } = new Prop<string>(Properties.Settings.Default.WorkingDirectory);
    }
}
